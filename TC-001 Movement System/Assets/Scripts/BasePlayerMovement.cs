using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    //Inputs
    float xzInput, yInput;

    //Non-Public Variables
    Vector3 moveDirection, boostDirection;
    Rigidbody rb;
    bool readyToJump, readyToBoost, grounded, sloped, exitingSlope;
    InputSystem_Actions controls;
    RaycastHit slopeHit;

    //Public Variables
    public Transform orient;
    public float jumpCooldown, boostCooldown, airMulti, moveSpeed, boostSpeed, jumpForce, speedModifier, boostModifier, jumpModifier, PlayerHeight, groundDrag, maxSlopeAngle;
    public LayerMask ground;
    public bool speedCapped;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        readyToBoost = true;
        controls = new InputSystem_Actions();
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        if(speedCapped){ SpeedControl(); }
        DragCalc();
    }

    // FixedUpdate is called at a frequency determined by editor/designer
    void FixedUpdate()
    {
        MovePlayer();
    }

    //Gets user directional input
    void PlayerInput()
    {
        xzInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        //Jump check
        grounded = GroundedCheck();
        if (controls.Player.Jump.ReadValue<float>() > 0.5f && readyToJump && grounded)
        {
            Jump();
            Invoke(nameof(JumpReset), jumpCooldown);
        }
        if (controls.Player.Sprint.ReadValue<float>() > 0.5f && readyToBoost)
        {
            QuickBoost();
            Invoke(nameof(BoostReset), boostCooldown);
        }
    }

    //Handles calcs for, and moves, the user
    void MovePlayer()
    {
        sloped = SlopedCheck();
        grounded = GroundedCheck();

        moveDirection = orient.forward * yInput + orient.right * xzInput;
        if (sloped && !exitingSlope) 
        {
            Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f + 0.2f);
            moveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
            rb.AddForce(moveDirection.normalized * (moveSpeed + speedModifier) * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded) { rb.AddForce(moveDirection.normalized * (moveSpeed + speedModifier) * 10f, ForceMode.Force); }
        else if (!grounded) { rb.AddForce(moveDirection.normalized * (moveSpeed + speedModifier) * 10f * airMulti, ForceMode.Force); }
        rb.useGravity = !SlopedCheck();
    }

    //Handles calcs for QB (Quick Boosts)
    void QuickBoost()
    {
        grounded = GroundedCheck();
        sloped = SlopedCheck();

        boostDirection = orient.forward * yInput + orient.right * xzInput;
        if (sloped && !exitingSlope) 
        {
            Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f + 0.2f);
            boostDirection = Vector3.ProjectOnPlane(boostDirection, slopeHit.normal).normalized;
            rb.AddForce(boostDirection.normalized * (boostSpeed + boostModifier) * 4f, ForceMode.Impulse);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded) { rb.AddForce(boostDirection.normalized * (boostSpeed + boostModifier) * 2f, ForceMode.Impulse); }
        else if (!grounded) { rb.AddForce(boostDirection.normalized * (boostSpeed + boostModifier) * 2f * airMulti, ForceMode.Impulse); }
        readyToBoost = false;
    }

    //Speed check & Speed limit
    void SpeedControl()
    {
        sloped = SlopedCheck();

        //Limits speed on a slope
        if (sloped && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
        //Limits speeed on ground or in the air
        else
        {
            Vector3 currentVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (currentVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = currentVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }

    //Handles all calcs and inputs for jumping
    void Jump()
    {
        exitingSlope = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * (jumpForce + jumpModifier), ForceMode.Impulse);
        readyToJump = false;
    }

    void JumpReset() { readyToJump = true; exitingSlope = false;}
    void BoostReset() { readyToBoost = true; }

    //Checks if the player is on a slope
    bool SlopedCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, PlayerHeight * 0.5f * 0.2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        else return false;
    }

    //Checks if the player is grounded
    bool GroundedCheck()
    {
        bool land = Physics.Raycast(transform.position, Vector3.down, PlayerHeight * 0.5f * 0.2f, ground);
        return land;
    }

    //Handles Drag
    void DragCalc()
    {
        if (grounded){ rb.linearDamping = groundDrag; }
        else {rb.linearDamping = 0;}
    }
}
