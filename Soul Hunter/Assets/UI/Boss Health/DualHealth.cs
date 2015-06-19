using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DualHealth : MonoBehaviour
{
	// Boss's health stats
	public int curBossHealth1;
	public int maxBossHealth1;
	
	//Image variables
	private float cachedY1;
	private float minX1;
	private float maxX1;
	
	//UI images
	public RectTransform healthTransform1;
	public Text bossHealthText1;
	public Text bossNameTxt1;
	public Image bossHealthImg1;
	
	//Helper Variables
	private bool updateHP1;
	private bool firstRun1;

	// Boss's health stats
	public int curBossHealth2;
	public int maxBossHealth2;
	
	//Image variables
	private float cachedY2;
	private float minX2;
	private float maxX2;
	
	//UI images
	public RectTransform healthTransform2;
	public Text bossHealthText2;
	public Text bossNameTxt2;
	public Image bossHealthImg2;
	
	//Helper Variables
	private bool updateHP2;
	private bool firstRun2;

	//Entire Image
	public GameObject bossObj;
	
	// Use this for initialization
	void Start ()
	{
		bossObj.SetActive(false);

		updateHP1 = true;
		firstRun1 = true;

		updateHP2 = true;
		firstRun2 = true;
		
		cachedY1 = healthTransform1.position.y;
		maxX1 = healthTransform1.position.x;
		minX1 = healthTransform1.position.x - healthTransform1.rect.width;

		cachedY2 = healthTransform2.position.y;
		maxX2 = healthTransform2.position.x;
		minX2 = healthTransform2.position.x - healthTransform2.rect.width;
		
		bossHealthImg1.color = new Color ((float)1.0, (float)0.0, (float)0.0, (float)0.75);
		bossHealthImg2.color = new Color ((float)1.0, (float)0.0, (float)0.0, (float)0.75);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (updateHP1)
		{
			HandleHpBar1();
		}
		if (updateHP2)
		{
			HandleHpBar2();
		}
	}
	
	private void HandleHpBar1()
	{
		updateHP1 = false;
		bossHealthText1.text = curBossHealth1 + "/" + maxBossHealth1;
		float currY = (curBossHealth1 - 0) * (maxX1 - minX1) / (maxBossHealth1 - 0) + minX1;
		healthTransform1.position = new Vector3 (currY, cachedY1);
		
		float damage = maxBossHealth1 - curBossHealth1;
		float percent = damage / maxBossHealth1 * (float)0.4;
		bossHealthImg1.color = new Color ((float)1.0 - percent * 1.75f, (float)0.0, (float)0.0, (float)0.75);
	}

	private void HandleHpBar2()
	{
		updateHP2 = false;
		bossHealthText2.text = curBossHealth2 + "/" + maxBossHealth2;
		float currY = (curBossHealth2 - 0) * (maxX2 - minX2) / (maxBossHealth2 - 0) + minX2;
		healthTransform2.position = new Vector3 (currY, cachedY2);
		
		float damage = maxBossHealth2 - curBossHealth2;
		float percent = damage / maxBossHealth2 * (float)0.4;
		bossHealthImg2.color = new Color ((float)1.0 - percent * 1.75f, (float)0.0, (float)0.0, (float)0.75);
	}
	
	public void SetBossHealthDisplay1(int _health)
	{
		curBossHealth1 = _health;
		if (firstRun1)
		{
			maxBossHealth1 = _health;
			firstRun1 = false;
		}
		updateHP1 = true;
	}

	public void SetBossHealthDisplay2(int _health)
	{
		curBossHealth2 = _health;
		if (firstRun2)
		{
			maxBossHealth2 = _health;
			firstRun2 = false;
		}
		updateHP2 = true;
	}
	
	public void ActivateDualBar()
	{
		bossObj.SetActive(true);
	}
	
	public void DeactivateDualBar()
	{
		bossObj.SetActive(false);
	}
	
	public void SetBossName1(string _name)
	{
		bossNameTxt1.text = _name;
	}
	public void SetBossName2(string _name)
	{
		bossNameTxt2.text = _name;
	}
}
