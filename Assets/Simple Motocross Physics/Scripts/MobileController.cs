using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMPScripts
{
    public class MobileController : MonoBehaviour
    {
        MotoController motoController;
        public Joystick joystick;  // Reference to the Joystick

        void Start()
        {
            motoController = GetComponent<MotoController>();
        }

        void FixedUpdate()
        {
            float horizontalInput = joystick.Horizontal;
            float verticalInput = joystick.Vertical;

            // Define a higher sensitivity multiplier for faster rotation
            float rotationSensitivityMultiplier = 3.0f;  // You can tweak this value

            // Apply joystick input to the bike controls
            MobileInput(horizontalInput, ref motoController.customSteerAxis, motoController.steerControls.x * rotationSensitivityMultiplier, motoController.steerControls.y, false);
            MobileInput(verticalInput, ref motoController.customAccelerationAxis, 1, 1, false);
            MobileInput(horizontalInput, ref motoController.customLeanAxis, motoController.steerControls.x * rotationSensitivityMultiplier, motoController.steerControls.y, false);
            MobileInput(verticalInput, ref motoController.rawCustomAccelerationAxis, 1, 1, true);

            // Assuming the joystick has a dedicated button for wheelie, you would handle that separately
            // motoController.wheelieInput = System.Convert.ToBoolean(wheelie.buttonPressed);
        }

        float MobileInput(float instruction, ref float axis, float sensitivity, float gravity, bool isRaw)
        {
            var r = instruction * 2;
            var s = sensitivity;
            var g = gravity;
            var t = Time.unscaledDeltaTime;

            if (isRaw)
                axis = r;
            else
            {
                if (r != 0)
                    axis = Mathf.Clamp(axis + r * s * t, -1f, 1f);
                else
                    axis = Mathf.Clamp01(Mathf.Abs(axis) - g * t) * Mathf.Sign(axis);
            }

            return axis;
        }
    }
}
