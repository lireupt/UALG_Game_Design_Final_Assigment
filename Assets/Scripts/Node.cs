using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Node
{
    public int x;
    public int z;
    public int cost;
    public int heuristic;  // Adicione o parâmetro heuristic
    public Node parent;    // Adicione o parâmetro parent

    // Modifique o construtor para incluir os novos parâmetros
    public Node(int x, int z, int cost, int heuristic, Node parent)
    {
        this.x = x;
        this.z = z;
        this.cost = cost;
        this.heuristic = heuristic;
        this.parent = parent;
    }

    public int CostEstimate()
    {
        return cost + heuristic;
    }
}
