using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D myRigidBody;
    [SerializeField] float playerSpeed = 10f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] Vector2 deathKick = new Vector2 (0, 15f);
    [SerializeField] GameObject bulletGO;
    [SerializeField] Transform gun;
    
    Animator myAnimator;
    CapsuleCollider2D myCapsuleCollider;
    BoxCollider2D myBoxCollider;
    float startingGravityScale;
    bool isAlive = true;
    
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        myCapsuleCollider = GetComponent<CapsuleCollider2D>();
        startingGravityScale = myRigidBody.gravityScale;
    }

    void Update()
    {
        if(isAlive){
            Run();
            FlipSprite();
            ClimbLadder();
            Die();
        }
    }
    
    void Run() {
        Vector2 playerVelocity = new Vector2(moveInput.x * playerSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;

        bool playerIsMoving = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; // if veolicit.x > 0 
        myAnimator.SetBool("isRunning", playerIsMoving);
    }

    void FlipSprite() {
        bool playerIsMoving = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon; // if veolicit.x > 0 
        
        if(playerIsMoving)
            transform.localScale = new Vector3(Mathf.Sign(myRigidBody.velocity.x), 1f);
    }

    void ClimbLadder() {
        bool playerIsMovingUp = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon; // if veolicit.x > 0 
        bool playerIsClimbing = myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ladder"));

        if(!playerIsClimbing){
            myAnimator.SetBool("isClimbing", false); 
            myRigidBody.gravityScale = startingGravityScale;
            return;
        }

        myAnimator.SetBool("isClimbing", playerIsMovingUp);

        Vector2 climbingVelocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
        myRigidBody.velocity = climbingVelocity;
        myRigidBody.gravityScale = 0f;
    }

    void Die(){
        bool playerIsTouchingHazards = myCapsuleCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")) 
                                        || myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"));
        
        if(playerIsTouchingHazards){
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    // these methods are created by InputSystem
    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value){
        if(!myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        if(value.isPressed) {
            myRigidBody.velocity += new Vector2(0, jumpSpeed);
        }
    }

    void OnFire(InputValue value) {
        if(isAlive){
            Instantiate(bulletGO, gun.position, transform.rotation);
        }
    }

}
