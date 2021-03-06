﻿using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
	public enum PowerUp {
		None,
		Jelly
	}

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
	private bool isInvincible = false;

	private AudioSource source;
	private float volLowRange = .5f;
	private float volHighRange = 1.0f;
	public AudioClip JumpSound;
	
	public float MaxSpeed;
	public float MaxRunSpeed;
	public float Acceleration;
	public float RunAcceleration;
	public float JumpSpeed;
	public float WallJumpHorizontalSpeed;
	public float WallJumpVerticalSpeed;
	public PowerUp PowerUpState = PowerUp.None;
	
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

	private bool isOnLeftWall(out bool isSlippery) {
		isSlippery = false;
		float lengthToSearch = 0.05f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.min.x - 0.001f, collider.bounds.center.y - collider.bounds.extents.y / 2f);
		Vector2 end = new Vector2(start.x - lengthToSearch, start.y);
		RaycastHit2D[] hits = Physics2D.LinecastAll(start, end, ALL_BUT_TRIGGERS);
		if (hits.Length == 0) {
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hits = Physics2D.LinecastAll(start, end, ALL_BUT_TRIGGERS);
		}

		bool result = false;
		foreach (var hit in hits) {
			if (PowerUpState != PowerUp.Jelly && hit.transform.gameObject.CompareTag("Slippery")) {
				isSlippery = true;
			}
			var normal = Mathf.Abs(hit.normal.x);
			if (normal >= 0.999 && normal <= 1.0001) {
				result = true;
			}
		}

		return result;
	}

	private bool isOnRightWall(out bool isSlippery) {
		isSlippery = false;
		float lengthToSearch = 0.05f;
		var collider = GetComponent<BoxCollider2D>();
		Vector2 start = new Vector2(collider.bounds.max.x + 0.001f, collider.bounds.center.y - collider.bounds.extents.y / 2f);
		Vector2 end = new Vector2(start.x + lengthToSearch, start.y);
		RaycastHit2D[] hits = Physics2D.LinecastAll(start, end, ALL_BUT_TRIGGERS);
		if (hits.Length == 0) {
			start.y -= collider.bounds.extents.y / 2f;
			end.y = start.y;
			hits = Physics2D.LinecastAll(start, end, ALL_BUT_TRIGGERS);
		}

		bool result = false;
		foreach (var hit in hits) {
			if (PowerUpState != PowerUp.Jelly && hit.transform.gameObject.CompareTag("Slippery")) {
				isSlippery = true;
			}
			var normal = Mathf.Abs(hit.normal.x);
			if (normal >= 0.999 && normal <= 1.0001) {
				result = true;
			}
		}
		return result;
	}


	void Awake(){
		source = GetComponent<AudioSource> ();
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
		Cheats();

		float horizontal = Input.GetAxisRaw("Horizontal");
		Vector2 groundNormal;
		bool slipperyLeft;
		bool slipperyRight;
		bool grounded = isOnGround(out groundNormal);
		bool hitLeftWall = isOnLeftWall(out slipperyLeft);
		bool hitRightWall = isOnRightWall(out slipperyRight);
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

		// Make sure that if the gravity has been tweaked, change it back
		rigidBody.gravityScale = initialGravity;

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
				source.PlayOneShot(JumpSound,1F);

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
		} else if (onWall && ((hitLeftWall && !slipperyLeft) || (hitRightWall && !slipperyRight))) {
			if (PowerUpState == PowerUp.Jelly) {
				verticalSpeed = 0f;
				rigidBody.gravityScale = 0f;
			}
			if (!prevJumpPressed && jumpPressed) {
				verticalSpeed = WallJumpVerticalSpeed;
				maxSpeed = (holdingRun) ? MaxRunSpeed : MaxSpeed;
				if (hitLeftWall) {
					horizontalSpeed = WallJumpHorizontalSpeed;
					source.PlayOneShot(JumpSound,1F);
				} else {
					horizontalSpeed = -WallJumpHorizontalSpeed;
					source.PlayOneShot(JumpSound,1F);
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

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag("Win")) {
			Application.LoadLevel(Application.loadedLevel + 1);
		} else if (other.CompareTag("Enemy") && !isInvincible) {
			Die();
		} else if (other.CompareTag("JellyJar")) {
			PowerUpState = PowerUp.Jelly;
			other.gameObject.SetActive(false);
		}
	}

	public void Die() {
		// Later, add sound effects etc.
		transform.position = spawnPoint;
		rigidBody.velocity = Vector2.zero;
		PowerUpState = PowerUp.None;

		// Reset powerups
		var powerups = GameObject.Find("PowerUps");
		if (powerups != null) {
			foreach (Transform childTransform in powerups.transform) {
				childTransform.gameObject.SetActive(true);
			}
		}
	}

	// Any and all cheat codes available here
	public void Cheats() {
		// Let the player choose their level, main menu through 1-4
		// or let them choose their powerup
		KeyCode[] levels = new KeyCode[] {
			KeyCode.Alpha0,
			KeyCode.Alpha1,
			KeyCode.Alpha2,
			KeyCode.Alpha3,
			KeyCode.Alpha4
		};
		PowerUp[] powerUps = new PowerUp[] {
			PowerUp.None,
			PowerUp.Jelly,
			PowerUp.None,
			PowerUp.None,
			PowerUp.None
		};
		// Change level
		if (Input.GetKey(KeyCode.Plus) || Input.GetKey(KeyCode.KeypadPlus)) {
			Application.LoadLevel(Application.loadedLevel + 1);
		} else if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus)) {
			Application.LoadLevel(Application.loadedLevel - 1);
		}
		for (int i = 0; i < powerUps.Length; i++) {
			if (Input.GetKeyDown(levels[i])) {
				if (Input.GetKey(KeyCode.Tab)) {
					PowerUpState = powerUps[i];
				}
			}
		}

		// Allow invincibility
		if (Input.GetKeyDown(KeyCode.Backspace)) {
			isInvincible = !isInvincible;
		}
	}
}
