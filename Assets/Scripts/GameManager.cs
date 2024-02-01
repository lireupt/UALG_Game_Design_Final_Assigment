using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private int lives = 3;
    private int maxBomb = 1;
    private int explodeRange = 1;
    private int enemyInLevel = 0;
    private float moveSpeed = 4f;
    private int speedCounter = 1;
    private float speedIncrease = 0.4f;
    private PlayerController currentPlayer;

    [SerializeField] private int bombLimit = 6;
    [SerializeField] private int explodeLimit = 5;
    [SerializeField] private int speedLimit = 5;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private float delayToPlayer = 1f;

    public float timeLimit = 60f;
    private float currentTime = 0f;
    private bool isTimerRunning = true;
    private bool isPaused = false;

    // Canvas text presentation
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI liveText;
    [SerializeField] private TextMeshProUGUI bombText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject winPanel;

    void Start()
    {
        SpawnPlayer();
        UpdateLivesText();
        UpdateBombsText();
        UpdateRangeText();
        UpdateSpeedText();
        GetEnemyInScene();
        enemyInLevel = GetEnemyInScene();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= timeLimit)
            {
                Debug.Log("Time Limit Reached!");
                isTimerRunning = false;
            }
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            float remainingTime = Mathf.Max(0f, timeLimit - currentTime);
            string formattedTime = FormatTime(remainingTime);
            timerText.text = "Time: " + formattedTime;
        }
    }

    private string FormatTime(float seconds)
    {
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
            GameOver();
        }
        Debug.Log("Player died");
    }

    private void SpawnPlayer()
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(1, 0.5f, 1), Quaternion.identity);
        currentPlayer = player.GetComponent<PlayerController>();
        currentPlayer.InitializePlayer(maxBomb, moveSpeed);
        UpdateLivesText();
    }

    private void UpdateLivesText()
    {
        liveText.text = "Lives: " + lives.ToString("D1");
    }

    private void UpdateBombsText()
    {
        bombText.text = "Bombs: " + lives.ToString("D1");
    }

    private void UpdateRangeText()
    {
        rangeText.text = "Range: " + explodeRange.ToString("D1");
    }

    private void UpdateSpeedText()
    {
        speedText.text = "Speed: " + moveSpeed.ToString();
    }

    public int GetExplodeRange()
    {
        return explodeRange;
    }

    public void PauseButton()
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

        if (enemyInLevel <= 0)
        {
            winPanel.SetActive(true);
        }
    }

    public void IncreaseMaxBombs()
    {
        maxBomb++;
        maxBomb = Mathf.Clamp(maxBomb, 1, bombLimit);
        UpdateBombsText();
        
        currentPlayer.InitializePlayer(maxBomb, moveSpeed);
    }

    public void IncreaseSpeed()
    {
        if (speedCounter < speedLimit)
        {
            moveSpeed += speedIncrease;
            //moveSpeed = Mathf.Clamp(moveSpeed, 4f, speedLimit);
            speedCounter++;
            UpdateSpeedText();
            currentPlayer.InitializePlayer(maxBomb, moveSpeed);
        }
    }

    public void IncreaseExplodeRange()
    {
        explodeRange++; 
        explodeRange = Mathf.Clamp(explodeRange, 1, explodeLimit);
        UpdateRangeText();
    }
}
