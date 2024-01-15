using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public GameObject cellPrefab; // Referência ao prefab da célula do tabuleiro em 3D
    public GameObject barrierPrefab; // Referência ao prefab da barreira em 3D
    public int width;
    public int height;

    public void CreateBoard()
    {
        float cellSize = 1.0f; // Tamanho de cada célula

        // Posição inicial do tabuleiro
        Vector3 boardStartPosition = new Vector3(-(width - 1) / 2f, 0, -(height - 1) / 2f);

        // Itera por todas as células do tabuleiro
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calcula a posição da célula com base na largura, altura e tamanho da célula
                Vector3 position = new Vector3(x, 0, y) * cellSize + boardStartPosition;

                // Instancia uma célula do tabuleiro em 3D na posição calculada
                GameObject newCell = Instantiate(cellPrefab, position, Quaternion.identity);

                // Define o objeto instanciado como filho do objeto "Tabuleiro" no Hierarchy
                newCell.transform.parent = transform; // "transform" refere-se ao transform do objeto "Tabuleiro"
            }
        }

        // Criação da barreira ao redor do tabuleiro
        CreateBarrier();
    }

    void CreateBarrier()
    {
        float cellSize = 1.0f; // Tamanho de cada célula

        // Posição inicial da barreira
        Vector3 barrierStartPosition = new Vector3(-(width - 1) / 2f, 0, -(height - 1) / 2f);

        // Tamanho da barreira
        float barrierWidth = width * cellSize;
        float barrierHeight = height * cellSize;

        // Criação da barreira
        for (int x = -1; x <= width; x++)
        {
            for (int y = -1; y <= height; y++)
            {
                // Verifica se é uma célula nos limites do tabuleiro
                if (x == -1 || x == width || y == -1 || y == height)
                {
                    // Calcula a posição da célula para criar a barreira
                    Vector3 position = new Vector3(x, 0, y) * cellSize + barrierStartPosition;

                    // Instancia um cubo (ou outro objeto) representando a barreira na posição calculada
                    GameObject newBarrier = Instantiate(barrierPrefab, position, Quaternion.identity);

                    // Define o objeto instanciado como filho do objeto "Tabuleiro" no Hierarchy
                    newBarrier.transform.parent = transform; // "transform" refere-se ao transform do objeto "Tabuleiro"
                }
            }
        }
    }

    // Exemplo de uso da função:
    void Start()
    {
        CreateBoard(); // Cria um tabuleiro de 10x10
    }
}


