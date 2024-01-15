using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using NUnit.Framework;
using Unity.VisualScripting;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;
using System.Linq;
using System;
using NUnit.Framework.Constraints;

[CustomEditor(typeof(CreateLevel))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    GameObject groundPrefab;
    GameObject wallPrefab;
    GameObject innerWallPrefab;
    GameObject destWallPrefab;

    Camera gameCamera;

    CreateLevel create;

    bool scriptActive;

    //Inner coordinates for the board game
    List<Vector2Int> innerCoordinates = new List<Vector2Int>();

    // Torna cleanCoordinates uma propriedade pública
    public List<Vector2Int> CleanCoordinates { get; private set; }

    //Function to create inspector game board
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        create = (CreateLevel)target;

        groundPrefab = create.ground;
        wallPrefab = create.wall;
        destWallPrefab = create.destructableWall;
        innerWallPrefab = create.innerWall;

        EditorGUILayout.BeginHorizontal();
        //Create a complete boarder from UI
        if (GUILayout.Button("Create Border"))
        {
            if (!scriptActive)
            {
                //Manager ground
                BuildGround();
                //Manager border
                BuildBorder();
                //Build destructable walls
                BuildDestructableWalls();
                //Inner walls
                BuildInnerWalls();
                //Ajust camera
                AdjustCameraToBoard();
            }
        }

        //Delete a complete boarder from UI
        if (GUILayout.Button("Delete Border"))
        {
            if (!scriptActive)
            {
                //Delete ground
                DeleteGround();
                //Delete ground
                DeleteBorder();
                //Delete innerWalls
                DeleteInnerWalls();
                //Delete destructable walls
                DeleteDestructableWalls();
            }
        }
        EditorGUILayout.EndHorizontal();

        //Inner walls
        EditorGUILayout.BeginHorizontal();
        //Create a complete boarder from UI
        if (GUILayout.Button("Create InnerWallls"))
        {
            if (!scriptActive)
            {
                BuildInnerWalls();
            }
        }

        //Delete a complete boarder from UI
        if (GUILayout.Button("Delete InnerWallls"))
        {
            if (!scriptActive)
            {
                //DeleteInnerWalls();
                DeleteDestructableWalls();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    //Function to create game ground
    void BuildGround()
    {
        //Check to have a minimal size board game
        if (create.gridSizeX < 5 || create.gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (create.gridSizeX % 2 == 0 || create.gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }

        DeleteGround();
        scriptActive = true;

        //Create board Game
        for (int i = 0; i < create.gridSizeX; i++)
        {
            for (int j = 0; j < create.gridSizeZ; j++)
            {
                // Se não estiver nas bordas, cria o solo
                if (i != 0 && i != create.gridSizeX - 1 && j != 0 && j != create.gridSizeZ - 1)
                {
                    GameObject ground = PrefabUtility.InstantiatePrefab(groundPrefab) as GameObject;
                    ground.transform.position = new Vector3(
                        create.start.x + i + create.offset.x,
                        create.start.y + create.offset.y,
                        create.start.z + j + create.offset.z);
                    ground.transform.parent = create.groundHolder;

                }
            }
        }
        //ResizeGroundPLane();
        scriptActive = false;
    }
    //Function to delete ground
    void DeleteGround()
    {
        int childCount = create.groundHolder.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(create.groundHolder.transform.GetChild(i).gameObject);
        }
    }
    //Function to create board boarder
    void BuildBorder()
    {
        //Check to have a minimal size board game
        if (create.gridSizeX < 5 || create.gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (create.gridSizeX % 2 == 0 || create.gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }

        DeleteBorder();
        scriptActive = true;

        //Create board Game
        for (int i = 0; i < create.gridSizeX; i++)
        {
            for (int j = 0; j < create.gridSizeZ; j++)
            {
                if (i == 0 || i == create.gridSizeX - 1)
                {
                    GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        create.start.x + i + create.offset.x,
                        create.start.y + create.offset.y,
                        create.start.z + j + create.offset.z);
                    wall.transform.parent = create.outerWallHolder;
                }


                if (j == 0 || j == create.gridSizeZ - 1)
                {
                    GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        create.start.x + i + create.offset.x,
                        create.start.y + create.offset.y,
                        create.start.z + j + create.offset.z);
                    wall.transform.parent = create.outerWallHolder;
                }
            }
        }

        //ResizeGroundPLane();
        scriptActive = false;
    }
    //Function to delete boarder
    void DeleteBorder()
    {
        int childCount = create.outerWallHolder.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(create.outerWallHolder.transform.GetChild(i).gameObject);
        }
    }
    //Function to create inner walls
    void BuildInnerWalls()
    {
        //Check to have a minimal size board game
        if (create.gridSizeX < 5 || create.gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (create.gridSizeX % 2 == 0 || create.gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }
        DeleteInnerWalls();
        scriptActive = true;

        int dist = 2;

        for (int i = dist; i <= create.gridSizeX - dist; i++) {
            for (int j = dist; j <= create.gridSizeZ - dist; j++)
            {
                if ((i % dist) == 0 && (j % dist) == 0)
                {

                    GameObject wall = PrefabUtility.InstantiatePrefab(innerWallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        create.start.x + i + create.offset.x,
                        create.start.y + create.offset.y,
                        create.start.z + j + create.offset.z);
                    wall.transform.parent = create.innerWallHolder;
                    Vector2Int currentInnerCoordinate = new Vector2Int(i, j);

                    //Inner coordinates
                    innerCoordinates.Add(currentInnerCoordinate);
                }
            }
        }
        scriptActive = false;
    }
    //Function to delete inner walls
    void DeleteInnerWalls()
    {
        int childCount = create.innerWallHolder.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(create.innerWallHolder.transform.GetChild(i).gameObject);
        }
    }
    //Function to create destructable walls
    void BuildDestructableWalls()
    {
        //Check to have a minimal size board game
        if (create.gridSizeX < 5 || create.gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (create.gridSizeX % 2 == 0 || create.gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }

        DeleteDestructableWalls();
        scriptActive = true;

        float wallPlacementProbability = 0.5f; // Probabilidade de colocação da parede
        System.Random random = new System.Random(); // Gerador de números aleatórios


        // Calcular a última posição considerando o offset
        int lastPositionX = Mathf.RoundToInt(create.gridSizeX + create.start.x + create.offset.x);
        int lastPositionZ = Mathf.RoundToInt(create.gridSizeZ + create.start.z + create.offset.z);

       
        //All available coordinates for the board game
        List<Vector2Int> gameCoordinates = new List<Vector2Int>();
        for (int i = Mathf.RoundToInt(create.start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(create.start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);

                // Verifica se a célula não está entre as peças fixas e não está fora dos limites do tabuleiro
                if (i < create.gridSizeX && j < create.gridSizeZ )
                {
                    gameCoordinates.Add(currentCoordinate);
                }
            }
        }

        // Creating a list of modified coordinates (avoidedCoordinates)
        // Generating avoidedCoordinates from gameCoordinates
        List<Vector2Int> avoidedCoordinates = new List<Vector2Int>();

        //For the first coordinates
        Vector2Int startCoordinate0 = new Vector2Int(1, 1);
        Vector2Int startCoordinate1 = new Vector2Int(2, 1);
        Vector2Int startCoordinate2 = new Vector2Int(1, 2);

        avoidedCoordinates.Add(startCoordinate0);
        avoidedCoordinates.Add(startCoordinate1);
        avoidedCoordinates.Add(startCoordinate2);
     
         //For the last coordinates 
         for (int i = 0; i < gameCoordinates.Count; i++)
         {
             Vector2Int lastCoordinate = gameCoordinates[gameCoordinates.Count - 1];

             int last1 = lastCoordinate.x - 1;
             int last2 = lastCoordinate.y - 1;
             int last3 = lastCoordinate.y - 2;           
             int last4 = lastCoordinate.x - 2;

             Vector2Int endCoordinate0 = new Vector2Int(last1, last2);
             Vector2Int endCoordinate1 = new Vector2Int(last1, last3);
             Vector2Int endCoordinate2 = new Vector2Int(last4, last2);

             avoidedCoordinates.Add(endCoordinate0);
             avoidedCoordinates.Add(endCoordinate1);
             avoidedCoordinates.Add(endCoordinate2);
         }

        // Remover duplicatas usando Distinct
        List<Vector2Int> uniqueAvoidedCoordinates = avoidedCoordinates.Distinct().ToList();

        //This list have only the available places to put a block, without a avoidplaces
        List<Vector2Int> availableCoordinates = new List<Vector2Int>();

        for (int i = Mathf.RoundToInt(create.start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(create.start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);

                // Verifica se a coordenada está nas bordas
                bool isOnBorder = i == 0 || i == create.gridSizeX - 1 || j == 0 || j == create.gridSizeZ - 1;

                // Verifica se a coordenada não está na lista de coordenadas a serem evitadas
                if (!isOnBorder && !avoidedCoordinates.Contains(currentCoordinate))
                {
                    availableCoordinates.Add(currentCoordinate);                   
                }
            }
        }

        //Remove inner coordinates wall from the availabe coordinates list
        availableCoordinates.RemoveAll(innerCoordinates.Contains);

        //Vai receber os espaços que ficam completamente livres no tabuleiro,
        //exepto os espaços definidos para os avatares
        List<Vector2Int> occupiedCoordinates = new List<Vector2Int>();

        //create random blocks inside board only in available coordinates
        foreach (Vector2Int coord in availableCoordinates)
        {
            
            // Verifica se a probabilidade permite a colocação de um bloco
            if (random.NextDouble() < wallPlacementProbability)
            {
                GameObject wall = PrefabUtility.InstantiatePrefab(destWallPrefab) as GameObject;
                wall.transform.position = new Vector3(
                    create.start.x + coord.x + create.offset.x,
                    create.start.y + create.offset.y,
                    create.start.z + coord.y + create.offset.z);
                wall.transform.parent = create.destructableHolder;

                // Adiciona a coordenada ocupada à lista
                occupiedCoordinates.Add(coord);
            }
        }

        // Agora, unoccupiedCoordinates contém as coordenadas não ocupadas
        List<Vector2Int> cleanCoordinates = availableCoordinates.Except(occupiedCoordinates).ToList();
        //Adicioma mos as restantes coordendas que foram removidas para intanciar os jogadores, para podermos ter todos os espaços em branco 
        cleanCoordinates.AddRange(uniqueAvoidedCoordinates);

        // Define a propriedade CleanCoordinates com a lista final
        CleanCoordinates = cleanCoordinates;
        /*
        foreach (Vector2Int cleancCord in cleanCoordinates)
        {
            Debug.Log("Coordenadas não ocupadas: " + cleancCord);
        }
        */
        
        scriptActive = false;
    }
    //Function to delete destructable walls
    void DeleteDestructableWalls()
    {
        int childCount = create.destructableHolder.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(create.destructableHolder.transform.GetChild(i).gameObject);
        }
    }
    // Function to adapt camera view
    private void AdjustCameraToBoard()
    {
        gameCamera = FindObjectOfType<Camera>(); // Find the camera in the scene

        if (gameCamera == null)
        {
            Debug.LogError("Camera not assigned.");
            return;
        }

        float boardWidth = create.gridSizeX;
        float boardHeight = create.gridSizeZ;

        Vector3 cameraPosition = new Vector3(
            create.start.x + boardWidth / 2,
            create.start.y + boardHeight * 1.5f, // Adjust height as needed
            create.start.z - boardHeight / 2);

        gameCamera.transform.position = cameraPosition;

        Vector3 lookAtPoint = new Vector3(
            create.start.x + boardWidth / 2f,
            create.start.y,
            create.start.z - boardHeight / 2f);

        gameCamera.transform.LookAt(lookAtPoint);

        // Apply rotation around the X-axis
        float xRotationAngle = 55f; // Replace this value with the desired angle
        Quaternion xRotation = Quaternion.Euler(xRotationAngle, 0f, 0f);
        gameCamera.transform.rotation = xRotation;

        // Calculate field of view based on the board size (if needed)
        // float aspectRatio = Screen.width / (float)Screen.height;
        // float distanceToBoard = Mathf.Max(boardWidth, boardHeight) * 0.5f / Mathf.Tan(gameCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        // float fov = Mathf.Atan(Mathf.Max(boardWidth, boardHeight) * 0.5f / distanceToBoard) * 2f * Mathf.Rad2Deg;
        // gameCamera.fieldOfView = fov;
    }
}
