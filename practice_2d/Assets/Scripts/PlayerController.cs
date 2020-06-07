using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private enum State {idle, running, jumping, falling}
    private State state = State.idle;
    private Collider2D coll;
    private LayerMask ground;
    private int score = 0;
    private Text scoreValue;

    void Start()
    {
      rb = GetComponent<Rigidbody2D> ();
      anim = GetComponent<Animator> ();
      coll = GetComponent<Collider2D> ();
      ground = LayerMask.GetMask("Ground");
      GetUIField();
    }
    // Update is called once per frame
    void Update()
    {
      Move();
      SetCharacterState();
      anim.SetInteger("state", (int)state);
      scoreValue.text = score.ToString();
    }

    void OnTriggerEnter2D(Collider2D collision) {
      if (collision.tag == "Collectibles") {
        Destroy(collision.gameObject);
        score += 1;
      }
    }

    /*
      Performs move operations:
      moves character right and left based on the key pressed
      flips the character sprite accordingly
      handles the jump making when an appropriate key is pressed
    */
    void Move() {
      float hDir = Input.GetAxis("Horizontal");
      float speed = 450f;
      float jumpForce = 10f;

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
        rb.velocity = new Vector2 (rb.velocity.x, jumpForce);
        state = State.jumping;
      }
    }

    /*
      A finite state machine that handles the character State based on the conds
      idle -> running,
      running -> or running -> idle
      jumping -> falling (jumping state is triggered in Move())
      falling -> idle
    */
    void SetCharacterState() {

      switch(state) {
        case State.idle:
          if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
            state = State.running;
          }
          break;
        case State.running:
          if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon) {
            state = State.running;
          }
          else {
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
        Debug.Log("A UI object is not found.")
      }
    }
}
