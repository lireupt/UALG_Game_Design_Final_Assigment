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
    public int value; // Adicionando um valor int na classe

    public Coordinate(int x, int y, int value)
    {
        this.x = x;
        this.y = y;
        this.value = value;
    }
}


public class GameLevelManager : MonoBehaviour
{
    public CreateLevel create;

    public GameObject destWallPrefab;

    public List<Coordinate> BoardGame { get; private set; }
    public List<Coordinate> CoordExt { get; private set; }
    public List<Coordinate> CoordInt { get; private set; }


    /*
     *  Lista de coordenadas que sao o espaço do Inimigo e do player
     */
     public List<Vector2Int> AvoidedCoordinates { get; private set; }
  
    /*
     * Lista que recebe as coordenadas filtradas 
     */
    public List<Coordinate> FilterGroundCoordinates { get; private set; }

    /*
     * Nova lista para armazenar as coordenadas das paredes destrutíveis
     */
    public List<Vector2Int> CleanCoordinates { get; private set; }

    // Todas as coordenadas juntas
    /*
     * Esta Lista contém separados por grupos 0 1 2 3 somentes os espaços ocupados por blocos
     */
   
    public List<Coordinate> TotalCoordinates { get; private set; }

    void Awake()
    {
        InitializeLists(); // Chame a inicialização no Awake ou Start
    }

    private void Start()
    {
        BuildDestructableWalls();
        //PrintAllCoordinates();
    }

    void InitializeLists()
    {
        int gridSizeX = create.gridSizeX;
        int gridSizeZ = create.gridSizeZ;

        BoardGame = CreateCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, value: 0);
        CoordExt = CreateCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, value: 1, exterior: true);
        CoordInt = CreateInteriorCoordinatesList(sizeX: gridSizeX, sizeZ: gridSizeZ, dist: 2, value: 2);

        /*
        // Inicialização da lista AllCoordinates
        AllCoordinates = new List<Coordinate>();

        // Adiciona os elementos às listas
        AllCoordinates.AddRange(CoordExt);
        AllCoordinates.AddRange(CoordInt);
        */

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

    // Método para obter coordenadas filtradas
    List<Coordinate> GetFilteredCoordinates(List<Coordinate> source, List<Coordinate> exclude1, List<Coordinate> exclude2, List<Coordinate> exclude3)
    {
        List<Coordinate> filteredCoordinates = new List<Coordinate>();

        foreach (Coordinate coord in source)
        {
            // Verificar se a coordenada não existe nas listas de exclusão
            if (!exclude1.Any(c => c.x == coord.x && c.y == coord.y) &&
                !exclude2.Any(c => c.x == coord.x && c.y == coord.y) &&
                !exclude3.Any(c => c.x == coord.x && c.y == coord.y))
            {
                // Adicionar a coordenada à lista filtrada
                filteredCoordinates.Add(coord);
            }
        }

        return filteredCoordinates;
    }


    /*Metodo que vai gerar aleatoriamente os blocos destrutiveis
        * 
        * Neste metodo tivemos de manipular os dados das várias listas 
        * a estrategia passou por receber as coordenadas das paredes
        * e de seguida recebemos as coordenadas dos blocos que são inquebraveis 
        * foram feitos calculos para deixar as coordenadas onde os avatares vao iniciar o jogo
        * Player freespace begin game -> (1,1)(1,2)(2,1)
        * Enemy freespace begin game -> como o tabuleiro pode ser gerado aleatoriamente com uma medida definida pelo utilizador,
        * para definir o espaço de inicio de jogo do inimigo, foi efetuado o calculo do tamanho final do tabuleiro e 
        * identificadas as coordenadas e removidas da lista de espaços do tabuleiro para poder ficar o espaço livre para o ENEMY
        * 
        * Os blocos destrutiveis são colocados aleatóriamente, para promover o auto - level.
        *  
       */

    void BuildDestructableWalls()
    {
        List<Coordinate> DestructibleWalls = new List<Coordinate>();
        
        float wallPlacementProbability = 0.5f;
        System.Random random = new System.Random();
        
        int lastPositionX = Mathf.RoundToInt(create.gridSizeX + create.start.x + create.offset.x);
        int lastPositionZ = Mathf.RoundToInt(create.gridSizeZ + create.start.z + create.offset.z);

        List<Vector2Int> gameCoordinates = new List<Vector2Int>();
        for (int i = Mathf.RoundToInt(create.start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(create.start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);
                if (i < create.gridSizeX && j < create.gridSizeZ)
                {
                    gameCoordinates.Add(currentCoordinate);
                }
            }
        }

        //List<Vector2Int> avoidedCoordinates = new List<Vector2Int>();

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
        for (int i = Mathf.RoundToInt(create.start.x); i < lastPositionX; i++)
        {
            for (int j = Mathf.RoundToInt(create.start.z); j < lastPositionZ; j++)
            {
                Vector2Int currentCoordinate = new Vector2Int(i, j);
                bool isOnBorder = i == 0 || i == create.gridSizeX - 1 || j == 0 || j == create.gridSizeZ - 1;

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
                    create.start.x + coord.x + create.offset.x,
                    create.start.y + create.offset.y,
                    create.start.z + coord.y + create.offset.z);
                wall.transform.parent = create.destructableHolder;

                destructibleWalls.Add(new Coordinate(coord.x, coord.y, 3));
            }
        }

        List<Vector2Int> cleanCoordinates = gameCoordinates.Except(occupiedCoordinates).ToList();
        cleanCoordinates.AddRange(uniqueAvoidedCoordinates);

        // Remova ou ajuste a lógica conforme necessário

        CleanCoordinates = cleanCoordinates;
        DestructibleWalls = GetFilteredCoordinates(destructibleWalls, CoordExt, CoordInt, DestructibleWalls);
        FilterGroundCoordinates = GetFilteredCoordinates(BoardGame, CoordExt, CoordInt, DestructibleWalls);

        // Junte as listas em TotalCoordinates
        TotalCoordinates = new List<Coordinate>();
        TotalCoordinates.AddRange(FilterGroundCoordinates);
        TotalCoordinates.AddRange(DestructibleWalls);


        TotalCoordinates.AddRange(CoordExt);
        TotalCoordinates.AddRange(CoordInt);
    }

    // Método para imprimir todas as coordenadas no console
    private void PrintAllCoordinates()
    {
        Debug.Log($"Type of AllCoordinates: {FilterGroundCoordinates.GetType()}");
        Debug.Log($"Total number of elements in AllCoordinates: {FilterGroundCoordinates.Count}");

        foreach (Coordinate coordinate in TotalCoordinates)
        {
            Debug.Log($"X: {coordinate.x}, Y: {coordinate.y}, Valor: {coordinate.value}");
        }
    }
}
