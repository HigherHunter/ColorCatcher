using UnityEngine;

namespace Managers
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;
        private int _highScore;

        public int GetHighScore() => _highScore;

        public void SetHighScore(int highScore) => _highScore = highScore;

        public bool IsNewHighScore()
        {
            if (_score <= _highScore) return false;

            _highScore = _score;
            return true;
        }

        public int GetScore() => _score;

        public void SetScore(int score) => _score = score;

        public void AddScore(int score) => _score += score;
    }
}