using System;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

// The object that a player controls.
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class CharSquare : CharSquareBehavior
{

    public GameObject bullet;
    public int playerId = 0;
    public float playerSpeed = 300.0f;
    public float bulletSpeed = 500.0f;
	public int bulletDamage = 1;
    public int maxBullets = 3;
    public int health = 1;

    private Color[] colors = { Color.white, Color.blue, Color.magenta, Color.red, Color.green };
    private bool shoot = false;
    private List<MoveDirection> movementQueue = new List<MoveDirection>();
    private IDictionary<MoveDirection, MoveAxisData> movementAxisData = new Dictionary<MoveDirection, MoveAxisData>()
    {
        { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
        { MoveDirection.LEFT, new MoveAxisData("Left", MoveAxis.HORIZONTAL, "Left") },
        { MoveDirection.RIGHT, new MoveAxisData("Right", MoveAxis.HORIZONTAL, "Right") },
        { MoveDirection.UP, new MoveAxisData("Up", MoveAxis.VERTICAL, "Up") },
        { MoveDirection.DOWN, new MoveAxisData("Down", MoveAxis.VERTICAL, "Down") }
    };
    private IDictionary<BulletDirection, List<Bullet>> activeBullets = new Dictionary<BulletDirection, List<Bullet>>()
    {
        { BulletDirection.LEFT, new List<Bullet>() },
        { BulletDirection.RIGHT, new List<Bullet>() },
        { BulletDirection.UP, new List<Bullet>() },
        { BulletDirection.DOWN, new List<Bullet>() }
    };
    private IDictionary<BulletDirection, Vector2> bulletVelocities = new Dictionary<BulletDirection, Vector2>()
    {
        { BulletDirection.LEFT, new Vector2(-1.0f,0.0f) },
        { BulletDirection.RIGHT, new Vector2(1.0f,0.0f) },
        { BulletDirection.UP, new Vector2(0.0f,1.0f) },
        { BulletDirection.DOWN, new Vector2(0.0f,-1.0f) }
    };
    private bool alive = true;
    private bool hasWon = false;

    private Rigidbody2D rb2d;

    private enum MoveDirection
    {
        NO_MOVE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private enum MoveAxis
    {
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

	public static int ConvertBulletDirectionToInt(BulletDirection bulletDirection)
	{
		switch (bulletDirection)
		{
		case BulletDirection.LEFT:
			return 1;
		case BulletDirection.RIGHT:
			return 2;
		case BulletDirection.UP:
			return 3;
		case BulletDirection.DOWN:
			return 4;
		default:
			return 0;
		}
	}

	public static BulletDirection ConvertIntToBulletDirection(int bulletDirection)
	{
		switch (bulletDirection)
		{
		case 1:
			return BulletDirection.LEFT;
		case 2:
			return BulletDirection.RIGHT;
		case 3:
			return BulletDirection.UP;
		case 4:
			return BulletDirection.DOWN;
		default:
			return BulletDirection.LEFT;
		}
	}

    private class MoveAxisData
    {
        private String axisName;
        private MoveAxis axisAffected;
        private String buttonName;

        public MoveAxisData(String axisName, MoveAxis axisAffected, String buttonName)
        {
            this.axisName = axisName;
            this.axisAffected = axisAffected;
            this.buttonName = buttonName;
        }

        public String AxisName
        {
            get { return axisName; }
            set { axisName = value; }
        }

        public MoveAxis AxisAffected
        {
            get { return axisAffected; }
            set { axisAffected = value; }
        }

        public String ButtonName
        {
            get { return buttonName; }
            set { buttonName = value; }
        }
    }

    void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[playerId];
        if (playerId == 1)
        {
            movementAxisData = new Dictionary<MoveDirection, MoveAxisData>()
            {
                { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
                { MoveDirection.LEFT, new MoveAxisData("Left", MoveAxis.HORIZONTAL, "Left") },
                { MoveDirection.RIGHT, new MoveAxisData("Right", MoveAxis.HORIZONTAL, "Right") },
                { MoveDirection.UP, new MoveAxisData("Up", MoveAxis.VERTICAL, "Up") },
                { MoveDirection.DOWN, new MoveAxisData("Down", MoveAxis.VERTICAL, "Down") }
            };
        }
        else if (playerId == 2)
        {
            movementAxisData = new Dictionary<MoveDirection, MoveAxisData>()
            {
                { MoveDirection.NO_MOVE, new MoveAxisData(null, MoveAxis.NO_MOVE, null)  },
                { MoveDirection.LEFT, new MoveAxisData("Left2", MoveAxis.HORIZONTAL, "Left2") },
                { MoveDirection.RIGHT, new MoveAxisData("Right2", MoveAxis.HORIZONTAL, "Right2") },
                { MoveDirection.UP, new MoveAxisData("Up2", MoveAxis.VERTICAL, "Up2") },
                { MoveDirection.DOWN, new MoveAxisData("Down2", MoveAxis.VERTICAL, "Down2") }
            };
        }
        foreach (MoveDirection direction in Enum.GetValues(typeof(MoveDirection)))
        {
            movementQueue.Add(direction);
        }
    }

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Run every frame.
    // Check for input from player.
    // If the player has lost all their health we need to process as needed.
    // If the player was determined to have won the level we need to process as needed.
    void Update()
    {

		if(!networkObject.IsOwner)
		{/*
			if (rb2d.position != networkObject.position)
			{
				rb2d.position = networkObject.position;
			}*/
			return;
		}

        // Process the case that the player has won the level.
        if (hasWon)
        {
            HasWon();
        }
        else
        {
            // Process the case that the player has lost all their health.
            if ((health < 1) && alive)
            {
                NoHealth();
                alive = false;
            }
            else if (alive)
            {
                // Detect input presses if the player has not won and has not lost all their health.
                foreach (MoveDirection moveDirection in movementAxisData.Keys)
                {
                    MoveAxisData axis;
                    movementAxisData.TryGetValue(moveDirection, out axis);
                    if (axis.ButtonName != null)
                    {
                        if (Input.GetButtonDown(axis.ButtonName))
                        {
							MoveDirectionToFrontOfQueue(moveDirection);
                        }
                        else if (Input.GetButtonUp(axis.ButtonName))
                        {
							MoveDirectionToBackOfQueue(moveDirection);
                        }
                    }
                }

                if (Input.GetButtonDown("Fire") && (playerId == 1))
                {
                    shoot = true;
                }
                else if (Input.GetButtonDown("Fire2") && (playerId == 2))
                {
                    shoot = true;
                }
            }
        }
		/*
		if(!networkObject.IsOwner)
		{
			if (rb2d.position != networkObject.position)
			{
				rb2d.position = networkObject.position;
			}
			return;
		}

		// Determine the direction and speed of movement for the current interval.
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

		// Use raycasting to detect a collision if movement in this interval would cause a collision with a relevant object.
		RaycastHit2D[] wallCollision = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D[] wallCollisionUpper = Physics2D.RaycastAll(new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f), transform.position.y + (Mathf.Abs(direction.x) * 0.16f)), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
		RaycastHit2D[] wallCollisionLower = Physics2D.RaycastAll(new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f), transform.position.y - (Mathf.Abs(direction.x) * 0.16f)), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));

		// Determine if any collisions at all were detected.
		RaycastHit2D[][] collisionsDetected = { wallCollision, wallCollisionUpper, wallCollisionLower };
		bool collisionDetectedFromRayCast = false;
		foreach (RaycastHit2D[] collisionDetected in collisionsDetected)
		{
			foreach (RaycastHit2D collision in collisionDetected)
			{
				if (collision)
				{
					collisionDetectedFromRayCast = true;
					break;
				}
			}
			if (collisionDetectedFromRayCast)
			{
				break;
			}
		}

		// If the raycasts return a collision we need to process what we want to do.
		// Otherwise we move as normal.
		if (collisionDetectedFromRayCast)
		{

			// We will use these attributes to determine which raycasts detected a collision and the distances between them to determine which collision is closest.
			RaycastHit2D[] collisionDetected = { new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D() };
			bool[] isWallCollision = { false, false, false };
			Vector2[] wallCollisionDistance = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

			// Here we calculate which raycasts detected collisions and the distances between the player and the object for the wall layer.
			/* Physics2D.RaycastAll returns an array of collisions in the order of closest first.
               We use this to assume that the first object we find in the RaycastHit2D array that we care about colliding with is the closest object to the player in the array that we care about colliding with.
               If this changes and Physics2D.RaycastAll returns an unsorted array this will need to be revisted to determine which object in the array is closest to the player.
            
			int count = 0;
			foreach (RaycastHit2D[] collisions in collisionsDetected)
			{
				foreach (RaycastHit2D collision in collisions)
				{
					if (collision && !isWallCollision[count])
					{
						if (collision.rigidbody.CompareTag("Wall"))
						{
							rb2d.velocity = Vector2.zero;
							Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
							Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
							Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

							wallCollisionDistance[count] = boundaryOffset - squareOffset;
							isWallCollision[count] = true;
							collisionDetected[count] = collision;
						}
						else if (collision.rigidbody.CompareTag("GameBoundary"))
						{
							rb2d.velocity = Vector2.zero;
							Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
							Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + (((1.0f * -direction.x) * wallTransform.localScale.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + (((1.0f * -direction.y) * wallTransform.localScale.y) / 2));
							Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

							wallCollisionDistance[count] = boundaryOffset - squareOffset;
							isWallCollision[count] = true;
							collisionDetected[count] = collision;
						}
						else if (collision.rigidbody.CompareTag("PlayerSpecialWall"))
						{
							if (((SquarePlayerWall)collision.rigidbody.GetComponent<SquarePlayerWall>()).playerId != playerId)
							{
								rb2d.velocity = Vector2.zero;
								Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
								Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
								Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

								wallCollisionDistance[count] = boundaryOffset - squareOffset;
								isWallCollision[count] = true;
								collisionDetected[count] = collision;
							}
						}
						else if (collision.rigidbody.CompareTag("WinnersSquare"))
						{
							rb2d.velocity = Vector2.zero;
							Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
							Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
							Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

							wallCollisionDistance[count] = boundaryOffset - squareOffset;
							isWallCollision[count] = true;
							collisionDetected[count] = collision;
						}
					}
					else
					{
						break;
					}
				}
				count++;
			}

			// Check to see if we actually detected any collisions that we care about colliding with.
			bool wallCollisions = false;
			foreach (bool wallCollisionDetected in isWallCollision)
			{
				if (wallCollisionDetected)
				{
					wallCollisions = true;
					break;
				}
			}

			// If we didn't detect any relevant collisions the player will move as normal.
			// If we detected a relevant collision we need to determine which raytrace (upper, middle, lower) found the closest collision and move the player the difference between themselves and the closest object they collided with to bring them flush against the object.
			// If the closest relevant collision is a WinnersSquare we set hasWon to true which will be processed through the next update frame.
			if (!wallCollisions)
			{
				rb2d.velocity = velocity;
			}
			else
			{
				int countIndex = 0;
				Vector2 useVector = Vector2.zero;
				RaycastHit2D useCollision = new RaycastHit2D();
				bool firstVectorFound = false;
				foreach (bool hasCollided in isWallCollision)
				{
					if (hasCollided)
					{
						if (!firstVectorFound)
						{
							useVector = wallCollisionDistance[countIndex];
							useCollision = collisionDetected[countIndex];
							firstVectorFound = true;
						}
						else
						{
							if (Mathf.Abs(wallCollisionDistance[countIndex].x + wallCollisionDistance[countIndex].y) < Mathf.Abs(useVector.x + useVector.y))
							{
								useVector = wallCollisionDistance[countIndex];
								useCollision = collisionDetected[countIndex];
							}
						}
					}
					countIndex++;
				}
				rb2d.position += useVector;
				if (useCollision.rigidbody.CompareTag("WinnersSquare"))
				{
					hasWon = true;
				}
			}
		}
		else
		{
			rb2d.velocity = velocity;
		}

		networkObject.position = rb2d.position;

		// Shoot bullets if the player pressed the shoot button.
		if (shoot)
		{
			Shoot();
		}*/
    }

    // Run at a consistant interval.
    // Handle input from player.
    // Collision detection using raycasting.
    void FixedUpdate()
    {
		if(!networkObject.IsOwner)
		{
			if (rb2d.position != networkObject.position)
			{
				rb2d.position = networkObject.position;
			}
			return;
		}

        // Determine the direction and speed of movement for the current interval.
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

        // Use raycasting to detect a collision if movement in this interval would cause a collision with a relevant object.
        RaycastHit2D[] wallCollision = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        RaycastHit2D[] wallCollisionUpper = Physics2D.RaycastAll(new Vector2(transform.position.x + (Mathf.Abs(direction.y) * 0.16f), transform.position.y + (Mathf.Abs(direction.x) * 0.16f)), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));
        RaycastHit2D[] wallCollisionLower = Physics2D.RaycastAll(new Vector2(transform.position.x - (Mathf.Abs(direction.y) * 0.16f), transform.position.y - (Mathf.Abs(direction.x) * 0.16f)), direction, (Mathf.Abs(direction.x * 0.16f) + (Mathf.Abs(velocity.x) * Time.fixedDeltaTime)) + (Mathf.Abs(direction.y * 0.16f) + (Mathf.Abs(velocity.y) * Time.fixedDeltaTime)), 1 << LayerMask.NameToLayer("Wall"));

        // Determine if any collisions at all were detected.
        RaycastHit2D[][] collisionsDetected = { wallCollision, wallCollisionUpper, wallCollisionLower };
        bool collisionDetectedFromRayCast = false;
        foreach (RaycastHit2D[] collisionDetected in collisionsDetected)
        {
            foreach (RaycastHit2D collision in collisionDetected)
            {
                if (collision)
                {
                    collisionDetectedFromRayCast = true;
                    break;
                }
            }
            if (collisionDetectedFromRayCast)
            {
                break;
            }
        }
        
        // If the raycasts return a collision we need to process what we want to do.
        // Otherwise we move as normal.
        if (collisionDetectedFromRayCast)
        {

            // We will use these attributes to determine which raycasts detected a collision and the distances between them to determine which collision is closest.
            RaycastHit2D[] collisionDetected = { new RaycastHit2D(), new RaycastHit2D(), new RaycastHit2D() };
            bool[] isWallCollision = { false, false, false };
            Vector2[] wallCollisionDistance = { new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f), new Vector2(0.0f, 0.0f) };

            // Here we calculate which raycasts detected collisions and the distances between the player and the object for the wall layer.
            /* Physics2D.RaycastAll returns an array of collisions in the order of closest first.
               We use this to assume that the first object we find in the RaycastHit2D array that we care about colliding with is the closest object to the player in the array that we care about colliding with.
               If this changes and Physics2D.RaycastAll returns an unsorted array this will need to be revisted to determine which object in the array is closest to the player.
            */
            int count = 0;
            foreach (RaycastHit2D[] collisions in collisionsDetected)
            {
                foreach (RaycastHit2D collision in collisions)
                {
                    if (collision && !isWallCollision[count])
                    {
                        if (collision.rigidbody.CompareTag("Wall"))
                        {
							rb2d.velocity = Vector2.zero;
                            Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                            Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                            Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                            wallCollisionDistance[count] = boundaryOffset - squareOffset;
                            isWallCollision[count] = true;
                            collisionDetected[count] = collision;
                        }
                        else if (collision.rigidbody.CompareTag("GameBoundary"))
                        {
							rb2d.velocity = Vector2.zero;
                            Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                            Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + (((1.0f * -direction.x) * wallTransform.localScale.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + (((1.0f * -direction.y) * wallTransform.localScale.y) / 2));
                            Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                            wallCollisionDistance[count] = boundaryOffset - squareOffset;
                            isWallCollision[count] = true;
                            collisionDetected[count] = collision;
                        }
                        else if (collision.rigidbody.CompareTag("PlayerSpecialWall"))
                        {
                            if (((SquarePlayerWall)collision.rigidbody.GetComponent<SquarePlayerWall>()).playerId != playerId)
                            {
								rb2d.velocity = Vector2.zero;
                                Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                                Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                                Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                                wallCollisionDistance[count] = boundaryOffset - squareOffset;
                                isWallCollision[count] = true;
                                collisionDetected[count] = collision;
                            }
                        }
                        else if (collision.rigidbody.CompareTag("WinnersSquare"))
                        {
							rb2d.velocity = Vector2.zero;
                            Transform wallTransform = collision.rigidbody.GetComponent<Transform>();
                            Vector2 boundaryOffset = new Vector2((wallTransform.position.x * Mathf.Abs(direction.x)) + ((0.33f * -direction.x) / 2), (wallTransform.position.y * Mathf.Abs(direction.y)) + ((0.33f * -direction.y) / 2));
                            Vector2 squareOffset = new Vector2((transform.position.x * Mathf.Abs(direction.x)) + ((0.33f * direction.x) / 2), (transform.position.y * Mathf.Abs(direction.y)) + ((0.33f * direction.y) / 2));

                            wallCollisionDistance[count] = boundaryOffset - squareOffset;
                            isWallCollision[count] = true;
                            collisionDetected[count] = collision;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                count++;
            }

            // Check to see if we actually detected any collisions that we care about colliding with.
            bool wallCollisions = false;
            foreach (bool wallCollisionDetected in isWallCollision)
            {
                if (wallCollisionDetected)
                {
                    wallCollisions = true;
                    break;
                }
            }

            // If we didn't detect any relevant collisions the player will move as normal.
            // If we detected a relevant collision we need to determine which raytrace (upper, middle, lower) found the closest collision and move the player the difference between themselves and the closest object they collided with to bring them flush against the object.
            // If the closest relevant collision is a WinnersSquare we set hasWon to true which will be processed through the next update frame.
            if (!wallCollisions)
            {
				rb2d.velocity = velocity;
            }
            else
            {
                int countIndex = 0;
				Vector2 useVector = Vector2.zero;
                RaycastHit2D useCollision = new RaycastHit2D();
                bool firstVectorFound = false;
                foreach (bool hasCollided in isWallCollision)
                {
                    if (hasCollided)
                    {
                        if (!firstVectorFound)
                        {
                            useVector = wallCollisionDistance[countIndex];
                            useCollision = collisionDetected[countIndex];
                            firstVectorFound = true;
                        }
                        else
                        {
                            if (Mathf.Abs(wallCollisionDistance[countIndex].x + wallCollisionDistance[countIndex].y) < Mathf.Abs(useVector.x + useVector.y))
                            {
                                useVector = wallCollisionDistance[countIndex];
                                useCollision = collisionDetected[countIndex];
                            }
                        }
                    }
                    countIndex++;
                }
                rb2d.position += useVector;
                if (useCollision.rigidbody.CompareTag("WinnersSquare"))
                {
                    hasWon = true;
                }
            }
        }
        else
        {
			rb2d.velocity = velocity;
        }

		networkObject.position = rb2d.position;

        // Shoot bullets if the player pressed the shoot button.
        if (shoot)
        {
            Shoot();
        }
    }

	public void SetPlayerId(int newPlayerId)
	{
		networkObject.SendRpc(RPC_UPDATE_PLAYER_ID, Receivers.AllBuffered, newPlayerId);
	}

	public override void UpdatePlayerId(RpcArgs args)
	{
		playerId = args.GetNext<int>();
	}
		
	/*
    // The shoot function shoots a bullet in each direction.
    // It will only spawn a bullet in a particular direction if there are less than maxBullets in that direction.
    private void Shoot()
    {
        foreach (BulletDirection bullets in activeBullets.Keys)
        {
            List<Bullet> bulletList;
            activeBullets.TryGetValue(bullets, out bulletList);
            if (bulletList.Count < maxBullets)
            {
                Vector2 bulletVelocity;
                bulletVelocities.TryGetValue(bullets, out bulletVelocity);
                
				Vector2 bulletDirection = bulletVelocity;
				bulletDirection.Normalize();
				var newBullet = NetworkManager.Instance.InstantiateBullet(0, transform.position, Quaternion.identity, true);
				((Bullet)newBullet.GetComponent<Bullet>()).InitialiseBullet(bulletDirection, bulletSpeed, bulletDamage, playerId, ConvertBulletDirectionToInt(bullets), bulletList.Count);
				bulletList.Add((Bullet)newBullet.GetComponent<Bullet>());
				/*
				Bullet newBullet = SpawnBullet(new Vector2(0.0f, 0.0f), bulletVelocity);
                newBullet.bulletDirection = bullets;
                newBullet.bulletListPosition = bulletList.Count;
                bulletList.Add(newBullet);*//*
            }
        }
        shoot = false;
    }

    // Function used by shoot to simply spawn a bullet moving in a particular direction.
    // Doesn't add the bullet to the activeBullets dictionary or any other useful overhead.
    private Bullet SpawnBullet(Vector2 position, Vector2 direction)
    {
        direction.Normalize();
        GameObject newBullet = (GameObject)Instantiate(bullet, transform.position + new Vector3(position.x, position.y, 0.0f), Quaternion.identity);
		((Bullet)newBullet.GetComponent<Bullet>()).direction = direction;
		((Bullet)newBullet.GetComponent<Bullet> ()).speed = bulletSpeed;
        //((Rigidbody2D)newBullet.GetComponent<Rigidbody2D>()).AddForce(direction * bulletSpeed);
        ((Bullet)newBullet.GetComponent<Bullet>()).player = this;
        ((Bullet)newBullet.GetComponent<Bullet>()).playerId = playerId;
        return (Bullet)newBullet.GetComponent<Bullet>();
    }*/

	// The shoot function shoots a bullet in each direction.
	// It will only spawn a bullet in a particular direction if there are less than maxBullets in that direction.
	private void Shoot()
	{
		foreach (BulletDirection bullets in activeBullets.Keys)
		{
			List<Bullet> bulletList;
			activeBullets.TryGetValue(bullets, out bulletList);
			if (bulletList.Count < maxBullets)
			{
				Vector2 bulletVelocity;
				bulletVelocities.TryGetValue(bullets, out bulletVelocity);
				//Bullet newBullet = SpawnBullet(new Vector2(0.0f, 0.0f), bulletVelocity);
				bulletVelocity.Normalize();
				Bullet newBullet = CreateBullet(bulletVelocity, bulletSpeed, bulletDamage, playerId, ConvertBulletDirectionToInt(bullets), bulletList.Count, transform.position);
				//newBullet.bulletDirection = bullets;
				//newBullet.bulletListPosition = bulletList.Count;
				bulletList.Add(newBullet);
			}
		}
		shoot = false;
	}

	/*
	// Function used by shoot to simply spawn a bullet moving in a particular direction.
	// Doesn't add the bullet to the activeBullets dictionary or any other useful overhead.
	private Bullet SpawnBullet(Vector2 position, Vector2 direction)
	{
		direction.Normalize();
		GameObject newBullet = (GameObject)Instantiate(bullet, transform.position + new Vector3(position.x, position.y, 0.0f), Quaternion.identity);
		((Bullet)newBullet.GetComponent<Bullet>()).player = this;
		((Bullet)newBullet.GetComponent<Bullet>()).playerId = playerId;
		return (Bullet)newBullet.GetComponent<Bullet>();
	}*/

	private Bullet CreateBullet(Vector2 newDirection, float newSpeed, int newDamage, int newPlayerId, int newBulletDirection, int newBulletListPosition, Vector2 newPosition)
	{
		networkObject.SendRpc(RPC_SPAWN_BULLET, Receivers.OthersBuffered, newDirection, newSpeed, newDamage, newPlayerId, newBulletDirection, newBulletListPosition, newPosition);
		Bullet newBullet = InstantiateBullet(newDirection, newSpeed, newDamage, newPlayerId, newBulletDirection, newBulletListPosition, newPosition);

		return newBullet;
	}

	private Bullet InstantiateBullet(Vector2 newDirection, float newSpeed, int newDamage, int newPlayerId, int newBulletDirection, int newBulletListPosition, Vector2 newPosition)
	{
		GameObject newBullet = (GameObject)Instantiate(bullet, newPosition, Quaternion.identity);
		//Vector2 newDirection = args.GetNext<Vector2>();
		//float newSpeed = args.GetNext<float>();
		//int newDamage = args.GetNext<int>();
		//int newPlayerId = args.GetNext<int>();
		//int newBulletDirection = args.GetNext<int>();
		//int newBulletListPosition = args.GetNext<int>();

		((Bullet)newBullet.GetComponent<Bullet>()).InitialiseBullet(newDirection, newSpeed, newDamage, newPlayerId, newBulletDirection, newBulletListPosition);
		//((Bullet)newBullet.GetComponent<Bullet>()).player = this;
		//((Bullet)newBullet.GetComponent<Bullet>()).playerId = playerId;

		return (Bullet)newBullet.GetComponent<Bullet>();
	}

	public override void SpawnBullet(RpcArgs args)
	{
		Vector2 newDirection = args.GetNext<Vector2>();
		float newSpeed = args.GetNext<float>();
		int newDamage = args.GetNext<int>();
		int newPlayerId = args.GetNext<int>();
		int newBulletDirection = args.GetNext<int>();
		int newBulletListPosition = args.GetNext<int>();
		Vector2 newPosition = args.GetNext<Vector2>();
		InstantiateBullet(newDirection, newSpeed, newDamage, newPlayerId, newBulletDirection, newBulletListPosition, newPosition);
		/*
		GameObject newBullet = (GameObject)Instantiate(bullet, transform.position, Quaternion.identity);
		Vector2 newDirection = args.GetNext<Vector2>();
		float newSpeed = args.GetNext<float>();
		int newDamage = args.GetNext<int>();
		int newPlayerId = args.GetNext<int>();
		int newBulletDirection = args.GetNext<int>();
		int newBulletListPosition = args.GetNext<int>();

		((Bullet)newBullet.GetComponent<Bullet>()).InitialiseBullet(newDirection, newSpeed, newDamage, newPlayerId, newBulletDirection, newBulletListPosition);
		//((Bullet)newBullet.GetComponent<Bullet>()).player = this;
		//((Bullet)newBullet.GetComponent<Bullet>()).playerId = playerId;
		//newBullet.bulletDirection = bullets;
		//newBullet.bulletListPosition = bulletList.Count;
		//bulletList.Add(newBullet);*/
	}

    // Removes a single bullet from the activeBullets dictionary.
    // Used by the Bullet class to remove itself when it detects it's own collisions and needs to destroy itself.
    public void RemoveBullet(BulletDirection direction, int listPosition)
    {
        List<Bullet> bulletList;
        activeBullets.TryGetValue(direction, out bulletList);
        bulletList.RemoveAt(listPosition);
        int count = 0;
        foreach (Bullet bullet in bulletList)
        {
            if (bullet.bulletListPosition != count)
            {
                bullet.bulletListPosition = count;
            }
            count++;
        }
    }

    // Puts a movement input to the back of the movement queue.
    // Used when releasing a movement input key to indicate that you no longer want to move in that direction.
    private void MoveDirectionToBackOfQueue(MoveDirection direction)
    {
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

    // Puts a movement input to the front of the movement queue.
    // Used when pressing a movement input key to indicate that you want to move in that direction.
    private void MoveDirectionToFrontOfQueue(MoveDirection direction)
    {
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

    // If the player runs out of health destroy all their bullets and player special walls, as well as themselves.
    private void NoHealth()
    {

        foreach (BulletDirection bulletLists in activeBullets.Keys)
        {
            List<Bullet> bulletList;
            activeBullets.TryGetValue(bulletLists, out bulletList);
            int count = bulletList.Count - 1;
            while (bulletList.Count > 0)
            {
				Bullet destroyBullet = bulletList[count];
				bulletList.RemoveAt(count);
				destroyBullet.DestroyObject();
                count--;
            }
        }

        GameObject[] playerSpecialWalls = GameObject.FindGameObjectsWithTag("PlayerSpecialWall");
        foreach (GameObject playerWall in playerSpecialWalls)
        {
			if (((SquarePlayerWall)playerWall.GetComponent<SquarePlayerWall>()).playerId == playerId)
            {
				((SquarePlayerWall)playerWall.GetComponent<SquarePlayerWall>()).DestroySelfOnNetwork();
            }
        }

		networkObject.Destroy();
    }

    // If the player has won the level process as desired.
    private void HasWon()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
