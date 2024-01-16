using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Node
{
    public int x;
    public int z;
    public int cost;

    public Node(int x, int z, int cost)
    {
        this.x = x;
        this.z = z;
        this.cost = cost;
    }
}