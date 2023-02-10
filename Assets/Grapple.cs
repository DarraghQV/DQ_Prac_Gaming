using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private LineRenderer LR;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;

    public Transform RWebTip, LWebTip, player;

    private float maxDistance = 100f;
    private SpringJoint leftWeb, rightWeb;

    public KeyCode pullKey = KeyCode.F;


    public float distanceFromPoint;

    private void Start()
    {
        distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
    }


    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    private void Update()
    {


        if (Input.GetKeyDown(KeyCode.E))
        {
            rightWeb = StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            StopGrapple(rightWeb);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            leftWeb = StartGrapple();
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            StopGrapple(leftWeb);
        }

        if (Input.GetKey(KeyCode.R))
        {
            pullWeb(ref rightWeb);
        }
    }

    private void pullWeb(ref SpringJoint rightWeb)
    {
        rightWeb.maxDistance *= 0.7f;
        rightWeb.spring *= 1.01f;
        
    }

    private void LateUpdate()
    {
        DrawRope(leftWeb);
        DrawRope(rightWeb);

    }

     SpringJoint StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            
            //grapplePoint = hit.point;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20f, Color.blue, 3);
            SpringJoint Web = player.gameObject.AddComponent<SpringJoint>();
            Web.autoConfigureConnectedAnchor = false;
            Web.connectedAnchor = hit.point;


            Web.maxDistance = distanceFromPoint * 0.8f;
            Web.minDistance = distanceFromPoint * 0.25f;

            Web.spring = 2.5f;
            Web.damper = 7f;
            Web.massScale = 4.5f;

            LR.positionCount = 2;
            //currentGrapplePosition = WebTip.position;
            return Web;
        }
        return null;
    }

    private Vector3 currentGrapplePosition;

    void DrawRope(SpringJoint Web)
    {
        if (!Web) return;

        RWebTip.position = Vector3.Lerp(RWebTip.position, Web.connectedAnchor, Time.deltaTime * 8f);

        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, Web.connectedAnchor);
    }

    void StopGrapple(SpringJoint Web)
    {
        LR.positionCount = 0;
        Destroy(Web);
    }
    public bool IsGrappling(SpringJoint Web)
    {
        return Web != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    }
