using UnityEngine;

namespace RP
{
    public interface IShotCommandProvider
    {
        /// <summary>
        /// <para> Provides the shot force, magnitude should be 1.0f at maximum. </para>
        /// <remarks> Used in the update method of the shot controller. Implementation should be less expensive as possible. </remarks>
        /// </summary>
        /// <returns> The shot force vector with clamped magnitude between 0f and 1f. </returns>
        Vector3 GetShotForce();

        /// <summary>
        /// <para> Is a shot command detected by the implementation ? </para>
        /// <remarks> Used in the update method of the shot controller. Implementation should be less expensive as possible. </remarks>
        /// </summary>
        /// <returns></returns>
        bool TriggerShot();
    }
}