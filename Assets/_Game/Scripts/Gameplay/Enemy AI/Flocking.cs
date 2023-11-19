using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Flocking : MonoBehaviour
{
    public GameObject birdPrefab;
    public int flockSize = 20;
    public List<GameObject> flock = new List<GameObject>();
    public Transform target;

    public float cohesionRadius = 5f;
    public float separationRadius = 2f;
    public float alignmentRadius = 5f;
    public float maxSpeed = 5f;
    public float seekWeight = 5f;

    void Start()
    {
        for (int i = 0; i < flockSize; i++)
        {
            Vector3 randomPos = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            GameObject bird = Instantiate(birdPrefab, randomPos, Quaternion.identity);
            flock.Add(bird);
        }
    }

    void Update()
    {
        foreach (GameObject bird in flock)
        {
            Flock(bird);
        }
    }

    void Flock(GameObject bird)
    {
        Vector3 cohesion = Vector3.zero;
        Vector3 separation = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        Vector3 targetDirection = (target.position - bird.transform.position).normalized; // Direction to the target
        int count = 0;

        foreach (GameObject otherBird in flock)
        {
            if (otherBird != bird)
            {
                float distance = Vector3.Distance(bird.transform.position, otherBird.transform.position);

                if (distance < cohesionRadius)
                {
                    cohesion += otherBird.transform.position;
                    count++;
                }

                if (distance < separationRadius)
                {
                    separation += (bird.transform.position - otherBird.transform.position) / (distance * distance);
                }

                if (distance < alignmentRadius)
                {
                    alignment += otherBird.GetComponent<Rigidbody>().velocity;
                }
            }
        }

        if (count > 0)
        {
            cohesion /= count;
            cohesion = (cohesion - bird.transform.position).normalized;
        }

        Rigidbody rb = bird.GetComponent<Rigidbody>();
        Vector3 velocity = rb.velocity;

        // Adjust the velocity towards the target position
        Vector3 seekDirection = (target.position - bird.transform.position).normalized;
        velocity += seekDirection * seekWeight;

        // Apply other flocking behaviors
        velocity += cohesion + separation + alignment;

        // Limit the velocity
        rb.velocity = velocity.normalized * Mathf.Min(velocity.magnitude, maxSpeed);

        // Optional: Rotate bird to face the direction of movement
        if (rb.velocity != Vector3.zero)
        {
            bird.transform.forward = rb.velocity.normalized;
        }
    }

}