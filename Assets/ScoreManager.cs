using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int score = 0;
    private int highscore = 0;

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highscoreText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        LoadHighscore();
    }

    private void OnEnable()
    {
        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from sceneLoaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign scoreText after the scene reloads
        ResetUI();
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (score > highscore)
        {
            highscore = score;
            SaveHighscore();
            UpdateHighscore(highscore);
        }
    }

    public void AddToScore(int value)
    {
        score += value;
        UpdateScore(score);
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        UpdateScore(0);
    }

    public void ResetUI()
    {
        // Find the reloaded Text object in the scene
        if (scoreText == null)
        {
            scoreText = GameObject.Find("Text")?.GetComponent<TextMeshProUGUI>();
        }
        if(highscoreText == null)
        {
            highscoreText = GameObject.Find("highscoreText")?.GetComponent<TextMeshProUGUI>();
        }

        // Ensure the Text object is active and updated
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(true);
            UpdateScore(score); // Update the score on the UI
        }
        else
        {
            Debug.LogError("Text object not found in the scene!");
        }
        if (highscoreText != null)
        {
            highscoreText.gameObject.SetActive(true);
            UpdateHighscore(highscore); 
        }
        else
        {
            Debug.LogError("Text object not found in the scene!");
        }
    }

    private void LoadHighscore()
    {
        highscore = PlayerPrefs.GetInt("Highscore", 0);
    }

    private void SaveHighscore()
    {
        PlayerPrefs.SetInt("Highscore", highscore);
        PlayerPrefs.Save();
    }

    private void UpdateHighscore(int val)
    {
        if(highscoreText != null)
        {
            LoadHighscore();
            highscoreText.text = "Highscore: " + highscore;
        }
    }







}
