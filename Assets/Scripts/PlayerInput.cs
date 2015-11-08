using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	private const float DEAD_ZONE = 0.1f;
	private const int ALL_BUT_TRIGGERS = ~(1 << 10);
	private new SpriteRenderer renderer;
	private Rigidbody2D rigidBody;
	private Collider2D wall;
	private bool jumpPressed = true;
	private bool inAir = false;
	private Vector3 spawnPoint;
	private float initialGravity;
	private float previousHorizontal = 0f;
	
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
		RaycastHit2D hit = Physics2D.Linecast(start, end, ALL_BUT_TRIGGERS);
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
		RaycastHit2D hit = Physics2D.Linecast(start, end, ALL_BUT_TRIGGERS);
		if (!hit) {
			// Try higher up
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hit = Physics2D.Linecast(start, end, ALL_BUT_TRIGGERS);
        }
		return hit && hit.normal.x == 1;
	}

	private bool isOnRightWall() {
		float lengthToSearch = 0.1f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.max.x + 0.001f, collider.bounds.center.y - collider.bounds.extents.y / 2f);
		Vector2 end = new Vector2(start.x + lengthToSearch, start.y);
		RaycastHit2D hit = Physics2D.Linecast(start, end, ALL_BUT_TRIGGERS);
		if (!hit) {
			// Try higher up
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hit = Physics2D.Linecast(start, end, ALL_BUT_TRIGGERS);
		}
		return hit && hit.normal.x == -1;
	}

	void Start() {
		renderer = GetComponent<SpriteRenderer>();
		rigidBody = GetComponent<Rigidbody2D>();
		spawnPoint = transform.position;
		jumpPressed = Input.GetButton("Jump");
		inAir = false;
		initialGravity = rigidBody.gravityScale;
	}

	void FixedUpdate() {
		// Determine if we're on a slope by determining
		// if the contact is angled and if we only make contact with one corner
		// or if the slope is next to us and it's angled
		var box = GetComponent<BoxCollider2D>();
		var centerStart = new Vector2(box.bounds.center.x, box.bounds.min.y - 0.001f);
		var centerEnd = new Vector2(centerStart.x, centerStart.y - 0.1f);
		var leftStart = new Vector2(box.bounds.min.x, centerStart.y);
		var leftEnd = new Vector2(leftStart.x, centerStart.y - 0.3f);
		var rightStart = new Vector2(box.bounds.max.x, centerStart.y);
		var rightEnd = new Vector2(rightStart.x, centerStart.y - 0.3f);
		var centerCollision = Physics2D.Linecast(centerStart, centerEnd, ALL_BUT_TRIGGERS);
		var leftCollision = Physics2D.Linecast(leftStart, leftEnd, ALL_BUT_TRIGGERS);
		var rightCollision = Physics2D.Linecast(rightStart, rightEnd, ALL_BUT_TRIGGERS);
		var rotated = false;
		var colliders = Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.y, LayerMask.GetMask("Ground"));
		foreach (var collider in colliders) {
			var angle = Mathf.Floor(collider.transform.eulerAngles.z);
			var isBelow = box.bounds.center.y < collider.bounds.max.y;
			var isLeftCollision = (leftCollision && leftCollision.collider == collider);
			var isRightCollision = (rightCollision && rightCollision.collider == collider);
			if (angle == 45f && (isBelow || (!centerCollision && !isLeftCollision && !isRightCollision))) { 
				rotated = true;
				transform.eulerAngles = new Vector3(0f, 0f, angle);
				break;
			}
		}
		if (!rotated) {
			transform.eulerAngles = Vector3.zero;
		}
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

		// Flip the sprite accordingly
		if (horizontal != 0f && Mathf.Sign(transform.localScale.x) != Mathf.Sign(horizontal)) {
			var localScale = transform.localScale;
			localScale.x *= -1;
			transform.localScale = localScale;
		}

		if (grounded) {
			if (horizontal == 0f) {
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
			if (prevJumpPressed && !jumpPressed && verticalSpeed > 0) {
				verticalSpeed = 0f;
			}
			if (horizontal > DEAD_ZONE) {
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
		var animator = GetComponent<Animator>();
		animator.SetBool("IsRunning", horizontalSpeed != 0f && !inAir);
		animator.SetBool("IsInAir", inAir);
		animator.SetBool("IsSliding", onWall);
		if (horizontalSpeed != 0f) {
			animator.speed = Mathf.Min(Mathf.Abs(horizontalSpeed) / 8f, 2f);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.CompareTag("LevelBoundary")) {
			Die();
		}
	}

	public void Die() {
		// Later, add sound effects etc.
		transform.position = spawnPoint;
		rigidBody.velocity = Vector2.zero;
	}
}
