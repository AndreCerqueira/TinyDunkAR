using MoreMountains.Feedbacks;
using TMPro;
using UnityEngine;

namespace Project.Runtime
{
    public class ScoreManager : Singleton<ScoreManager>
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private MMF_Player _scoreFeedback;
        
        private int _currentScore;

        private void Start()
        {
            UpdateScoreText();
        }

        public void AddScore(int points = 1)
        {
            _currentScore += points;
            _scoreFeedback?.PlayFeedbacks();
            UpdateScoreText();
        }

        private void UpdateScoreText()
        {
            _scoreText.text = $"SCORE: {_currentScore}";
        }
    }
}