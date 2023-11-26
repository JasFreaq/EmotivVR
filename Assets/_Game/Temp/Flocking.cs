using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Drawing;

public class Flocking : MonoBehaviour
{
    public GameObject birdPrefab;
    public int flockSize = 20;
    public List<GameObject> flock = new List<GameObject>();
    public Transform target;
    
    public float separationRadius = 2f;
    
    public float seekWeight = 1f;
    
    public float maxSpeed = 5f;

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
        

        foreach (GameObject otherBird in flock)
        {
            if (otherBird != bird)
            {
                float distance = Vector3.Distance(bird.transform.position, otherBird.transform.position);

                cohesion += otherBird.transform.position;

                if (distance < separationRadius)
                {
                    separation += (bird.transform.position - otherBird.transform.position) / (distance * distance);
                }
            }
        }

        cohesion /= flockSize;
        cohesion = (cohesion - bird.transform.position).normalized;

        Rigidbody rb = bird.GetComponent<Rigidbody>();
        Vector3 velocity = rb.velocity;

        velocity += cohesion + separation;

        Vector3 seekDirection = (target.position - bird.transform.position).normalized;
        velocity += seekDirection * seekWeight;

        rb.velocity = velocity.normalized * Mathf.Min(velocity.magnitude, maxSpeed);

        Quaternion rotation = Quaternion.LookRotation(velocity.normalized, bird.transform.up);
        bird.transform.rotation = Quaternion.Slerp(bird.transform.rotation, rotation, Time.deltaTime);
    }
}