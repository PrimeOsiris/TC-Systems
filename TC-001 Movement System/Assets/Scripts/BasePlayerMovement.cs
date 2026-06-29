using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    //Inputs
    float xzInput;
    float yInput;

    //Non-Public Variables
    float moveSpeed;
    float boostSpeed;
    Vector3 moveDirection;
    Vector3 boostDirection;
    Rigidbody rb;
    float jumpForce;
    bool readyToJump;
    bool readyToBoost;
    FrictionCalc calc;
    StatCheck stat;
    InputAbstract controls;
    bool grounded;
    bool sloped;
    bool exitingSlope;
    RaycastHit slopeHit;

    //Public Variables
    public Transform orient;
    public float jumpCooldown;
    public float boostCooldown;
    public float airMulti;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        readyToBoost = true;
        stat = gameObject.GetComponent<StatCheck>();
        calc = gameObject.GetComponent<FrictionCalc>();
        controls = new InputAbstract();
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearDamping = calc.DragCalc();

        PlayerInput();
        SpeedControl();
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
        calc = gameObject.GetComponent<FrictionCalc>();
        grounded = calc.GroundedCheck();
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
        sloped = calc.SlopedCheck();
        grounded = calc.GroundedCheck();
        moveSpeed = stat.SpeedCheck(gameObject.tag);

        moveDirection = orient.forward * yInput + orient.right * xzInput;
        if (sloped && !exitingSlope) 
        {
            Physics.Raycast(transform.position, Vector3.down, out slopeHit, calc.PlayerHeight * 0.5f + 0.2f);
            moveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
            rb.AddForce(moveDirection.normalized * moveSpeed * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (grounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); }
        else if (!grounded) { rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMulti, ForceMode.Force); }
        rb.useGravity = !calc.SlopedCheck();
    }

    //Handles calcs for QB (Quick Boosts)
    void QuickBoost()
    {
        grounded = calc.GroundedCheck();
        sloped = calc.SlopedCheck();
        boostSpeed = stat.BoostCheck(gameObject.tag);

        boostDirection = orient.forward * yInput + orient.right * xzInput;
        if (sloped && !exitingSlope) 
        {
            Physics.Raycast(transform.position, Vector3.down, out slopeHit, calc.PlayerHeight * 0.5f + 0.2f);
            boostDirection = Vector3.ProjectOnPlane(boostDirection, slopeHit.normal).normalized;
            rb.AddForce(boostDirection.normalized * boostSpeed * 4f, ForceMode.Impulse);

            if (rb.linearVelocity.y > 0)
            {
                rb.AddForce(Vector3.down * 16f, ForceMode.Force);
            }
        }
        else if (grounded) { rb.AddForce(boostDirection.normalized * boostSpeed * 2f, ForceMode.Impulse); }
        else if (!grounded) { rb.AddForce(boostDirection.normalized * boostSpeed * 2f * airMulti, ForceMode.Impulse); }
        readyToBoost = false;
    }

    //Speed check & Speed limit
    void SpeedControl()
    {
        moveSpeed = stat.SpeedCheck(gameObject.tag);
        sloped = calc.SlopedCheck();

        if (sloped && !exitingSlope)
        {
            if(rb.linearVelocity.magnitude > moveSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
            }
        }
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
        jumpForce = stat.JumpCheck(gameObject.tag);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        readyToJump = false;
    }
    void JumpReset() { readyToJump = true; exitingSlope = false;}
    void BoostReset() { readyToBoost = true; }
}
