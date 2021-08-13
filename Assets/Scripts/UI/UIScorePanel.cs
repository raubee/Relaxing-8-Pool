using UnityEngine;
using UnityEngine.UI;

namespace RP.UI
{
    /// <summary>
    /// <para> Updates the score UI text. Converts int score to a text displayable. </para>
    /// </summary>
    public class UIScorePanel : MonoBehaviour
    {
        [SerializeField] private Text scoreText;

        public void UpdateScore(int score)
        {
            scoreText.text = score.ToString();
        }
    }
}