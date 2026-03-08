using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour {

    public Transform StartPosition;//This is where the program will start the pathfinding from.
    public LayerMask WallMask;//This is the mask that the program will look for when trying to find obstructions to the path.
    public Vector2 vGridWorldSize;//A vector2 to store the width and height of the graph in world units.
    public float fNodeRadius;//This stores how big each square on the graph will be
    public float fDistanceBetweenNodes;//The distance that the squares will spawn from eachother.

    DijkstraTile[,] NodeArray;//The array of nodes that the A Star algorithm uses.

    float fNodeDiameter;//Twice the amount of the radius (Set in the start function)
    int iGridSizeX, iGridSizeY;//Size of the Grid in Array units.
    
    public bool HasGenerated { get { return NodeArray != null; } }


    private void Start()//Ran once the program starts
    {
        // Prevent division by zero or practically zero values that balloon the grid size
        if (fNodeRadius <= 0.05f) {
            Debug.LogError("Node Radius is too small! Clamping to 0.5f to prevent out of memory.");
            fNodeRadius = 0.5f;
        }

        fNodeDiameter = fNodeRadius * 2;//Double the radius to get diameter
        
        iGridSizeX = Mathf.RoundToInt(vGridWorldSize.x / fNodeDiameter);
        iGridSizeY = Mathf.RoundToInt(vGridWorldSize.y / fNodeDiameter);

        // Prevent Unity from freezing or overflowing by imposing a maximum grid capacity limit
        if (iGridSizeX > 200) iGridSizeX = 200;
        if (iGridSizeY > 200) iGridSizeY = 200;

        CreateGrid();//Draw the grid
        DijkstraTile[,] dijtraGrid = DijkstraGrid.generateDijkstraGrid(this.NodeArray, new Vector2Int(iGridSizeX, iGridSizeY), NodeFromWorldPoint(StartPosition.position));
        DijkstraTile[,] flowFieldGrid = LegacyFlowFieldGrid.generateFlowField(new Vector2Int(iGridSizeX, iGridSizeY), dijtraGrid);
    }

    void CreateGrid() {
        NodeArray = new DijkstraTile[iGridSizeX, iGridSizeY];//Declare the array of nodes.
        Vector3 bottomLeft = transform.position - Vector3.right * vGridWorldSize.x / 2 - Vector3.forward * vGridWorldSize.y / 2;//Get the real world position of the bottom left of the grid.
        for (int x = 0; x < iGridSizeX; x++)//Loop through the array of nodes.
        {
            for (int y = 0; y < iGridSizeY; y++)//Loop through the array of nodes
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * fNodeDiameter + fNodeRadius) + Vector3.forward * (y * fNodeDiameter + fNodeRadius);//Get the world co ordinates of the bottom left of the graph
                DijkstraTile tile = new DijkstraTile(new Vector2Int(x, y), worldPoint);

                if (Physics.CheckSphere(worldPoint, fNodeRadius, WallMask)) {
                    tile.setWeight(int.MaxValue);
                }
                NodeArray[x, y] = tile;//Create a new node in the array.
            }
        }
    }

    //Gets the closest node to the given world position.
    public DijkstraTile NodeFromWorldPoint(Vector3 a_vWorldPos) {
        float ixPos = ((a_vWorldPos.x + vGridWorldSize.x / 2) / vGridWorldSize.x);
        float iyPos = ((a_vWorldPos.z + vGridWorldSize.y / 2) / vGridWorldSize.y);

        ixPos = Mathf.Clamp01(ixPos);
        iyPos = Mathf.Clamp01(iyPos);

        int ix = Mathf.RoundToInt((iGridSizeX - 1) * ixPos);
        int iy = Mathf.RoundToInt((iGridSizeY - 1) * iyPos);

        return NodeArray[ix, iy];
    }
}