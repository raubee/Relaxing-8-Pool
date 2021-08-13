using UnityEngine;

namespace RP
{
    static class InputHelper
    {
        public static bool GetAnyMouseButton()
        {
            return Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);
        }

        public static bool GetAnyMouseButtonDown()
        {
            return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);
        }

        public static bool GetAnyMouseButtonUp()
        {
            return Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2);
        }
    }
}