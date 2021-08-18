using UnityEngine;

namespace RP
{
    [CreateAssetMenu(menuName = "RP/Controllers/Touch")]
    public class TouchShotCommandProvider : ShotCommandProvider
    {
        private const float DRAG_DISTANCE = 5f;

        private Vector3 _force = Vector3.zero;
        private Vector3 _touchPosition = Vector3.zero;

        public override Vector3 GetShotForce()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchPosition = touch.position;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    Vector3 mousePosition = touch.position;
                    _force = (_touchPosition - mousePosition) * DRAG_DISTANCE;
                    _force.x /= Screen.currentResolution.width;
                    _force.y /= Screen.currentResolution.height;
                    _force.z = 0;

                    // Clamp vector to 1f
                    if (_force.magnitude > 1.0f)
                    {
                        _force = _force.normalized;
                    }
                }
            }

            return _force;
        }

        public override bool TriggerShot()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                return touch.phase == TouchPhase.Ended;
            }

            return false;
        }
    }
}