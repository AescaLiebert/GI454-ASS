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
    public float CoeffFlowField = 1.0f;

    private WorldGrid worldGrid;
    private DijkstraTile lastValidTile;

    GameObject[] avoidance;

    void Start()
    {
        velocity = Random.insideUnitSphere * speed;
        try
        {
            avoidance = GameObject.FindGameObjectsWithTag("avoid");
        }
        catch (UnityException)
        {
            // Tag might not exist, ignore it.
            avoidance = new GameObject[0];
        }

        // Try to find the WorldGrid in the scene
        worldGrid = Object.FindFirstObjectByType<WorldGrid>();
    }

    void Update()
    {
        neighbors = GetNeighbors();
        Vector3 separation = CalculateSeparation();
        Vector3 alignment = CalculateAlignment();
        Vector3 cohesion = CalculateCohesion();
        Vector3 avoid = CalculateAvoidance();
        Vector3 flowField = CalculateFlowFieldNavigation();

        // Combine forces
        velocity += separation + alignment + cohesion + avoid + flowField;
        velocity = velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
        if (velocity != Vector3.zero)
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

    Vector3 CalculateFlowFieldNavigation()
    {
        if (worldGrid == null || !worldGrid.HasGenerated) return Vector3.zero;

        DijkstraTile currentTile = worldGrid.NodeFromWorldPoint(transform.position);
        if (currentTile == null) return Vector3.zero;

        if (lastValidTile == null) { lastValidTile = currentTile; }

        Vector3 moveDir = Vector3.zero;

        if (currentTile.getFlowFieldVector().Equals(Vector2Int.zero)) 
        {
            Vector2Int flowVector = lastValidTile.getVector2d() - currentTile.getVector2d();
            moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
        }
        else 
        {
            lastValidTile = currentTile;
            Vector2Int flowVector = currentTile.getFlowFieldVector();
            moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
        }

        return moveDir * CoeffFlowField;
    }

    Vector3 CalculateAvoidance()
    {
        if (avoidance == null) return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        int count = 0;

        foreach (GameObject avoidpoint in avoidance)
        {
            if (avoidpoint == null) continue;

            Vector3 dist = avoidpoint.transform.position - transform.position;
            if (dist.magnitude < (neighborRadius/2))
            {
                count++;
                centerOfMass += avoidpoint.transform.position;
            }
        }

        if(count > 0)
            return (centerOfMass - transform.position) * 2.0f;
        else
            return Vector3.zero;
    }
}
