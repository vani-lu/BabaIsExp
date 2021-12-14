using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GameDefeatPage : UIPage
    {
        public Button restartGameButton;

        public Button resumeWithUndoButton;

        private void Awake() 
        {
            resumeWithUndoButton.onClick.AddListener(OnCloseButtonClicked);
            restartGameButton.onClick.AddListener(OnRestartGameButtonClicked);
        }
        
        private void OnCloseButtonClicked()
        {
            m_gameManager.ResumeGameWithUndo();
        }

        private void OnRestartGameButtonClicked()
        {
            m_gameManager.RestartGame();
        }
    }
}
