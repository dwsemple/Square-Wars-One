using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class NonNetworkedSettings : MonoBehaviour {

	private int count;
	private bool instantiated;
	// Use this for initialization
	void Start () {
		count = 0;
		instantiated = false;
		//handleConnection();
	}
	
	// Update is called once per frame
	void Update () {
		if(count > 60 && !instantiated)
		{
			instantiated = true;
			handleConnection();
		}
		count++;
	}

	public void handleConnection()
	{
		GlobalSettings globalSettings = (GlobalSettings)FindObjectOfType(typeof(GlobalSettings));
		globalSettings.handleConnection();
		/*Debug.Log("Current Number of Players " + globalSettings.numberOfPlayers);
		var newPlayer = NetworkManager.Instance.InstantiateCharSquare();
		CharSquare newPlayerCasted = (CharSquare)newPlayer;
		newPlayerCasted.SetPlayerId(globalSettings.numberOfPlayers+1);
		globalSettings.UpdatePlayers();*/
	}
}
