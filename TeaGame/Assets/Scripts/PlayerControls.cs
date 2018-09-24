using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using PDX.Network;

public class PlayerControls : PunBehaviour, IPunObservable 
{
	[Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
	public static GameObject LocalPlayerInstance;
	public float CurrentScore{get{return Score;}}
	public int PlayerID;
    public bool IsCurrentlyTurning;
	public GameObject HUD;
	public Text ActionButtonText;
	private bool CanPour;
	private int inputCount = 0;
	[SerializeField]
	private float Score = 0;

	public float TeaLevel;
	public bool IsPlayerPouring;

	[Header("Flow Variables")]
	public float currentSwitchTime;
    public float switchTargetTime;
    public float maximumRange;
	public float pourMultiplier;

	#region UNITY_CALLBACKS

	// Use this for initialization
	void Start ()
	{
		// #Important
		// used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
		if ( photonView.isMine)
		{
			LocalPlayerInstance = this.gameObject;
            NetworkManager.Instance.LocalPlayer = this;
		}

		// #Critical
		// we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
		DontDestroyOnLoad(this.gameObject);

		if(this != NetworkManager.Instance.LocalPlayer)
		{
			NetworkManager.Instance.RemotePlayer = this;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{

		if(photonView.isMine && PhotonNetwork.connected)
		{
			PlayerID = PhotonNetwork.player.ID;

			if(IsCurrentlyTurning)
			{
				if(!HUD.GetActive())
					HUD.SetActive(true);
				


				switch(inputCount)
				{
					case 0:
						ActionButtonText.text = "Pour";
						break;
					case 1:
						ActionButtonText.text = "Stop Pouring";
						break;
					default:
						ActionButtonText.text = "Continue";
						break;
				}

				CanPour = true;

			}
			else
			{
				HUD.SetActive(false);
				CanPour = false;


			}
			


		}
		if(IsPlayerPouring)
		{
			UpdatePourMultiplier();
			TeaLevel += Time.deltaTime * pourMultiplier;

		}
	}

	#endregion

	#region PUBLIC_METHODS

	public void PourFlask()
	{
		if(GameController.instance.isTracking == true)
		{
			if(!IsPlayerPouring)
			{

				IsPlayerPouring = true;	
			}
			else
			{
				
				IsPlayerPouring = false;
			}
		}
	}

	public void Pour()
	{
		if(CanPour){
			
			if(inputCount < 2)
			{
				inputCount++;	
				PourFlask();

			}
			else
			{
				this.Score = TeaLevel;
				GameController.instance.ResetGame();
				NetworkManager.Instance.MakeTurn();
			}	
		}
	}
	#endregion

	private void UpdatePourMultiplier()
	{
		currentSwitchTime += Time.deltaTime;
		if(currentSwitchTime >= switchTargetTime)
		{
			float random = Random.Range(0,maximumRange);
			pourMultiplier = random;
			currentSwitchTime = 0;
		}
	}

	/// <summary>
	/// Sends the streamed data.
	/// </summary>
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo msg)
	{
		if(stream.isWriting)
		{
			stream.SendNext(PlayerID);
            stream.SendNext(IsCurrentlyTurning);
			stream.SendNext(Score);
			stream.SendNext(TeaLevel);
			stream.SendNext(IsPlayerPouring);
			stream.SendNext(currentSwitchTime);
			stream.SendNext(pourMultiplier);
			
		}
		else
		{
			this.PlayerID = (int)stream.ReceiveNext();
            this.IsCurrentlyTurning = (bool)stream.ReceiveNext();
			this.Score = (float)stream.ReceiveNext();
			this.TeaLevel = (float)stream.ReceiveNext();
			this.IsPlayerPouring = (bool)stream.ReceiveNext();
			this.currentSwitchTime = (float)stream.ReceiveNext();
			this.pourMultiplier = (float)stream.ReceiveNext();
		}
	}
}
