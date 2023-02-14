using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateShooter : MonoBehaviour
{

    public Grapple grappling;

    private Quaternion desiredRotation;
    private float rotationSpeed = 5f;

    void Update()
    {
        if (!grappling.IsGrappling(null))
        {
            desiredRotation = transform.parent.rotation;
        }
        else
        {
            desiredRotation = Quaternion.LookRotation(grappling.GetGrapplePoint() - transform.position);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * rotationSpeed);
    }

}
