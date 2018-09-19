using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PDX.Network {
	public class RoomListing : MonoBehaviour {
		
		[SerializeField]
		private Text roomNameText;
		public string roomName = "";
		public bool Updated{ get; set;}

		// Use this for initialization
		void Start () 
		{
			NetworkManager netManager = FindObjectOfType<NetworkManager>();
			GetComponent<Button>().onClick.AddListener(() =>  netManager.JoinRoom(roomName));
		}
		
		private void OnDestroy()
		{
			GetComponent<Button>().onClick.RemoveAllListeners();
		}

		public void SetRoomName(string NewRoomName)
		{
			roomName = NewRoomName;
			roomNameText.text = roomName;
		}
	}
}