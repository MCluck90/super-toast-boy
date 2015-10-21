using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	private new SpriteRenderer renderer;
	private Rigidbody2D rigidBody;
	private Collider2D wall;
	private bool horizontalReleased = false;
	private bool jumpReleased = true;
	private Vector3 spawnPoint;

	public float MaxSpeed;
	public float MaxRunSpeed;
	public float Acceleration;
	public float RunAcceleration;
	public float JumpSpeed;
	public float WallSlideRatio;
	public float TurnAroundAcceleration;
	
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
	}

	void Update() {
		float horizontal = Input.GetAxis("Horizontal");
		bool grounded = isOnGround();
		float acceleration;
		float maxSpeed;
		if (Input.GetButton("Run")) {
			acceleration = RunAcceleration;
			maxSpeed = MaxRunSpeed;
		} else {
			acceleration = Acceleration;
			maxSpeed = MaxSpeed;
		}
		if (horizontal < -0.1f) {
			// Left
			if (rigidBody.velocity.x > -maxSpeed) {
				if (rigidBody.velocity.x > 0) {
					acceleration *= TurnAroundAcceleration;
				}
				rigidBody.AddForce(new Vector2(-acceleration, 0.0f));
			} else {
				rigidBody.velocity = new Vector2(-maxSpeed, rigidBody.velocity.y);
			}
		} else if (horizontal > 0.1f) {
			// Right
			if (rigidBody.velocity.x < maxSpeed) {
				if (rigidBody.velocity.x < 0) {
					acceleration *= TurnAroundAcceleration;
				}
				rigidBody.AddForce(new Vector2(acceleration, 0.0f));
			}
			else {
				rigidBody.velocity = new Vector2(maxSpeed, rigidBody.velocity.y);
			}
		} else {
			if (horizontalReleased) {
				horizontalReleased = false;
				rigidBody.velocity = new Vector2(0.0f, rigidBody.velocity.y);
			}
			horizontalReleased = true;
		}
		
		if (Input.GetButton("Jump") && jumpReleased) {
			jumpReleased = false;
			if (grounded) {
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, this.JumpSpeed);
			} else if (wall != null) {
				float wallJumpDirection = (wall.transform.position.x < transform.position.x) ? 1.0f : -1.0f;
				rigidBody.velocity = new Vector2(this.JumpSpeed * wallJumpDirection, this.JumpSpeed);
			}
		} else if (!grounded && rigidBody.velocity.y > 0 && jumpReleased) {
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0.0f);
		}

		if (!Input.GetButton("Jump")) {
			jumpReleased = true;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (collision.collider.CompareTag("Enemy")) {
			transform.position = spawnPoint;
            GameObject[] resetables = GameObject.FindGameObjectsWithTag("Resetable");

            for(int i=0; i<resetables.Length; i++)
            {
                resetables[i].gameObject.GetComponent<reset_script>().levelReset();
            }

            //make sure these are defaults
            this.gameObject.GetComponent<Rigidbody2D>().gravityScale = 6;
            WallSlideRatio = 2;
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
