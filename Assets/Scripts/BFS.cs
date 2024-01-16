using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : MonoBehaviour
{
    private List<Node> path;  // Removi a declaração da variável aqui

    public GameLevelManager gameLevelManager;
    public GameObject markerPrefab;

    private int[,] grid;
    private Dictionary<Node, Node> cameFrom;

    private void Start()
    {
        path = new List<Node>();  // Adicionei a inicialização da variável aqui
        SetupGrid();
        InvokeRepeating("StartEnemyChase", 0f, 0.5f);  // Chama a função a cada 0.5 segundos (ajuste conforme necessário)
    }

    public void SetGameLevelManager(GameLevelManager manager)
    {
        gameLevelManager = manager;
    }

    private void SetupGrid()
    {
        if (gameLevelManager != null && gameLevelManager.TotalCoordinates != null)
        {
            int gridSizeX = CreateLevel.Instance.gridSizeX;
            int gridSizeZ = CreateLevel.Instance.gridSizeZ;

            grid = new int[gridSizeX, gridSizeZ];

            foreach (Coordinate coordinate in gameLevelManager.TotalCoordinates)
            {
                int x = coordinate.x;
                int z = coordinate.y;
                int value = coordinate.value;

                grid[x, z] = value;
            }
        }
        else
        {
            Debug.LogWarning("GameLevelManager or AllCoordinates is null. Cannot setup grid.");
        }
    }

    private void StartEnemyChase()
    {
        Vector2Int startPosition = Vector2Int.zero;

        List<Vector2Int> avoidedCoordinates = gameLevelManager.AvoidedCoordinates;
        int count = avoidedCoordinates.Count;

        if (count >= 3)
        {
            startPosition = avoidedCoordinates[count - 3];
        }
        else
        {
            Debug.LogWarning("Less than 3 coordinates available. Cannot get antepenultimate coordinate.");
            return;  // Sai da função se não houver coordenadas suficientes
        }

        Vector2Int targetPosition = new Vector2Int(1, 1);

        path = BreadthFirstSearch(startPosition, targetPosition);

      
    }

    private List<Node> BreadthFirstSearch(Vector2Int start, Vector2Int target)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        cameFrom = new Dictionary<Node, Node>();

        Node startNode = new Node(start.x, start.y, 0);
        queue.Enqueue(startNode);
        visited.Add(new Vector2Int(start.x, start.y));

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            if (currentNode.x == target.x && currentNode.z == target.y)
            {
                return ReconstructPath(currentNode, start);
            }

            ExploreNeighbors(currentNode, queue, visited);
        }

        Debug.Log("Path not found.");
        return null;
    }

    private void ExploreNeighbors(Node node, Queue<Node> queue, HashSet<Vector2Int> visited)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dz = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = node.x + dx[i];
            int newZ = node.z + dz[i];

            Vector2Int neighborPos = new Vector2Int(newX, newZ);

            if (IsValidMove(neighborPos) && !visited.Contains(neighborPos))
            {
                Node neighborNode = new Node(newX, newZ, node.cost + 1);
                queue.Enqueue(neighborNode);
                visited.Add(neighborPos);
                cameFrom.Add(neighborNode, node);

                Debug.Log($"Creating marker at: {newX}, {newZ}");
                //Instantiate(markerPrefab, new Vector3(newX, 0, newZ), Quaternion.identity);
            }
        }
    }

    private List<Node> ReconstructPath(Node endNode, Vector2Int startPosition)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode.x != startPosition.x || currentNode.z != startPosition.y)
        {
            path.Add(currentNode);
            Debug.Log($"X: {currentNode.x}, Z: {currentNode.z}");
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();
        return path;
    }

    private bool IsValidMove(Vector2Int position)
    {
        return position.x >= 0 && position.x < CreateLevel.Instance.gridSizeX
            && position.y >= 0 && position.y < CreateLevel.Instance.gridSizeZ
            && grid[position.x, position.y] != 1;
    }
}
