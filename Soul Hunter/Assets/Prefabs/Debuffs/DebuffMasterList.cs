using UnityEngine;
using System.Collections;

public class DebuffMasterList : MonoBehaviour 
{
	// This script is only to hold an instance of each spell
	// attach directly to GameBrain to always have access to a reference of each debuff

	public GameObject blinded;
	public GameObject burning;
	public GameObject crippled;
	public GameObject frozen;
	public GameObject stunned;
	public GameObject wet;
}
