using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private LineRenderer LR;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;

    public Transform WebTip, player;

    private float maxDistance = 100f;
    private SpringJoint joint;

    private void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    private void Update()
    {


        if (Input.GetMouseButtonDown(1))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopGrapple();
        }
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 20f, Color.blue, 3);
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 2.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            LR.positionCount = 2;
            currentGrapplePosition = WebTip.position;
        }

    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        LR.SetPosition(0, transform.position);
        LR.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        LR.positionCount = 0;
        Destroy(joint);
    }
    public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
    }
