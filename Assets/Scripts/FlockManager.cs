using UnityEngine;

public class FlockManager : MonoBehaviour
{
    public GameObject boidPrefab;
    public GameObject sharkPrefab;
    public int boidCount;
    public Vector3 boundaryArea = new Vector3(10, 10, 10);

    public static FlockManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

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

        Vector3 sharkPosition = transform.position + new Vector3(
            Random.Range(-boundaryArea.x, boundaryArea.x),
            Random.Range(-boundaryArea.y, boundaryArea.y),
            Random.Range(-boundaryArea.z, boundaryArea.z)
        );

        Instantiate(sharkPrefab, sharkPosition, Quaternion.identity);
    }

    void Update()
    {
        // Boids handle their own boundary turning in Boid.cs
    }

    // Draw the boundary box in Scene View
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set Gizmo color
        Gizmos.DrawWireCube(transform.position, boundaryArea * 2); // Draw boundary box
    }
}