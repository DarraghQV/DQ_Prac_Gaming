using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootWeb : MonoBehaviour
{
    public GameObject webBallPrefab; 
    public float shootingForce = 10f; 
    public Transform shootingPoint; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject webBall = Instantiate(webBallPrefab, shootingPoint.position, shootingPoint.rotation);
        webBall.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * shootingForce, ForceMode.Impulse);
        Destroy(webBall, 2f); // Change the lifetime of the web ball as needed
    }
}

