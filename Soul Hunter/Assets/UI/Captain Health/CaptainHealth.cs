using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CaptainHealth : MonoBehaviour
{
	// captain's health stats
	public int curCaptainHealth;
	public int maxCaptainHealth;

	//Image variables
	private float cachedY;
	private float minX;
	private float maxX;

	//UI images
	public RectTransform healthTransform;
	public Text captainHealthText;
	public Image captainHealthImg;
	public GameObject captainObj;

	//Helper Variables
	private bool updateHP;

	//Render Variable
	private bool displayCaptBar;

	// Use this for initialization
	void Start ()
	{
		updateHP = true;
		displayCaptBar = true;

		maxCaptainHealth = curCaptainHealth = 100;

		cachedY = healthTransform.position.y;
		maxX = healthTransform.position.x;
		minX = healthTransform.position.x - healthTransform.rect.width;
		
		captainHealthImg.color = new Color ((float)1.0, (float)0.0, (float)0.0, (float)0.75);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (updateHP)
		{
			HandleHpBar();
		}
		captainObj.SetActive(displayCaptBar);
	}

	private void HandleHpBar()
	{
		updateHP = false;
		captainHealthText.text = curCaptainHealth + "/" + maxCaptainHealth;
		float currX = (curCaptainHealth - 0) * (maxX - minX) / (maxCaptainHealth - 0) + minX;
		healthTransform.position = new Vector3 (currX, cachedY);
		
		float damage = maxCaptainHealth - curCaptainHealth;
		float percent = damage / maxCaptainHealth * (float)0.4;
		captainHealthImg.color = new Color ((float)1.0 - percent * 1.75f, (float)0.0, (float)0.0, (float)0.75);
	}

	public void SetCaptainHealthDisplay(int _health)
	{
		curCaptainHealth = _health;
		updateHP = true;
	}
}
