using System;
using System.Collections.Generic;
using UnityEngine;

public class CharSquare : MonoBehaviour {

    public GameObject bullet;
    public int playerId = 0;
    public float playerSpeed = 300.0f;
    public float bulletSpeed = 1000.0f;
    public int maxBullets = 3;
    [HideInInspector] public List<Bullet> bulletsUp = new List<Bullet>();
    [HideInInspector] public List<Bullet> bulletsDown = new List<Bullet>();
    [HideInInspector] public List<Bullet> bulletsLeft = new List<Bullet>();
    [HideInInspector] public List<Bullet> bulletsRight = new List<Bullet>();

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

        foreach(MoveDirection direction in Enum.GetValues(typeof(MoveDirection))) {
            movementQueue.Add(direction);
        }
    }

    void Awake() {
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        foreach (MoveDirection moveDirection in movementAxisData.Keys) {
            MoveAxisData axis;
            movementAxisData.TryGetValue(moveDirection, out axis);
            if (axis.ButtonName != null) {
                if (Input.GetButtonDown(axis.ButtonName)) {
                    moveDirectionToFrontOfQueue(moveDirection);
                }
                else if (Input.GetButtonUp(axis.ButtonName)) {
                    moveDirectionToBackOfQueue(moveDirection);
                }
            }
        }
        
        if (Input.GetButtonDown("Fire"))
        {
            shoot = true;
        }
    }

    void FixedUpdate() {
        float h = 0.0f;
        float v = 0.0f;
        
        MoveAxisData currentAxis;
        movementAxisData.TryGetValue(movementQueue[0], out currentAxis);
        if (currentAxis.AxisAffected == MoveAxis.HORIZONTAL) {
            h = Input.GetAxis(currentAxis.AxisName);
        } else if (currentAxis.AxisAffected == MoveAxis.VERTICAL) {
            v = Input.GetAxis(currentAxis.AxisName);
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
        if (bulletsLeft.Count < 3)
        {
            Bullet newBullet = spawnBullet(new Vector2(-0.1f, 0.0f), new Vector2(-1.0f, 0.0f));
            newBullet.bulletDirection = 3;
            newBullet.bulletListPosition = bulletsLeft.Count;
            bulletsLeft.Add(newBullet);
        }
        if(bulletsRight.Count < 3)
        {
            Bullet newBullet = spawnBullet(new Vector2(0.1f, 0.0f), new Vector2(1.0f, 0.0f));
            newBullet.bulletDirection = 4;
            newBullet.bulletListPosition = bulletsRight.Count;
            bulletsRight.Add(newBullet);
        }
        if(bulletsUp.Count < 3)
        {
            Bullet newBullet = spawnBullet(new Vector2(0.0f, 0.1f), new Vector2(0.0f, 1.0f));
            newBullet.bulletDirection = 1;
            newBullet.bulletListPosition = bulletsUp.Count;
            bulletsUp.Add(newBullet);
        }
        if(bulletsDown.Count < 3)
        {
            Bullet newBullet = spawnBullet(new Vector2(0.0f, -0.1f), new Vector2(0.0f, -1.0f));
            newBullet.bulletDirection = 2;
            newBullet.bulletListPosition = bulletsDown.Count;
            bulletsDown.Add(newBullet);
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
}
