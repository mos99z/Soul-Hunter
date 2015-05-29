using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Upgrades_Script : MonoBehaviour 
{
	public GameBrain gameBrain = null;
	public GameObject UpgradeMenu = null;

	//int FireLevel;
	//int WindLevel;
	//int EarthLevel;
	//int ElectricLevel;
	//int WaterLevel;

	int[] LevelCost = {0,1000,2000,3000};

	public Button FireUpgrade;
	public Button WindUpgrade;
	public Button EarthUpgrade;
	public Button ElectricUpgrade;
	public Button WaterUpgrade;

	public Text FireText;
	public Text WindText;
	public Text EarthText;
	public Text ElectricText;
	public Text WaterText;

	public Text FireCost;
	public Text WindCost;
	public Text EarthCost;
	public Text ElectricCost;
	public Text WaterCost;

	// Use this for initialization
	void Start () 
	{
		//currentSouls = gameBrain.SoulCount;
		//FireLevel = gameBrain.gameInfo.FireLevel;
		//WindLevel = gameBrain.gameInfo.WindLevel;
		//EarthLevel = gameBrain.gameInfo.EarthLevel;
		//ElectricLevel = gameBrain.gameInfo.ElectricLevel;
		//WaterLevel = gameBrain.gameInfo.WaterLevel;
		
		CheckLevelAvailability ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void CheckLevelAvailability()
	{
		if (gameBrain.FireLevel == 3 || gameBrain.SoulCount < LevelCost [gameBrain.FireLevel + 1]) 
		{
			FireUpgrade.interactable = false;
		}
		else
			FireUpgrade.interactable = true;
			

		if (gameBrain.WindLevel == 3 || gameBrain.SoulCount < LevelCost [gameBrain.WindLevel + 1]) 
		{
			WindUpgrade.interactable = false;
		}
		else
			WindUpgrade.interactable = true;
			

		if (gameBrain.EarthLevel == 3 || gameBrain.SoulCount < LevelCost [gameBrain.EarthLevel + 1]) {
			EarthUpgrade.interactable = false;
		}
		else
			EarthUpgrade.interactable = true;
			

		if (gameBrain.ElectricLevel == 3 || gameBrain.SoulCount < LevelCost [gameBrain.ElectricLevel + 1]) 
		{
			ElectricUpgrade.interactable = false;
		}
		else
			ElectricUpgrade.interactable = true;
			

		if (gameBrain.WaterLevel == 3 || gameBrain.SoulCount < LevelCost [gameBrain.WaterLevel + 1]) 
		{
			WaterUpgrade.interactable = false;
		}
		else
			WaterUpgrade.interactable = true;
			

		FireText.text = "Upgrade Fire (" + gameBrain.FireLevel.ToString() + "/3)";
		WindText.text = "Upgrade Wind (" + gameBrain.WindLevel.ToString() + "/3)";
		EarthText.text = "Upgrade Earth (" + gameBrain.EarthLevel.ToString() + "/3)";
		ElectricText.text = "Upgrade Electric (" + gameBrain.ElectricLevel.ToString() + "/3)";
		WaterText.text = "Upgrade Water (" + gameBrain.WaterLevel.ToString() + "/3)";

		if (gameBrain.FireLevel < 3)
			FireCost.text = "Fire Upgrade Cost: " + LevelCost [gameBrain.FireLevel + 1];
		else
			FireCost.text = "Fire Upgrade Cost: Fully Upgraded";

		if (gameBrain.WindLevel < 3)
			WindCost.text = "Wind Upgrade Cost: " + LevelCost [gameBrain.WindLevel + 1];
		else
			WindCost.text = "Wind Upgrade Cost: Fully Upgraded";

		if (gameBrain.EarthLevel < 3)
			EarthCost.text = "Earth Upgrade Cost: " + LevelCost [gameBrain.EarthLevel + 1];
		else
			EarthCost.text = "Earth Upgrade Cost: Fully Upgraded";

		if (gameBrain.ElectricLevel < 3)
			ElectricCost.text = "Electric Upgrade Cost: " + LevelCost [gameBrain.ElectricLevel + 1];
		else
			ElectricCost.text = "Electric Upgrade Cost: Fully Upgraded";

		if (gameBrain.WaterLevel < 3)
			WaterCost.text = "Water Upgrade Cost: " + LevelCost [gameBrain.WaterLevel + 1];
		else
			WaterCost.text = "Water Upgrade Cost: Fully Upgraded";
			
	}

	public void UpgradeFire()
	{
		gameBrain.SoulCount -= LevelCost [gameBrain.FireLevel + 1];
		//gameBrain.GetComponent<StatsDisplay> ().updateSoul = true;
		gameBrain.FireLevel++;
		CheckLevelAvailability ();
	}

	public void UpgradeWind()
	{
		gameBrain.SoulCount -= LevelCost [gameBrain.WindLevel + 1];
		gameBrain.WindLevel++;
		CheckLevelAvailability ();
	}

	public void UpgradeEarth()
	{
		gameBrain.SoulCount -= LevelCost [gameBrain.EarthLevel + 1];
		gameBrain.EarthLevel++;
		CheckLevelAvailability ();
	}

	public void UpgradeElectric()
	{
		gameBrain.SoulCount -= LevelCost [gameBrain.ElectricLevel + 1];
		gameBrain.ElectricLevel++;
		CheckLevelAvailability ();
	}

	public void UpgradeWater()
	{
		gameBrain.SoulCount -= LevelCost [gameBrain.WaterLevel + 1];
		gameBrain.WaterLevel++;
		CheckLevelAvailability ();
	}

	public void Close()
	{
		UpgradeMenu.SetActive (false);
	}


}
