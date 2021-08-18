using UnityEngine;

namespace RP
{
    [CreateAssetMenu(menuName="RP/Controllers/Keyboard")]
    public class KeyboardShotCommandProvider : ShotCommandProvider
    {
        private const float ANGLE_STEP = 0.005f; // radians
        private const float FORCE_STEP = 0.005f;

        private float _angle = Mathf.PI; // In radians
        private float _force = 0.5f;

        public override Vector3 GetShotForce()
        {
            UpdateAngle();
            UpdateForce();

            return new Vector3(Mathf.Cos(_angle), Mathf.Sin(_angle), 0) * _force;
        }

        public override bool TriggerShot()
        {
            return Input.GetKey(KeyCode.Space);
        }

        public void UpdateAngle()
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _angle += ANGLE_STEP;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                _angle -= ANGLE_STEP;
            }
        }

        public void UpdateForce()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _force += FORCE_STEP;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                _force -= FORCE_STEP;
            }

            _force = Mathf.Clamp(_force, 0.01f, 1f);
        }
    }
}