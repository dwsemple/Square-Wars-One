using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;

// Bullet objects shot by players.

public class Bullet : BulletBehavior
{

	public Vector2 direction = Vector2.zero;
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
        
    }

	public void InitialiseBullet(Vector2 new_direction, float new_speed, int new_damage, int new_playerId, int new_bulletDirection, int new_bulletListPosition)
	{
		networkObject.SendRpc(RPC_CONSTRUCT_BULLET, Receivers.AllBuffered, new_direction, new_speed, new_damage, new_playerId, new_bulletDirection, new_bulletListPosition);

		direction = new_direction;
		speed = new_speed;
		damage = new_damage;
		playerId = new_playerId;
		bulletDirection = CharSquare.ConvertIntToBulletDirection(new_bulletDirection);
		bulletListPosition = new_bulletListPosition;
		rb2d.AddForce (direction * speed);
		GetComponent<SpriteRenderer>().color = colors[playerId];

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject playerObject in players)
		{
			if (((CharSquare)playerObject.GetComponent<CharSquare>()).playerId == playerId)
			{
				player = ((CharSquare)playerObject.GetComponent<CharSquare>());
				break;
			}
		}
	}

	public override void ConstructBullet(RpcArgs args)
	{
		direction = args.GetNext<Vector2>();
		speed = args.GetNext<float>();
		damage = args.GetNext<int>();
		playerId = args.GetNext<int>();
		bulletDirection = CharSquare.ConvertIntToBulletDirection(args.GetNext<int>());
		bulletListPosition = args.GetNext<int>();
		rb2d.AddForce (direction * speed);
		GetComponent<SpriteRenderer>().color = colors[playerId];

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject playerObject in players)
		{
			if (((CharSquare)playerObject.GetComponent<CharSquare>()).playerId == playerId)
			{
				player = ((CharSquare)playerObject.GetComponent<CharSquare>());
				break;
			}
		}
	}

    // Handle collisions using Unitys inbuild physics.
    void OnTriggerEnter2D(Collider2D other)
    {
		if(!networkObject.IsOwner)
		{
			return;
		}
        if (other.gameObject.CompareTag("GameBoundary") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("WinnersSquare"))
        {
            DestroySelf();
        }
        else if(other.gameObject.CompareTag("Player"))
        {
			if (((CharSquare)other.GetComponent<CharSquare>()).playerId != playerId && playerId != 0)
            {
                ((CharSquare)other.GetComponent<CharSquare>()).health -= damage;
                DestroySelf();
            }
        }
        else if(other.gameObject.CompareTag("PlayerSpecialWall"))
        {
			if (((SquarePlayerWall)other.GetComponent<SquarePlayerWall>()).playerId != playerId && playerId != 0)
            {
                DestroySelf();
            }
        }
    }

    // Used if the bullet detects that it needs to destroy itself, rather than the player object destroying it.
    // Handles removing itself from the player objects activeBullets dictionaries as well as destroying it's own GameObject.
    public void DestroySelf()
    {
		if (bulletState != 1) {
			bulletState = 1;
			player.RemoveBullet(bulletDirection, bulletListPosition);
			networkObject.Destroy();
		}
    }

	public void DestroyObject()
	{
		if (bulletState != 1) {
			bulletState = 1;
			networkObject.Destroy();
		}
	}
}