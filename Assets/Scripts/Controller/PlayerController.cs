using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    //=====Variables=====//
    [SerializeField] private float jumpPower = 200.0f;
    [SerializeField] private float dashSpeed = 20.0f;
    [SerializeField] private float dashDistance = 3.0f;
    private int maxDashTimes = 2;
    private float dashRestoreDelay = 0.85f; //second

    
    private bool toJump = false;
    private bool toDash = false;
    private bool jumping = false;
    [SerializeField] private int dashTimes;
    private float? dashRestoreDelayTimeLeft = null;
    public float display;
    private Vector2 moveDirection;
    
    private PlayerStatus status;
    private Rigidbody2D rigidBody;
    //=====Variables=====//

    //=====Functions=====//
    private void HandleWASDInput() {
        moveDirection = new Vector2(0.0f, 0.0f);
        if (Input.GetKey(KeyCode.W)) moveDirection += new Vector2(0.0f, 1.0f);
        if (Input.GetKey(KeyCode.A)) moveDirection += new Vector2(-1.0f, 0.0f);
        if (Input.GetKey(KeyCode.S)) moveDirection += new Vector2(0.0f, -1.0f);
        if (Input.GetKey(KeyCode.D)) moveDirection += new Vector2(1.0f, 0.0f);
    }

    //Move
    private void Move() {
        rigidBody.velocity = new Vector2(moveDirection.x * status.Speed(), rigidBody.velocity.y);
    }

    //Dash
    private IEnumerator __dash(Vector2 direction) {
        if (direction == Vector2.zero) yield break;        

        direction.Normalize();
        float distanceLeft = dashDistance;
        float deltaDistance;
        Collider2D collider = gameObject.GetComponent<Collider2D>(); ;
        RaycastHit2D[] hits = new RaycastHit2D[1];        

        collider.Cast(direction, hits, dashDistance);
        if (hits[0] != new RaycastHit2D())
            distanceLeft = Mathf.Min(dashDistance, Vector2.Distance(hits[0].point, collider.transform.position) - (collider.bounds.size.x / 2));

        while (distanceLeft > Mathf.Epsilon) {
            deltaDistance = dashSpeed * Time.deltaTime;
            deltaDistance = Mathf.Min(deltaDistance, distanceLeft);
            distanceLeft -= deltaDistance;

            rigidBody.position += direction * deltaDistance;
            yield return null;
        }
    }
    private void Dash() {
        StartCoroutine(__dash(moveDirection));
        dashTimes--;
        toDash = false;
    }
    
    //Jump
    private void Jump() {
        rigidBody.AddForce(new Vector2(0, jumpPower));
        jumping = true;
        toJump = false;
    }
    //=====Functions=====//

    //=====UnityControl=====//
    void Start() {        
        status = gameObject.GetComponent<PlayerStatus>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        dashTimes = maxDashTimes;
    }

    void Update() {
        if (dashRestoreDelayTimeLeft != null) display = (float)dashRestoreDelayTimeLeft;
        else display = -1.0f;
        
        //handle input
        HandleWASDInput();
        if (!jumping && Input.GetKey(KeyCode.Space)) toJump = true;
        if (dashTimes > 0 && Input.GetMouseButtonDown(0)) toDash = true;

        //dash delay count down
        if (dashRestoreDelayTimeLeft != null) dashRestoreDelayTimeLeft -= Time.deltaTime;
        if (dashTimes < maxDashTimes && dashRestoreDelayTimeLeft == null) {
            dashRestoreDelayTimeLeft = dashRestoreDelay;
        }
        
    }

    void FixedUpdate() {
        Move();
        if (toJump) Jump();
        if (toDash) Dash();
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.tag == "Floor") {
            jumping = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        if (collision.collider.gameObject.tag == "Floor") {
            if (dashRestoreDelayTimeLeft != null && dashRestoreDelayTimeLeft <= 0.0f) {
                dashRestoreDelayTimeLeft = null;
                dashTimes = maxDashTimes;
            }
        }
    }
    //=====UnityControl=====//
}
