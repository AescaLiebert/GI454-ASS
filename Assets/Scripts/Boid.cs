using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public float speed = 5f;
    public float neighborRadius = 3f;
    public float separationDistance = 1.5f;

    private Vector3 velocity;
    private List<Boid> neighbors;

    public float CoeffSeparation = 1.0f;
    public float CoeffAlignment = 0.05f;
    public float CoeffCohesion = 0.01f;
    public float CoeffBoundary = 1.0f;

    GameObject[] avoidance;

    void Start()
    {
        velocity = Random.insideUnitSphere * speed;
        avoidance = GameObject.FindGameObjectsWithTag("avoid");
    }

    void Update()
    {
        neighbors = GetNeighbors();
        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 avoid = CalculateAvoidance();
        Vector3 bound = CalculateBoundaryForce();

        // Combine forces
        velocity += separation + alignment + cohesion + avoid + bound;
        velocity = velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = velocity.normalized;
    }

    List<Boid> GetNeighbors()
    {
        List<Boid> nearbyBoids = new List<Boid>();
        foreach (Boid boid in FindObjectsByType<Boid>(FindObjectsSortMode.None))
        {
            if (boid != this && Vector3.Distance(transform.position, boid.transform.position) < neighborRadius)
            {
                nearbyBoids.Add(boid);
            }
        }
        return nearbyBoids;
    }

    Vector3 CalculateSeparation()
    {
        Vector3 separationForce = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            if (distance < separationDistance)
            {
                separationForce += (transform.position - boid.transform.position).normalized / distance;
            }
        }
        return separationForce * CoeffSeparation;
    }

    Vector3 CalculateAlignment()
    {
        if (neighbors.Count == 0) return Vector3.zero;
        Vector3 avgVelocity = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            avgVelocity += boid.velocity;
        }
        avgVelocity /= neighbors.Count;
        return (avgVelocity - velocity) * CoeffAlignment;
    }

    Vector3 CalculateCohesion()
    {
        if (neighbors.Count == 0) return Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        foreach (Boid boid in neighbors)
        {
            centerOfMass += boid.transform.position;
        }
        centerOfMass /= neighbors.Count;
        return (centerOfMass - transform.position) * CoeffCohesion;
    }

    Vector3 CalculateAvoidance()
    {
        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (GameObject avoidpoint in avoidance)
        {
            Vector3 dist = avoidpoint.transform.position - transform.position;
            if (dist.magnitude < (neighborRadius/2))
            {
                count++;
                centerOfMass += avoidpoint.transform.position;
            }
        }

        if(count > 0)
            return (transform.position - centerOfMass) * 2.0f;
        else
            return Vector3.zero;
    }

    Vector3 CalculateBoundaryForce()
    {
        if (FlockManager.instance == null) return Vector3.zero;

        Vector3 center = FlockManager.instance.transform.position;
        Vector3 bounds = FlockManager.instance.boundaryArea;

        Vector3 pos = transform.position;
        Vector3 force = Vector3.zero;

        // If outside the boundary, smoothly steer back towards the center
        if (Mathf.Abs(pos.x - center.x) > bounds.x ||
            Mathf.Abs(pos.y - center.y) > bounds.y ||
            Mathf.Abs(pos.z - center.z) > bounds.z)
        {
            force = (center - pos).normalized;
        }

        return force * CoeffBoundary;
    }
}
