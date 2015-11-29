using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharSquare : MonoBehaviour {

    public GameObject bullet;
    public int playerId = 0;
    public float playerSpeed = 300.0f;
    public float bulletSpeed = 1000.0f;

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    private bool shoot = false;
    private int moveUp = 0;
    private int moveDown = 1;
    private int moveLeft = 2;
    private int moveRight = 3;
    private int noMove = 4;
    private List<int> movementQueue = new List<int>();

    private Rigidbody2D rb2d;

    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().color = colors[playerId];
        movementQueue.Add(noMove);
        movementQueue.Add(moveLeft);
        movementQueue.Add(moveRight);
        movementQueue.Add(moveUp);
        movementQueue.Add(moveDown);
    }

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        int count = 0;

        if (Input.GetButtonDown("Up")) {
            foreach(int i in movementQueue)
            {
                if(i == moveUp)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Insert(0, moveUp);
            count = 0;
        } else if (Input.GetButtonUp("Up")) {
            foreach (int i in movementQueue)
            {
                if (i == moveUp)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Add(moveUp);
            count = 0;
        }
        
        if (Input.GetButtonDown("Down"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveDown)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Insert(0, moveDown);
            count = 0;
        }
        else if (Input.GetButtonUp("Down"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveDown)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Add(moveDown);
            count = 0;
        }

        if (Input.GetButtonDown("Left"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveLeft)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Insert(0, moveLeft);
            count = 0;
        }
        else if (Input.GetButtonUp("Left"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveLeft)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Add(moveLeft);
            count = 0;
        }

        if (Input.GetButtonDown("Right"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveRight)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Insert(0, moveRight);
            count = 0;
        }
        else if (Input.GetButtonUp("Right"))
        {
            foreach (int i in movementQueue)
            {
                if (i == moveRight)
                {
                    movementQueue.RemoveAt(count);
                    break;
                }
                count++;
            }
            movementQueue.Add(moveRight);
            count = 0;
        }
        
        if (Input.GetButtonDown("Fire"))
        {
            shoot = true;
        }
    }

    void FixedUpdate() {
        float h = 0.0f;
        float v = 0.0f;
        if(movementQueue[0] == 0) {
            v = Input.GetAxis("Up");
        } else if(movementQueue[0] == 1) {
            v = Input.GetAxis("Down");
        } else if (movementQueue[0] == 2) {
            h = Input.GetAxis("Left");
        } else if (movementQueue[0] == 3) {
            h = Input.GetAxis("Right");
        }

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();
        rb2d.velocity = new Vector2(direction.x * playerSpeed, direction.y * playerSpeed);

        if(shoot) {
            Shoot();
        }
    }

    void Shoot()
    {
        spawnBullet(new Vector2(-0.1f, 0.0f), new Vector2(-1.0f, 0.0f));
        spawnBullet(new Vector2(0.1f, 0.0f), new Vector2(1.0f, 0.0f));
        spawnBullet(new Vector2(0.0f, 0.1f), new Vector2(0.0f, 1.0f));
        spawnBullet(new Vector2(0.0f, -0.1f), new Vector2(0.0f, -1.0f));
        shoot = false;
    }

    Bullet spawnBullet(Vector2 position, Vector2 direction)
    {
        direction.Normalize();
        GameObject newBullet = (GameObject)Instantiate(bullet, transform.position + new Vector3(position.x, position.y, 0.0f), Quaternion.identity);
        ((Rigidbody2D)newBullet.GetComponent<Rigidbody2D>()).AddForce(direction * bulletSpeed);
        ((Bullet)newBullet.GetComponent<Bullet>()).player = this;
        ((Bullet)newBullet.GetComponent<Bullet>()).playerId = playerId;
        return (Bullet)newBullet.GetComponent<Bullet>();
    }
}
