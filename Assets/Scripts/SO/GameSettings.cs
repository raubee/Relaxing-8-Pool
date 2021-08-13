using UnityEngine;

namespace RP
{
    [CreateAssetMenu(menuName = "RP/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public int StartingScore;
        public int LoseScore;
        public int WinScore;

        public int StartLevel;
        public Level[] Levels;
    }
}