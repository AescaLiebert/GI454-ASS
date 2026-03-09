using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentNavigation : MonoBehaviour
{
    public WorldGrid worldGrid;
    public Transform agentPosition;
    public Rigidbody rb;
    public float force =1.0f;

    private void Start() {
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DijkstraTile currentTile = worldGrid.NodeFromWorldPoint(agentPosition.position);
        Vector2Int flowVector = currentTile.getFlowFieldVector();

        if (flowVector.Equals(Vector2Int.zero)) {
            rb.velocity = Vector3.zero; // Stop completely if at target or wall
        }
        else {
            Vector3 moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
            rb.velocity = moveDir * force;
            
            if (moveDir != Vector3.zero) {
                // Smoothly rotate the agent towards its movement direction
                Quaternion targetRotation = Quaternion.LookRotation(moveDir);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f));
            }
        }
    }
}
