using UnityEngine;
using System.Collections;

public class SquareWinners : MonoBehaviour
{

    private Color[] colors = { Color.white, Color.blue, Color.white, Color.magenta, Color.white, Color.red, Color.white, Color.green };
    private int count = 0;
    private int colorChange = 5;
    private int colorIndex = 0;
	
	void FixedUpdate ()
    {
        GetComponent<SpriteRenderer>().color = colors[colorIndex];
        count++;
        if ((count % colorChange) == 0)
        {
            colorIndex++;
        }
        if (colorIndex >= colors.Length)
        {
            colorIndex = 0;
            count = 0;
        }        
	}
}
