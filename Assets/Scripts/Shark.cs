using UnityEngine;

public class Shark : MonoBehaviour
{
    public float speed = 5.5f;
    public float rotationSpeed = 3f;
    public float searchRadius = 15f;
    public float CoeffBoundary = 2.0f; // Stronger boundary force so it turns around quickly

    private Boid targetBoid;

    void Update()
    {
        FindTarget();
        
        Vector3 targetDirection = transform.forward;

        if (targetBoid != null)
        {
            // Steer towards the target boid
            targetDirection = (targetBoid.transform.position - transform.position).normalized;
        }

        // Keep the shark inside the boundary
        Vector3 bound = CalculateBoundaryForce();
        
        // Combine the forces (direction towards boid + staying inside boundary)
        Vector3 finalDirection = (targetDirection + bound).normalized;

        if (finalDirection != Vector3.zero)
        {
            // Smoothly rotate towards the final direction
            Quaternion targetRotation = Quaternion.LookRotation(finalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move forward constantly
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void FindTarget()
    {
        Boid[] allBoids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
        float closestDistance = Mathf.Infinity;
        targetBoid = null;

        foreach (Boid boid in allBoids)
        {
            float distance = Vector3.Distance(transform.position, boid.transform.position);
            
            // Target the closest boid within search radius
            if (distance < closestDistance && distance < searchRadius)
            {
                closestDistance = distance;
                targetBoid = boid;
            }
        }
    }

    Vector3 CalculateBoundaryForce()
    {
        if (FlockManager.instance == null) return Vector3.zero;

        Vector3 center = FlockManager.instance.transform.position;
        Vector3 bounds = FlockManager.instance.boundaryArea;

        Vector3 pos = transform.position;
        Vector3 force = Vector3.zero;

        // If outside the boundary, steer back towards the center
        if (Mathf.Abs(pos.x - center.x) > bounds.x ||
            Mathf.Abs(pos.y - center.y) > bounds.y ||
            Mathf.Abs(pos.z - center.z) > bounds.z)
        {
            force = (center - pos).normalized;
        }

        return force * CoeffBoundary;
    }
}
