using UnityEngine;
using UnityEngine.SceneManagement;

namespace RP
{
    public class TrajectoryPrediction : MonoBehaviour
    {
        private const string BALL_TAG = "Ball";

        public GameManager GameManager;
        public LineRenderer TrajectoryRenderer;

        [SerializeField] private GameObject physicsBallPrefab;
        [SerializeField] private int steps = 30;

        private Scene _simScene;
        private PhysicsScene2D _simPhysicsScene2D;

        private GameObject _simPhysicsTable;
        private GameObject[] _balls;
        private GameObject[] _simBalls;
        private Rigidbody2D _simWhiteBallRigidBody;

        private Vector3 _lastSimulatedForce;
        private Vector3[] _points;

        #region MonoBehaviour Functions

        private void Start()
        {
            TrajectoryRenderer.positionCount = steps;
            _points = new Vector3[steps];
        }

        private void OnEnable()
        {
            GameManager.OnLevelChangedEvent.AddListener(OnLevelChanged);
            GameManager.ShotController.OnShotTriggeredEvent.AddListener(OnShotTriggered);

            CreateSimulationScene();
            PrepareBallsSimulation();
            PrepareTableSimulation();
        }

        private void OnDisable()
        {
            GameManager.OnLevelChangedEvent.RemoveListener(OnLevelChanged);
            GameManager.ShotController.OnShotTriggeredEvent.RemoveListener(OnShotTriggered);

            TrajectoryRenderer.enabled = false;
            SceneManager.UnloadSceneAsync(_simScene);
        }

        private void FixedUpdate()
        {
            if (GameManager.ShotController.enabled)
            {
                TrajectoryRenderer.enabled = true;

                var force = GameManager.ShotController.GetShotForce();
                var rawForce = GameManager.ShotController.GetRawForce();

                if (_lastSimulatedForce != force)
                {
                    SimulateWhiteBallTrajectory(force);
                    UpdateTrajectoryRenderer(rawForce.magnitude);
                    _lastSimulatedForce = force;
                }
            }
            else
            {
                TrajectoryRenderer.enabled = false;
            }
        }

        #endregion

        #region Private Functions

        private void OnShotTriggered()
        {
            _lastSimulatedForce = Vector3.zero;
        }

        private void OnLevelChanged()
        {
            if (_simPhysicsTable != null)
                DestroyImmediate(_simPhysicsTable);

            PrepareTableSimulation();
        }

        private void CreateSimulationScene()
        {
            _simScene = SceneManager.CreateScene("PhysicsScene",
                new CreateSceneParameters(LocalPhysicsMode.Physics2D));
            _simPhysicsScene2D = _simScene.GetPhysicsScene2D();
        }

        private void PrepareTableSimulation()
        {
            if (TryGetPhysicsTable(out var physicsTable))
            {
                _simPhysicsTable = Instantiate(physicsTable);
                SceneManager.MoveGameObjectToScene(_simPhysicsTable, _simScene);
            }
        }

        /// <summary>
        /// <para> Tries to get the physics table object for the current level. </para>
        /// <para> This physics table is used in simulation and represent only the colliders of the pool table. </para>
        /// <remarks> Will changed probably later. </remarks>
        /// </summary>
        /// <param name="physicsTable"> the physics table game object. </param>
        /// <returns> True if a physicsTable object was found. </returns>
        public bool TryGetPhysicsTable(out GameObject physicsTable)
        {
            var levelId = GameManager.LevelId;
            var levels = GameManager.GameData.Levels;

            if (levelId < 0 || levelId >= levels.Length || levels[levelId].physicTable == null)
            {
                physicsTable = null;
                return false;
            }

            physicsTable = levels[levelId].physicTable;
            return true;
        }

        /// <summary>
        /// <para> Construct the ball physics part in the new physics scene. </para>
        /// </summary>
        private void PrepareBallsSimulation()
        {
            _balls = GameObject.FindGameObjectsWithTag(BALL_TAG);
            _simBalls = new GameObject[_balls.Length];

            for (var i = 0; i < _balls.Length; i++)
            {
                _simBalls[i] = Instantiate(physicsBallPrefab, _balls[i].transform.position,
                    _balls[i].transform.rotation);
                SceneManager.MoveGameObjectToScene(_simBalls[i], _simScene);

                if (_balls[i] == GameManager.ShotController.gameObject)
                {
                    _simWhiteBallRigidBody = _simBalls[i].GetComponent<Rigidbody2D>();
                }
            }
        }

        /// <summary>
        /// <para> Simulates the white ball trajectory in the dedicated physics scene. </para>
        /// <remarks> Very expensive function. Should be avoided on low-end devices</remarks>
        /// </summary>
        /// <param name="force"> Force apply on the white ball. </param>
        private void SimulateWhiteBallTrajectory(in Vector2 force)
        {
            // Sets all ball positions corresponding to actual game ball positions.
            for (var i = 0; i < _balls.Length; i++)
            {
                _simBalls[i].SetActive(_balls[i].activeSelf);
                _simBalls[i].transform.position = _balls[i].transform.position;
            }

            // Apply current shot force in the physics scene.
            _simWhiteBallRigidBody.velocity = Vector3.zero;
            _simWhiteBallRigidBody.AddForce(force, ForceMode2D.Impulse);

            // Register positions of the white ball step by step in the position array
            for (var i = 0; i < steps; i++)
            {
                _simPhysicsScene2D.Simulate(Time.fixedDeltaTime);
                var pos = _simWhiteBallRigidBody.transform.position;
                _points[i] = pos;
            }
        }

        /// <summary>
        /// <para> Sets positions and color the line renderer. </para>
        /// </summary>
        /// <param name="forceMagnitude"> Shot force magnitude used for lerp line color. </param>
        private void UpdateTrajectoryRenderer(float forceMagnitude)
        {
            TrajectoryRenderer.SetPositions(_points);
            TrajectoryRenderer.startColor =
                Color.Lerp(Color.blue, Color.red, forceMagnitude);
        }

        #endregion
    }
}