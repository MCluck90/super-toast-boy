using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	private new SpriteRenderer renderer;
	private Rigidbody2D rigidBody;
	private Collider2D wall;
	private bool horizontalReleased = false;
	private bool jumpPressed = true;
	private Vector3 spawnPoint;
	private const float DEAD_ZONE = 0.1f;
	
	public float MaxSpeed;
	public float MaxRunSpeed;
	public float Acceleration;
	public float RunAcceleration;
	public float JumpSpeed;
	public float WallSlideRatio;
	public float TurnAroundAcceleration;
    public bool cheat = false;
	
	private bool isOnGround() {
		float lengthToSearch = 0.1f;
		float colliderThreshold = 0.001f;
		Vector2 lineStart = new Vector2(
			transform.position.x, 
			transform.position.y - renderer.bounds.extents.y - colliderThreshold
		);
		Vector2 vectorToSearch = new Vector2(transform.position.x, lineStart.y - lengthToSearch);
		RaycastHit2D hit = Physics2D.Linecast(lineStart, vectorToSearch);
		return hit;
	}

	void Start() {
		renderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
		spawnPoint = transform.position;
		jumpPressed = Input.GetButton("Jump");
	}

	void Update() {
		float horizontal = Input.GetAxisRaw("Horizontal");
		bool grounded = isOnGround();
		float acceleration;
		float maxSpeed;
		float horizontalSpeed = rigidBody.velocity.x;
		float verticalSpeed = rigidBody.velocity.y;
		bool prevJumpPressed = jumpPressed;
		jumpPressed = Input.GetButton("Jump");
		if (Input.GetButton("Run") || Input.GetAxis("Run") != 0) {
			acceleration = RunAcceleration;
			maxSpeed = MaxRunSpeed;
		} else {
			acceleration = Acceleration;
			maxSpeed = MaxSpeed;
		}

		if (horizontal == 0f) {
			horizontalSpeed = 0f;
		} else if (horizontal > DEAD_ZONE) {
			if (horizontalSpeed < 0f) {
				horizontalSpeed = 0f;
			} else {
				horizontalSpeed += acceleration * Time.deltaTime;
			}
		} else if (horizontal < DEAD_ZONE) {
			if (horizontalSpeed > 0f) {
				horizontalSpeed = 0f;
			} else {
				horizontalSpeed -= acceleration * Time.deltaTime;
			}
		}

		if (horizontalSpeed < -maxSpeed) {
			horizontalSpeed = -maxSpeed;
		} else if (horizontalSpeed > maxSpeed) {
			horizontalSpeed = maxSpeed;
		}

		if (!prevJumpPressed && jumpPressed) {
			verticalSpeed = JumpSpeed;
		} else if (prevJumpPressed && !jumpPressed) {
			verticalSpeed = 0f;
		}

		rigidBody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.CompareTag("Enemy")) {
            if (!cheat)
            {
                transform.position = spawnPoint;
                GameObject[] resetables = GameObject.FindGameObjectsWithTag("Resetable");

                for (int i = 0; i < resetables.Length; i++)
                {
                    resetables[i].gameObject.GetComponent<reset_script>().levelReset();
                }

                //make sure these are defaults
                this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;
                WallSlideRatio = 2;
            }
		} else if (!isOnGround() && collision.collider.CompareTag("Wall") && wall == null) {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;
            wall = collision.collider;
			rigidBody.velocity = new Vector2(0, rigidBody.velocity.y * WallSlideRatio);
		}else if (collision.collider.CompareTag("Finish")) {
			int i = Application.loadedLevel;
			Application.LoadLevel(i + 1);
		}else if (collision.collider.CompareTag("Win")){
            Application.LoadLevel("win");
        }
    }

	void OnCollisionExit2D(Collision2D collision) {
        if (collision.collider == wall) {
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;
            wall = null;
		}
	}
}
