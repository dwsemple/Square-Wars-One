using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float directionX = 0.0f;
    public float directionY = 0.0f;
    public float speed = 0.0f;
    [HideInInspector]
    public CharSquare player;
    public int playerId = 0;
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
}
