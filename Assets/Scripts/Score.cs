using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public static Score Instance { get; private set; }
    
    [SerializeField]
    private int _pointsPerEnemy = 10;

    [SerializeField]
    private TextMeshProUGUI _scoreDisplay;
    
    private int _totalScore = 0;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // If the display wasn't assigned in the Inspector, warn the developer.
        if (_scoreDisplay == null)
        {
            Debug.LogWarning("Score._scoreDisplay is not assigned. Assign a TextMeshProUGUI in the Inspector to show the score.");
        }
        UpdateScoreDisplay();
    }

    void Update()
    {
        
    }

    public void AddScore(int points = 0)
    {
        if (points == 0)
        {
            _totalScore += _pointsPerEnemy;
        }
        else
        {
            _totalScore += points;
        }
        UpdateScoreDisplay();
    }


    private void UpdateScoreDisplay()
    {
        if (_scoreDisplay != null)
        {
            _scoreDisplay.text = "Score: " + _totalScore;
        }
    }


    public int GetScore()
    {
        return _totalScore;
    }


    public void ResetScore()
    {
        _totalScore = 0;
        UpdateScoreDisplay();
    }
}
