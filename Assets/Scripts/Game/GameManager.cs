using System.IO;
using Gfen.Game.Config;
using Gfen.Game.Logic;
using Gfen.Game.Presentation;
using Gfen.Game.UI;
using Vani.Data;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

namespace Gfen.Game {
    public class GameManager : MonoBehaviour 
    {
        public GameConfig gameConfig;

        public Camera gameCamera;

        public UIManager uiManager;

        public ReplayManager replayManager;

        public int bonusChapterIndex;

        private LevelManager m_levelManager;
        public LevelManager LevelManager { get { return m_levelManager; } }

        private SolutionDataManager m_solutionDataManager;
        public SolutionDataManager SolutionDataManager { get { return m_solutionDataManager; } }

        private LogicGameManager m_logicGameManager;

        private PresentationGameManager m_presentationGameManager;

        private bool m_isInGame;
        private bool m_isPreviouslyInGame;

        private bool m_isPause;
        private bool m_isPreviouslyInPause;

        private bool m_isRestart;
        private bool m_isResume;
        private bool m_isResumeWithUndo;

        private bool m_isSuccess;
        private bool m_isDefeat;

        private int m_currentChapterIndex;
        public int CurrentChapterIndex {get { return m_currentChapterIndex;}}
        private int m_currentLevelIndex;
        public int CurrentLevelIndex {get { return m_currentLevelIndex;}}
        private float m_lastInputTime;
        private float m_currentLevelElapsedTime;
        public float CurrentLevelElapsedTime {get {return m_currentLevelElapsedTime;}}

        // Record frame data in a list
        private string m_user;
        public string User { get { return m_user; } }
        private string m_date;
        public string Date { get { return m_date; } }

        private string m_dataPath;
        public string DataPath { get { return m_dataPath; } }
        private string m_dataFile;
        public string DataFile { get { return m_dataFile; } }

        private const string PathInfoKey = "DataPath";
        private const string UserInfoKey = "UserName";
        private const string DateInfoKey = "LoginDate";

        private async void Start() 
        {
            gameConfig.Init();

            // Set data path
            m_user = PlayerPrefs.GetString(UserInfoKey, "test");
            m_date = PlayerPrefs.GetString(DateInfoKey, "00000000");
            
            if (!Directory.Exists("./Save")){
                 Directory.CreateDirectory("./Save");
            }
            m_dataPath = PlayerPrefs.GetString(PathInfoKey, "./Save");
            PlayerPrefs.SetString("Path", m_dataPath);
            m_dataFile = "/data_" + m_date + "_" + m_user + ".csv";

            // Create data path and initialize the file
            var createTask = FrameDataUtility.InitFrameData(m_dataPath + m_dataFile);

            // Initialize level and UI managers with the current Game Manager 
            m_levelManager = new LevelManager();
            m_levelManager.Init(this); 

            uiManager.Init(this);

            m_logicGameManager = new LogicGameManager(this);

            m_presentationGameManager = new PresentationGameManager(this, m_logicGameManager);

            replayManager.Init(this, m_logicGameManager, m_presentationGameManager);

            m_solutionDataManager = new SolutionDataManager();
            m_solutionDataManager.Init(this, m_logicGameManager);

            // Initialize indicator variables 
            m_isInGame = false;
            m_isPause = false;
            m_isPreviouslyInGame = false;
            m_isPreviouslyInPause = false;
            m_isRestart = false;
            m_isResume = false;
            m_isSuccess = false;
            m_isDefeat = false;

            var stayChapterIndex = m_levelManager.GetStayChapterIndex();

            // Show Chapter Selection Page or Level Selection Page if the user has record
            uiManager.ShowPage<ChapterPage>();
            if (stayChapterIndex >= 0)
            {
                var levelPage = uiManager.ShowPage<LevelPage>();
                levelPage.SetContent(stayChapterIndex);
            }

            // Complete data file initialzation
            await createTask;
            var loginTask = FrameDataUtility.MarkLogin(m_dataPath + m_dataFile);
            await loginTask;
        }

        private async void Update() 
        {
            float frameTimeStamp = Time.unscaledTime;

            // Initialize frame data
            GameControlType gameControlInput = GameControlType.None;
            OperationType operationInput = OperationType.None;
            int numCommandsOutput = -1;

            // Listen to inputs when in gameplay
            if (m_isInGame)
            {
                // Detect switch in game state: New level starts
                if (!m_isPreviouslyInGame) {
                    gameControlInput = GameControlType.Start;
                    m_isPreviouslyInGame = true;
                }
                else
                {
                    m_currentLevelElapsedTime += Time.deltaTime; // increment level timer
                    // Wait for key press sticky time
                    var isWithinInputDelay = (frameTimeStamp - m_lastInputTime) < gameConfig.inputRepeatDelay;

                    if (!isWithinInputDelay) {
                        HandleInput(ref gameControlInput, ref operationInput, ref numCommandsOutput); 
                    }
                }
            }
            else {
                if (m_isSuccess) {
                    gameControlInput = GameControlType.Success;
                    m_isSuccess = false;
                    m_isPreviouslyInGame = false;
                }
                else if (m_isPreviouslyInGame) {
                    gameControlInput = GameControlType.Stop;
                    m_isPreviouslyInGame = false;
                }
            }
            // DebugLog
            if (gameControlInput != GameControlType.None || operationInput != OperationType.None)
            {
                FrameData fData = new FrameData(frameTimeStamp,
                                                m_currentChapterIndex,
                                                m_currentLevelIndex,
                                                gameControlInput,
                                                operationInput,
                                                numCommandsOutput);
                var writeTask =  FrameDataUtility.AppendOneFrameAsync(m_dataPath + m_dataFile, fData);
                Debug.Log(string.Format("{0}, {1}", gameControlInput, operationInput));
                await writeTask;
                if (!Application.isEditor){
                    m_logicGameManager.BlockListMap2BlockList();
                }
            }
        }

        private void HandleInput(ref GameControlType gameControlType, ref OperationType operationType, ref int numOfCommands)
        {
            /* Handle cross-platform inputs:
            Disabled: r   -   Restart
            Disabled: esc -   Pause
            z   -   Undo
            y   -   Redo
            Key Bindings can be modified in Project settings */

            // // Keypress Restart
            // var restart = CrossPlatformInputManager.GetButton("Restart");
            // if (restart)
            // {
            //     m_lastInputTime = Time.unscaledTime;
            //     RestartGame();
            //     gameControlType = GameControlType.Restart;
            //     m_isRestart = false;
            //     return;
            // }

            // Do not listen to inputs when in pause
            if (m_isPause)
            {
                // Detect Pause Menu Onset
                if (!m_isPreviouslyInPause) {
                    m_lastInputTime = Time.unscaledTime;
                    if ( m_isDefeat ) {
                        gameControlType = GameControlType.Defeat;
                        m_isDefeat = false;
                    }
                    else {
                        gameControlType = GameControlType.Pause;
                    }
                    m_isPreviouslyInPause = true;
                }
                return;
            }
            else {
                if (m_isPreviouslyInPause) {
                    if (m_isRestart) { // Detect Restart from Pause UI
                        m_lastInputTime = Time.unscaledTime;
                        gameControlType = GameControlType.Restart;
                        m_isRestart = false;
                    }
                    else if (m_isResume) { // Detect Resume from Pause UI
                        m_lastInputTime = Time.unscaledTime;
                        gameControlType = GameControlType.Resume;
                        m_isResume = false;
                    }
                    else if (m_isResumeWithUndo) { // Detect Resume with Undo from UI, after Defeat
                        m_lastInputTime = Time.unscaledTime;
                        gameControlType = GameControlType.Undo;
                        m_isResumeWithUndo = false;
                    }
                    m_isPreviouslyInPause = false;
                    return;
                }
            }   

            // // Keypress Pause
            // var pause = CrossPlatformInputManager.GetButton("Pause");
            // if (pause)
            // {
            //     m_lastInputTime = Time.unscaledTime;
            //     gameControlType = GameControlType.Pause;
            //     PauseGame();
            //     UpdateGameStatus();
            //     return;
            // }

            // Keypress Undo or Redo
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

        private void UpdateGameStatus()
        {
            m_isPreviouslyInPause = m_isPause;
            m_isPreviouslyInGame = m_isInGame;
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
                // Disable Wait
                // operationType = OperationType.Wait;
            }

            return operationType;
        }

        public bool IsInBonusChapter(){
            return m_currentChapterIndex == bonusChapterIndex;
        }
        
        public void StartGame(int chapterIndex, int levelIndex)
        {
            m_currentChapterIndex = chapterIndex;
            m_currentLevelIndex = levelIndex;
            
            m_currentLevelElapsedTime = m_levelManager.GetTimeSpent(chapterIndex, levelIndex);

            uiManager.HideAllPages();

            m_logicGameManager.StartGame(gameConfig.chapterConfigs[chapterIndex].levelConfigs[levelIndex].map);
            m_presentationGameManager.StartPresent();
            UpdateGameStatus();
            m_isInGame = true;
            m_isPause = false;

            m_logicGameManager.GameEnd += OnGameEnd;

            // Show in game UI
            uiManager.ShowPage<GamePlayPage>();
        }

        public void StopGame()
        {
            m_logicGameManager.GameEnd -= OnGameEnd;

            m_levelManager.SetTimeSpent();
            m_presentationGameManager.StopPresent();
            m_logicGameManager.StopGame();

            UpdateGameStatus();
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

            m_isPreviouslyInPause = m_isPause;
            m_isPause = false;
            
            m_isRestart = true;

            uiManager.ShowPage<GamePlayPage>();
        }

        public void PauseGame()
        {
            m_isPreviouslyInPause = m_isPause;
            m_isPause = true;

            uiManager.ShowPage<InGameSettingsPage>();
        }

        public void ResumeGame()
        {
            m_isPreviouslyInPause = m_isPause;
            m_isPause = false;
            m_isResume = true;

            uiManager.HidePage();
        }

        public void ResumeGameWithUndo()
        {
            m_logicGameManager.Undo();
            m_presentationGameManager.RefreshPresentation();

            m_isPreviouslyInPause = m_isPause;
            m_isPause = false;

            m_isResumeWithUndo = true;
            
            uiManager.HidePage();
        }

        public async void ExitApp()
        {
            var logoutTask = FrameDataUtility.MarkLogout(m_dataPath + m_dataFile);

            int bonus = m_levelManager.CountBonus();
            var ts = Time.unscaledTime;

            m_levelManager.SetTimeSpent();

            PlayerPrefs.SetInt("ExpTime", Mathf.CeilToInt(ts/60f));
            PlayerPrefs.SetInt("Bonus", bonus);

            await logoutTask;
            SceneManager.LoadScene(5);
        }

        private void OnGameEnd(bool success)
        {
            if (success)
            {
                m_isInGame = false;
                m_isSuccess = true;
                m_levelManager.PassLevel(m_currentChapterIndex, m_currentLevelIndex);

                uiManager.ShowPage<GameSuccessPage>();
            }
            else {

                m_isPause = true;
                m_isDefeat = true;

                uiManager.ShowPage<GameDefeatPage>();
            }
        }
    }
}
