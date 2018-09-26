using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;

namespace PDX.Network
{
    public class NetworkManager : PunBehaviour, IPunTurnManagerCallbacks
    {
        #region VARIABLES

        public static NetworkManager Instance;
        [Tooltip("The Turn Manager that will be used throughout the game")]
        public PunTurnManager TurnManager;

        public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players and so new room will be created")]
        public byte MaxPlayersPerRoom = 2;
        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        public GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;
        [Tooltip("The UI Panel to join or create rooms")]
        public GameObject lobbyPanel;
        
        [Header("Room Options and Settings")]
        [SerializeField]
        Text NewRoomName;
        [SerializeField]
        string _gameVersion = "1";

       [Header("Turning Information")]
        public Text TurnIndicator;
        public PlayerControls LocalPlayer;
        public PlayerControls RemotePlayer;
        public GameObject LocalPlayerObject;
        public int CurrentTurnCount;
        public int FinalTurnCount;
        private bool HasTriggeredEnd = false;
        #endregion

        #region MONOBEHAVIOR_CALLBACKS       
        private void Awake()
        {
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.logLevel = Loglevel;
            Instance = this;
            this.TurnManager = this.gameObject.AddComponent<PunTurnManager>();
            this.TurnManager.TurnManagerListener = this;
            this.TurnManager.TurnDuration = 500f;
        }

        private void Update() 
        {
            if(Instance != this)
            {
                Destroy(this);
            }

            if ( ! PhotonNetwork.inRoom)
            {
                return;
            }
            // Checks wether players are in the room and not just one
            if(PhotonNetwork.room.PlayerCount > 1)
            {
                CurrentTurnCount = this.TurnManager.Turn;

                // Checks and does final commands for if the Current Session is Done
                if(this.TurnManager.Turn == FinalTurnCount)
                {
                    // Display End Screen and Calculate Winner Here
                    if(!HasTriggeredEnd)
                        FinishGame();
                    return;
                }
                else
                {
                    // Sets the text indicator on which player's turn it is
                    if(CurrentTurnCount == LocalPlayer.PlayerID)
                    {
                        TurnIndicator.text = "It's Your Turn!";
                        LocalPlayer.IsCurrentlyTurning = true;
                    }
                    else
                    {
                        TurnIndicator.text = "It's " + 
                        PhotonNetwork.player.GetNext().NickName + "'s Turn";
                        LocalPlayer.IsCurrentlyTurning = false;

                    }
                }

                CurrentTurnCount = this.TurnManager.Turn;
                

            }            
        }
        #endregion

        #region PUBLIC_METHODS     
        
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public void InstantianePlayer()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
            }
            else
            {
                Debug.Log("We are Instantiating LocalPlayer from "+Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
                LocalPlayerObject = player;
            }
        }
        
        public void ConnectToPhoton()
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are , else we initiate the connection to the server.         
            if (PhotonNetwork.connected)
            {             
                Debug.Log("Connected To Photon");
            }
            else
            {
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        public void CreateNewRoom()
        {
            RoomOptions roomOptions = new RoomOptions(){ IsVisible = true, IsOpen = true, MaxPlayers = 2 };
            if(PhotonNetwork.CreateRoom(NewRoomName.text, roomOptions, TypedLobby.Default))
            {
                Debug.Log("Currently Creating Room");
            }   
            else
            {
                Debug.Log("Creating Room Failed at startup");
            }     
        }

        public void JoinRoom(string roomName)
        {
            if(PhotonNetwork.JoinRoom(roomName))
            {
                Debug.Log("Joining Room " + roomName);


            }
            else
            {
                Debug.Log("Failed to try joining Room");
            }
        }

        public void MakeTurn()
        {
            if(CurrentTurnCount == 0)
            {
                StartTurn();
            }
            else if(CurrentTurnCount == 1) // Turn will be 2nd player
            {
                NextTurn();
            }
            else
            {
                this.TurnManager.BeginTurn();
            }
        }

        public void StartTurn()
        {
            this.TurnManager.BeginTurn();
            
        }

        public void NextTurn()
        {
            this.TurnManager.BeginTurn();

        }

        public void FinishGame()
        {
            HasTriggeredEnd = true;
            // Activate the Win Lose and check wether the player can do stuff
           if(DidLocalPlayerWin())
           {
                CanvasManager.instance.ActivatePanel(CanvasManager.instance.WinScreen);

               // Activate Multiplayer Win 
                CanvasManager.instance.SecondaryTextByPanel(CanvasManager.instance.WinScreen).text = "Reset Game";
                CanvasManager.instance.ActiveButtonByPanel(CanvasManager.instance.WinScreen, 0).onClick.AddListener(ResetGame);
           }
           else
           {
               CanvasManager.instance.ActivatePanel(CanvasManager.instance.LoseScreen);

               // Activate Lose UI
                CanvasManager.instance.SecondaryTextByPanel(CanvasManager.instance.LoseScreen).text = "Reset Game";
                CanvasManager.instance.ActiveButtonByPanel(CanvasManager.instance.LoseScreen, 0).onClick.AddListener(ResetGame);
           }
        }

        void ResetGame()
        {

            GameController.instance.ResetGame();
            HasTriggeredEnd = false;
            TurnExtensions.SetTurn(PhotonNetwork.room, 0, false);
            CanvasManager.instance.DisableAllPanels();
            PhotonNetwork.Disconnect();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
        #endregion

        #region PRIVATE_METHODS 

        bool DidLocalPlayerWin()
        {
           if(LocalPlayer.CurrentScore <= GameController.instance.targetValue/10f)
           {
               if(LocalPlayer.CurrentScore > RemotePlayer.CurrentScore)
               {
                   return true;
               }
               else
                    return false;
           }
           else
            return false;
        }

        #endregion

        #region Photon.PunBehaviour CallBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
            PhotonNetwork.JoinLobby(TypedLobby.Default);
            progressLabel.SetActive(false);
            lobbyPanel.SetActive(true);

        }


        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            lobbyPanel.SetActive(false);
            InstantianePlayer();
            
            // Play The Game here 
            if (PhotonNetwork.room.PlayerCount == 2)
            {


                if (this.TurnManager.Turn == 0)
                {
                    this.MakeTurn();
                    
                }
                
                
            }
            else
            {
                Debug.Log("Waiting for another player");

            }
            
        }

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Debug.Log("Other player arrived");

            if (PhotonNetwork.room.PlayerCount == 2)
            {
                if (this.TurnManager.Turn == 0)
                {
                    // when the room has two players, start the first turn (later on, joining players won't trigger a turn)
                    this.MakeTurn();
                }
            }
        }
        
        public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
        {
            Debug.Log("Creating Room Failed" + codeAndMsg[1]);        
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Succesfully Created Room");
        }

        #endregion

        #region TURNMANAGER_CALLBACKS

        /// <summary>Called when a turn begins (Master Client set a new Turn number).</summary>
        public void OnTurnBegins(int turn)
        {
            Debug.Log("OnTurnBegins() turn: "+ turn);

        }


        public void OnTurnCompleted(int obj)
        {
            Debug.Log("OnTurnCompleted: " + obj);

        }


        // when a player moved (but did not finish the turn)
        public void OnPlayerMove(PhotonPlayer photonPlayer, int turn, object move)
        {
            Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);

        }


        // when a player made the last/final move in a turn
        public void OnPlayerFinished(PhotonPlayer photonPlayer, int turn, object move)
        {
            Debug.Log("OnTurnFinished: " + photonPlayer + " turn: " + turn + " action: " + move);

            if (photonPlayer.IsLocal)
            {

            }
            else
            {

            }
        }



        public void OnTurnTimeEnds(int obj)
        {

            OnTurnCompleted(-1);

        }

        private void UpdateScores()
        {

            PhotonNetwork.player.AddScore(1);   // this is an extension method for PhotonPlayer. you can see it's implementation

        }

        #endregion
    }
}