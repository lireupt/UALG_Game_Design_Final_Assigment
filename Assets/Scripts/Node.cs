using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Node
{
    public int x;
    public int z;
    public int cost;
    //public int heuristic;  // Add the heuristic parameter
    //public Node parent;    // Add the parent parameter

    // Modify the constructor to include the new parameters
    public Node(int x, int z, int cost, int heuristic, Node parent)
    {
        this.x = x;
        this.z = z;
        this.cost = cost;
       // this.heuristic = heuristic;
       // this.parent = parent;
    }

    // Calculates the total estimated cost for the node
    /*
    public int CostEstimate()
    {
        return cost + heuristic;
    }
    */
}

