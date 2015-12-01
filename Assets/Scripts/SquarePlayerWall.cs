using UnityEngine;
using System.Collections;

public class SquarePlayerWall : MonoBehaviour
{

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    public int playerId = 0;
    private int count = 0;
    private int colorChange = 5;
    private int colorIndex = 0;
	
	void FixedUpdate ()
    {
        GetComponent<SpriteRenderer>().color = colors[colorIndex];
        count++;
        if ((count % colorChange) == 0)
        {
            if(colorIndex == playerId)
            {
                colorIndex = 0;
                count = 0;
            }
            else
            {
                colorIndex = playerId;
            }
        }
    }
}
