using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private int score = 0;
    private Text scoreValue;
    private enum State {idle, running, jumping, falling, hurt, dead}
    private State state = State.idle;
    private Collider2D coll;
    private LayerMask ground;
    private int health;
    private float hurtForce = 8f;
    private float push = 2f;

    void Start ()
    {
      rb = GetComponent<Rigidbody2D> ();
      anim = GetComponent<Animator> ();
      ground = LayerMask.GetMask("Ground");
      coll = GetComponent<Collider2D> ();
      GetUIField();
      health = 2;
    }
    // Update is called once per frame
    void Update ()
    {
      if (state != State.hurt) {
        Move();
      }
      SetCharacterState();
      anim.SetInteger("state", (int)state);
      scoreValue.text = score.ToString();
    }

    /* ================= Movement ================= */

    /*
      Performs move operations:
      moves character right and left based on the key pressed
      flips the character sprite accordingly
      handles the jump making when an appropriate key is pressed
    */
    void Move () {
      float hDir = Input.GetAxis("Horizontal");
      float speed = 450f;

      // moving left
      if (hDir < 0) {
        rb.velocity = new Vector2 (hDir * speed * Time.deltaTime, rb.velocity.y);
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
      }
      // moving right
      else if (hDir > 0) {
        rb.velocity = new Vector2 (hDir * speed * Time.deltaTime, rb.velocity.y);
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
      }
      // allowing the jump to happen only on the ground
      if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground)) {
        Jump(10f);
      }
    }

    // Abstracts the jump movement into a separate function
    void Jump(float jumpForce) {
      rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
      state = State.jumping;
    }

    /* ================= State ================= */

    /*
      A finite state machine that handles the character State based on the conds
      idle -> running,
      running -> or running -> idle
      jumping -> falling (jumping state is triggered in Move())
      falling -> idle
    */
    void SetCharacterState () {

      switch(state) {
        case State.hurt:
          if (health == 0) {
            state = State.dead;
          }
          if (coll.IsTouchingLayers(ground)) {
            state = State.idle;
          }
            break;
        case State.idle:
          if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
            state = State.running;
          }
          break;
        case State.running:
          if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
            state = State.running;
          } else {
              state = State.idle;
          }
          break;
        case State.jumping:
          if (rb.velocity.y < 0.1f) {
            state = State.falling;
          }
          break;
        case State.falling:
          if (coll.IsTouchingLayers(ground)) {
            state = State.idle;
          }
          break;
        default:
          state = State.idle;
          break;
      }
    }

    /* ================= Collisions ================= */

    // Handles the collision with collectible items
    void OnTriggerEnter2D (Collider2D collision) {
      if (collision.tag == "Collectibles") {
        Destroy(collision.gameObject);
        // TODO: find a better way to identify the object
        if (collision.name == "SpecialBerry") {
          score += 3;
        } else {
          score += 1;
        }
      }
    }

    // Handles the collision with other (ex. Enemy) entities
    void OnCollisionEnter2D (Collision2D other) {
      // jumping on the enemy will kill it
      if (other.gameObject.tag == "Enemy") {
        if (state == State.falling) {
          Destroy(other.gameObject);
          Jump(5f);
        } else if (other.gameObject.transform.position.x > transform.position.x) {
          // the enemy is to the right of the player
          // the player will be pushed to the left
          state = State.hurt;
          rb.velocity = new Vector2(-push, hurtForce);
          //health -= 1;
        } else {
          // the enemy is to the left of the player
          // the player will be pushed to the right
          state = State.hurt;
          rb.velocity = new Vector2(push, hurtForce);
          //health -= 1;
        }
      }
    }

    /* ================= ScoringUI ================= */

    /*
      Retrieves the Text object and stores it as a score value variable
      if the object is not found, log an error message
      Call in the Start()
    */
    void GetUIField () {
      GameObject scoreObj = GameObject.FindWithTag("ScoreValue");
      if (scoreObj != null) {
        scoreValue = scoreObj.GetComponent<Text>();
      } else {
          Debug.Log("A UI object is not found.");
        }
      }
}
