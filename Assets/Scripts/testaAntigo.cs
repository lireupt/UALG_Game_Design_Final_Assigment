using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testeAntigoOK : MonoBehaviour
{
    /*
Este aqui so anda as voltas, mas anda para trás
    */
    public GameObject enemyPrefab;  // Prefab do inimigo
    private GameObject enemyInstance;  // Instância do inimigo

    public GameLevelManager gameLevelManager;
    public CreateLevel create;

    private Vector2Int targetPosition;  // Posição alvo (por exemplo, a posição do jogador)

    private int[,] grid;

    private float movementSpeed = 5f;  // Velocidade de movimento do bot


    private void Start()
    {
        SpawnEnemy();
        //StartCoroutine(UpdateEnemyMovement()); // Descomente isso se quiser que o bot se mova automaticamente
        SetupGrid();
        //MoveBotToAdjacentNodes();
    }

    /* private Coroutine moveCoroutine;  // Referência para a coroutine em execução
     */



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //PrintAdjacentNodes();
            //MoveBotBasedOnNodeValue(targetPosition);  // Exemplo: Move o bot para a posição alvo
            StartCoroutine(MoveBotToAdjacentNodes());

            PrintVisitedNodes();


            /* para desligar uma courtime quando acelera o processo de cliques no space, evita o atrufiu 
            // Interrompe a coroutine anterior, se houver uma em execução
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            // Inicia a nova coroutine
            moveCoroutine = StartCoroutine(MoveBotToAdjacentNodes());
            */

        }


    }

    private void PrintVisitedNodes()
    {
        Debug.Log("Visited Nodes:");

        foreach (Vector2Int node in visitedNodes)
        {
            Debug.Log($"Dentro da lista esta atualmente({node.x}, {node.y})");
        }
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

    private void SpawnEnemy()
    {
        List<Vector2Int> avoidedCoordinates = gameLevelManager.AvoidedCoordinates;
        Vector2Int startPosition = avoidedCoordinates.Count >= 3 ? avoidedCoordinates[avoidedCoordinates.Count - 3] : Vector2Int.zero;

        // Instanciar o inimigo na posição inicial evitada
        enemyInstance = Instantiate(enemyPrefab, new Vector3(startPosition.x, 0.5f, startPosition.y), Quaternion.identity);

        // Definir a posição alvo inicial (pode ser a posição do jogador)
        targetPosition = new Vector2Int(0, 0);  // Defina sua lógica para determinar a posição alvo inicial
    }

    private void PrintAdjacentNodes()
    {
        // Obtém a posição atual do inimigo
        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(enemyInstance.transform.position.x),
                                               Mathf.RoundToInt(enemyInstance.transform.position.z));

        Debug.Log("Posição Atual: " + currentPos);

        // Obtém os nós vizinhos ao redor do inimigo (assumindo um grid 2D)
        List<Vector2Int> adjacentNodes = GetAdjacentNodes(currentPos);

        // Imprime os nós vizinhos
        Debug.Log("Nós Vizinhos:");
        foreach (Vector2Int node in adjacentNodes)
        {
            Debug.Log($"({node.x}, {node.y}) {grid[node.x, node.y]}");
        }

        Debug.Log("Grid:");

        for (int z = 0; z < CreateLevel.Instance.gridSizeZ; z++)
        {
            string row = "";

            for (int x = 0; x < CreateLevel.Instance.gridSizeX; x++)
            {
                row += $"{grid[x, z]} ";
            }

            Debug.Log(row);
        }
    }

    private List<Vector2Int> GetAdjacentNodes(Vector2Int centerNode)
    {
        // Lógica para obter os nós vizinhos (por exemplo, os nós ao norte, sul, leste e oeste)
        List<Vector2Int> adjacentNodes = new List<Vector2Int>
        {
            new Vector2Int(centerNode.x, centerNode.y + 1), // Norte
            new Vector2Int(centerNode.x, centerNode.y - 1), // Sul
            new Vector2Int(centerNode.x + 1, centerNode.y), // Leste
            new Vector2Int(centerNode.x - 1, centerNode.y)  // Oeste
        };

        return adjacentNodes;
    }
    private List<Vector2Int> visitedNodes = new List<Vector2Int>();  // Lista de nós visitados
    private List<Vector2Int> allVisitedNodes = new List<Vector2Int>();  // Lista de todos os nós visitados

    private IEnumerator MoveBotToAdjacentNodes()
    {
        // Obtém a posição atual do inimigo
        Vector2Int currentPos = new Vector2Int(Mathf.RoundToInt(enemyInstance.transform.position.x),
                                               Mathf.RoundToInt(enemyInstance.transform.position.z));

        // Obtém os nós vizinhos ao redor do inimigo (assumindo um grid 2D)
        List<Vector2Int> adjacentNodes = GetAdjacentNodes(currentPos);

        // Move o bot para o primeiro nó vizinho que não é uma parede e não foi visitado
        foreach (Vector2Int node in adjacentNodes)
        {
            if (IsDestructibleBlock(node))  // Se o próximo nó for um bloco destrutível
            {
                // Permite que o bot volte para até 5 posições anteriores apenas se encontrar um bloco destrutível (valor 3)
                for (int i = visitedNodes.Count - 1; i >= Mathf.Max(0, visitedNodes.Count - 5); i--)
                {
                    Vector2Int previousPos = visitedNodes[i];
                    if (!IsWall(previousPos))
                    {
                        // Simula a colocação de uma bomba e a destruição do bloco
                        yield return StartCoroutine(SimulateBombPlacement(previousPos));

                        // Move o bot para a posição anterior
                        yield return StartCoroutine(MoveBotToPositionGradual(previousPos));

                        visitedNodes.Add(previousPos);  // Adiciona o nó visitado à lista
                        allVisitedNodes.Add(previousPos);
                        break;
                    }
                }
            }
            else if (!IsWall(node) && !visitedNodes.Contains(node))
            {
                yield return StartCoroutine(MoveBotToPositionGradual(node));
                visitedNodes.Add(node);  // Adiciona o nó visitado à lista

                // Mantém no máximo as últimas 5 posições na lista
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
        // Armazena a posição atual antes do movimento
        Vector2Int currentPosition = new Vector2Int(Mathf.RoundToInt(enemyInstance.transform.position.x),
                                                     Mathf.RoundToInt(enemyInstance.transform.position.z));

        // Limpa a lista e adiciona a nova posição
        visitedNodes.Clear();
        visitedNodes.Add(currentPosition);


        visitedNodes.Add(targetNode);

        Vector3 targetPosition = new Vector3(targetNode.x, 0.5f, targetNode.y);

        while (Vector3.Distance(enemyInstance.transform.position, targetPosition) > 0.01f)
        {
            //ANTIGO
            // Verifica se o próximo nó é uma parede
            if (IsWall(currentPosition))
            {
                Debug.Log($"Encontrou uma parede em ({currentPosition.x}, {currentPosition.y})! Movimento interrompido.");
                yield break;  // Sai da coroutine se encontrou uma parede
            }

            // Move gradualmente em direção ao próximo nó
            enemyInstance.transform.position = Vector3.MoveTowards(enemyInstance.transform.position, targetPosition, movementSpeed * Time.deltaTime);
            yield return null;
        }

        // Garante que o bot está exatamente no nó
        enemyInstance.transform.position = targetPosition;
    }





    private bool IsWall(Vector2Int node)
    {
        // Lógica para verificar se o nó é uma parede
        // Pode ser uma verificação na sua matriz grid[x, y]
        // Retorne true se for uma parede, false caso contrário
        int nodeValue = grid[node.x, node.y];
        return nodeValue == 1 || nodeValue == 2;  // Exemplo: 1 representa uma parede, 2 representa um bloco indestrutível
    }

    private bool IsDestructibleBlock(Vector2Int node)
    {
        // Lógica para verificar se o nó é um bloco destrutível
        // Pode ser uma verificação na sua matriz grid[x, y]
        // Retorne true se for um bloco destrutível, false caso contrário
        int nodeValue = grid[node.x, node.y];
        return nodeValue == 3;  // Exemplo: 3 representa um bloco destrutível
    }

    /*
     * TODO falta ver aqui o rebentamento simultaneo da bomba em duas casas adjecentes
     */
    private IEnumerator SimulateBombPlacement(Vector2Int bombPosition)
    {
        // Simula a colocação de uma bomba (pode adicionar lógica adicional aqui)
        Debug.Log($"Colocou uma bomba em ({bombPosition.x}, {bombPosition.y})");

        // Move o bot de volta para a posição anterior
        yield return StartCoroutine(MoveBotToPositionGradual(bombPosition));

        // Adiciona o nó visitado à lista
        visitedNodes.Add(bombPosition);

        // Atualiza os blocos destrutíveis diretamente adjacentes à bomba
        for (int i = 0; i < gameLevelManager.TotalCoordinates.Count; i++)
        {
            Vector2Int blockPosition = new Vector2Int(gameLevelManager.TotalCoordinates[i].x, gameLevelManager.TotalCoordinates[i].y);

            if (IsDestructibleBlock(blockPosition) &&
                ((Mathf.Abs(blockPosition.x - bombPosition.x) == 1 && blockPosition.y == bombPosition.y) ||
                 (Mathf.Abs(blockPosition.y - bombPosition.y) == 1 && blockPosition.x == bombPosition.x)))
            {
                // Atualiza o valor do bloco destrutível para 0
                gameLevelManager.TotalCoordinates[i].value = 0;
                grid[blockPosition.x, blockPosition.y] = 0;  // Atualiza o valor na grid
                Debug.Log($"Bloco destruído em ({blockPosition.x}, {blockPosition.y})");
            }
        }
    }
    
    



}