using Gfen.Game.Config;
using Gfen.Game.Logic;
using Gfen.Game.Presentation;
using Gfen.Game.UI;
using UnityEngine;

namespace Gfen.Game {
    public class ReplayManager : MonoBehaviour{

        private GameManager m_gameManager;

        private LogicGameManager m_logicGameManager;

        private PresentationGameManager m_presentationGameManager;

        private string m_mapPath;

        private bool m_isActive = false;

        public void Init(GameManager gameManager, LogicGameManager logicGameManager, PresentationGameManager presentationGameManager){
            m_gameManager = gameManager;
            m_logicGameManager = logicGameManager;
            m_presentationGameManager = presentationGameManager;
        }

        public void SetPath(string path){
            m_mapPath = path;
        }

        public void SetActive(){
            m_isActive = true;
        }

    }
}