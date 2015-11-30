using UnityEngine;
using System.Collections;

public class SquarePlayerWall : MonoBehaviour {

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    public int playerId = 0;
    private int count = 0;
    private int colorChange = 5;


    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().color = colors[playerId];
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (count == 0)
        {
            GetComponent<SpriteRenderer>().color = colors[0];
        }
        else if (count == colorChange)
        {
            GetComponent<SpriteRenderer>().color = colors[playerId];
        }
        else if (count == colorChange * 2)
        {
            GetComponent<SpriteRenderer>().color = colors[0];
            count = 0;
        }
        count++;
    }
}
