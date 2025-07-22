using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;

public class UILobbyRoomList : MonoBehaviour
{
	public GameObject originSlot;
	public List<LobbyRoomSlot> slots = new List<LobbyRoomSlot>();
	public Transform holder;
	List<Lobby> lobbys = new List<Lobby>();
	// Start is called before the first frame update
	bool isUpdated = false;
	public GameObject loadingPanel;
    void Start()
    {
		originSlot.SetActive(false);
		Refresh();
	}

    // Update is called once per frame
    void Update()
    {
		if(isUpdated)
		{
			UpdateLobbyUI();
			isUpdated = false;
			SetLoading(false);
		}
    }

	public void SetLoading(bool state)
	{
		loadingPanel.SetActive(state);
	}

	public void Refresh()
	{
		ClearSlots();
		SetLoading(true);
		SearchRoomList();
	}

	async void SearchRoomList()
    {
		// 2. 클라이언트가 방 목록 조회
		QueryLobbiesOptions options = new QueryLobbiesOptions
		{
			Count = 25,
			Filters = new List<QueryFilter>
			{
				new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots, op: QueryFilter.OpOptions.GT, value: "0"),
			}
		};

		QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);
		lobbys = response.Results;
		// UI에 response.Results를 나열하면 방 목록 표시 가능
		foreach (var res in lobbys)
		{
			//Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(res.Id);
			string joinCode = res.Data["joinCode"].Value;

			Debug.Log("RoomList joinCode :" + joinCode);
		}
		isUpdated = true;
	}

	void UpdateLobbyUI()
	{
		if(lobbys == null || lobbys.Count == 0)
		{
			Debug.Log("No lobbies found.");
			return;
		}
		ClearSlots();

		foreach (var lobby in lobbys)
		{
			var slot = Instantiate(originSlot, holder);
			slot.SetActive(true);
			LobbyRoomSlot lobbyRoomSlot = slot.GetComponent<LobbyRoomSlot>();
			lobbyRoomSlot.SetLobbyData(lobby);
			slots.Add(lobbyRoomSlot);
		}

	}

	void ClearSlots()
	{
		foreach (var slot in slots)
		{
			Destroy(slot.gameObject);
		}
		slots.Clear();
	}
}
