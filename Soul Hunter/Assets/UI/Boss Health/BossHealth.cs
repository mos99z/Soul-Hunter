using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
	// captain's health stats
	public int curBossHealth;
	public int maxBossHealth;
	
	//Image variables
	private float cachedY;
	private float minX;
	private float maxX;
	
	//UI images
	public RectTransform healthTransform;
	public Text bossHealthText;
	public Image bossHealthImg;
	public GameObject bossObj;
	
	//Helper Variables
	private bool updateHP;
	
	//Render Variable
	private bool displayBossBar;
	
	// Use this for initialization
	void Start ()
	{
		updateHP = true;
		displayBossBar = false;
		
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
		bossObj.SetActive(displayBossBar);
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
	
	public void SetCaptainHealthDisplay(int _health)
	{
		curBossHealth = _health;
		updateHP = true;
	}
}
