using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	private new SpriteRenderer renderer;
	private Rigidbody2D rigidBody;
	private Collider2D wall;
	private bool jumpPressed = true;
	private bool inAir = false;
	private Vector3 spawnPoint;
	private const float DEAD_ZONE = 0.1f;
	private float initialGravity;
	private float previousHorizontal = 0f;
	private float prevXSpeed = 0f;
	
	public float MaxSpeed;
	public float MaxRunSpeed;
	public float Acceleration;
	public float RunAcceleration;
	public float JumpSpeed;
	public float WallJumpHorizontalSpeed;
	public float WallJumpVerticalSpeed;
	
	private bool isOnGround(out Vector2 normal) {
		float lengthToSearch = 0.1f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.center.x, collider.bounds.min.y - lengthToSearch);
		Vector2 end = new Vector2(start.x, start.y - lengthToSearch);
		RaycastHit2D hit = Physics2D.Linecast(start, end);
		if (hit) {
			normal = hit.normal;
		} else {
			normal = Vector2.zero;
		}
		return hit;
	}

	private bool isOnLeftWall() {
		float lengthToSearch = 0.05f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.min.x - 0.001f, collider.bounds.center.y - collider.bounds.extents.y / 2f);
		Vector2 end = new Vector2(start.x - lengthToSearch, start.y);
		RaycastHit2D hit = Physics2D.Linecast(start, end);
		if (!hit) {
			// Try higher up
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hit = Physics2D.Linecast(start, end);
        }
		return hit && hit.normal.x == 1;
	}

	private bool isOnRightWall() {
		float lengthToSearch = 0.1f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.max.x + 0.001f, collider.bounds.center.y - collider.bounds.extents.y / 2f);
		Vector2 end = new Vector2(start.x + lengthToSearch, start.y);
		RaycastHit2D hit = Physics2D.Linecast(start, end);
		if (!hit) {
			// Try higher up
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hit = Physics2D.Linecast(start, end);
		}
		return hit && hit.normal.x == -1;
	}

	void Start() {
		renderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
		spawnPoint = transform.position;
		jumpPressed = Input.GetButton("Jump");
		initialGravity = rigidBody.gravityScale;
	}

	void Update() {
		float horizontal = Input.GetAxisRaw("Horizontal");
		Vector2 groundNormal;
		bool grounded = isOnGround(out groundNormal);
		bool hitLeftWall = isOnLeftWall();
		bool hitRightWall = isOnRightWall();
		bool onWall = hitLeftWall || hitRightWall;
		bool wasInAir = inAir;
		float horizontalSpeed = rigidBody.velocity.x;
		float verticalSpeed = rigidBody.velocity.y;
		bool prevJumpPressed = jumpPressed;
		bool holdingRun = (Input.GetButton("Run") || Input.GetAxis("Run") != 0);
		float acceleration;
		float maxSpeed;
		jumpPressed = Input.GetButton("Jump");
		inAir = !grounded && !onWall;
		if (holdingRun) {
			acceleration = RunAcceleration;
			maxSpeed = MaxRunSpeed;
		} else {
			acceleration = Acceleration;
			maxSpeed = MaxSpeed;
		}

		if (grounded) {
			if (horizontal == 0f || wasInAir) {
				horizontalSpeed = 0f;
			} else if (horizontal > DEAD_ZONE) {
				if (horizontalSpeed < 0f) {
					horizontalSpeed = 0f;
				}
				else {
					horizontalSpeed += acceleration * Time.deltaTime;
				}
			} else if (horizontal < -DEAD_ZONE) {
				if (horizontalSpeed > 0f) {
					horizontalSpeed = 0f;
				}
				else {
					horizontalSpeed -= acceleration * Time.deltaTime;
				}
			}
			
			if (!prevJumpPressed && jumpPressed) {
				verticalSpeed = JumpSpeed;
			}

			var normalAngle = Mathf.Floor(Vector2.Angle(transform.up, groundNormal));
			if (normalAngle == 45f) {
				verticalSpeed = horizontalSpeed;
			}
		} else if (inAir) {
			transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.up);
			if (prevJumpPressed && !jumpPressed) {
				verticalSpeed = 0f;
			}
			if (Mathf.Sign(previousHorizontal) != Mathf.Sign(horizontal)) {
				horizontalSpeed = 0f;
			} else if (horizontal > DEAD_ZONE) {
				horizontalSpeed += acceleration * Time.deltaTime;
			}
			else if (horizontal < -DEAD_ZONE) {
				horizontalSpeed -= acceleration * Time.deltaTime;
			}
		} else if (onWall) {
			if (!prevJumpPressed && jumpPressed) {
				verticalSpeed = WallJumpVerticalSpeed;
				maxSpeed = (holdingRun) ? MaxRunSpeed : MaxSpeed;
				if (hitLeftWall) {
					horizontalSpeed = WallJumpHorizontalSpeed;
				} else {
					horizontalSpeed = -WallJumpHorizontalSpeed;
				}
			}
		}
		
		if (horizontalSpeed < -maxSpeed) {
			horizontalSpeed = -maxSpeed;
		} else if (horizontalSpeed > maxSpeed) {
			horizontalSpeed = maxSpeed;
		}

		previousHorizontal = horizontal;
        rigidBody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
		prevXSpeed = horizontalSpeed;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		var angle = 0f;
		Vector2 normal = Vector2.zero;
		foreach (var contact in collision.contacts) {
			var possibleAngle = Vector2.Angle(Vector2.up, contact.normal);
			if (Mathf.Abs(angle) < 46) {
				angle = possibleAngle;
				normal = contact.normal;
				if (angle == 0f) {
					break;
				}
			}
		}
		if (Mathf.Abs(angle) < 46) {
			transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
		}
    }
}
