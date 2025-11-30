using TMPro;
using UnityEngine;

namespace Project.Runtime
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        private int _currentScore;

        private void Start()
        {
            UpdateScoreText();
        }

        public void AddScore(int points = 1)
        {
            _currentScore += points;
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            _scoreText.text = $"SCORE: {_currentScore}";
        }
    }
}