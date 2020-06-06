using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private enum State {idle, running, jumping, falling}
    private State state = State.idle;
    private Collider2D coll;
    [SerializeField]private LayerMask ground;

    void Start()
    {
      rb = GetComponent<Rigidbody2D> ();
      anim = GetComponent<Animator> ();
      coll = GetComponent<Collider2D> ();
    }
    // Update is called once per frame
    void Update()
    {
      Move();
      VelocityState();
      anim.SetInteger("state", (int)state);
    }

    void Move() {
      float hDir = Input.GetAxis("Horizontal");
      float speed = 500f;

      if (hDir < 0) {
        rb.velocity = new Vector2 (hDir * speed * Time.deltaTime, rb.velocity.y);
        gameObject.GetComponent<SpriteRenderer>().flipX = true;
      }

      else if (hDir > 0) {
        rb.velocity = new Vector2 (hDir * speed * Time.deltaTime, rb.velocity.y);
        gameObject.GetComponent<SpriteRenderer>().flipX = false;
      }

      if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground)) {
        rb.velocity = new Vector2 (rb.velocity.x, 10);
        state = State.jumping;
      }
    }

    void VelocityState() {

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
}
