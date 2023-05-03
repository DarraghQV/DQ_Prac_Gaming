using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float swingSpeed;
    public float pullSpeed;
    public float grappleForce;
    public float throwForce;
    public float airMultiplier;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;

    
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode dashKey = KeyCode.R;

    [Header("Ground Check")]
    public LayerMask Ground;
    public float playerHeight;

    public Transform orientation;
    public Transform LeftShooterTip;
    public Transform RightShooterTip;

    [Header("Debug")]
    public LayerMask Webbable;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;
    [HideInInspector] public bool grounded;
    [HideInInspector] public bool swinging;

    private float maxDistance = 100f;
    private float horizontalInput;
    private float verticalInput;
    private bool readyToJump;
    private Vector3 moveDirection;
    private Vector3 extragravity;

    private Transform webbedObject;
    private Rigidbody rb;
    private LineRenderer LRLeft, LRRight;
    private SpringJoint rightWeb, leftWeb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        LRLeft = LeftShooterTip.parent.GetComponent<LineRenderer>();
        LRRight = RightShooterTip.parent.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, Ground);

        MyInput();
        SpeedControl();

        if (Input.GetMouseButtonDown(1))
            rightWeb = StartGrapple(transform, LRRight);
        else if (Input.GetMouseButtonUp(1))
            StopGrapple(rightWeb, LRRight);

        if (Input.GetMouseButtonDown(0))
            leftWeb = StartGrapple(transform, LRLeft);
        else if (Input.GetMouseButtonUp(0))
            StopGrapple(leftWeb, LRLeft);

        if (Input.GetKey(KeyCode.G))
            pullWeb(ref rightWeb);

        if (Input.GetKey(KeyCode.F))
            pullWeb(ref leftWeb);

        if (Input.GetKey(KeyCode.Q))
            pullObject(ref leftWeb);

        if (Input.GetKey(KeyCode.E))
            pullObject(ref rightWeb);

        if (Input.GetKey(KeyCode.B))
            FlingToGrapplePoint(ref rightWeb);

        if (Input.GetKey(KeyCode.O))
            ThrowWebbedObject(ref leftWeb, throwForce);

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

        if (Input.GetKeyUp(dashKey) && grounded)
        {
            Dash();
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

        if (swinging)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * swingSpeed, ForceMode.Force);


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

        if (swinging)
            rb.AddForce(moveDirection.normalized * swingSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void Dash()
    {
        rb.AddForce(moveDirection.normalized * 5f * airMultiplier, ForceMode.Impulse);
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
            webbedObject = hit.transform;



            Web.maxDistance = hit.distance * 1f;
            Web.minDistance = hit.distance * 0f;

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
        currentWeb.spring *= 1.0075f;

    }
    void StopGrapple(SpringJoint Web, LineRenderer LR)
    {

        LR.positionCount = 0;
        Destroy(Web);
    }

    void pullObject(ref SpringJoint currentWeb)
    {
        if(webbedObject != null)
        {
            var pull = pullSpeed * Time.deltaTime;
            webbedObject.transform.position = Vector3.MoveTowards(webbedObject.transform.position, rb.position, pull);
        }
    }

    void DrawRope(SpringJoint Web,LineRenderer LR)
    {
        if (!Web) return;
        LR.SetPosition(0, LR.transform.position);
        LR.SetPosition(1, Web.connectedAnchor);
    }

    public void FlingToGrapplePoint(ref SpringJoint currentWeb)
    {
        // Check if a grapple point is currently targeted
        if (webbedObject != null)
        {
            // Calculate the direction from the player to the grapple point
            Vector3 direction = webbedObject.transform.position - transform.position;
            direction.Normalize();

            // Calculate the force to apply to the player
            float force = grappleForce;

            // Apply the force in the calculated direction
            rb.AddForce(direction * force, ForceMode.Impulse);
        }
    }

    public void ThrowWebbedObject(ref SpringJoint currentweb, float throwForce)
    {
        // Check if there is an object attached to the spring joint
        if (leftWeb.connectedBody != null)
        {
            // Release the spring joint
            leftWeb.connectedBody = null;

            // Get the direction to throw the object
            Vector3 throwDirection = transform.forward;

            // Apply force to the rigidbody in the direction of the throw
            Rigidbody rb = leftWeb.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            }

            
        }
    }

}