using UnityEngine;

namespace RP
{
    [CreateAssetMenu(menuName = "RP/Level")]
    public class Level : ScriptableObject
    {
        public string Name;
        public string SceneName;
        public GameObject physicTable;
    }
}