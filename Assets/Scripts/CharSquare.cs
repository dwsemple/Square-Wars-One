using System;
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

        RaycastHit2D wallCollision = Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x + (direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime), transform.position.y + (direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        RaycastHit2D wallCollisionUpper = Physics2D.Linecast(new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f), transform.position.y + (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y + (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Wall"));
        RaycastHit2D wallCollisionLower = Physics2D.Linecast(new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f), transform.position.y - (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y - (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Wall"));

        RaycastHit2D wallCollisionP1 = Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x + (direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime), transform.position.y + (direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Player1Wall"));
        RaycastHit2D wallCollisionP1Upper = Physics2D.Linecast(new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f), transform.position.y + (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y + (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Player1Wall"));
        RaycastHit2D wallCollisionP1Lower = Physics2D.Linecast(new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f), transform.position.y - (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y - (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Player1Wall"));

        RaycastHit2D wallCollisionP2 = Physics2D.Linecast(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x + (direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime), transform.position.y + (direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Player2Wall"));
        RaycastHit2D wallCollisionP2Upper = Physics2D.Linecast(new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f), transform.position.y + (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y + (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Player2Wall"));
        RaycastHit2D wallCollisionP2Lower = Physics2D.Linecast(new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f), transform.position.y - (Mathf.Abs(direction.x) * 0.16f)), new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f) + (Mathf.Abs(direction.x) * ((direction.x * 0.16f) + (velocity.x * Time.fixedDeltaTime))), transform.position.y - (Mathf.Abs(direction.x) * 0.16f) + (Mathf.Abs(direction.y) * ((direction.y * 0.16f) + (velocity.y * Time.fixedDeltaTime)))), 1 << LayerMask.NameToLayer("Player2Wall"));

        if (wallCollision || wallCollisionUpper || wallCollisionLower || wallCollisionP1 || wallCollisionP1Upper || wallCollisionP1Lower || wallCollisionP2 || wallCollisionP2Upper || wallCollisionP2Lower)
        {
            RaycastHit2D[] collisionDetected = { wallCollision, wallCollisionUpper, wallCollisionLower };
            bool[] isWallCollision = { false, false, false };
            Vector2[] wallCollisionDistance = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

            RaycastHit2D[] collisionDetectedP1 = { wallCollisionP1, wallCollisionP1Upper, wallCollisionP1Lower };
            bool[] isWallP1Collision = { false, false, false };
            Vector2[] wallP1CollisionDistance = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

            RaycastHit2D[] collisionDetectedP2 = { wallCollisionP2, wallCollisionP2Upper, wallCollisionP2Lower };
            bool[] isWallP2Collision = { false, false, false };
            Vector2[] wallP2CollisionDistance = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

            int count = 0;
            foreach(RaycastHit2D collision in collisionDetected)
            {
                if(collision)
                {
                    if (collision.rigidbody.tag == "Wall")
                    {
                        rb2d.velocity = new Vector2(0.0f, 0.0f);
                        Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                        Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                        Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                        wallCollisionDistance[count] = boundaryOffset - squareOffset;
                        isWallCollision[count] = true;
                    }
                    else if (collision.rigidbody.tag == "GameBoundary")
                    {
                        rb2d.velocity = new Vector2(0.0f, 0.0f);
                        Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                        Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + (((1.0f * -direction.x) * wallTransform.localScale.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + (((1.0f * -direction.y) * wallTransform.localScale.y) / 2));
                        Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                        wallCollisionDistance[count] = boundaryOffset - squareOffset;
                        isWallCollision[count] = true;
                    }
                }
                count++;
            }

            int countP1 = 0;
            foreach (RaycastHit2D collision in collisionDetectedP1)
            {
                if (collision)
                {
                    if (collision.rigidbody.tag == "PlayerSpecialWall")
                    {
                        if (((SquarePlayerWall)collision.rigidbody.GetComponent<SquarePlayerWall>()).playerId != playerId)
                        {
                            rb2d.velocity = new Vector2(0.0f, 0.0f);
                            Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                            Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                            Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                            wallP1CollisionDistance[countP1] = boundaryOffset - squareOffset;
                            isWallP1Collision[countP1] = true;
                        }
                    }
                }
                countP1++;
            }

            int countP2 = 0;
            foreach (RaycastHit2D collision in collisionDetectedP2)
            {
                if (collision)
                {
                    if (collision.rigidbody.tag == "PlayerSpecialWall")
                    {
                        if (((SquarePlayerWall)collision.rigidbody.GetComponent<SquarePlayerWall>()).playerId != playerId)
                        {
                            rb2d.velocity = new Vector2(0.0f, 0.0f);
                            Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                            Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                            Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                            wallP2CollisionDistance[countP2] = boundaryOffset - squareOffset;
                            isWallP2Collision[countP2] = true;
                        }
                    }
                }
                countP2++;
            }

            int countTest = 0;
            foreach(bool collisionTest in isWallCollision)
            {
                if (isWallP1Collision[countTest] && collisionTest)
                {
                    if (Mathf.Abs(wallP1CollisionDistance[countTest].x + wallP1CollisionDistance[countTest].y) < Mathf.Abs(wallCollisionDistance[countTest].x + wallCollisionDistance[countTest].y))
                    {
                        wallCollisionDistance[countTest] = wallP1CollisionDistance[countTest];
                    }
                }
                else if(isWallP1Collision[countTest])
                {
                    isWallCollision[countTest] = true;
                    wallCollisionDistance[countTest] = wallP1CollisionDistance[countTest];
                }

                if (isWallP2Collision[countTest] && isWallCollision[countTest])
                {
                    if (Mathf.Abs(wallP2CollisionDistance[countTest].x + wallP2CollisionDistance[countTest].y) < Mathf.Abs(wallCollisionDistance[countTest].x + wallCollisionDistance[countTest].y))
                    {
                        wallCollisionDistance[countTest] = wallP2CollisionDistance[countTest];
                    }
                }
                else if (isWallP2Collision[countTest])
                {
                    isWallCollision[countTest] = true;
                    wallCollisionDistance[countTest] = wallP2CollisionDistance[countTest];
                }

                countTest++;
            }

            bool wallCollisions = false;
            foreach(bool wallCollisionDetected in isWallCollision)
            {
                if(wallCollisionDetected)
                {
                    wallCollisions = true;
                    break;
                }
            }

            if (!wallCollisions)
            {
                bool ignoreCollision = true;
                foreach(RaycastHit2D collision in collisionDetected)
                {
                    if(collision)
                    {
                        if (collision.rigidbody.tag == "WinnersSquare")
                        {
                            Application.LoadLevel(Application.loadedLevel);
                            ignoreCollision = false;
                        }
                    }
                }

                if (ignoreCollision)
                {
                    rb2d.velocity = velocity;
                }
            }
            else
            {
                int countIndex = 0;
                Vector2 useVector = new Vector2(0.0f, 0.0f);
                bool firstVectorFound = false;
                foreach (bool hasCollided in isWallCollision)
                {
                    if(hasCollided)
                    {
                        if(!firstVectorFound)
                        {
                            useVector = wallCollisionDistance[countIndex];
                            firstVectorFound = true;
                        }
                        else
                        {
                            if(Mathf.Abs(wallCollisionDistance[countIndex].x + wallCollisionDistance[countIndex].y) < Mathf.Abs(useVector.x + useVector.y))
                            {
                                useVector = wallCollisionDistance[countIndex];
                            }
                        }
                    }
                    countIndex++;
                }
                rb2d.position += useVector;
            }
        }
        else
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
