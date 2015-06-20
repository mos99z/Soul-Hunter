using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hydrant_Controller : MonoBehaviour {

	public float Damage = 2.2f;
	public float DamageTickRate = 0.1f;
	public float RecoveryCostRate = 1.0f/3.0f;
	public float PushBackForce = 0.75f;
	public float Range = 10.0f;
	public float StartHeight = 1.0f;
	public GameObject ImpactEffectObj = null;
	public float Duration = 10.0f;
	public BoxCollider TheCollider = null;
    public GameObject SpellEffect = null;
	public GameObject WetDebuff = null;
	public float DebuffChance = 0.1f;
	public float DebuffDuration = 10.0f;
    public LayerMask WALLS;
	public GameObject PlayerFaceingIndicator = null;	// this object must be manually set, or an enemy indicator may be assigned instead

	private float DeltaT = 0.0f;
	private float Size = 1.0f;
	private float oldSize = 0.0f;
	private float smallestSize = float.MaxValue;
	private float RecoveryCost = 0.5f;
	private float TickTime = 0.0f;
    private List<GameObject> Hitting = new List<GameObject>();
    private bool Once = true;

	void Start ()
	{
		int waterValue = GameBrain.Instance.WaterLevel;
		PushBackForce += waterValue * 0.05f;
		DebuffChance += waterValue * 0.1f;
		Duration += waterValue * 1.0f;
		RecoveryCost -= waterValue * 0.02777777f;
		
		oldSize = Size;
		//PlayerFaceingIndicator = GameObject.Find ("Direction Indicator");
        transform.position = new Vector3(transform.position.x, StartHeight, transform.position.z);
		if (WetDebuff == null) 
			WetDebuff = GameBrain.Instance.GetComponent<DebuffMasterList>().wet;
		SpellEffect.SetActive (false);
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
        smallestSize = float.MaxValue;
        for (int i = 0; i < Hitting.Count; i++)
        {
			if(Hitting[i] == null)
			{
				Hitting.RemoveAt(i);
				i--;
				continue;
			}

            if (Hitting[i].tag == "Enemy")
            {
				Vector3 objPos = Hitting[i].transform.position;
				objPos.y = PlayerFaceingIndicator.transform.position.y;
				float sizeTest = (objPos - PlayerFaceingIndicator.transform.position).magnitude;
				if (smallestSize > sizeTest)
					smallestSize = sizeTest;

                if (TickTime >= DamageTickRate){
                    Hitting[i].transform.SendMessage ("TakeDamage", Damage);
					float roll = Random.Range(0.0f, 1.0f);
					if (roll <= DebuffChance)
					{
						GameObject debuff = Instantiate(WetDebuff);
						debuff.transform.parent = Hitting[i].transform;
						debuff.transform.localPosition = Vector3.zero;
						debuff.GetComponent<Wet_Controller>().Duration = DebuffDuration;
					}
				}

                if (Hitting[i].transform.GetComponent<Rigidbody>())
                    Hitting[i].transform.GetComponent<Rigidbody>().velocity += (transform.forward.normalized * PushBackForce);
            }
        }

        if (TickTime >= DamageTickRate)
            TickTime = 0.0f;

		Vector3 rayOrigin = PlayerFaceingIndicator.transform.position;
		rayOrigin.y = StartHeight;
		Ray wallRangeCheck = new Ray(rayOrigin, transform.forward);
		
		RaycastHit RayInfo;
		Physics.Raycast(wallRangeCheck, out RayInfo, Range, WALLS);
		if (smallestSize > RayInfo.distance && RayInfo.distance != 0.0f)
			smallestSize = RayInfo.distance;

        Size = smallestSize;
		if (Size > Range || Size <= 0.0f)
            Size = Range;
		if (Size < Range && !ImpactEffectObj.activeSelf)
			ImpactEffectObj.SetActive (true);
		else if (Size == Range)
			ImpactEffectObj.SetActive (false);
	}
	
	void FixedUpdate ()
    {
		if (oldSize > Size)
        	TheCollider.transform.localScale = new Vector3(1.0f, 1.0f, Size);
			
        transform.forward = PlayerFaceingIndicator.transform.forward;
        Vector3 newPosition = PlayerFaceingIndicator.transform.position;
        newPosition.y = StartHeight;
        Vector3 posChange = transform.forward.normalized * Size * 0.5f;
        newPosition += posChange;
        transform.position = newPosition;

		if (oldSize <= Size)
        	TheCollider.transform.localScale = new Vector3(1.0f, 1.0f, Size);

		oldSize = Size;

        if (Once)
        {
            Once = false;
            SpellEffect.SetActive(true);
			SpellEffect.GetComponent<ParticleSystem>().Play();
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
