using UnityEngine;
using System.Collections;

public class SpellMasterList : MonoBehaviour 
{
	// attach this to GameBrain to always get a single reference to each spell
	public GameObject barrier;
	public GameObject bolt;
	public GameObject boltChain;
	public GameObject concrete;
	public GameObject crystalSpikes;
	public GameObject explosion;
	public GameObject fireBall;
	public GameObject fog;
	public GameObject freeze;
	public GameObject gravityWell;
	public GameObject hydrant;
	public GameObject laser;
	public GameObject magma;
	public GameObject meteor;
	public GameObject muck;
	public GameObject plasma;
	public GameObject poisonCloud;
	public GameObject rockSpike;
	public GameObject sandBlast;
	public GameObject shockPrism;
	public GameObject steam;
	public GameObject tidalWave;
	public GameObject torch;
	public GameObject whirlwind;
	public GameObject windBlade;

	public GameObject GetSpell(string _name)
	{
		switch (_name)
		{
		case "Barrier":
			return barrier;
		case "Bolt":
			return bolt;
		case "Bolt Chain":
			return boltChain;
		case "Concrete":
			return concrete;
		case "Crystal Spikes":
			return crystalSpikes;
		case "Explosion":
			return explosion;
		case "Fire Ball":
			return fireBall;
		case "Fog":
			return fog;
		case "Freeze":
			return freeze;
		case "Gravity Well":
			return gravityWell;
		case "Hydrant":
			return hydrant;
		case "Laser":
			return laser;
		case "Magma":
			return magma;
		case "Meteor":
			return meteor;
		case "Muck":
			return muck;
		case "Plasma":
			return plasma;
		case "Poison Cloud":
			return poisonCloud;
		case "Rock Spike":
			return rockSpike;
		case "Sand Blast":
			return sandBlast;
		case "Shock Prism":
			return shockPrism;
		case "Steam":
			return steam;
		case "Tidal Wave":
			return tidalWave;
		case "Torch":
			return torch;
		case "Whirlwind":
			return whirlwind;
		case "Wind Blade":
			return windBlade;
		default:
			Debug.LogError("SpellMasterList Is returning nothing!!!!!! - Check passed in name or code.");
			GameObject nothing = new GameObject();
			return nothing;
		}
	}
}
