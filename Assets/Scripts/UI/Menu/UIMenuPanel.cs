using UnityEngine;

namespace RP.UI.Menu
{
    /// <summary>
    /// <para> Class for panels handled by the UIMenu. </para>
    /// <remarks> Custom panel animation transitions could be processed here. </remarks>
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class UIMenuPanel : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        public void Show()
        {
            _canvas.enabled = true;
        }
    }
}