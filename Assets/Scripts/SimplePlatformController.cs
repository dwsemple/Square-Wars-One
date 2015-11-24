using UnityEngine;
using System.Collections;

public class SimplePlatformController : MonoBehaviour {

    [HideInInspector] public bool facingRight = true;
    [HideInInspector] public bool jump = false;
    [HideInInspector] public bool shoot = false;

    public float moveForce = 365.0f;
    public float maxSpeed = 5.0f;
    public float jumpForce = 1000.0f;
    public Transform groundCheck;
    public GameObject bullet;
    public float bulletSpeed = 1000.0f;

    private bool grounded = false;
    private Animator anim;
    private Rigidbody2D rb2d;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));

        if(Input.GetButtonDown("Jump") && grounded) {
            jump = true;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            shoot = true;
        }
    }

    void FixedUpdate() {
        float h = Input.GetAxis("Horizontal");

        anim.SetFloat("Speed", Mathf.Abs(h));

        if (h * rb2d.velocity.x < maxSpeed) {
            rb2d.AddForce(Vector2.right * h * moveForce);
        }

        if (Mathf.Abs(rb2d.velocity.x) > maxSpeed) {
            rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);
        }

        if (h > 0 && !facingRight) {
            Flip();
        } else if (h < 0 && facingRight) {
            Flip();
        }

        if(jump) {
            anim.SetTrigger("Jump");
            rb2d.AddForce(new Vector2(0f, jumpForce));
            jump = false;
        }

        if(shoot) {
            Shoot();
        }
    }

    void Flip() {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void Shoot() {
        spawnBullet(new Vector2(-2.0f, 0.0f), new Vector2(-1.0f, 0.0f));
        spawnBullet(new Vector2(2.0f, 0.0f), new Vector2(1.0f, 0.0f));
        spawnBullet(new Vector2(0.0f, 2.0f), new Vector2(0.0f, 1.0f));
        spawnBullet(new Vector2(0.0f, -2.0f), new Vector2(0.0f, -1.0f));/*
        GameObject newBullet1 = (GameObject)Instantiate(bulletLeft, transform.position + new Vector3(-2.0f, 0.0f, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet1.GetComponent<Rigidbody2D>()).AddForce(new Vector2(-1.0f, 0.0f) * bulletSpeed);
        ((BulletSpawn)newBullet1.GetComponent<BulletSpawn>()).player = this;
        GameObject newBullet2 = (GameObject)Instantiate(bulletLeft, transform.position + new Vector3(2.0f, 0.0f, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet2.GetComponent<Rigidbody2D>()).AddForce(new Vector2(1.0f, 0.0f) * bulletSpeed);
        ((BulletSpawn)newBullet1.GetComponent<BulletSpawn>()).player = this;
        GameObject newBullet3 = (GameObject)Instantiate(bulletLeft, transform.position + new Vector3(0.0f, 2.0f, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet3.GetComponent<Rigidbody2D>()).AddForce(new Vector2(0.0f, 1.0f) * bulletSpeed);
        ((BulletSpawn)newBullet1.GetComponent<BulletSpawn>()).player = this;
        GameObject newBullet4 = (GameObject)Instantiate(bulletLeft, transform.position + new Vector3(0.0f, -2.0f, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet4.GetComponent<Rigidbody2D>()).AddForce(new Vector2(0.0f, -1.0f) * bulletSpeed);
        ((BulletSpawn)newBullet1.GetComponent<BulletSpawn>()).player = this;*/
        shoot = false;
    }

    BulletSpawn spawnBullet(Vector2 position, Vector2 direction) {
        direction.Normalize();
        GameObject newBullet = (GameObject)Instantiate(bullet, transform.position + new Vector3(position.x, position.y, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet.GetComponent<Rigidbody2D>()).AddForce(direction * bulletSpeed);
        ((BulletSpawn)newBullet.GetComponent<BulletSpawn>()).player = this;
        return (BulletSpawn)newBullet.GetComponent<BulletSpawn>();
    }
}
