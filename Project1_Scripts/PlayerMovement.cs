using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float walkSpeed = 2f;
    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpSpeed = 500f;

    int playerHealth = 100;
    int currentAttack = 0;
    float timeSinceAttack = 0f;
    public float inputRaw;
    private float speed = 0f;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    BoxCollider2D myFeet;
    bool isAlive = true;
    bool isJumping = false;
    [SerializeField] bool isGrounded;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myFeet = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        inputRaw = Input.GetAxisRaw("Horizontal");
        timeSinceAttack += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.S) && isGrounded) {
            myAnimator.SetBool("isCrouching", true);
        } else if (Input.GetKeyUp(KeyCode.S)) {
            myAnimator.SetBool("isCrouching", false);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            myAnimator.SetBool("isJumping", true);
            isJumping = true;
        } else if (Input.GetKeyUp(KeyCode.Space)) {
            isJumping = false;
        }

        myAnimator.SetFloat("yVelocity", myRigidbody.velocity.y);

        Attack();
        Jump(isJumping);
    }

    void FixedUpdate() {
        if (!isAlive) {return;}
        GroundCheck();
        Movement(inputRaw);
        FlipSprite();
        Die();
    }

    void Movement(float dir) {
        if (Mathf.Abs(dir) > 0) {
            speed = walkSpeed;
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isWalking", true);
        } 
        if (isGrounded && Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(dir) > 0) {
            speed = runSpeed;
            myAnimator.SetBool("isRunning", true);
            myAnimator.SetBool("isWalking", false);
        } 
        if (dir == 0) {
            myAnimator.SetBool("isRunning", false);
            myAnimator.SetBool("isWalking", false);
        }
        myRigidbody.velocity = new Vector2(dir * speed, myRigidbody.velocity.y);
    }
    void FlipSprite() {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed) {
            transform.localScale = new Vector2 (Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void GroundCheck() {
        isGrounded = false;
        if (myFeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
            isGrounded = true;
        } 
        myAnimator.SetBool("isJumping", !isGrounded);
    }

    void Jump(bool jumpFlag) {
        if (isGrounded && isJumping) {
            isGrounded = false;
            isJumping = false;
            myRigidbody.AddForce(new Vector2(0f, jumpSpeed));
        }
    }

    void Die() {
        if (playerHealth <= 0) {
            isAlive = false;
            myAnimator.SetTrigger("isDead");
        }
    }

    void Attack() {

        if ((Input.GetMouseButtonDown(0) && Input.GetKey("w") && isGrounded && timeSinceAttack > 0.2f)) {
            myAnimator.SetTrigger("AttackUp");
            timeSinceAttack = 0.0f;
        } else if (Input.GetMouseButtonDown(0) && isGrounded && timeSinceAttack > 0.2f) {
            timeSinceAttack = 0.0f;
            currentAttack++;

            if (currentAttack > 2)
                currentAttack = 1;
            
            if (timeSinceAttack > 1.0f)
                currentAttack = 1;

            myAnimator.SetTrigger("Attack" + currentAttack);
        }
        else if (Input.GetMouseButtonDown(0) && Input.GetKey("s") && !isGrounded) 
        {
            myAnimator.SetTrigger("AirAttackSlam");
            myRigidbody.velocity = new Vector2(0f, -10f);
            timeSinceAttack = 0.0f;
        } 
        else if (Input.GetMouseButtonDown(0) && Input.GetKey("w") && !isGrounded && timeSinceAttack > 0.2f) 
        {
            myAnimator.SetTrigger("AirAttackUp");
            timeSinceAttack = 0.0f;
        }
        else if (Input.GetMouseButtonDown(0) && !isGrounded && timeSinceAttack > 0.2f) 
        {
            myAnimator.SetTrigger("AirAttack");
            timeSinceAttack = 0.0f;
        } 
    }
}
