using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BFS : MonoBehaviour
{


    public GameLevelManager gameLevelManager;
    public GameObject movableObjectPrefab;
    public float gridSize = 1.0f;
    public float movementSpeed = 0.1f;

    private int[,] grid;
    private Dictionary<Node, Node> cameFrom;
    private Vector2Int targetPosition;
    private Vector2Int startPosition;
    private List<Node> path = new List<Node>();
    private GameObject movableObjectInstance;
    private Transform player;

    private bool isMoving = false;

    private Vector2Int currentPosition;

    private void Start()
    {
        SetupGrid();
        StartEnemyChase();
        currentPosition = startPosition;
    }

    private void Update()
    {
        MovePrefabAlongPath();

        // Atualize o targetPosition diretamente sem depender do movimento do jogador
        targetPosition = ConvertToWorldToGridCoordinates(player.position);

        // Tenta encontrar um caminho para o novo targetPosition
        path = BreadthFirstSearch(currentPosition, targetPosition);

        MovePrefabAlongPath();
    }

    private void MovePrefabAlongPath()
    {
        if (path != null && path.Count > 0)
        {
            if (path[0] != null) // Verifica se o primeiro nó é válido
            {
                MovePrefabToPosition(new Vector2Int(path[0].x, path[0].z));
                // Remove o nó atual do caminho
                path.RemoveAt(0);
            }
        }
    }

    private void MovePrefabToPosition(Vector2Int position)
    {
        Debug.Log($"Moving to: {position.x}, {position.y}");

        // Move o prefabEnemy para a nova posição
        movableObjectInstance.transform.position = new Vector3(position.x * gridSize, 1f, position.y * gridSize);

        // Atualiza a posição atual
        currentPosition = position;

        // Aguarde um curto período para visualização
        StartCoroutine(WaitForMovement());
    }

    private IEnumerator WaitForMovement()
    {
        yield return new WaitForSeconds(0.1f);
    }

    public void SetGameLevelManager(GameLevelManager manager)
    {
        gameLevelManager = manager;
    }

    private void SetupGrid()
    {
        if (gameLevelManager == null || gameLevelManager.TotalCoordinates == null)
        {
            Debug.LogWarning("GameLevelManager or AllCoordinates is null. Cannot setup grid.");
            return;
        }

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

    private void StartEnemyChase()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found in the scene. Make sure the player has the 'Player' tag.");
            return;
        }

        List<Vector2Int> avoidedCoordinates = gameLevelManager.AvoidedCoordinates;
        startPosition = avoidedCoordinates.Count >= 3 ? avoidedCoordinates[avoidedCoordinates.Count - 3] : Vector2Int.zero;

        Vector2Int playerGridPosition = ConvertToWorldToGridCoordinates(player.position);
        targetPosition = playerGridPosition;

        // Tenta encontrar um caminho direto para o jogador
        path = BreadthFirstSearch(startPosition, targetPosition);

        // Se não há um caminho direto, encontre um caminho para uma posição próxima ao jogador
        if (path == null)
        {
            Vector2Int randomTarget = GetDestructibleWallPosition(playerGridPosition);
            path = BreadthFirstSearch(startPosition, randomTarget);
        }

        // Instancia o objeto móvel
        movableObjectInstance = Instantiate(movableObjectPrefab, new Vector3(startPosition.x * gridSize, 1f, startPosition.y * gridSize), Quaternion.identity);
        movableObjectInstance.transform.localScale = new Vector3(gridSize, gridSize, gridSize);
    }


    private Vector2Int GetDestructibleWallPosition(Vector2Int playerPosition)
    {
        // Define uma área ao redor do jogador para procurar um bloco destrutível
        int searchRadius = 5;

        for (int i = 0; i < searchRadius; i++)
        {
            int randomX = Random.Range(-i, i + 1);
            int randomZ = Random.Range(-i, i + 1);

            Vector2Int randomPos = new Vector2Int(playerPosition.x + randomX, playerPosition.y + randomZ);

            if (IsValidMove(randomPos) && grid[randomPos.x, randomPos.y] == 3)
            {
                return randomPos;
            }
        }

        // Se nenhum bloco destrutível for encontrado nas posições aleatórias, retorna a posição original do jogador
        return playerPosition;
    }





    public void UpdateTargetPosition(Vector3 playerPosition)
    {
        targetPosition = ConvertToWorldToGridCoordinates(playerPosition);
        path = BreadthFirstSearch(startPosition, targetPosition);
    }

    private List<Node> BreadthFirstSearch(Vector2Int start, Vector2Int targetPosition)
    {
        Queue<Node> queue = new Queue<Node>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        cameFrom = new Dictionary<Node, Node>();

        queue.Enqueue(new Node(start.x, start.y, 0, 0, null));

        visited.Add(new Vector2Int(start.x, start.y));

        List<Vector2Int> destructibleBlocks = new List<Vector2Int>();

        while (queue.Count > 0)
        {
            Node currentNode = queue.Dequeue();

            if (currentNode.x == targetPosition.x && currentNode.z == targetPosition.y)
            {
                destructibleBlocks.Add(targetPosition);
                return ReconstructPath(currentNode, start);
            }

            if (!CanReachPlayerFromNode(currentNode))
            {
                Vector2Int destructibleBlock = FindDestructibleBlock(currentNode);
                if (destructibleBlock != Vector2Int.zero && !destructibleBlocks.Contains(destructibleBlock))
                {
                    PlaceBomb(destructibleBlock);
                    StartCoroutine(WaitForExplosion(destructibleBlock));
                    destructibleBlocks.Add(destructibleBlock);
                }
            }

            ExploreNeighbors(currentNode, queue, visited, destructibleBlocks);
        }

        // Se chegamos aqui, não encontramos um caminho direto para o jogador
        // Em vez de retornar null, podemos chamar uma função para explorar randomicamente a grid
        return ExploreRandomly(start);
    }

    private List<Node> ExploreRandomly(Vector2Int start)
    {
        List<Vector2Int> exploredPositions = new List<Vector2Int>();

        for (int attempt = 0; attempt < 5; attempt++) // Limitar a 5 tentativas para evitar loops infinitos
        {
            // Gere uma posição aleatória ao redor do ponto de início
            Vector2Int randomOffset = new Vector2Int(Random.Range(-2, 3), Random.Range(-2, 3));
            Vector2Int randomPosition = start + randomOffset;

            // Verifique se a posição é válida e não foi explorada antes
            if (IsValidMove(randomPosition) && !exploredPositions.Contains(randomPosition))
            {
                exploredPositions.Add(randomPosition);

                // Se a posição contiver um bloco destrutível, coloque uma bomba
                if (grid[randomPosition.x, randomPosition.y] == 3)
                {
                    PlaceBomb(randomPosition);
                    StartCoroutine(WaitForExplosion(randomPosition));
                }

                // Tente encontrar um caminho a partir dessa posição
                List<Node> path = BreadthFirstSearch(start, randomPosition);

                if (path != null)
                {
                    // Caminho encontrado, retorne-o
                    return path;
                }
            }
        }

        // Se não for possível encontrar um caminho após várias tentativas, retorne null
        return null;
    }



    private Vector2Int FindDestructibleBlock(Node node)
    {
        int[] dx = { -1, 1, 0, 0 };
        int[] dz = { 0, 0, -1, 1 };

        for (int i = 0; i < 4; i++)
        {
            int newX = node.x + dx[i];
            int newZ = node.z + dz[i];

            Vector2Int neighborPos = new Vector2Int(newX, newZ);

            if (IsValidMove(neighborPos) && grid[newX, newZ] == 3)
            {
                return neighborPos;
            }
        }

        return Vector2Int.zero;
    }

    private bool CanReachPlayerFromNode(Node node)
    {
        // Verifica se o nó atual está na posição do jogador
        if (node.x == targetPosition.x && node.z == targetPosition.y)
        {
            return true; // O jogador é alcançável a partir deste nó
        }

        // Verifica se há um caminho livre até a posição do jogador
        if (IsValidMove(new Vector2Int(node.x, node.z)) && grid[node.x, node.z] == 0)
        {
            return true; // O jogador é alcançável a partir deste nó
        }

        // Se nenhum dos casos acima for atendido, o jogador não é alcançável a partir deste nó
        return false;
    }

    private void ExploreNeighbors(Node node, Queue<Node> queue, HashSet<Vector2Int> visited, List<Vector2Int> destructibleBlocks)
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
                Node neighborNode = new Node(newX, newZ, node.cost + 1, 0, null);



                visited.Add(neighborPos);
                cameFrom.Add(neighborNode, node);

                // Se o vizinho possui um bloco de parede, ignora
                if (grid[newX, newZ] == 1)
                {
                    // Debug.Log($"Wall detected at: {newX}, {newZ}");
                    continue;
                }

                // Se o vizinho possui um bloco destrutível e ainda não colocamos uma bomba aqui, coloca uma bomba
                if (grid[newX, newZ] == 3 && !destructibleBlocks.Contains(neighborPos))
                {
                    PlaceBomb(neighborPos);
                    StartCoroutine(WaitForExplosion(neighborPos));
                    destructibleBlocks.Add(neighborPos);
                }
                else
                {
                    // Senão, avança para o vizinho
                    queue.Enqueue(neighborNode);
                    Debug.Log($"Advancing to: {newX}, {newZ}");
                }
            }
        }
    }



    private IEnumerator WaitForExplosion(Vector2Int bombPosition)
    {
        //Debug.Log($"Simulating waiting for explosion at {bombPosition}");
        yield return new WaitForSeconds(5); // Simulando espera de 3 segundos
        //Debug.Log($"Simulating explosion at {bombPosition}");
    }

    private void PlaceBomb(Vector2Int bombPosition)
    {
        // Verifica se já há uma bomba na posição
        if (grid[bombPosition.x, bombPosition.y] != 4)
        {
            // Coloca uma bomba
            Debug.Log($"Placed bomb at {bombPosition}");
            grid[bombPosition.x, bombPosition.y] = 4;
        }
        // Caso contrário, já existe uma bomba nessa posição
    }

    private Vector2Int ConvertToWorldToGridCoordinates(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSize);
        int z = Mathf.FloorToInt(worldPosition.z / gridSize);
        return new Vector2Int(x, z);
    }

    private List<Node> ReconstructPath(Node endNode, Vector2Int startPosition)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode.x != startPosition.x || currentNode.z != startPosition.y)
        {
            if (!cameFrom.ContainsKey(currentNode))
            {
                Debug.LogError("Key not found in cameFrom dictionary!");
                break;
            }

            path.Add(currentNode);
            currentNode = cameFrom[currentNode];
        }

        path.Reverse();
        return path;
    }

    private bool IsValidMove(Vector2Int position)
    {
        bool valid = position.x >= 0 && position.x < CreateLevel.Instance.gridSizeX
            && position.y >= 0 && position.y < CreateLevel.Instance.gridSizeZ;

        if (valid)
        {
            int cellValue = grid[position.x, position.y];
            valid = cellValue == 0 || cellValue == 3;
        }

        // Adicione a verificação para paredes (cellValue == 1)
        valid = valid && grid[position.x, position.y] != 1;

        //Debug.Log($"Position: {position}, Valid: {valid}");
        return valid;
    }

}