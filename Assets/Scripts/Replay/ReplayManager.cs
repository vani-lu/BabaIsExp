using System.Collections;
using Gfen.Game.Logic;
using Gfen.Game.Presentation;
using UnityEngine;

namespace Gfen.Game {
    public class ReplayManager : MonoBehaviour{

        private const string UserInfoKey = "UserName";

        private const string DateInfoKey = "LoginDate";

        private GameManager m_gameManager;

        private LogicGameManager m_logicGameManager;

        private PresentationGameManager m_presentationGameManager;

        private string m_dataPath;

        private bool m_isActive = false;

        public void Init(GameManager gameManager, LogicGameManager logicGameManager, PresentationGameManager presentationGameManager){
            m_gameManager = gameManager;
            m_logicGameManager = logicGameManager;
            m_presentationGameManager = presentationGameManager;
        }

        void Start(){
            if (Application.isEditor)
            {
                print("We are running this from inside of the editor!");
                SetActive();
                string user = PlayerPrefs.GetString(UserInfoKey, "");
                string date = PlayerPrefs.GetString(DateInfoKey, "");
                SetDataPath("Imports/data_" + date + "_" + user + ".csv");
            }
        }

        public void SetDataPath(string path){
            m_dataPath = path;
            Debug.Log(path);
        }

        public void SetActive(){
            m_isActive = true;
        }

        void Update(){
            if(m_isActive){
                StartCoroutine(WaitFixedFrame());
                Debug.Log("Get Data");
            }
        }

        IEnumerator WaitFixedFrame()
        {
            yield return new WaitForSecondsRealtime(2f);

        }
    }
}