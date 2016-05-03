using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

// Bullet objects shot by players.

public class Bullet : NetworkBehaviour
{

    public float directionX = 0.0f;
    public float directionY = 0.0f;
    public float speed = 0.0f;
    [SyncVar]
    public int damage = 1;
    [HideInInspector]
    public CharSquare player;
    [SyncVar]
    public int playerId = 0;

    public CharSquare.BulletDirection bulletDirection;
    public int bulletListPosition = 0;
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
        if (other.gameObject.CompareTag("GameBoundary") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("WinnersSquare"))
        {
            DestroySelf();
        }
        else if(other.gameObject.CompareTag("Player"))
        {
            if (((CharSquare)other.GetComponent<CharSquare>()).playerId != playerId)
            {
                ((CharSquare)other.GetComponent<CharSquare>()).health -= damage;
                DestroySelf();
            }
        }
        else if(other.gameObject.CompareTag("PlayerSpecialWall"))
        {
            if (((SquarePlayerWall)other.GetComponent<SquarePlayerWall>()).playerId != playerId)
            {
                DestroySelf();
            }
        }
    }

    // Used if the bullet detects that it needs to destroy itself, rather than the player object destroying it.
    // Handles removing itself from the player objects activeBullets dictionaries as well as destroying it's own GameObject.
    private void DestroySelf()
    {
        player.RpcRemoveBullet(bulletDirection, bulletListPosition);
        player.CmdDestroyObject(gameObject.GetComponent<NetworkIdentity>().netId);
        //NetworkServer.Destroy(gameObject);
    }
}