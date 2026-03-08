using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public int boidCount = 20;
    public Vector3 boundaryArea = new Vector3(10, 10, 10);

    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 position = transform.position + new Vector3(
                Random.Range(-boundaryArea.x, boundaryArea.x),
                Random.Range(-boundaryArea.y, boundaryArea.y),
                Random.Range(-boundaryArea.z, boundaryArea.z)
            );

            Instantiate(boidPrefab, position, Quaternion.identity);
        }
    }

    void Update()
    {
        foreach (Boid boid in FindObjectsByType<Boid>(FindObjectsSortMode.None)) // Loop through all boids
        {
            WrapPosition(boid.transform);
        }
    }

    void WrapPosition(Transform obj)
    {
        Vector3 pos = obj.position;

        // Check each axis and wrap the position
        if (pos.x > boundaryArea.x) pos.x = -boundaryArea.x;
        else if (pos.x < -boundaryArea.x) pos.x = boundaryArea.x;

        if (pos.y > boundaryArea.y) pos.y = -boundaryArea.y;
        else if (pos.y < -boundaryArea.y) pos.y = boundaryArea.y;

        if (pos.z > boundaryArea.z) pos.z = -boundaryArea.z;
        else if (pos.z < -boundaryArea.z) pos.z = boundaryArea.z;

        obj.position = pos; // Apply the wrapped position
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;//Set the color of the wireframe
        Gizmos.DrawWireCube(transform.position, boundaryArea * 2); //Draw a wire cube for the boid bounding area
    }
}