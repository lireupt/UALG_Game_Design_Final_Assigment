using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    /*
    This one only moves around but moves backward as well.
    */
    public GameObject enemyPrefab;  // Enemy prefab
    private GameObject enemyInstance;  // Enemy instance
    public GameLevelManager gameLevelManager;
    public CreateLevel create;
    private Vector2Int targetPosition;  // Target position (e.g., player position)
    private int[,] grid;
    private EnemyController enemyController;  // Add this line
    private float movementSpeed = 25f;  // Bot movement speed
    [SerializeField] private GameObject bombPrefab;

    private List<Vector2Int> visitedNodes = new List<Vector2Int>();  // List of visited nodes
    private List<Vector2Int> allVisitedNodes = new List<Vector2Int>();  // List of all visited nodes


    private void Start()
    {
        SpawnEnemy();
        // Uncomment this if you want the bot to move automatically
        // StartCoroutine(UpdateEnemyMovement());
        SetupGrid();
        // MoveBotToAdjacentNodes();
        StartCoroutine(AutoMoveBot());

        enemyController = enemyInstance.GetComponent<EnemyController>();
    }

    private IEnumerator AutoMoveBot()
    {
        while (true)
        {
            yield return StartCoroutine(MoveBotToAdjacentNodes());
            yield return new WaitForSeconds(0.5f); // Wait for 1 second before moving again (adjust as needed)
        }
    }

    private void SetupGrid()
    {
        if (gameLevelManager != null && gameLevelManager.TotalCoordinates != null)
        {
            int gridSizeX = gameLevelManager.gridSizeX;
            int gridSizeZ = gameLevelManager.gridSizeZ;

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

    private void SpawnEnemy()
    {
        List<Vector2Int> avoidedCoordinates = gameLevelManager.AvoidedCoordinates;
        Vector2Int startPosition = avoidedCoordinates.Count >= 3 ? avoidedCoordinates[avoidedCoordinates.Count - 3] : Vector2Int.zero;

        // Instantiate the enemy at the avoided initial position
        enemyInstance = Instantiate(enemyPrefab, new Vector3(startPosition.x, 0.5f, startPosition.y), Quaternion.identity);

        // Set the initial target position (can be the player's position)
        targetPosition = new Vector2Int(0, 0);  // Define your logic to determine the initial target position
    }

    private List<Vector2Int> GetAdjacentNodes(Vector2Int centerNode)
    {
        // Logic to get adjacent nodes (e.g., nodes to the north, south, east, and west)
        List<Vector2Int> adjacentNodes = new List<Vector2Int>
        {
            new Vector2Int(centerNode.x, centerNode.y + 1), // North
            new Vector2Int(centerNode.x, centerNode.y - 1), // South
            new Vector2Int(centerNode.x + 1, centerNode.y), // East
            new Vector2Int(centerNode.x - 1, centerNode.y)  // West
        };

        return adjacentNodes;
    }


    private IEnumerator MoveBotToAdjacentNodes()
    {
        // Get the current position of the enemy
        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(enemyInstance.transform.position.x),
                                               Mathf.RoundToInt(enemyInstance.transform.position.z));

        // Get the neighboring nodes around the enemy (assuming a 2D grid)
        List<Vector2Int> adjacentNodes = GetAdjacentNodes(currentPos);

        // Move the bot to the first neighboring node that is not a wall and has not been visited
        foreach (Vector2Int node in adjacentNodes)
        {
            if (IsDestructibleBlock(node))
            {
                for (int i = visitedNodes.Count - 1; i >= Mathf.Max(0, visitedNodes.Count - 5); i--)
                {
                    Vector2Int previousPos = visitedNodes[i];
                    if (!IsWall(previousPos))
                    {
                        yield return StartCoroutine(SimulateBombPlacement(previousPos));

                        // Check if enemyInstance is still valid before accessing enemyController
                        if (enemyInstance != null)
                        {
                            enemyController = enemyInstance.GetComponent<EnemyController>();
                        }

                        if (enemyController != null)
                        {
                            // Move the bot to the previous position
                            yield return StartCoroutine(MoveBotToPositionGradual(previousPos));

                            visitedNodes.Add(previousPos);
                            allVisitedNodes.Add(previousPos);
                            break;
                        }
                    }
                }
            }
            else if (!IsWall(node) && !visitedNodes.Contains(node))
            {
                yield return StartCoroutine(MoveBotToPositionGradual(node));
                visitedNodes.Add(node);

                if (visitedNodes.Count > 5)
                {
                    visitedNodes.RemoveAt(0);
                }

                break;
            }
        }
    }


    private IEnumerator MoveBotToPositionGradual(Vector2Int targetNode)
    {
        // Store the current position before movement
        Vector2Int currentPosition = new Vector2Int(Mathf.RoundToInt(enemyInstance.transform.position.x),
                                                      Mathf.RoundToInt(enemyInstance.transform.position.z));

        // Clear the list and add the new position
        visitedNodes.Clear();
        visitedNodes.Add(currentPosition);

        visitedNodes.Add(targetNode);

        Vector3 targetPosition = new Vector3(targetNode.x, 0.5f, targetNode.y);

        while (Vector3.Distance(enemyInstance.transform.position, targetPosition) > 0.01f)
        {
            /*
             * THE CODE HERE NEEDS TO BE REVIEWED, AS THE LOGIC SHOULD BE MODIFIED, 
             * BECAUSE THE BOT DOESN'T RESPOND THE SAME WAY IF THERE IS A DESTRUCTIBLE BLOCK OR NOT
             */
            // OLD
            // Check if the next node is a wall
            /*
            if (IsWall(currentPosition))
            {
                Debug.Log($"Found a wall at ({currentPosition.x}, {currentPosition.y})! Movement interrupted.");
                yield break;  // Exit the coroutine if a wall is encountered
            }
            /*
            */
            // NEW
            // Check if the next node is a wall
            if (IsWall(targetNode))
            {
                Debug.Log($"Found a wall at ({targetNode.x}, {targetNode.y})! Movement interrupted.");
                yield break;  // Exit the coroutine if a wall is encountered
            }

            // Gradually move towards the next node
            enemyInstance.transform.position = Vector3.MoveTowards(enemyInstance.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the bot is exactly on the node
        enemyInstance.transform.position = targetPosition;
    }




    private bool IsWall(Vector2Int node)
    {
        // Logic to check if the node is a wall
        // This could be a check in your grid matrix grid[x, y]
        // Return true if it's a wall, false otherwise
        int nodeValue = grid[node.x, node.y];
        return nodeValue == 1 || nodeValue == 2;  // Example: 1 represents a wall, 2 represents an indestructible block
    }

    private bool IsDestructibleBlock(Vector2Int node)
    {
        // Logic to check if the node is a destructible block
        // This could be a check in your grid matrix grid[x, y]
        // Return true if it's a destructible block, false otherwise
        int nodeValue = grid[node.x, node.y];
        return nodeValue == 3;  // Example: 3 represents a destructible block
    }

   
    /*
     * TODO: Check here for the simultaneous explosion of the bomb in two adjacent cells
     */
    private IEnumerator SimulateBombPlacement(Vector2Int bombPosition)
    {
        yield return StartCoroutine(MoveBotToPositionGradual(bombPosition));

        visitedNodes.Add(bombPosition);

        for (int i = 0; i < gameLevelManager.TotalCoordinates.Count; i++)
        {
            Vector2Int blockPosition = new Vector2Int(gameLevelManager.TotalCoordinates[i].x, gameLevelManager.TotalCoordinates[i].y);

            if (IsDestructibleBlock(blockPosition) &&
                ((Mathf.Abs(blockPosition.x - bombPosition.x) == 1 && blockPosition.y == bombPosition.y) ||
                 (Mathf.Abs(blockPosition.y - bombPosition.y) == 1 && blockPosition.x == bombPosition.x)))
            {
                if (enemyController != null)
                {
                    GameObject bomb = Instantiate(bombPrefab);
                    bomb.transform.position = new Vector3(Mathf.Round(enemyController.transform.position.x), 0.5f, Mathf.Round(enemyController.transform.position.z));
                }

                gameLevelManager.TotalCoordinates[i].value = 0;
                grid[blockPosition.x, blockPosition.y] = 0;
            }
        }
    }
}
