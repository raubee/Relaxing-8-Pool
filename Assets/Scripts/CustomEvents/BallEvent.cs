using UnityEngine.Events;

namespace RP.Events
{
    /// <summary>
    /// <para> Custom Unity event sending a ball reference. </para>
    /// <para> Used when ball is pocketed. </para>
    /// </summary>
    public class BallEvent : UnityEvent<BallBehaviour>
    {
    }
}