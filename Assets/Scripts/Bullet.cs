using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking;

// Bullet objects shot by players.

public class Bullet : SimpleNetworkedMonoBehaviour
{

    public float directionX = 0.0f;
    public float directionY = 0.0f;
    public float speed = 0.0f;
    public int damage = 1;
    [HideInInspector]
    public CharSquare player;
    public int playerId = 0;
    public CharSquare.BulletDirection bulletDirection;
    public int bulletListPosition = 0;
	private int bulletState = 0;
    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };

    private Rigidbody2D rb2d;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[playerId];
    }

    // Handle collisions using Unitys inbuild physics.
    void OnTriggerEnter2D(Collider2D other)
    {
		if (!isOwner)
		{
			return;
		}

        if (other.gameObject.CompareTag("GameBoundary") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("WinnersSquare"))
        {
			RPC("DestroySelf");
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            if (((CharSquare)other.GetComponent<CharSquare>()).playerId != playerId)
            {
				RPC("DamagePlayer", ((CharSquare)other.GetComponent<CharSquare>()));
				RPC("DestroySelf");
            }
        }
        else if(other.gameObject.CompareTag("PlayerSpecialWall"))
        {
            if (((SquarePlayerWall)other.GetComponent<SquarePlayerWall>()).playerId != playerId)
            {
				RPC("DestroySelf");
            }
        }
    }

    // Used if the bullet detects that it needs to destroy itself, rather than the player object destroying it.
    // Handles removing itself from the player objects activeBullets dictionaries as well as destroying it's own GameObject.
	[BRPC]
    private void DestroySelf()
    {
		if (bulletState != 1) {
			bulletState = 1;
			player.RemoveBullet (bulletDirection, bulletListPosition);
			Networking.Destroy(this);
		}
    }

	// Used to ensure all clients apply damage correctly on the network when the bullet collides with a player.
	[BRPC]
	private void DamagePlayer(CharSquare otherPlayer)
	{
		otherPlayer.health -= damage;
	}
}