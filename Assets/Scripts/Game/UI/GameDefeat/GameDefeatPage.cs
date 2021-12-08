using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GameDefeatPage : UIPage
    {
        public Button closeButton;

        public Button restartGameButton;

        private void Awake() 
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
            restartGameButton.onClick.AddListener(OnRestartGameButtonClicked);
        }
        
        private void OnCloseButtonClicked()
        {
            m_gameManager.ResumeGame();
        }

        private void OnRestartGameButtonClicked()
        {
            m_gameManager.RestartGame();
        }
    }
}
