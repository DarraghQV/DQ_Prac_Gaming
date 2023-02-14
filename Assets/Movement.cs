using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    private float maxDistance = 100f;
    public Transform LeftShooterTip, RightShooterTip;
    [Header("Movement")]
    public float moveSpeed;

    private LineRenderer LRLeft, LRRight;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode dashKey = KeyCode.LeftShift;


    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 extragravity;

    Rigidbody rb;
    private SpringJoint rightWeb, leftWeb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        LRLeft =LeftShooterTip.parent.GetComponent<LineRenderer>();
        LRRight = RightShooterTip.parent.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, Ground);

        

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
            rb.AddForce(Vector3.down * 2, ForceMode.Acceleration);

   
        if (Input.GetKeyDown(KeyCode.E))
        {
            rightWeb = StartGrapple(transform, LRRight);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            StopGrapple(rightWeb, LRRight);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            leftWeb = StartGrapple(transform, LRLeft);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            StopGrapple(leftWeb, LRLeft);
        }

        if (Input.GetKey(KeyCode.G))
        {
            pullWeb(ref rightWeb);
        }

        if (Input.GetKey(KeyCode.F))
        {
            pullWeb(ref leftWeb);
        }


        DrawRope(leftWeb, LRLeft);
        DrawRope(rightWeb, LRRight);

        
}

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(sprintKey) && grounded)
        {
            Sprint();
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void Sprint()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 25f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 25f * airMultiplier, ForceMode.Force);
    }

    private void Dash()
    {
        if (!grounded)
            rb.AddForce(Vector3.forward * 5, ForceMode.Impulse);
    }


    SpringJoint StartGrapple(Transform shooter, LineRenderer LR)
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20f, Color.blue, 3);
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
        {

            //grapplePoint = hit.point;

            SpringJoint Web = shooter.gameObject.AddComponent<SpringJoint>();
            Web.autoConfigureConnectedAnchor = false;
            Web.connectedAnchor = hit.point;


            Web.maxDistance = hit.distance * 0.8f;
            Web.minDistance = hit.distance * 0.25f;

            Web.spring = 2.5f;
            Web.damper = 7f;
            Web.massScale = 4.5f;

            LR.positionCount = 2;

            //currentGrapplePosition = WebTip.position;
            return Web;
        }
        return null;
    }



    private void pullWeb(ref SpringJoint currentWeb)
    {
        currentWeb.maxDistance *= 0.7f;
        currentWeb.spring *= 1.01f;

    }
    void StopGrapple(SpringJoint Web, LineRenderer LR)
    {

        LR.positionCount = 0;
        Destroy(Web);
    }


    void DrawRope(SpringJoint Web,LineRenderer LR)
    {
        if (!Web) return;
        LR.SetPosition(0, LR.transform.position);
        LR.SetPosition(1, Web.connectedAnchor);
    }

}