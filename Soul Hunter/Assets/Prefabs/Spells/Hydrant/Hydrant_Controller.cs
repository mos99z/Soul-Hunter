using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hydrant_Controller : MonoBehaviour {

	public float Damage = 20.2f;
	public float DamageTickRate = 0.1f;
	public float RecoveryCostRate = 1.0f/3.0f;
	public float PushBackForce = 0.75f;
	public float Range = 10.0f;
	public float StartHeight = 1.0f;
	public GameObject ImpactEffectObj = null;
	public float Duration = 10.0f;
	public BoxCollider TheCollider = null;
    public GameObject SpellEffect = null;
    public LayerMask WALLS;

	private float DeltaT = 0.0f;
	private GameObject PlayerFaceingIndicator = null;
	private float Size = 10.0f;
	private float RecoveryCost = 0.0f;
	private float TickTime = 0.0f;
    private List<GameObject> Hitting = new List<GameObject>();
    private bool Once = true;

	void Start ()
	{
		PlayerFaceingIndicator = GameObject.Find ("Direction Indicator");
        transform.position = new Vector3(transform.position.x, StartHeight, transform.position.z);
	}
	
	void Update ()
	{
        DeltaT += Time.deltaTime;
        RecoveryCost += RecoveryCostRate * Time.deltaTime;
        if (Input.GetMouseButtonUp(0) || DeltaT >= Duration)
        {
            GameObject.FindGameObjectWithTag("Player").SendMessage("SetRecoverTime", RecoveryCost, SendMessageOptions.RequireReceiver);
            Destroy(gameObject);
        }

        TickTime += Time.deltaTime;
        float smallestSize = float.MaxValue;
        for (int i = 0; i < Hitting.Count; i++)
        {
            if (Hitting[i].tag == "Solid")
            {
            Vector3 objPos = Hitting[i].transform.position;
            objPos.y = PlayerFaceingIndicator.transform.position.y;
            float sizeTest = (objPos - PlayerFaceingIndicator.transform.position).magnitude;
            if (smallestSize > sizeTest)
                smallestSize = sizeTest;
            }
            else
            {
                Vector3 rayOrigin = PlayerFaceingIndicator.transform.position;
                rayOrigin.y = StartHeight;
                Ray wallRangeCheck = new Ray(rayOrigin, transform.forward);

                RaycastHit RayInfo;
                Physics.Raycast(wallRangeCheck, out RayInfo, Range, WALLS);
                if (smallestSize > RayInfo.distance)
                    smallestSize = RayInfo.distance;
            }

            if (Hitting[i].tag == "Enemy")
            {
                if (TickTime >= DamageTickRate)
                    Hitting[i].transform.SendMessage ("TakeDamage", Damage);

                if (Hitting[i].transform.GetComponent<Rigidbody>())
                    Hitting[i].transform.GetComponent<Rigidbody>().AddRelativeForce (transform.forward.normalized * PushBackForce);
            }
        }
        if (TickTime >= DamageTickRate)
            TickTime = 0.0f;
        Size = smallestSize;
        if (Size > Range)
            Size = Range;
	}
	
	void FixedUpdate ()
    {
        transform.forward = PlayerFaceingIndicator.transform.forward;
        Vector3 newPosition = PlayerFaceingIndicator.transform.position;
        newPosition.y = StartHeight;
        Vector3 posChange = transform.forward.normalized * Size * 0.5f;
        newPosition += posChange;
        transform.position = newPosition;
        TheCollider.transform.localScale = new Vector3(1.0f, 1.0f, Size);

        if (Once)
        {
            Once = false;
            SpellEffect.SetActive(true);
        }
	}
	
	void OnTriggerEnter(Collider _object)
    {
		if (_object.tag == "Enemy" || _object.tag == "Solid")
		{
            bool unique = true;
            for (int i = 0; i < Hitting.Count; i++)
            {
                if (_object.gameObject == Hitting[i])
                {
                    unique = false;
                    break;
                }
            }
            if (unique)
                Hitting.Add(_object.gameObject);
		}
	}

    void OnTriggerExit(Collider _object)
    {
        if (_object.tag == "Enemy" || _object.tag == "Solid")
        {
            for (int i = 0; i < Hitting.Count; i++)
            {
                if (_object.gameObject == Hitting [i])
                {
                    Hitting.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
