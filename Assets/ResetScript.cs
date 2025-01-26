using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        print("Scene restarted");
        Time.timeScale = 1f;

        // Subscribe to sceneLoaded event to reset UI after scene loads
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reinitialize the UI
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetUI(); // Reassign the scoreText reference
        }

        // Unsubscribe after handling the event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



}
