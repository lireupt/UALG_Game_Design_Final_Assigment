using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Coordinate
{
    public int x;
    public int y;
    public int value; // Adding an int value to the class

    public Coordinate(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }
}

public class GameLevelManager : MonoBehaviour
{
    //public CreateLevel create;

    public GameObject destWallPrefab;
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject innerWallPrefab;

    public Vector3 start;
    public Vector3 offset;


    NumberChoice number;
    

    public int gridSizeX;
    public int gridSizeZ;

    NumberChoiceController numberGrid;

    public List<Coordinate> BoardGame { get; private set; }
    public List<Coordinate> CoordExt { get; private set; }
    public List<Coordinate> CoordInt { get; private set; }

    /*
     * List of coordinates that represent the space for the enemy and the player
     */
    public List<Vector2Int> AvoidedCoordinates { get; private set; }

    //Inner coordinates for the board game
    List<Vector2Int> innerCoordinates = new List<Vector2Int>();

    /*
     * List that receives the filtered coordinates
     */
    public List<Coordinate> FilterGroundCoordinates { get; private set; }

    /*
     * New list to store the coordinates of destructible walls
     */
    public List<Vector2Int> CleanCoordinates { get; private set; }

    // All coordinates together
    /*
     * This List contains, separated by groups 0, 1, 2, 3, only the spaces occupied by blocks
     */
    public List<Coordinate> TotalCoordinates { get; private set; }

    void Awake()
    {
        InitializeLists(); // Call initialization in Awake or Start

   
    }

    private void Start()
    {
        BuildDestructableWalls();
        BuildGround();
        BuildBorder();
        BuildInnerWalls();
    }

    void InitializeLists()
    {
        //int gridSizeX = gridSizeX;
        //Debug.Log(gridSizeX);
        //int gridSizeZ = gridSizeZ;

        BoardGame = CreateCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, value: 0);
        CoordExt = CreateCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, value: 1, exterior: true);
        CoordInt = CreateInteriorCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, dist: 2, value: 2);
    }

    List<Coordinate> CreateCoordinatesList(int sizeX, int sizeZ, int value, bool exterior = false)
    {
        List<Coordinate> coordinatesList = new List<Coordinate>();
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeZ; j++)
            {
                if ((!exterior && i != 0 && i != sizeX - 1 && j != 0 && j != sizeZ - 1) ||
                    (exterior && (i == 0 || i == sizeX - 1 || j == 0 || j == sizeZ - 1)))
                {
                    coordinatesList.Add(new Coordinate(i, j, value));
                }
            }
        }
        return coordinatesList;
    }

    List<Coordinate> CreateInteriorCoordinatesList(int sizeX, int sizeZ, int dist, int value)
    {
        List<Coordinate> interiorCoordinatesList = new List<Coordinate>();
        for (int i = dist; i <= sizeX - dist; i += dist)
        {
            for (int j = dist; j <= sizeZ - dist; j += dist)
            {
                interiorCoordinatesList.Add(new Coordinate(i, j, value));
            }
        }
        return interiorCoordinatesList;
    }

    List<Coordinate> GetFilteredCoordinates(List<Coordinate> source, List<Coordinate> exclude1, List<Coordinate> exclude2, List<Coordinate> exclude3)
    {
        List<Coordinate> filteredCoordinates = new List<Coordinate>();

        foreach (Coordinate coord in source)
        {
            // Check if the coordinate does not exist in the exclusion lists
            if (!exclude1.Any(c => c.x == coord.x && c.y == coord.y) &&
                !exclude2.Any(c => c.x == coord.x && c.y == coord.y) &&
                !exclude3.Any(c => c.x == coord.x && c.y == coord.y))
            {
                // Add the coordinate to the filtered list
                filteredCoordinates.Add(coord);
            }
        }

        return filteredCoordinates;
    }

    void BuildGround()
    {
        //Check to have a minimal size board game
        if (gridSizeX < 5 || gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (gridSizeX % 2 == 0 || gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }

        //DeleteGround();
        //scriptActive = true;

        //Create board Game
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                // Se não estiver nas bordas, cria o solo
                if (i != 0 && i != gridSizeX - 1 && j != 0 && j != gridSizeZ - 1)
                {
                    GameObject ground = PrefabUtility.InstantiatePrefab(groundPrefab) as GameObject;
                    ground.transform.position = new Vector3(
                        start.x + i + offset.x,
                        start.y + offset.y,
                        start.z + j + offset.z);
                    //ground.transform.parent = create.groundHolder;

                }
            }
        }
        //ResizeGroundPLane();
        //scriptActive = false;
    }

    void BuildBorder()
    {
        //Check to have a minimal size board game
        if (gridSizeX < 5 || gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (gridSizeX % 2 == 0 || gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }

       // DeleteBorder();
      //  scriptActive = true;

        //Create board Game
        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeZ; j++)
            {
                if (i == 0 || i == gridSizeX - 1)
                {
                    GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        start.x + i + offset.x,
                        start.y + offset.y,
                        start.z + j + offset.z);
                    //wall.transform.parent = create.outerWallHolder;
                }


                if (j == 0 || j == gridSizeZ - 1)
                {
                    GameObject wall = PrefabUtility.InstantiatePrefab(wallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        start.x + i + offset.x,
                        start.y + offset.y,
                        start.z + j + offset.z);
                    //wall.transform.parent = create.outerWallHolder;
                }
            }
        }

        //ResizeGroundPLane();
       // scriptActive = false;
    }


    //Function to create inner walls
    void BuildInnerWalls()
    {
        //Check to have a minimal size board game
        if (gridSizeX < 5 || gridSizeZ < 5)
        {
            Debug.LogWarning("Grid size need's to be bigger or equal than 5");
            return;
        }
        //Check to have a round board game
        if (gridSizeX % 2 == 0 || gridSizeZ % 2 == 0)
        {
            Debug.LogWarning("Grid size need to be uneven numbers");
            return;
        }
       // DeleteInnerWalls();
       // scriptActive = true;

        int dist = 2;

        for (int i = dist; i <= gridSizeX - dist; i++)
        {
            for (int j = dist; j <= gridSizeZ - dist; j++)
            {
                if ((i % dist) == 0 && (j % dist) == 0)
                {

                    GameObject wall = PrefabUtility.InstantiatePrefab(innerWallPrefab) as GameObject;
                    wall.transform.position = new Vector3(
                        start.x + i + offset.x,
                        start.y + offset.y,
                        start.z + j + offset.z);
                    //wall.transform.parent = create.innerWallHolder;
                    Vector2Int currentInnerCoordinate = new Vector2Int(i, j);

                    //Inner coordinates
                    innerCoordinates.Add(currentInnerCoordinate);
                }
            }
        }
        //scriptActive = false;
    }

    /* Method that will randomly generate the destructible blocks
     * In this method, we had to manipulate the data from various lists.
     * The strategy was to receive the coordinates of the walls
     * and then we received the coordinates of the blocks that are unbreakable
     * calculations were made to leave the coordinates where the avatars will start the game
     * Player freespace begin game -> (1,1)(1,2)(2,1)
     * Enemy freespace begin game -> as the board can be generated randomly with a size defined by the user,
     * to define the starting space of the enemy, the calculation of the final size of the board was performed and
     * the coordinates were identified and removed from the list of spaces on the board so that space would be free for the ENEMY
     * 
     * The destructible blocks are placed randomly to promote auto-leveling.
     * 
    */

    void BuildDestructableWalls()
    {
        List<Coordinate> DestructibleWalls = new List<Coordinate>();

        float wallPlacementProbability = 0.5f;
        System.Random random = new System.Random();

        int lastPositionX = Mathf.RoundToInt(gridSizeX + start.x + offset.x);
        int lastPositionZ = Mathf.RoundToInt(gridSizeZ + start.z + offset.z);

        List<Vector2Int> gameCoordinates = new List<Vector2Int>();
        for (int i = Mathf.RoundToInt(start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);
                if (i < gridSizeX && j < gridSizeZ)
                {
                    gameCoordinates.Add(currentCoordinate);
                }
            }
        }

        AvoidedCoordinates = new List<Vector2Int>();

        Vector2Int startCoordinate0 = new Vector2Int(1, 1);
        Vector2Int startCoordinate1 = new Vector2Int(2, 1);
        Vector2Int startCoordinate2 = new Vector2Int(1, 2);

        AvoidedCoordinates.Add(startCoordinate0);
        AvoidedCoordinates.Add(startCoordinate1);
        AvoidedCoordinates.Add(startCoordinate2);

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

            AvoidedCoordinates.Add(endCoordinate0);
            AvoidedCoordinates.Add(endCoordinate1);
            AvoidedCoordinates.Add(endCoordinate2);
        }

        List<Vector2Int> uniqueAvoidedCoordinates = AvoidedCoordinates.Distinct().ToList();

        List<Vector2Int> availableCoordinates = new List<Vector2Int>();
        for (int i = Mathf.RoundToInt(start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);
                bool isOnBorder = i == 0 || i == gridSizeX - 1 || j == 0 || j == gridSizeZ - 1;

                if (!isOnBorder && !AvoidedCoordinates.Contains(currentCoordinate))
                {
                    availableCoordinates.Add(currentCoordinate);
                }
            }
        }

        availableCoordinates.RemoveAll(coord => CoordInt.Any(c => c.x == coord.x && c.y == coord.y));

        List<Vector2Int> occupiedCoordinates = new List<Vector2Int>();
        List<Coordinate> destructibleWalls = new List<Coordinate>();
        foreach (Vector2Int coord in availableCoordinates)
        {
            if (random.NextDouble() < wallPlacementProbability)
            {
                GameObject wall = PrefabUtility.InstantiatePrefab(destWallPrefab) as GameObject;
                wall.transform.position = new Vector3(
                    start.x + coord.x + offset.x,
                    start.y + offset.y,
                    start.z + coord.y + offset.z);
                //wall.transform.parent = create.destructableHolder;

                destructibleWalls.Add(new Coordinate(coord.x, coord.y, 3));
            }
        }

        List<Vector2Int> cleanCoordinates = gameCoordinates.Except(occupiedCoordinates).ToList();
        cleanCoordinates.AddRange(uniqueAvoidedCoordinates);

        CleanCoordinates = cleanCoordinates;
        DestructibleWalls = GetFilteredCoordinates(destructibleWalls, CoordExt, CoordInt, DestructibleWalls);
        FilterGroundCoordinates = GetFilteredCoordinates(BoardGame, CoordExt, CoordInt, DestructibleWalls);

        // Join the lists into TotalCoordinates
        TotalCoordinates = new List<Coordinate>();
        TotalCoordinates.AddRange(FilterGroundCoordinates);
        TotalCoordinates.AddRange(DestructibleWalls);

        TotalCoordinates.AddRange(CoordExt);
        TotalCoordinates.AddRange(CoordInt);
    }

    // Method to print all coordinates to the console
    private void PrintAllCoordinates()
    {
        Debug.Log($"Type of AllCoordinates: {FilterGroundCoordinates.GetType()}");
        Debug.Log($"Total number of elements in AllCoordinates: {FilterGroundCoordinates.Count}");

        foreach (Coordinate coordinate in TotalCoordinates)
        {
            Debug.Log($"X: {coordinate.x}, Y: {coordinate.y}, Value: {coordinate.value}");
        }
    }
}
