using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour 
{
	#region PUBLIC_VARIABLES
	public static CanvasManager instance;
	public PanelController StartScreen;
	public PanelController ModeScreen;
	public PanelController SinglePlayerUI;
	public PanelController VSUI;
	public PanelController WinScreen;
	public PanelController LoseScreen;
	public PanelController NextPlayerScreen;
	public bool hasSelectedMode = false;
	public List<PanelController> PanelControllers;

	#endregion

	#region UNITY_CALLBACKS

	private void Start()
	{
		instance = this;
		PanelControllers = GroupPanels();
		DisableAllPanels();
		ActivatePanel(StartScreen);

	}

	private void Update() 
	{
		if(GameController.instance.isTracking && !hasSelectedMode)
		{
			ActivateModeScreen();
		}
	}

	#endregion

	#region PUBLIC_VARIABLES

	public void DisableAllPanels()
	{
				
		foreach(PanelController a in PanelControllers)
		{
			a.Panel.SetActive(false);
		}

	}

	public void ActivatePanel(PanelController panelToActivate)
	{
		foreach(PanelController p in PanelControllers)
		{
			PanelController activeController = p;

			if(activeController.IsCurrentlyActive)
				activeController.IsCurrentlyActive = false;
			
			activeController.Panel.SetActive(false);
			if(activeController.ActionButton != null)
			{
				activeController.ActionButton.onClick.RemoveAllListeners();
			}
			if(activeController.ActionButton2 != null)
			{
				activeController.ActionButton2.onClick.RemoveAllListeners();
			}
		}
		panelToActivate.IsCurrentlyActive = true;
		panelToActivate.Panel.SetActive(true);
	}

	public void ActivatePanel(PanelController panelToActivate, PanelMode panelMode)
	{
		switch(panelMode)
		{
			case PanelMode.SINGLE:
				ActivatePanel(panelToActivate);
				break;
			case PanelMode.MULTIPLE:
				panelToActivate.Panel.SetActive(true);
				break;
		}
	}

	public Button ActiveButtonByPanel(PanelController panelTarget, int id)
	{
		if(id == 0)
		{
			return panelTarget.ActionButton;
		}
		else
		{
			return panelTarget.ActionButton2;
		}
	}

	public Text PrimaryTextByPanel(PanelController panelTarget)
	{
		
		return panelTarget.PrimaryText;
	}

	public Text SecondaryTextByPanel(PanelController panelTarget)
	{
		return panelTarget.SecondaryText;
	}

	#endregion

	#region PRIVATE_VARIABLES

	public void ActivateModeScreen()
	{
		hasSelectedMode = true;
		ActivatePanel(ModeScreen);
		Button A = ActiveButtonByPanel(ModeScreen, 0);
		Button B = ActiveButtonByPanel(ModeScreen, 1);
		A.onClick.AddListener(ActivateSingleplayerUI);
		B.onClick.AddListener(ActivateVSUI);

	}

	public void ActivateSingleplayerUI()
	{
		GameController.instance.isGamePlaying = true;

		ActivatePanel(SinglePlayerUI);

		

	}

	public void ActivateVSUI()
	{
		GameController.instance.isGamePlaying = true;
		ActivatePanel(VSUI);
		
	}

	private void ActivateWinScreen()
	{

	}

	private void ActivateLoseScreen()
	{

	}

	private void ActivateWinScreenForMultiplayer()
	{

	}

	private void ActivateLoseScreenForMultiplayer()
	{
		
	}

	private void ActivateNextPlayerScreen()
	{

	}

	private List<PanelController> GroupPanels()
	{
		List<PanelController> nPanelControllers = new List<PanelController>();

		nPanelControllers.Add(StartScreen);
		nPanelControllers.Add(ModeScreen);
		nPanelControllers.Add(SinglePlayerUI);
		nPanelControllers.Add(VSUI);
		nPanelControllers.Add(WinScreen);		
		nPanelControllers.Add(LoseScreen);	
		nPanelControllers.Add(NextPlayerScreen);	

		return nPanelControllers;	
				
	}

	#endregion

	#region UTILITY
	public enum PanelMode
	{
		SINGLE,
		MULTIPLE
	}
	#endregion

}

[Serializable]
public struct PanelController
{
	public GameObject Panel;
	public Text PrimaryText;
	public Text SecondaryText;
	public Button ActionButton;
	public Button ActionButton2;
	public bool IsCurrentlyActive{get;set;}

}	
