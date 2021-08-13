using System;
using System.Collections;
using RP.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RP
{
    public enum GameState
    {
        Idling, // Waiting for a game to start
        Paused, // Pause when a game is running
        Running
    }

    /// <summary>
    /// <para> Handles all game states and enable shot controller when needed. </para>
    /// <para> It dispatches game events and data related to the game state and calculates the score according the game rules. </para>
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameSettings GameData;
        public ShotController ShotController;

        #region Events

        [Header("Events")] public UnityEvent OnGameStartEvent = new UnityEvent();
        public BoolEvent OnGameOverEvent = new BoolEvent();
        public IntEvent OnScoreChangedEvent = new IntEvent();
        public UnityEvent OnLevelChangedEvent = new UnityEvent();

        #endregion

        #region Properties

        /// <summary>
        /// <para> Id of the current played level. It corresponds in the id of the level in the game data levels table. </para>
        /// </summary>
        public int LevelId { get; private set; } = -1;

        /// <summary>
        /// <para> Current game state. </para>
        /// </summary>
        public GameState GameState { get; private set; } = GameState.Idling;

        /// <summary>
        /// <para> The score of the current game. </para>
        /// </summary>
        public int Score { get; private set; }

        #endregion

        /// <summary>
        /// <para> Table of the tracked balls. Filled at start time. </para>
        /// </summary>
        private BallBehaviour[] _balls;

        private readonly WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        #region MonoBehaviour Functions

        private void Start()
        {
            // Expensive function called once
            _balls = FindObjectsOfType<BallBehaviour>();

            // Load the first level
            LoadLevel(GameData.StartLevel);
        }

        private void OnEnable()
        {
            PocketBehaviour.OnPocketedEvent.AddListener(OnBallPocketed);
            ShotController.OnShotTriggeredEvent.AddListener(OnShotTriggered);
        }

        private void OnDisable()
        {
            PocketBehaviour.OnPocketedEvent.RemoveListener(OnBallPocketed);
            ShotController.OnShotTriggeredEvent.RemoveListener(OnShotTriggered);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// <para> Starts a game. Ensures all is reset and start. </para>
        /// </summary>
        public void StartGame()
        {
            ResetScore();
            ResetBalls();
            SetGameStartState();
            OnGameStartEvent.Invoke();
        }

        /// <summary>
        /// <para> Pauses the game only when the game is currently running. </para>
        /// </summary>
        public void PauseIfRunning()
        {
            if (GameState == GameState.Running)
            {
                SetPauseState();
            }
        }

        /// <summary>
        /// <para> Resumes the game only after pause. </para>
        /// </summary>
        public void ResumeIfPaused()
        {
            if (GameState == GameState.Paused)
            {
                SetRunningState();
            }
        }

        /// <summary>
        /// <para> Changes the pool table level and starts a new game. </para>
        /// </summary>
        /// <param name="levelId"> The level id to load. </param>
        public void ChangeLevelAndStart(int levelId)
        {
            LoadLevel(levelId);
            StartGame();
        }

        public void QuitApplication()
        {
            Application.Quit(0);
        }

        #endregion

        #region Private Functions

        #region Game States

        private void SetGameStartState()
        {
            GameState = GameState.Running;
            ShotController.enabled = true;
        }

        private void SetRunningState()
        {
            GameState = GameState.Running;
            ShotController.enabled = true;
        }

        private void SetGameOverState()
        {
            GameState = GameState.Idling;
            ShotController.enabled = false;
            StopAllCoroutines();
        }

        private void SetPauseState()
        {
            GameState = GameState.Paused;
            ShotController.enabled = false;
            StopAllCoroutines();
        }

        #endregion

        /// <summary>
        /// <para> Callback when a new shot was triggered from a the shot controller. </para>
        /// </summary>
        private void OnShotTriggered()
        {
            ShotController.enabled = false;
            StartCoroutine(WaitForBallsStationary(SetRunningState));
        }

        /// <summary>
        /// <para> Handles pocketed ball. </para>
        /// <para> Callback from HoleBehaviour.OnPocketedEvent. </para>
        /// </summary>
        /// <param name="ball"> Ball which has been pocketed. </param>
        private void OnBallPocketed(BallBehaviour ball)
        {
            switch (ball.Color)
            {
                case BallColor.Red:
                    Score++;
                    ball.gameObject.SetActive(false);
                    break;

                default:
                    Score--;
                    ball.ResetAndFreezePosition();
                    break;
            }

            OnScoreChangedEvent.Invoke(Score);

            if (CheckScoreReached() || CheckNotEnoughRedBallsRemaining())
            {
                SetGameOverState();

                var hasWin = Score >= GameData.WinScore;
                OnGameOverEvent.Invoke(hasWin);
            }
        }

        /// <summary>
        /// <para> Loads the given level id in the table of levels of the game settings. </para>
        /// </summary>
        /// <param name="levelId"> The level id to load. </param>
        private void LoadLevel(int levelId)
        {
            if (levelId == LevelId || levelId < 0 || levelId >= GameData.Levels.Length)
                return;

            // Unload the current level if exist
            if (LevelId != -1)
                SceneManager.UnloadSceneAsync(GameData.Levels[LevelId].SceneName);

            if (!SceneManager.GetSceneByName(GameData.Levels[levelId].SceneName).isLoaded)
                SceneManager.LoadSceneAsync(GameData.Levels[levelId].SceneName, LoadSceneMode.Additive);

            LevelId = levelId;

            OnLevelChangedEvent.Invoke();
        }

        /// <summary>
        /// <para> Coroutines that waits for balls stop moving on the pool table. </para>
        /// <remarks> This function is expensive. </remarks>
        /// </summary>
        /// <param name="callback"> Callback when all balls are ready. aka stopped moving. </param>
        /// <returns> Yield Coroutine. </returns>
        private IEnumerator WaitForBallsStationary(Action callback)
        {
            // Waits for the next physics fixed cycle.
            // Avoids detection of all balls stationary just after a shot. ( because yield null is after update )
            // https://docs.unity3d.com/Manual/ExecutionOrder.html
            yield return _waitForFixedUpdate;

            var allBallsReady = false;

            while (!allBallsReady)
            {
                yield return null;

                allBallsReady = true;

                foreach (var ball in _balls)
                {
                    allBallsReady = allBallsReady && ball.Stationary;
                }
            }

            callback?.Invoke();
        }

        /// <summary>
        /// <para> Checks if the game is over by score. </para>
        /// </summary>
        /// <returns> True if the limit score min or max was reached. </returns>
        private bool CheckScoreReached()
        {
            return Score <= GameData.LoseScore || Score >= GameData.WinScore;
        }

        /// <summary>
        /// <para> Check if enough red balls remaining on the pool table in order to reach the score to win. </para>
        /// </summary>
        /// <returns> True the winning score is not reachable. </returns>
        private bool CheckNotEnoughRedBallsRemaining()
        {
            var countRedRemaining = 0;

            foreach (var ball in _balls)
            {
                if (ball.Color == BallColor.Red && ball.gameObject.activeSelf)
                    countRedRemaining++;
            }

            return (countRedRemaining + Score) < GameData.WinScore;
        }

        /// <summary>
        /// <para> Resets the score and notify score has changed for observers. </para>
        /// </summary>
        private void ResetScore()
        {
            Score = GameData.StartingScore;
            OnScoreChangedEvent.Invoke(Score);
        }

        /// <summary>
        /// <para> Reset balls at start positions and visible state. </para>
        /// </summary>
        private void ResetBalls()
        {
            foreach (var ball in _balls)
            {
                ball.gameObject.SetActive(true);
                ball.ResetAndFreezePosition();
            }
        }

        #endregion
    }
}