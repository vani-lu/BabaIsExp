using UnityEngine.UI;

namespace Gfen.Game.UI
{
    public class GameDefeatPage : UIPage
    {
        public Button restartGameButton;

        public Button resumeWithUndoButton;

        private void Awake() 
        {
            restartGameButton.onClick.AddListener(OnRestartGameButtonClicked);
            resumeWithUndoButton.onClick.AddListener(OnResumeButtonClicked);
        }
        
        private void OnRestartGameButtonClicked()
        {
            m_gameManager.RestartGame();
        }
        
        private void OnResumeButtonClicked()
        {
            m_gameManager.ResumeGameWithUndo();
        }

    }
}
