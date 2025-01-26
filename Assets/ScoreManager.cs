using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    
    public static ScoreManager Instance { get; private set; }

    private int score = 0; 

    [Header("UI Settings")]
    public TextMeshProUGUI scoreText; 

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this GameObject alive across scenes
        }
    }

    
    public void UpdateScore(int newScore)
    {
        score = newScore; 

        
        if (scoreText != null)
        {
            if (score == 1)
            {
                scoreText.text = score + " soldier left!";
            }
            else if (score < 1)
                scoreText.text = "You lost! Try again";
            else
            {
                scoreText.text = score + " soldiers left!";
            }
        }

        
    }

    
    public int GetScore()
    {
        return score;
    }

    
    public void ResetScore()
    {
        UpdateScore(0);
    }
}
