using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;  
    public GameObject pauseMenuPanel;  
    public Text soldiersLeftText;      // Text showing soldiers left
    public PlayerController playerController;  // Reference to PlayerController script

    private bool isPaused = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
        // Escape key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // This method is called from the PlayerController when soldiers are 0
    public void EndGame()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;  // Pause the game
    }

    // Function to restart the game (reload the scene)
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload current scene
        Time.timeScale = 1f;  // Unpause the game
    }

    // Function to exit the game
    public void ExitGame()
    {
        Application.Quit(); // Exit the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;  // Stop play mode if in Unity Editor
#endif
    }

    // Toggle pause menu (called when Escape is pressed)
    void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;  // Pause the game
        }
        else
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;  // Unpause the game
        }
    }
}
