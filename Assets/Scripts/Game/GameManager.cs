using Gfen.Game.Config;
using Gfen.Game.Logic;
using Gfen.Game.Manager;
using Gfen.Game.Presentation;
using Gfen.Game.UI;
using Vani.Data;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections.Generic;

namespace Gfen.Game {
    public class GameManager : MonoBehaviour 
    {
        public GameConfig gameConfig;

        public Camera gameCamera;

        public UIManager uiManager;

        private LevelManager m_levelManager;

        public LevelManager LevelManager { get { return m_levelManager; } }

        private LogicGameManager m_logicGameManager;

        private PresentationGameManager m_presentationGameManager;

        private bool m_isInGame;

        private bool m_isPause;

        private int m_currentChapterIndex;

        private int m_currentLevelIndex;

        private float m_lastInputTime;

        // Record frame data in a list
        private List<FrameData> m_data = new List<FrameData>();

        private void Start() 
        {
            gameConfig.Init();

            // Initialize level and UI managers with the current Game Manager 
            m_levelManager = new LevelManager();
            m_levelManager.Init(this); 

            uiManager.Init(this);

            m_logicGameManager = new LogicGameManager(this);
            m_presentationGameManager = new PresentationGameManager(this, m_logicGameManager);

            // Initialize indicator variables 
            m_isInGame = false;
            m_isPause = false;

            var stayChapterIndex = m_levelManager.GetStayChapterIndex();

            // Show Chapter Selection Page 
            uiManager.ShowPage<ChapterPage>();
            if (stayChapterIndex >= 0)
            {
                var levelPage = uiManager.ShowPage<LevelPage>();
                levelPage.SetContent(stayChapterIndex);
            }
        }

        private void Update() 
        {
            if (m_isInGame)
            {
                HandleInput(out GameControlType gameControlInput,
                            out OperationType operationInput,
                            out int numCommandsOutput);
                FrameData newFrameData = new FrameData(Time.unscaledTime,
                                                       m_currentChapterIndex,
                                                       m_currentLevelIndex,
                                                       (int)gameControlInput,
                                                       (int)operationInput,
                                                       numCommandsOutput);
                // DebugLog
                if (gameControlInput != GameControlType.None || operationInput != OperationType.None){
                    Debug.Log("Chapter " + newFrameData.chapter + ", Level " + newFrameData.level);
                    Debug.Log("Game Control " + gameControlInput + ", Operation " + newFrameData.operation);
                }
                
            }
        }

        private void HandleInput(out GameControlType gameControlType, out OperationType operationType, out int numOfCommands)
        {
            /* Handle cross-platform inputs:
            r   -   Restart
            esc -   Pause
            z   -   Undo
            y   -   Redo
            Key Bindings can be modified in Project settings */

            // Default output
            gameControlType = GameControlType.None;
            operationType = OperationType.None;
            numOfCommands = -1;

            var restart = CrossPlatformInputManager.GetButton("Restart");
            if (restart)
            {
                m_lastInputTime = 0f;
                RestartGame();
                gameControlType = GameControlType.Restart;
                return;
            }

            // Wait for key press time
            var isTimeToInputDelay = (Time.unscaledTime - m_lastInputTime) >= gameConfig.inputRepeatDelay;

            if (!isTimeToInputDelay) {
                return;
            }

            var pause = CrossPlatformInputManager.GetButton("Pause");
            if (pause)
            {
                m_lastInputTime = Time.unscaledTime;
                PauseGame();
                gameControlType = GameControlType.Pause;
                return;
            }

            if (m_isPause)
            {
                return;
            }

            var undo = CrossPlatformInputManager.GetButton("Undo");
            var redo = CrossPlatformInputManager.GetButton("Redo");
            if (undo)
            {
                m_lastInputTime = Time.unscaledTime;
                numOfCommands = m_logicGameManager.Undo();
                m_presentationGameManager.RefreshPresentation();
                gameControlType = GameControlType.Undo;
            }
            else if (redo)
            {
                m_lastInputTime = Time.unscaledTime;
                numOfCommands = m_logicGameManager.Redo();
                m_presentationGameManager.RefreshPresentation();
                gameControlType = GameControlType.Redo;
            }
            else
            {
                // Handle movements
                operationType = GetLogicOperation();

                if (operationType != OperationType.None)
                {
                    // Record Movement Input
                    m_lastInputTime = Time.unscaledTime;
                    numOfCommands = m_logicGameManager.Tick(operationType);
                    m_presentationGameManager.RefreshPresentation();
                }
            }
        }

        private OperationType GetLogicOperation() 
        {
            var horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            var vertical = CrossPlatformInputManager.GetAxis("Vertical");
            var wait = CrossPlatformInputManager.GetButton("Wait");

            var operationType = OperationType.None;
            if (vertical > 0.1f && vertical >= Mathf.Abs(horizontal))
            {
                operationType = OperationType.Up;
            }
            else if (vertical < -0.1f && vertical <= -Mathf.Abs(horizontal))
            {
                operationType = OperationType.Down;
            }
            else if (horizontal < -0.1f && horizontal <= -Mathf.Abs(vertical))
            {
                operationType = OperationType.Left;
            }
            else if (horizontal > 0.1f && horizontal >= Mathf.Abs(vertical))
            {
                operationType = OperationType.Right;
            }
            else if (wait)
            {
                operationType = OperationType.Wait;
            }

            return operationType;
        }

        public void StartGame(int chapterIndex, int levelIndex)
        {
            uiManager.HideAllPages();

            m_logicGameManager.StartGame(gameConfig.chapterConfigs[chapterIndex].levelConfigs[levelIndex].map);
            m_presentationGameManager.StartPresent();
            m_isInGame = true;
            m_isPause = false;
            m_currentChapterIndex = chapterIndex;
            m_currentLevelIndex = levelIndex;

            m_logicGameManager.GameEnd += OnGameEnd;

            uiManager.ShowPage<GamePlayPage>();
        }

        public void StopGame()
        {
            m_logicGameManager.GameEnd -= OnGameEnd;

            m_presentationGameManager.StopPresent();
            m_logicGameManager.StopGame();
            m_isInGame = false;
            m_isPause = false;

            uiManager.HideAllPages();

            uiManager.ShowPage<ChapterPage>();
            var levelPage = uiManager.ShowPage<LevelPage>();
            levelPage.SetContent(m_currentChapterIndex);
        }

        public void RestartGame()
        {
            uiManager.HideAllPages();

            m_logicGameManager.RestartGame();
            m_presentationGameManager.RefreshPresentation();
            m_isPause = false;

            uiManager.ShowPage<GamePlayPage>();
        }

        public void PauseGame()
        {
            m_isPause = true;
            uiManager.ShowPage<InGameSettingsPage>();
        }

        public void ResumeGame()
        {
            m_isPause = false;
            uiManager.HidePage();
        }

        private void OnGameEnd(bool success)
        {
            if (success)
            {
                m_isInGame = false;
                m_levelManager.PassLevel(m_currentChapterIndex, m_currentLevelIndex);

                uiManager.ShowPage<GameSuccessPage>();
            }
        }
    }
}
