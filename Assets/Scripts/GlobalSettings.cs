using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class GlobalSettings : GlobalSettingsBehavior {

	public List<SpawnLocation> spawnLocations;
	public List<PlayerData> playerData;
	public int numberOfPlayers;

	// Use this for initialization
	void Start () {
		spawnLocations = new List<SpawnLocation>();
		playerData = new List<PlayerData>();
		//SpawnLocation newSpawnLocation = new SpawnLocation (0, 0, 0);
		//Object defaultSpawnLocation = (Object)newSpawnLocation;
		//PlayerData newPlayerData = new PlayerData (0, 0, 0, 0, 0, 0);
		//Object defaultPlayerData = (Object)newPlayerData;

		//spawnLocations.Add (new SpawnLocation (0, 0, 0));

		//playerData.Add (new PlayerData(0, 0, 0, 0, 0, 0));
		numberOfPlayers = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdatePlayers()
	{
		networkObject.SendRpc(RPC_UPDATE_PLAYER_NUMBERS, Receivers.AllBuffered);
	}

	public override void UpdatePlayerNumbers(RpcArgs args)
	{
		numberOfPlayers++;
	}

	public struct SpawnLocation
	{
		public int playerId;
		public int x;
		public int y;

		public SpawnLocation(int newPlayerId, int newX, int newY)
		{
			playerId = newPlayerId;
			x = newX;
			y = newY;
		}
	}

	public struct PlayerData
	{
		public int playerId;
		public int playerSpeed;
		public int bulletSpeed;
		public int bulletDamage;
		public int maxBullets;
		public int health;

		public PlayerData(int newPlayerId, int newPlayerSpeed, int newBulletSpeed, int newBulletDamage, int newMaxBullets, int newHealth)
		{
			playerId = newPlayerId;
			playerSpeed = newPlayerSpeed;
			bulletSpeed = newBulletSpeed;
			bulletDamage = newBulletDamage;
			maxBullets = newMaxBullets;
			health = newHealth;
		}
	}
}