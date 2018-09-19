using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PDX.Network
{
	public class RoomsController : MonoBehaviour {

		[SerializeField]
		private GameObject roomObjectPrefab;
		[SerializeField]
		private List<RoomListing> roomListingGroup;

		private void Start() {
			StartCoroutine(UpdateRooms());
		}
		IEnumerator UpdateRooms()
		{
			yield return new WaitForSeconds(1);
			OnRecievedRoomListUpdate();
		}

		// Updates the room list
		private void OnRecievedRoomListUpdate()
		{
			RoomInfo[] rooms = PhotonNetwork.GetRoomList();
			foreach(RoomInfo room in rooms)
			{

				RoomRecieved(room);

			}
			RemoveOldRooms();
		}

		private void RoomRecieved(RoomInfo Room)
		{
			// Runs through the list and checks wether a room with the same info is found in the list
			int index = roomListingGroup.FindIndex(x => x.roomName == Room.Name);

			if(index == -1)
			{
				if(Room.IsVisible && Room.PlayerCount < Room.MaxPlayers)
				{
					GameObject roomObject = Instantiate(roomObjectPrefab);
					roomObject.transform.SetParent(transform, false);
					
					RoomListing roomListing = roomObject.GetComponent<RoomListing>();
					roomListing.SetRoomName(Room.Name);
					roomListingGroup.Add(roomListing);

					index = (roomListingGroup.Count - 1);
				}
			}

			if(index != -1)
			{
				RoomListing roomListing = roomListingGroup[index];
				roomListing.SetRoomName(Room.Name);
				roomListing.Updated = true;
			}
		}

		private void RemoveOldRooms()
		{
			List<RoomListing> removeRooms = new List<RoomListing>();
			
			foreach(RoomListing room in roomListingGroup)
			{

				if(!room.Updated)
				{
					removeRooms.Add(room);
				}
				else
				{
					room.Updated = false;
				}

			}

			foreach(RoomListing room in removeRooms)
			{
				GameObject roomListingObject = room.gameObject;
				roomListingGroup.Remove(room);
				Destroy(roomListingObject);
			}
		}
	}
}