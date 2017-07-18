using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Unity;

public class SpawnPlayer : MonoBehaviour {


	// Use this for initialization
	void Start () {
		var newPlayer = NetworkManager.Instance.InstantiateCharSquare();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
