using UnityEngine;
using System.Collections;

public class SquareWinners : MonoBehaviour {

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    private int count = 0;
    private int colorChange = 5;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (count == 0 || count == colorChange*2 || count == colorChange*4 || count == colorChange * 6)
        {
            GetComponent<SpriteRenderer>().color = colors[0];
        } else if(count == colorChange)
        {
            GetComponent<SpriteRenderer>().color = colors[1];
        } else if(count == colorChange*3)
        {
            GetComponent<SpriteRenderer>().color = colors[2];
        } else if (count == colorChange*5)
        {
            GetComponent<SpriteRenderer>().color = colors[3];
        } else if (count == colorChange*7)
        {
            GetComponent<SpriteRenderer>().color = colors[4];
        } else if (count == colorChange*8)
        {
            GetComponent<SpriteRenderer>().color = colors[0];
            count = 0;
        }
        count++;
	}
}
