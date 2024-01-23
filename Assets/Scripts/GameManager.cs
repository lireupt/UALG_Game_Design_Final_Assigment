using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private int lives = 3;
    private int maxBomb = 1;
    private int explodeRange = 1;
    private int enemyInLevel = 0;

    [SerializeField] private GameObject playerPrefab;
    // Start is called before the first frame update
    [SerializeField] private float delayPlayer = 1f;

    private PlayerController currentPlayer;
    [SerializeField] float delayToPlayer = 1f;

    [SerializeField] private Text timerText;
    public float timeLimit = 60f; // Tempo limite em segundos
    private float currentTime = 0f;
    private bool isTimerRunning = true;
    private bool isPaused = false;
    [SerializeField] private Text liveText;
    [SerializeField] private Text bombText;
    [SerializeField] private Text rangeText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;



    void Start()
    {
        SpawnPlayer();
        UpdateLivesText();
        UpdateBombsText();
        UpdateRangeText();
        GetEnemyInScene();
       
        enemyInLevel = GetEnemyInScene();
        Debug.Log(enemyInLevel);


    }

    private void Update()
    {
        if (isTimerRunning)
        {
            // Atualiza o tempo atual
            currentTime += Time.deltaTime;

            // Verifica se o tempo limite foi atingido
            if (currentTime >= timeLimit)
            {
                // O tempo limite foi atingido, faça algo aqui (por exemplo, fim do jogo)
                Debug.Log("Tempo Limite Atingido!");
                isTimerRunning = false; // Pausa o timer
            }
            // Atualiza o texto com o tempo formatado
            UpdateTimerText();
        }
    }

    // Função para reiniciar o timer
    private void UpdateTimerText()
    {
        // Atualiza o texto com o tempo formatado
        if (timerText != null)
        {
            float remainingTime = Mathf.Max(0f, timeLimit - currentTime);
            string formattedTime = FormatTime(remainingTime);
            timerText.text = "Tempo: " + formattedTime;
        }
    }

    private string FormatTime(float seconds)
    {
        // Formata o tempo como minutos:segundos
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }

    public void PlayerDied()
    {
        if (lives > 1)
        {
            lives--;
            Invoke("SpawnPlayer", currentPlayer.GetDestroyerTime() + delayToPlayer);
        }
        else
        {
            //Debug.Log("Game Over");
            GameOver();
        }
        Debug.Log("Player died");
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(1, 0.5f, 1), Quaternion.identity);
        currentPlayer = player.GetComponent<PlayerController>();
        currentPlayer.initializePlayer(maxBomb);
        UpdateLivesText();
    }

    private void UpdateLivesText()
    {
        liveText.text = "Vidas: " + lives.ToString("D1");
    }


    private void UpdateBombsText()
    {
        bombText.text = "Bombs: " + lives.ToString("D1");
    }

    private void UpdateRangeText()
    {
        rangeText.text = "Range: " + explodeRange.ToString("D1");
    }

    public int GetExplodeRange()
    {
        return explodeRange;
    }

    public void PauseButton ()
    {
        if (isPaused)
        {
            pausePanel.SetActive(false);
            currentPlayer.SetPaused(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
        else
        {
            pausePanel.SetActive(true);
            currentPlayer.SetPaused(true);
            isPaused = true;
            Time.timeScale = 0f;
        }
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private int GetEnemyInScene()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = enemies.Length;
        return count;
    }

    public void EnemyHasDied()
    {
        enemyInLevel--;

        Debug.Log("Enemies inthis level: " + enemyInLevel);
        if (enemyInLevel <= 0)
        {
            // Esta desativado pk o inimigo esta constantemente a bater na bomba  e mata-se a ele proprio  
            //winPanel.SetActive(true);
            Debug.Log("Level complete: ");
        }
    }

}