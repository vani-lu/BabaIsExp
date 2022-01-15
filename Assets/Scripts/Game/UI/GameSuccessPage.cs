using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

namespace Gfen.Game.UI
{
    public class GameSuccessPage : UIPage
    {
        public Button okButton;

        private void Awake() 
        {
            okButton.onClick.AddListener(OnOkButtonClicked);
        }

        private void Update(){
            if (CrossPlatformInputManager.GetButton("Submit")){
                OnOkButtonClicked();
            }
        }
        
        private void OnOkButtonClicked()
        {
            m_gameManager.uiManager.HidePage();
            
            m_gameManager.StopGame();
        }
    }
}
