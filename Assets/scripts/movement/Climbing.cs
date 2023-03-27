using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public PlayerMovementAdvanced pm;
    public LayerMask whatIsWall;


    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool climbing;

    [Header("ClimbJUmping")]
    public float climbJumpUpForce;
    public float climbbackForce;

    public KeyCode JumpKey = KeyCode.Space;
    public int climbJumps;
    private int climbJumpsLeft;


    [Header("Detection")]
    public float detectionLenght;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Exiting")]
    public bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        WallCheck();
        StateMachine();

        if (climbing && !exitingWall) ClimbingMovement() ;
    }

    private void StateMachine()
    {
        // state 1 - climbing 
        if (wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle && !exitingWall);
        {
            if (!climbing && climbTimer > 0) StartClimbing();

            //timer
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
        }
        // state 2 Exiting
        else if (exitingWall)
        {
            if (climbing) StopClimbing();

            if (exitWallTimer > 0) exitWallTimer -= exitWallTimer.deltaTime;
            if (exitWallTimer < 0) exitingwall = false;
        }

        // state 3 - none
        else
        {
            if(climbing) StopClimbing() ;
        }
        if (wallFront && Input.GetKeyDown(JumpKey) && climbJumpsLeft > 0) ClimbJump();

    }
    private void WallCheck()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLenght, whatIsWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if (wallFront && newWall || pm.grounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }
    private void StartClimbing()
    {
        climbing = true;
        pm.climbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;



        // cam fov change
    }
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
        // sound effect
    }
    private void StopClimbing()
    {
        climbing = false;
        pm.climbing = false;
        //particles effect 
    }
    private void ClimbJump()
    {
        exitingWall = true;
        exitingWallTImer = exitWallTimer;
        Vector3 forceToApply = transform.up * climbJumpUpForce + frontWallHit.normal * climbbackForce;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;

    }
}
