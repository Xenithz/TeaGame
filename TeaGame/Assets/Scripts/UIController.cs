	using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
	public GameObject resultPanel;

	public Text resultText;

	public static UIController instance;

	private void Start()
	{
		instance = this;
		resultPanel.SetActive(false);
		resultText.text = "";
	}
	
	private void Update()
	{
		if(instance != this)
		{
			Destroy(this);
		}
	}

	public void EnableResultPanel()
	{
		resultPanel.SetActive(true);
	}

	public void WinResult()
	{
		resultText.text = "You win!";
	}
	
	public void LoseResult()
	{
		resultText.text = "You lose!";
	}

	public void DisableResultPanel()
	{
		resultPanel.SetActive(false);
		resultText.text = "";
	}
}
