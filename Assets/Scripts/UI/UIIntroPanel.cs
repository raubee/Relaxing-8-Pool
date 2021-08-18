using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RP
{
    /// <summary>
    /// <para> Shows or Hides controller choices. Dirty script please see README. </para>
    /// </summary>
    public class UIIntroPanel : MonoBehaviour
    {
        [Header("Controllers")]
        [SerializeField] private ShotController shotController;

        [Space]
        [SerializeField] private ShotCommandProvider mouseCommandProvider;
        [SerializeField] private ShotCommandProvider keyboardCommandProvider;
        [SerializeField] private ShotCommandProvider touchCommandProvider;

        [SerializeField] private Text controlText;

        [SerializeField] private Button MouseControlButton;
        [SerializeField] private Button KeyboardControlButton;
        [SerializeField] private Button TouchControlButton;
        [SerializeField] private Button QuitGameButton;

        public UnityEvent OnControllerSelected = new UnityEvent();

        private void Start()
        {
            if (Application.isMobilePlatform)
            {
                MouseControlButton.gameObject.SetActive(false);
                KeyboardControlButton.gameObject.SetActive(false);
                TouchControlButton.gameObject.SetActive(true);
                QuitGameButton.gameObject.SetActive(true);
                return;
            }

            controlText.gameObject.SetActive(true);
            QuitGameButton.gameObject.SetActive(false);
            TouchControlButton.gameObject.SetActive(false);
            MouseControlButton.gameObject.SetActive(true);
            KeyboardControlButton.gameObject.SetActive(true);
        }

        public void SelectController(ShotCommandProvider provider)
        {
            shotController.SetController(provider);
            OnControllerSelected.Invoke();
        }
    }
}