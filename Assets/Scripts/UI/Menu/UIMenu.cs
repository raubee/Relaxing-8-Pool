using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RP.UI.Menu
{
    /// <summary>
    /// <para> Simple stack based menu. </para>
    /// <para> Each menu panel is put into a stack in order to handle the back button.
    /// When back button is clicked the stack pops the current panel and displays the parent panel on top of the stack instead. </para>
    /// </summary>
    public class UIMenu : MonoBehaviour
    {
        /// <summary>
        /// <para> Minimum stack capacity for optimization, not really relevant here though. </para>
        /// </summary>
        private const int MIN_STACK_CAPACITY = 2;

        /// <summary>
        /// <para> Menu panel stack. </para>
        /// </summary>
        private readonly Stack<UIMenuPanel> _stack = new Stack<UIMenuPanel>(MIN_STACK_CAPACITY);

        /// <summary>
        /// <para> Panel considered as the root panel when menu opens. </para>
        /// </summary>
        [SerializeField] private UIMenuPanel menuPanel;

        [SerializeField] private Canvas openMenuCanvas;
        [SerializeField] private Canvas quitButtonCanvas;
        [SerializeField] private Canvas returnButtonCanvas;

        /// <summary>
        /// <para> To do custom actions when menu was opened. </para>
        /// <remarks> Pause the game for example. </remarks>
        /// </summary>
        public UnityEvent OnMenuOpenEvent = new UnityEvent();

        /// <summary>
        /// <para> To do custom actions when menu was closed. </para>
        /// <remarks> Resume the game for example. </remarks>
        /// </summary>
        public UnityEvent OnMenuCloseEvent = new UnityEvent();

        /// <summary>
        /// <para> Stacks the root menu panel and enable the close button. </para>
        /// <remarks> It sends an open menu event if action need to be performed when the menu opens. </remarks>
        /// </summary>
        public void OpenMenu()
        {
            OpenMenuPanel(menuPanel);
            quitButtonCanvas.enabled = true;
            openMenuCanvas.enabled = false;
            OnMenuOpenEvent.Invoke();
        }

        /// <summary>
        /// <para> Pops the current menu panel and go to the parent panel. </para>
        /// </summary>
        public void GoBack()
        {
            _stack.Peek().Hide();
            _stack.Pop();

            if (_stack.Count > 0)
                _stack.Peek().Show();

            if (_stack.Count <= 1)
                returnButtonCanvas.enabled = false;
        }

        /// <summary>
        /// <para> Closes the menu by hiding the current panel and clearing the stack. </para>
        /// <remarks> It sends an close menu event if action need to be performed when the menu closes. </remarks>
        /// </summary>
        public void Close()
        {
            // Hide current panel
            if (_stack.Count > 0)
                _stack.Peek().Hide();

            // Clear stack 
            while (_stack.Count != 0)
            {
                _stack.Pop();
            }

            returnButtonCanvas.enabled = false;
            quitButtonCanvas.enabled = false;
            openMenuCanvas.enabled = true;
            OnMenuCloseEvent.Invoke();
        }

        /// <summary>
        /// <para> Hides the current panel on top of the stack.
        /// Displays the given panel and add it to the stack. </para>
        /// </summary>
        /// <param name="panel"> The panel to stack. </param>
        public void OpenMenuPanel(UIMenuPanel panel)
        {
            // Hide last panel
            if (_stack.Count > 0)
                _stack.Peek().Hide();

            // Show new panel
            _stack.Push(panel);
            _stack.Peek().Show();

            if (_stack.Count > 1)
                returnButtonCanvas.enabled = true;
        }
    }
}