using System;
using UnityEngine.Events;

namespace RP.Events
{
    /// <summary>
    /// <para> Custom Unity event sending a boolean. </para>
    /// </summary>
    [Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }
}