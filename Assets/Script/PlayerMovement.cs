using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float speedWalk;
    public float speedSprint;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier ;
    
    [Header("Ground")]
    public LayerMask groundLayer;
    public float groundDrag;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    
    
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody playerRB;


    //Player Information
    public float playerHeight;

    //Player State
    public bool grounded=true;
    bool jumpReady=true;

    public MovementState state;
    public enum MovementState{
        walking,
        sprinting,
        air, 
        swimming
    }

    private void StateHandler(){

        //Mode - Sprinting
        if(grounded && Input.GetKey(sprintKey)){
            state=MovementState.sprinting;
            moveSpeed=speedSprint;
        }
        //Mode - Walking
        else if (grounded){
            state=MovementState.walking;
            moveSpeed=speedWalk;
        //Mode = Air
        }else{
            state=MovementState.air;
        }
    }
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        playerRB.freezeRotation=true;
        playerHeight=transform.lossyScale.y;
        moveSpeed=speedWalk;
    }


    void Update()
    {
        //state update
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight, groundLayer);

        //input
        GetInput();
        SpeedControl();
        StateHandler();

        //state update
        if(grounded){
            playerRB.drag = groundDrag;
        }else{
            playerRB.drag = 0;
        }
    }
    void FixedUpdate(){
        MovePlayer();
    }

    private void GetInput(){
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(jumpKey) && grounded && jumpReady){
            jumpReady=false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void MovePlayer(){
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if(grounded){
            playerRB.AddForce(moveDirection.normalized*moveSpeed*10f, ForceMode.Force);
        }else if(!grounded){
            playerRB.AddForce(moveDirection.normalized*moveSpeed*10f*airMultiplier, ForceMode.Force);
        }

    }
    private void SpeedControl(){
        Vector3 flatVelocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);
        if(flatVelocity.magnitude > moveSpeed){
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            playerRB.velocity = new Vector3(limitedVelocity.x, playerRB.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump(){
        playerRB.velocity = new Vector3(playerRB.velocity.x, 0f, playerRB.velocity.z);
        playerRB.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump(){
        jumpReady=true;
    }
}
