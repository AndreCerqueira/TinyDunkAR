using UnityEngine;

namespace Project.Runtime
{
    [RequireComponent(typeof(Collider))]
    public class HoopTrigger : MonoBehaviour
    {
        private ScoreManager _scoreManager;
        
        private void Awake()
        {
            _scoreManager = ScoreManager.Instance;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<BallLauncher>(out var ball))
            {
                _scoreManager.AddScore();
            }
        }
    }
}