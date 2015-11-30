using UnityEngine;
using System.Collections;

public class SquarePlayerWall : MonoBehaviour {

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    public int playerId = 0;


    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().color = colors[playerId];
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
