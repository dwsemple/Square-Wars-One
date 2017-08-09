using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLocationList : MonoBehaviour
{
	[System.Serializable]
	public class SpawnLocation
	{

		public int playerId;
		public int x;
		public int y;

		public SpawnLocation()
		{
			playerId = 0;
			x = 0;
			y = 0;
		}

		public SpawnLocation(int newPlayerId, int newX, int newY)
		{
			playerId = newPlayerId;
			x = newX;
			y = newY;
		}
	}

	public List<SpawnLocation> spawnLocations = new List<SpawnLocation>(1);

	public void AddNew()
	{
		spawnLocations.Add(new SpawnLocation());
	}

	public void AddNew(int playerId, int x, int y)
	{
		spawnLocations.Add(new SpawnLocation(playerId, x, y));
	}

	public void Remove(int index)
	{
		spawnLocations.RemoveAt(index);
	}

	public void RemoveLast()
	{
		spawnLocations.RemoveAt(spawnLocations.Count - 1);
	}
}
