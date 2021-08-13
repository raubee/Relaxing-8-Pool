using UnityEngine;
using UnityEngine.UI;

namespace RP
{
    /// <summary>
    /// <para> Shows or Hides controller choices. Dirty script please see README. </para>
    /// </summary>
    public class UIIntroPanel : MonoBehaviour
    {
        private const string NO_CONTROLLER_MESSAGE = "No controller supported :/!";

        [SerializeField] private ShotController shotController;

        [SerializeField] private Text controlText;

        [SerializeField] private Button MouseControlButton;
        [SerializeField] private Button KeyboardControlButton;
        [SerializeField] private Button StartGameButton;
        [SerializeField] private Button QuitGameButton;

        private void Start()
        {
            var controllers = shotController.GetSupportedControllers();

            if (controllers.Length == 0)
            {
                controlText.text = NO_CONTROLLER_MESSAGE;
                MouseControlButton.gameObject.SetActive(false);
                KeyboardControlButton.gameObject.SetActive(false);
                StartGameButton.gameObject.SetActive(false);
                QuitGameButton.gameObject.SetActive(true);
                return;
            }

            QuitGameButton.gameObject.SetActive(false);

            if (controllers.Length < 2)
            {
                controlText.gameObject.SetActive(false);
                MouseControlButton.gameObject.SetActive(false);
                KeyboardControlButton.gameObject.SetActive(false);

                if (StartGameButton != null)
                    StartGameButton.gameObject.SetActive(true);

                shotController.SetController(controllers[0]);
                return;
            }

            StartGameButton.gameObject.SetActive(false);
            controlText.gameObject.SetActive(true);

            foreach (var controller in controllers)
            {
                if (controller == ControllerType.Mouse)
                    MouseControlButton.gameObject.SetActive(true);

                if (controller == ControllerType.Keyboard)
                    KeyboardControlButton.gameObject.SetActive(true);
            }
        }
    }
}