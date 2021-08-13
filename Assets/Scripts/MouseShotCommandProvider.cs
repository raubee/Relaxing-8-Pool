using UnityEngine;
using UnityEngine.EventSystems;

namespace RP
{
    public class MouseShotCommandProvider : IShotCommandProvider
    {
        private const float DRAG_DISTANCE = 5f;

        private Vector3 _force;
        private Vector3 _pressPosition;

        private bool _pressed;

        public Vector3 GetShotForce()
        {
            if (InputHelper.GetAnyMouseButtonDown() &&
                !EventSystem.current.IsPointerOverGameObject()) // fix: avoid UI click
            {
                _pressed = true;
                _pressPosition = Input.mousePosition;
            }

            if (_pressed)
            {
                var mousePosition = Input.mousePosition;

                _force = (_pressPosition - mousePosition) * DRAG_DISTANCE;
                _force.x /= Screen.currentResolution.width;
                _force.y /= Screen.currentResolution.height;
                _force.z = 0;

                // Clamp vector to 1f
                if (_force.magnitude > 1.0f)
                {
                    _force = _force.normalized;
                }
            }

            return _force;
        }

        public bool TriggerShot()
        {
            if (_pressed && InputHelper.GetAnyMouseButtonUp())
            {
                _pressed = false;
                return true;
            }

            return false;
        }
    }
}