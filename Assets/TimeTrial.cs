using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTrial : MonoBehaviour
{
    public GameObject checkpointPrefab;
    public int numCheckpoints = 10;
    public float checkpointRadius = 100f;

    private List<GameObject> checkpoints = new List<GameObject>();
    private int checkpointsPassed = 0;
    private float startTime = 0f;

    void Start()
    {
        // Generate checkpoints
        for (int i = 0; i < numCheckpoints; i++)
        {
            Vector3 randomPos = Random.insideUnitSphere * checkpointRadius;
            randomPos.y = 4f; 
            GameObject checkpoint = Instantiate(checkpointPrefab, randomPos, Quaternion.identity);
            checkpoints.Add(checkpoint);

            MeshRenderer renderer = checkpoint.GetComponent<MeshRenderer>();
            renderer.material.color = Color.yellow;
        }

        // Start timer
        startTime = Time.time;
    }

    void Update()
    {
        // Check if all checkpoints have been passed
        if (checkpointsPassed >= numCheckpoints)
        {
            // End time trial and print results
            float timeElapsed = Time.time - startTime;
            Debug.Log("Time trial complete! Time: " + timeElapsed.ToString("F2") + "s");
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player collided with a checkpoint
        if (other.CompareTag("Checkpoint"))
        {
            // Remove checkpoint and increment count
            Destroy(other.gameObject);
            checkpointsPassed++;
        }
    }
}
