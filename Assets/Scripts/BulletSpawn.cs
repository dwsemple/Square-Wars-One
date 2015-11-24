using UnityEngine;
using System.Collections;

public class BulletSpawn : MonoBehaviour {

    public float directionX = 0.0f;
    public float directionY = 0.0f;
    public float speed = 0.0f;
    [HideInInspector] public SimplePlatformController player;

    private Rigidbody2D rb2d;

	// Use this for initialization
	void Awake () {
        rb2d = GetComponent<Rigidbody2D>();
    }
	
    void Start ()
    {
        player.jump = true;
    }

	// Update is called once per frame
	void Update () {
        
    }
    /*
    void FixedUpdate() {
        rb2d.velocity = new Vector2(directionX, directionY);
        rb2d.velocity.Normalize();
        rb2d.velocity = rb2d.velocity * speed;
    }*/
}
