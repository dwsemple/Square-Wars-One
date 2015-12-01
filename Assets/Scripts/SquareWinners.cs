using UnityEngine;
using System.Collections;

// Colliding with this object wins the game for the player colliding with it.

public class SquareWinners : MonoBehaviour
{

    private Color[] colors = { Color.white, Color.blue, Color.white, Color.magenta, Color.white, Color.red, Color.white, Color.green };
    private int count = 0;
    private int colorChange = 5;
    private int colorIndex = 0;
	
	void FixedUpdate ()
    {
        // Animate the object.
        // It simply cycles through the player colors and the wall color.
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
