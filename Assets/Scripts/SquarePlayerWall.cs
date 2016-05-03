using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// This object acts as a normal wall for all players, except the player that has the same playerId as it.
// This player can move and shoot freely through the wall.

public class SquarePlayerWall : NetworkBehaviour
{

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    [SyncVar]
    public int playerId = 0;
    private int count = 0;
    private int colorChange = 5;
    private int colorIndex = 0;
	
	void FixedUpdate ()
    {
        // Animate the object.
        // It simply cycles between the relevant player color and the wall color.
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
