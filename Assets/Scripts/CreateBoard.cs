using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject cellPrefab; // Refer�ncia ao prefab da c�lula do tabuleiro em 3D
    public GameObject barrierPrefab; // Refer�ncia ao prefab da barreira em 3D
    public int width;
    public int height;

    public void CreateBoard()
    {
        float cellSize = 1.0f; // Tamanho de cada c�lula

        // Posi��o inicial do tabuleiro
        Vector3 boardStartPosition = new Vector3(-(width - 1) / 2f, 0, -(height - 1) / 2f);

        // Itera por todas as c�lulas do tabuleiro
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calcula a posi��o da c�lula com base na largura, altura e tamanho da c�lula
                Vector3 position = new Vector3(x, 0, y) * cellSize + boardStartPosition;

                // Instancia uma c�lula do tabuleiro em 3D na posi��o calculada
                GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity);

                // Define o objeto instanciado como filho do objeto "Tabuleiro" no Hierarchy
                newCell.transform.parent = transform; // "transform" refere-se ao transform do objeto "Tabuleiro"
            }
        }

        // Cria��o da barreira ao redor do tabuleiro
        CreateBarrier();
    }

    void CreateBarrier()
    {
        float cellSize = 1.0f; // Tamanho de cada c�lula

        // Posi��o inicial da barreira
        Vector3 barrierStartPosition = new Vector3(-(width - 1) / 2f, 0, -(height - 1) / 2f);

        // Tamanho da barreira
        float barrierWidth = width * cellSize;
        float barrierHeight = height * cellSize;

        // Cria��o da barreira
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                // Verifica se � uma c�lula nos limites do tabuleiro
                if (x == -1 || x == width || y == -1 || y == height)
                {
                    // Calcula a posi��o da c�lula para criar a barreira
                    Vector3 position = new Vector3(x, 0, y) * cellSize + barrierStartPosition;

                    // Instancia um cubo (ou outro objeto) representando a barreira na posi��o calculada
                    GameObject newBarrier = Instantiate(barrierPrefab, position, Quaternion.identity);

                    // Define o objeto instanciado como filho do objeto "Tabuleiro" no Hierarchy
                    newBarrier.transform.parent = transform; // "transform" refere-se ao transform do objeto "Tabuleiro"
                }
            }
        }
    }

    // Exemplo de uso da fun��o:
    void Start()
    {
        CreateBoard(); // Cria um tabuleiro de 10x10
    }
}


