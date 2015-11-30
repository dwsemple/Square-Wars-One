﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class CharSquare : MonoBehaviour {

    public GameObject bullet;
    public int playerId = 0;
    public float playerSpeed = 300.0f;
    public float bulletSpeed = 500.0f;
    public int maxBullets = 3;
    public int health = 1;

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    private bool shoot = false;
    private List<MoveDirection> movementQueue = new List<MoveDirection>();
    private IDictionary<MoveDirection, MoveAxisData> movementAxisData = new Dictionary<MoveDirection, MoveAxisData>() {
        { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
        { MoveDirection.LEFT, new MoveAxisData("Left", MoveAxis.HORIZONTAL, "Left") },
        { MoveDirection.RIGHT, new MoveAxisData("Right", MoveAxis.HORIZONTAL, "Right") },
        { MoveDirection.UP, new MoveAxisData("Up", MoveAxis.VERTICAL, "Up") },
        { MoveDirection.DOWN, new MoveAxisData("Down", MoveAxis.VERTICAL, "Down") }
    };
    private IDictionary<BulletDirection, List<Bullet>> activeBullets = new Dictionary<BulletDirection, List<Bullet>>() {
        { BulletDirection.LEFT, new List<Bullet>() },
        { BulletDirection.RIGHT, new List<Bullet>() },
        { BulletDirection.UP, new List<Bullet>() },
        { BulletDirection.DOWN, new List<Bullet>() }
    };
    private IDictionary<BulletDirection, Vector2> bulletVelocities = new Dictionary<BulletDirection, Vector2>() {
        { BulletDirection.LEFT, new Vector2(-1.0f,0.0f) },
        { BulletDirection.RIGHT, new Vector2(1.0f,0.0f) },
        { BulletDirection.UP, new Vector2(0.0f,1.0f) },
        { BulletDirection.DOWN, new Vector2(0.0f,-1.0f) }
    };
    private bool alive = true;

    private Rigidbody2D rb2d;

    private enum MoveDirection {
        NO_MOVE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private enum MoveAxis {
        NO_MOVE,
        HORIZONTAL,
        VERTICAL
    }

    public enum BulletDirection
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private class MoveAxisData {
        private String axisName;
        private MoveAxis axisAffected;
        private String buttonName;

        public MoveAxisData(String axisName, MoveAxis axisAffected, String buttonName) {
            this.axisName = axisName;
            this.axisAffected = axisAffected;
            this.buttonName = buttonName;
        }

        public String AxisName {
            get { return axisName; }
            set { axisName = value; }
        }

        public MoveAxis AxisAffected {
            get { return axisAffected; }
            set { axisAffected = value; }
        }

        public String ButtonName {
            get { return buttonName; }
            set { buttonName = value; }
        }
    }

    // Use this for initialization
    void Start () {
        GetComponent<SpriteRenderer>().color = colors[playerId];
        if (playerId == 1)
        {
            movementAxisData = new Dictionary<MoveDirection, MoveAxisData>() {
                { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
                { MoveDirection.LEFT, new MoveAxisData("Left", MoveAxis.HORIZONTAL, "Left") },
                { MoveDirection.RIGHT, new MoveAxisData("Right", MoveAxis.HORIZONTAL, "Right") },
                { MoveDirection.UP, new MoveAxisData("Up", MoveAxis.VERTICAL, "Up") },
                { MoveDirection.DOWN, new MoveAxisData("Down", MoveAxis.VERTICAL, "Down") }
            };
        } else if(playerId == 2)
        {
            movementAxisData = new Dictionary<MoveDirection, MoveAxisData>() {
                { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
                { MoveDirection.LEFT, new MoveAxisData("Left2", MoveAxis.HORIZONTAL, "Left2") },
                { MoveDirection.RIGHT, new MoveAxisData("Right2", MoveAxis.HORIZONTAL, "Right2") },
                { MoveDirection.UP, new MoveAxisData("Up2", MoveAxis.VERTICAL, "Up2") },
                { MoveDirection.DOWN, new MoveAxisData("Down2", MoveAxis.VERTICAL, "Down2") }
            };
        }
        foreach(MoveDirection direction in Enum.GetValues(typeof(MoveDirection))) {
            movementQueue.Add(direction);
        }
    }

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        if (health < 1 && alive)
        {
            noHealth();
            alive = false;
        }
        else if (alive)
        {
            foreach (MoveDirection moveDirection in movementAxisData.Keys)
            {
                MoveAxisData axis;
                movementAxisData.TryGetValue(moveDirection, out axis);
                if (axis.ButtonName != null)
                {
                    if (Input.GetButtonDown(axis.ButtonName))
                    {
                        moveDirectionToFrontOfQueue(moveDirection);
                    }
                    else if (Input.GetButtonUp(axis.ButtonName))
                    {
                        moveDirectionToBackOfQueue(moveDirection);
                    }
                }
            }

            if (Input.GetButtonDown("Fire") && playerId == 1)
            {
                shoot = true;
            }
            else if (Input.GetButtonDown("Fire2") && playerId == 2)
            {
                shoot = true;
            }
        }
    }

    void FixedUpdate() {
        float h = 0.0f;
        float v = 0.0f;

        MoveAxisData currentAxis;
        movementAxisData.TryGetValue(movementQueue[0], out currentAxis);
        if (currentAxis.AxisAffected == MoveAxis.HORIZONTAL)
        {
            h = Input.GetAxis(currentAxis.AxisName);
        }
        else if (currentAxis.AxisAffected == MoveAxis.VERTICAL)
        {
            v = Input.GetAxis(currentAxis.AxisName);
        }

        Vector2 direction = new Vector2(h, v);
        direction.Normalize();
        Vector2 velocity = direction * playerSpeed;

        RaycastHit2D wallCollision = Physics2D.Linecast(new Vector2(transform.position.x + (direction.x * 0.33f), transform.position.y + (direction.y * 0.33f)), new Vector2(transform.position.x + (velocity.x * Time.fixedDeltaTime), transform.position.y + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        RaycastHit2D wallCollisionUpper = new RaycastHit2D();
        RaycastHit2D wallCollisionLower = new RaycastHit2D();
        if (direction.x != 0.0f)
        {
            wallCollisionUpper = Physics2D.Linecast(new Vector2(transform.position.x + (direction.x * 0.33f), transform.position.y + (0.16f)), new Vector2(transform.position.x + (velocity.x * Time.fixedDeltaTime), transform.position.y + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
            wallCollisionLower = Physics2D.Linecast(new Vector2(transform.position.x + (direction.x * 0.33f), transform.position.y - (0.16f)), new Vector2(transform.position.x + (velocity.x * Time.fixedDeltaTime), transform.position.y + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        }
        else if (direction.y != 0.0f)
        {
            wallCollisionUpper = Physics2D.Linecast(new Vector2(transform.position.x + (0.16f), transform.position.y + (direction.y * 0.33f)), new Vector2(transform.position.x + (velocity.x * Time.fixedDeltaTime), transform.position.y + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
            wallCollisionLower = Physics2D.Linecast(new Vector2(transform.position.x - (0.16f), transform.position.y + (direction.y * 0.33f)), new Vector2(transform.position.x + (velocity.x * Time.fixedDeltaTime), transform.position.y + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        }
        if (wallCollision || wallCollisionUpper || wallCollisionLower)
        {
            RaycastHit2D collisionDetected = new RaycastHit2D();
            if (wallCollision)
            {
                collisionDetected = wallCollision;
            }
            else if (wallCollisionUpper)
            {
                collisionDetected = wallCollisionUpper;
            }
            else if (wallCollisionLower)
            {
                collisionDetected = wallCollisionLower;
            }

            if (collisionDetected.rigidbody.tag == "Wall")
            {
                rb2d.velocity = new Vector2(0.0f, 0.0f);
                Transform wallTransform = collisionDetected.rigidbody.GetComponent<Transform>();
                Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                velocity = boundaryOffset - squareOffset;
                rb2d.position += velocity;
            } else if (collisionDetected.rigidbody.tag == "GameBoundary")
            {
                rb2d.velocity = new Vector2(0.0f, 0.0f);
                Transform wallTransform = collisionDetected.rigidbody.GetComponent<Transform>();
                Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + (((1.0f * -direction.x) * wallTransform.localScale.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + (((1.0f * -direction.y) * wallTransform.localScale.y) / 2));
                Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                velocity = boundaryOffset - squareOffset;
                rb2d.position += velocity;
            } else if (collisionDetected.rigidbody.tag == "PlayerSpecialWall") {
                if(((SquarePlayerWall)collisionDetected.rigidbody.GetComponent<SquarePlayerWall>()).playerId != playerId)
                {
                    rb2d.velocity = new Vector2(0.0f, 0.0f);
                    Transform wallTransform = collisionDetected.rigidbody.GetComponent<Transform>();
                    Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                    Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                    velocity = boundaryOffset - squareOffset;
                    rb2d.position += velocity;
                } else
                {
                    rb2d.velocity = velocity;
                }
            } else if (collisionDetected.rigidbody.tag == "WinnersSquare")
            {
                Application.LoadLevel(Application.loadedLevel);
            }
        }

        if (!wallCollision && !wallCollisionUpper && !wallCollisionLower)
        {
            rb2d.velocity = velocity;
        }
        

        if(shoot) {
            Shoot();
        }
    }

    private void Shoot()
    {
        foreach(BulletDirection bullets in activeBullets.Keys)
        {
            List<Bullet> bulletList;
            activeBullets.TryGetValue(bullets, out bulletList);
            if (bulletList.Count < maxBullets)
            {
                Vector2 bulletVelocity;
                bulletVelocities.TryGetValue(bullets, out bulletVelocity);
                Bullet newBullet = spawnBullet(new Vector2(0.0f, 0.0f), bulletVelocity);
                newBullet.bulletDirection = bullets;
                newBullet.bulletListPosition = bulletList.Count;
                bulletList.Add(newBullet);
            }
        }
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

    public void removeBullet(BulletDirection direction, int listPosition)
    {
        List<Bullet> bulletList;
        activeBullets.TryGetValue((BulletDirection)direction, out bulletList);
        bulletList.RemoveAt(listPosition);
        int count = 0;
        foreach (Bullet bullets in bulletList)
        {
            if (bullets.bulletListPosition != count)
            {
                bullets.bulletListPosition = count;
            }
            count++;
        }
    }

    private void moveDirectionToBackOfQueue(MoveDirection direction) {
        int count = 0;
        foreach (MoveDirection directionInQueue in movementQueue)
        {
            if (directionInQueue == direction)
            {
                movementQueue.RemoveAt(count);
                break;
            }
            count++;
        }
        movementQueue.Add(direction);
    }

    private void moveDirectionToFrontOfQueue(MoveDirection direction) {
        int count = 0;
        foreach (MoveDirection directionInQueue in movementQueue)
        {
            if (directionInQueue == direction)
            {
                movementQueue.RemoveAt(count);
                break;
            }
            count++;
        }
        movementQueue.Insert(0, direction);
    }

    public void noHealth()
    {

        foreach (BulletDirection bulletLists in activeBullets.Keys)
        {
            List<Bullet> bulletList;
            activeBullets.TryGetValue(bulletLists, out bulletList);
            int count = 0;
            foreach (Bullet bullets in bulletList)
            {
                bulletList.RemoveAt(count);
                Destroy(bullets.gameObject);
                count++;
            }
            count = 0;
        }

        GameObject[] playerSpecialWalls = GameObject.FindGameObjectsWithTag("PlayerSpecialWall");
        foreach(GameObject playerWall in playerSpecialWalls)
        {
            if(((SquarePlayerWall)playerWall.GetComponent<SquarePlayerWall>()).playerId == playerId)
            {
                DestroyObject(playerWall);
            }
        }

        Destroy(gameObject);
    }
}
