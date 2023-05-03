using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{

    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;

    public Transform RWebTip, LWebTip, player;


    private SpringJoint leftWeb, rightWeb;

    public KeyCode pullKey = KeyCode.F;


    public float distanceFromPoint;

    private void Start()
    {
        distanceFromPoint = Vector3.Distance(player.position, grapplePoint);
    }


    private Vector3 currentGrapplePosition;


    public bool IsGrappling(SpringJoint Web)
    {
        return Web != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    }
