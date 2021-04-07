using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text highScoreText;
        [SerializeField] private Text gameOverMenuScoreText;

        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private GameObject paletteMenuPanel;
        [SerializeField] private GameObject gameOverMenuPanel;

        public void SetHighScoreUI(int score) => highScoreText.text = score.ToString();

        public void SetGameOverMenuHighScoreUI(int score) => gameOverMenuScoreText.text = "Best " + score;

        public void SetScoreUI(int score) => scoreText.text = score.ToString();

        public void OpenMainMenu() => menuPanel.SetActive(true);

        public void CloseMainMenu() => menuPanel.SetActive(false);

        public void OpenPauseMenu() => pauseMenuPanel.SetActive(true);

        public void ClosePauseMenu() => pauseMenuPanel.SetActive(false);

        public void OpenPaletteMenu() => paletteMenuPanel.SetActive(true);

        public void ClosePaletteMenu() => paletteMenuPanel.SetActive(false);

        public void OpenGameOverMenu() => gameOverMenuPanel.SetActive(true);

        public void CloseGameOverMenu() => gameOverMenuPanel.SetActive(false);
    }
}