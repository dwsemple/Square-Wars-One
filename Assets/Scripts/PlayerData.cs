using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : MonoBehaviour {

	public int playerId { get; set; }
	public int playerSpeed { get; set; }
	public int bulletSpeed { get; set; }
	public int bulletDamage { get; set; }
	public int maxBullets { get; set; }
	public int health { get; set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
