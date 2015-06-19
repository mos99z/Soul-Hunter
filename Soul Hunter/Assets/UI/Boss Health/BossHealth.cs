using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
	// Boss's health stats
	public int curBossHealth;
	public int maxBossHealth;
	
	//Image variables
	private float cachedY;
	private float minX;
	private float maxX;
	
	//UI images
	public RectTransform healthTransform;
	public Text bossHealthText;
	public Text bossNameTxt;
	public Image bossHealthImg;
	public GameObject bossObj;
	
	//Helper Variables
	private bool updateHP;
	private bool firstRun;
	
	// Use this for initialization
	void Start ()
	{
		updateHP = true;
		bossObj.SetActive(false);
		firstRun = true;
		
		cachedY = healthTransform.position.y;
		maxX = healthTransform.position.x;
		minX = healthTransform.position.x - healthTransform.rect.width;
		
		bossHealthImg.color = new Color ((float)1.0, (float)0.0, (float)0.0, (float)0.75);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (updateHP)
		{
			HandleHpBar();
		}
	}
	
	private void HandleHpBar()
	{
		updateHP = false;
		bossHealthText.text = curBossHealth + "/" + maxBossHealth;
		float currY = (curBossHealth - 0) * (maxX - minX) / (maxBossHealth - 0) + minX;
		healthTransform.position = new Vector3 (currY, cachedY);
		
		float damage = maxBossHealth - curBossHealth;
		float percent = damage / maxBossHealth * (float)0.4;
		bossHealthImg.color = new Color ((float)1.0 - percent * 1.75f, (float)0.0, (float)0.0, (float)0.75);
	}
	
	public void SetBossHealthDisplay(int _health)
	{
		curBossHealth = _health;
		if (firstRun)
		{
			maxBossHealth = _health;
			firstRun = false;
		}
		updateHP = true;
	}

	public void ActivateBossBar()
	{
		bossObj.SetActive(true);
	}

	public void DeactivateBossBar()
	{
		bossObj.SetActive(false);
	}

	public void SetBossName(string _name)
	{
		bossNameTxt.text = _name;
	}
}
