using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomManager : MonoBehaviour
{
	public static RoomManager Instance { get; private set; }

	[Header("Room Prefabs")]
	public GameObject startingRoomPrefab;
	public List<GameObject> roomPrefabs;

	[Header("Parent to Organize Spawned Rooms")]
	public Transform roomsParent;

	[Header("Spawn Position for New Rooms")]
	public Transform spawnPoint;

	private GameObject currentRoom;
	private int lastRoomIndex = -1;  // track the last used prefab index

	private void Awake()
	{
		// Singleton pattern
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		SpawnStartingRoom();
	}

	private void SpawnStartingRoom()
	{
		if (startingRoomPrefab == null)
		{
			Debug.LogError("Starting room prefab is not assigned.");
			return;
		}

		currentRoom = Instantiate(startingRoomPrefab, spawnPoint.position, spawnPoint.rotation, roomsParent);
		MovePlayerToEntrance(currentRoom);
	}

	public void SpawnRandomRoom()
	{
		if (roomPrefabs.Count == 0)
		{
			Debug.LogWarning("No room prefabs assigned to RoomManager.");
			return;
		}

		// Optional: Destroy the current room.
		if (currentRoom != null)
		{
			Destroy(currentRoom);
		}

		// Pick a new random room index; if more than one prefab exists, ensure it's not the same as last time.
		int index = Random.Range(0, roomPrefabs.Count);
		if (roomPrefabs.Count > 1)
		{
			while (index == lastRoomIndex)
			{
				index = Random.Range(0, roomPrefabs.Count);
			}
		}
		lastRoomIndex = index;

		GameObject selectedRoom = roomPrefabs[index];
		currentRoom = Instantiate(selectedRoom, spawnPoint.position, spawnPoint.rotation, roomsParent);
		MovePlayerToEntrance(currentRoom);
	}

	private void MovePlayerToEntrance(GameObject room)
	{
		// Try to find an "EntrancePoint" even if it's nested.
		Transform entrance = room.GetComponentsInChildren<Transform>()
								 .FirstOrDefault(t => t.name == "EntrancePoint");

		if (entrance == null)
		{
			Debug.LogError($"EntrancePoint not found in room prefab: {room.name}");
			return;
		}

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			// Disable CharacterController if present (or adjust for other movement scripts)
			CharacterController cc = player.GetComponent<CharacterController>();
			if (cc != null)
			{
				cc.enabled = false;
			}

			player.transform.position = entrance.position;
			player.transform.rotation = entrance.rotation;

			if (cc != null)
			{
				cc.enabled = true;
			}
			Debug.Log($"[RoomManager] Moved player to EntrancePoint at {entrance.position}");
		}
		else
		{
			Debug.LogError("Player not found. Make sure it's tagged 'Player'.");
		}
	}
}
