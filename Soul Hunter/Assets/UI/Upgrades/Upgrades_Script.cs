using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Upgrades_Script : MonoBehaviour 
{
	public GameObject UpgradeMenu = null;
	private GameBrain GB;

	int[] LevelCost = {0,1000,2000,3000};

	//Upgrade Buttons
	public Button FireUpgrade;
	public Button WindUpgrade;
	public Button EarthUpgrade;
	public Button ElectricUpgrade;
	public Button WaterUpgrade;

	//Button Images
	public Image FireImage;
	public Image WindImage;
	public Image EarthImage;
	public Image ElectricImage;
	public Image WaterImage;

	//Button text
	public Text FireText;
	public Text WindText;
	public Text EarthText;
	public Text ElectricText;
	public Text WaterText;

	//Description Text
	public Text FireCost;
	public Text WindCost;
	public Text EarthCost;
	public Text ElectricCost;
	public Text WaterCost;

	//Helper variales
	private byte[] ELVs = new byte[3];

	// Use this for initialization
	void Start () 
	{
		ELVs[0] = 128;
		ELVs[1] = 64;
		ELVs[2] = 0;
		//currentSouls = GB.SoulCount;
		//FireLevel = GB.gameInfo.FireLevel;
		//WindLevel = GB.gameInfo.WindLevel;
		//EarthLevel = GB.gameInfo.EarthLevel;
		//ElectricLevel = GB.gameInfo.ElectricLevel;
		//WaterLevel = GB.gameInfo.WaterLevel;

		GB = GameBrain.Instance;

		CheckLevelAvailability ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void CheckLevelAvailability()
	{
		if (GB.FireLevel == 3 || GB.SoulCount < LevelCost [GB.FireLevel + 1])
		{
			FireUpgrade.interactable = false;
		}
		else
		{
			FireUpgrade.interactable = true;
		}			

		if (GB.WindLevel == 3 || GB.SoulCount < LevelCost [GB.WindLevel + 1])
		{
			WindUpgrade.interactable = false;
		}
		else
		{
			WindUpgrade.interactable = true;
		}
			

		if (GB.EarthLevel == 3 || GB.SoulCount < LevelCost [GB.EarthLevel + 1])
		{
			EarthUpgrade.interactable = false;
		}
		else
		{
			EarthUpgrade.interactable = true;
		}

		if (GB.ElectricLevel == 3 || GB.SoulCount < LevelCost [GB.ElectricLevel + 1]) 
		{
			ElectricUpgrade.interactable = false;
		}
		else
		{
			ElectricUpgrade.interactable = true;
		}

		if (GB.WaterLevel == 3 || GB.SoulCount < LevelCost [GB.WaterLevel + 1]) 
		{
			WaterUpgrade.interactable = false;
		}
		else
		{
			WaterUpgrade.interactable = true;
		}
			

		FireText.text = "Upgrade Fire (" + GB.FireLevel.ToString() + "/3)";
		WindText.text = "Upgrade Wind (" + GB.WindLevel.ToString() + "/3)";
		EarthText.text = "Upgrade Earth (" + GB.EarthLevel.ToString() + "/3)";
		ElectricText.text = "Upgrade Electric (" + GB.ElectricLevel.ToString() + "/3)";
		WaterText.text = "Upgrade Water (" + GB.WaterLevel.ToString() + "/3)";

		if (GB.FireLevel < 3)
			FireCost.text = "Fire Upgrade Cost: " + LevelCost [GB.FireLevel + 1];
		else
			FireCost.text = "Fire Upgrade Cost: Fully Upgraded";

		if (GB.WindLevel < 3)
			WindCost.text = "Wind Upgrade Cost: " + LevelCost [GB.WindLevel + 1];
		else
			WindCost.text = "Wind Upgrade Cost: Fully Upgraded";

		if (GB.EarthLevel < 3)
			EarthCost.text = "Earth Upgrade Cost: " + LevelCost [GB.EarthLevel + 1];
		else
			EarthCost.text = "Earth Upgrade Cost: Fully Upgraded";

		if (GB.ElectricLevel < 3)
			ElectricCost.text = "Electric Upgrade Cost: " + LevelCost [GB.ElectricLevel + 1];
		else
			ElectricCost.text = "Electric Upgrade Cost: Fully Upgraded";

		if (GB.WaterLevel < 3)
			WaterCost.text = "Water Upgrade Cost: " + LevelCost [GB.WaterLevel + 1];
		else
			WaterCost.text = "Water Upgrade Cost: Fully Upgraded";
			
		UpdateColors ();
	}

	public void UpgradeFire()
	{
		GB.SoulCount -= LevelCost [GB.FireLevel + 1];
		//GB.GetComponent<StatsDisplay> ().updateSoul = true;
		GB.FireLevel++;
		CheckLevelAvailability ();
		GB.ChangeSoulHud();
	}

	public void UpgradeWind()
	{
		GB.SoulCount -= LevelCost [GB.WindLevel + 1];
		GB.WindLevel++;
		CheckLevelAvailability ();
		GB.ChangeSoulHud();
	}

	public void UpgradeEarth()
	{
		GB.SoulCount -= LevelCost [GB.EarthLevel + 1];
		GB.EarthLevel++;
		CheckLevelAvailability ();
		GB.ChangeSoulHud();
	}

	public void UpgradeElectric()
	{
		GB.SoulCount -= LevelCost [GB.ElectricLevel + 1];
		GB.ElectricLevel++;
		CheckLevelAvailability ();
		GB.ChangeSoulHud();
	}

	public void UpgradeWater()
	{
		GB.SoulCount -= LevelCost [GB.WaterLevel + 1];
		GB.WaterLevel++;
		CheckLevelAvailability ();
		GB.ChangeSoulHud();
	}

	public void UpdateColors()
	{
// Fire
		if (GB.FireLevel > 0)
			FireImage.color = new Color32(255, ELVs[GB.FireLevel - 1], ELVs[GB.FireLevel - 1], 255);
		else
			FireImage.color = new Color32 (255, 255, 255, 255);
// Wind
		byte R = 255;
		byte G = 255;
		byte B = 255;
		if (GB.WindLevel > 0) {
			
			switch (GB.WindLevel) {
			case 1:
				{
					R = G = B = 192;
					break;
				}
			case 2:
				{
					R = G = B = 128;
					break;
				}
			case 3:
				{
					R = G = B = 64;
					break;
				}
			default:
				break;
			}
		}
		WindImage.color = new Color32 (R, G, B, 255);

//Earth
		R = 255;
		G = 255;
		B = 255;
		
		if (GB.EarthLevel > 0)
		{
			switch (GB.EarthLevel)
			{
			case 1:
				{
					R = 160;
					G = 125;
					B = 65;
					break;
				}
			case 2:
				{
					R = 180;
					G = 130;
					B = 50;
					break;
				}
			case 3:
				{
					R = 170;
					G = 100;
					B = 0;
					break;
				}
			default:
				break;
			}
		}
		EarthImage.color = new Color32 (R, G, B, 255);

// Electric
		if (GB.ElectricLevel > 0)
			ElectricImage.color = new Color32(255, 255, ELVs[GB.ElectricLevel - 1], 255);
		else
			ElectricImage.color = new Color32 (255, 255, 255, 255);
// Water
		if (GB.WaterLevel > 0)
			WaterImage.color = new Color32(ELVs[GB.WaterLevel - 1], ELVs[GB.WaterLevel - 1], 255, 255);
		else
			WaterImage.color = new Color32 (255, 255, 255, 255);
	}

	public void Close()
	{
		UpgradeMenu.SetActive (false);
	}


}
