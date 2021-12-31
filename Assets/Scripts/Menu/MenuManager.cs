using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Vani.UI
{
    public class MenuManager : MonoBehaviour
    {
        public Button playButton;

        public InputField nameInputField;

        private string m_userName;

        private string m_loginDate;

        private int conditionIndex;

        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

        private const string ConditionInforKey = "Condition";

        // Start is called before the first frame update
        void Start()
        {
            // System.Random rnd = new System.Random();
            // conditionIndex = rnd.Next(1,5);
            conditionIndex = 1;
            m_loginDate = DateTime.Now.ToString("yyyyMMdd");
        }

        // Get user (participant's) name from input
        public void PlayGame()
        {
            m_userName = nameInputField.text;

            if (!string.IsNullOrEmpty(m_userName)){
                SaveUserInfo();
                SceneManager.LoadScene(conditionIndex);
            }
        }

        private void SaveUserInfo()
        {
            PlayerPrefs.SetString(UserInfoKey, m_userName);
            PlayerPrefs.SetString(DateInfoKey, m_loginDate);
        }
    }
}
