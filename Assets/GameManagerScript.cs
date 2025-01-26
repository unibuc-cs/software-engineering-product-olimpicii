using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject pauseMenuPanel;  
    public Text soldiersLeftText;      
    public PlayerController playerController;  

    private bool isPaused = false;

    void Start()
    {
        
        pauseMenuPanel.SetActive(false);
    }

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void EndGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;  
    }


    public void RestartGame()
    {
        
        Time.timeScale = 1f;
        playerController.ResetGame();
        Debug.Log("Restarting the game... Time Scale: " + Time.timeScale);
        pauseMenuPanel.SetActive(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  
      
    }

    
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
