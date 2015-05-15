using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour
{
	//Health variables
	public int curHealth;
	public int maxHealth;

	//Soul count
	public uint numSouls;

	//Live count
	public uint numLives;

	//Image variables
	private float cachedX;
	private float minY;
	private float maxY;

	//UI images
	public RectTransform healthTransform;
	public Text healthText;
	public Text soulText;
	public Text liveText;
	public Image healthImg;

	//Helper Variables
	private bool updateHP;
	private bool updateSoul;
	private bool updateLive;

	// Use this for initialization
	void Start ()
	{
		updateHP = true;
		updateSoul = true;
		updateLive = true;

		cachedX = healthTransform.position.x;
		maxY = healthTransform.position.y;
		minY = healthTransform.position.y - healthTransform.rect.height;

		healthImg.color = new Color ((float)1.0, (float)0.0, (float)0.0, (float)0.75);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if (updateHP)
		{
			HandleHpBar ();
		}
		if (updateSoul)
		{
			HandleSouls ();
		}
		if (updateLive)
		{
			HandleLives ();
		}
	}

	private void HandleHpBar()
	{
		updateHP = false;
		healthText.text = curHealth + "/" + maxHealth;
		float currY = (curHealth - 0) * (maxY - minY) / (maxHealth - 0) + minY;
		healthTransform.position = new Vector3 (cachedX, currY);

		float damage = maxHealth - curHealth;
		float percent = damage / maxHealth * (float)0.4;
		healthImg.color = new Color ((float)1.0 - percent, (float)0.0, (float)0.0, (float)0.75);
	}

	private void HandleSouls()
	{
		updateSoul = false;
		soulText.text = numSouls.ToString();
	}

	private void HandleLives ()
	{
		updateLive = false;
		liveText.text = numLives.ToString();
	}

	//messages
	public void SetHealthDisplay(int _health)
	{
		curHealth = _health;
		updateHP = true;
	}
	
	public void SetMaxHealthDisplay(int _health)
	{
		maxHealth = _health;
		updateHP = true;
	}

	public void SetSoulsDisplay(uint _souls)
	{
		numSouls = _souls;
		if (numSouls < 0)
			numSouls = 0;
		updateSoul = true;
	}

	public void SetLivesDisplay(uint _lives)
	{
		numLives = _lives;
		if (numLives < 0)
			numLives = 0;
		updateLive = true;
	}
}
