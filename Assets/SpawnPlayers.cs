using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnPlayers : NetworkManager {

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        int count = 0;
        foreach (NetworkConnection connection in NetworkServer.connections)
        {
            count++;
        }

        PlayerSpawnLocation[] spawnLocations = GameObject.FindObjectsOfType(typeof(PlayerSpawnLocation)) as PlayerSpawnLocation[];
        if(spawnLocations != null)
        {
            ArrayList spawnLocationsForPlayer = new ArrayList();
            foreach(PlayerSpawnLocation location in spawnLocations)
            {
                if (location.playerId == count)
                {
                    spawnLocationsForPlayer.Add(location);
                }
            }
            if(spawnLocationsForPlayer.Count != 0)
            {
                int spawnLocationIndex = 0;
                if(spawnLocationsForPlayer.Count > 1)
                {
                    spawnLocationIndex = Random.Range(0, spawnLocationsForPlayer.Count);
                }
                GameObject player = (GameObject)Instantiate(playerPrefab, ((PlayerSpawnLocation)spawnLocationsForPlayer[spawnLocationIndex]).transform.position, Quaternion.identity);
                player.GetComponent<CharSquare>().playerId = count;
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            }
            else
            {
                Debug.LogError("No valid starting locations for player ID " + count + " in level. Add a starting location for this player before level can be run.");
            }
        }
        else
        {
            Debug.LogError("No valid starting locations in level. Add a starting location before level can be run.");
        }
    }
}