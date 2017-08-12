using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class GlobalSettings : GlobalSettingsBehavior {

	[System.Serializable]
	public class SpawnLocation
	{

		public int playerId;
		public float x;
		public float y;

		public SpawnLocation()
		{
			playerId = 0;
			x = 0.0f;
			y = 0.0f;
		}

		public SpawnLocation(int newPlayerId, float newX, float newY)
		{
			playerId = newPlayerId;
			x = newX;
			y = newY;
		}
	}

	public List<SpawnLocation> spawnLocations = new List<SpawnLocation>(1);
	//public SpawnLocationList spawnLocations;
	public List<PlayerData> playerData;
	public int numberOfPlayers = 0;

	// Use this for initialization
	void Start () {
		spawnLocations.Add(new SpawnLocation(1, -3.165f, 0.19f));
		spawnLocations.Add(new SpawnLocation(2, 4.618733f, 0.19f));
		/*spawnLocations = new List<Object>();
		playerData = new List<Object>();
		SpawnLocation newSpawnLocation = new SpawnLocation (0, 0, 0);
		Object defaultSpawnLocation = (Object)newSpawnLocation;
		PlayerData newPlayerData = new PlayerData (0, 0, 0, 0, 0, 0);
		Object defaultPlayerData = (Object)newPlayerData;*/

		//spawnLocations.Add (new SpawnLocation (0, 0, 0));

		//playerData.Add (new PlayerData(0, 0, 0, 0, 0, 0));
		//numberOfPlayers = 0;

		handleConnection();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void handleConnection()
	{
		var newPlayer = NetworkManager.Instance.InstantiateCharSquare();
		CharSquare newPlayerCasted = (CharSquare)newPlayer;
		newPlayerCasted.SetPlayerId(numberOfPlayers+1);
		UpdatePlayers();
	}

	public void UpdatePlayers()
	{
		networkObject.SendRpc(RPC_UPDATE_PLAYER_NUMBERS, Receivers.AllBuffered);
	}

	public override void UpdatePlayerNumbers(RpcArgs args)
	{
		numberOfPlayers++;
	}
}