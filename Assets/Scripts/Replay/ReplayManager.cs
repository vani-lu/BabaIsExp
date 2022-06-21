using System;
using System.IO;
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

        private float m_lastInputTime;

        private StreamReader m_streamReader;

        private int m_count;
        private int m_chapter;
        private int m_level;

        public void Init(GameManager gameManager, LogicGameManager logicGameManager, PresentationGameManager presentationGameManager){
            m_gameManager = gameManager;
            m_logicGameManager = logicGameManager;
            m_presentationGameManager = presentationGameManager;
        }

        void Start(){
            if (Application.isEditor)
            {
                print("We are running this from inside of the editor!");
                string user = PlayerPrefs.GetString(UserInfoKey, "");
                string date = PlayerPrefs.GetString(DateInfoKey, "");
                string path = "Imports/data_" + date + "_" + user + ".csv";
                if (File.Exists(path))
                {
                    Debug.Log(path);
                    Debug.Log("Importing replay data found!");
                    m_dataPath = path;
                    m_isActive = true;
                    m_streamReader = new StreamReader(m_dataPath);
                    m_streamReader.ReadLine(); // The first line is header
                    m_lastInputTime = Time.unscaledTime;
                }
                else{
                    Debug.Log("New user! " + date + "_" + user);
                }
            }
        }

        void Update(){
            if(m_isActive){
                float currentFrameTime = Time.unscaledTime;
                if (currentFrameTime - m_lastInputTime > 0.15f)
                {
                    string line = m_streamReader.ReadLine();
                    if (line != null)
                    {
                        string[] data = line.Split(',');
                        m_count   = int.Parse(data[0]);
                        m_chapter = int.Parse(data[2]);
                        m_level   = int.Parse(data[3]);
                        GameControlType controlType = (GameControlType)Enum.Parse(typeof(GameControlType), data[4]);
                        OperationType operationType = (OperationType)Enum.Parse(typeof(OperationType), data[5]);
                        if (!HandleControlType(controlType)) {
                            HandleOperationType(operationType);
                            Debug.Log(string.Format("{0}, {1}, {2}, {3}, {4}", m_count, m_chapter, m_level, controlType, operationType));
                            m_logicGameManager.BlockListMap2BlockList(m_count);
                        }
                    }
                    else {
                        m_streamReader.Close();
                        m_isActive = false;
                    }
                    m_lastInputTime = currentFrameTime;
                }
            }
        }

        private bool HandleControlType(GameControlType controlType){
            // Return: true if pure control, false if map potentially changed
            switch(controlType){
                case GameControlType.Start:
                    m_gameManager.StartGame(m_chapter, m_level);
                    return false;
                case GameControlType.Restart:
                    m_gameManager.RestartGame();
                    return false;
                case GameControlType.Resume:
                    m_gameManager.ResumeGame();
                    break;
                case GameControlType.Pause:
                    m_gameManager.PauseGame();
                    break;
                case GameControlType.Undo:
                    m_logicGameManager.Undo();
                    m_presentationGameManager.RefreshPresentation();
                    return false;
                case GameControlType.Redo:
                    m_logicGameManager.Redo();
                    m_presentationGameManager.RefreshPresentation();
                    return false;
                case GameControlType.Success:
                case GameControlType.Stop:
                    m_gameManager.StopGame();
                    break;
                case GameControlType.Defeat:
                    m_gameManager.uiManager.HidePage();
                    break;
                case GameControlType.Logout:
                case GameControlType.Login:
                case GameControlType.ToggleHint:
                case GameControlType.EnableHint:
                    break;
                default:
                    return false;
            }
            return true;
        }

        private void HandleOperationType(OperationType operationType){
            if (operationType == OperationType.None)
            {
                return;
            }
            m_logicGameManager.Tick(operationType);
            m_presentationGameManager.RefreshPresentation();
        }

    }
}