using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float directionX = 0.0f;
    public float directionY = 0.0f;
    public float speed = 0.0f;
    public int damage = 1;
    [HideInInspector]
    public CharSquare player;
    public int playerId = 0;
    public CharSquare.BulletDirection bulletDirection;
    public int bulletListPosition = 0;
    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };

    private Rigidbody2D rb2d;

    // Use this for initialization
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[playerId];
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "GameBoundary" || other.gameObject.tag == "Wall" || other.gameObject.tag == "WinnersSquare")
        {
            destroySelf();
        } else if(other.gameObject.tag == "Player")
        {
            if (((CharSquare)other.GetComponent<CharSquare>()).playerId != playerId)
            {
                ((CharSquare)other.GetComponent<CharSquare>()).health -= damage;
                destroySelf();
            }
        } else if(other.gameObject.tag == "PlayerSpecialWall")
        {
            if (((SquarePlayerWall)other.GetComponent<SquarePlayerWall>()).playerId != playerId)
            {
                destroySelf();
            }
        }
    }

    void destroySelf()
    {
        player.removeBullet(bulletDirection, bulletListPosition);
        Destroy(gameObject);
    }
}
