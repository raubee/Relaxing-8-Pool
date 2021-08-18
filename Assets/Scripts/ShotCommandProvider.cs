using UnityEngine;

namespace RP
{
    public abstract class ShotCommandProvider : ScriptableObject, IShotCommandProvider
    {
        public abstract Vector3 GetShotForce();
        public abstract bool TriggerShot();
    }
}