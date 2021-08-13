using System;
using UnityEngine.Events;

namespace RP.Events
{
    /// <summary>
    /// <para> Custom Unity event sending an integer. </para>
    /// </summary>
    [Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
}