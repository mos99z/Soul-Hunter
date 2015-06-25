using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleControl : MonoBehaviour
{
	//title screen stuff
	public GameObject titleScreen;
	public GameObject logos;
	public GameObject PressKey;
	private float blinkTicker = 0;
	private bool hasPressed = false;
	private float animateTicker = 0;
	AsyncOperation ao = null;
	

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (ao == null)
		{
			TitleScreenControl ();
		}
		else
		{
			Debug.Log (ao.progress);
			if (ao.progress == 0.9f) {
				ao.allowSceneActivation = true;
			}
		}
		if (ao != null)
		{
			logos.GetComponent<Image>().color = new Color(1, 1, 1, (1 - (float)ao.progress));
			titleScreen.GetComponent<Image>().color = new Color(1, 1, 1, (1 - (float)ao.progress));
		}
	}

	private void TitleScreenControl()
	{
		blinkTicker += Time.deltaTime;
		if (blinkTicker >= 1)
		{
			blinkTicker = 0;
			if (animateTicker > 0 && animateTicker < 0.5f)
			{
				blinkTicker = 0.99f;
			}
			PressKey.SetActive(!PressKey.activeSelf);
		}
		if (Input.anyKeyDown && !hasPressed)
		{
			hasPressed = true;
			blinkTicker = 0.99f;
		}
		if (hasPressed)
		{
			animateTicker += Time.deltaTime;
			if (animateTicker >= 0.5f)
			{
				PressKey.SetActive(false);
				ao = Application.LoadLevelAsync ("Main menu");
				ao.allowSceneActivation = false;
			}
		}
	}
}
