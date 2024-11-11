using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class AimCamera : MonoBehaviour
{
     Camera mmainCamera;

    // Define the bottom-right area as a percentage of screen size
    [Range(0, 1)] public float screenWidth = 0.75f;
    [Range(0, 1)] public float screenHeight = 1f;

    private void Start()
    {
        mmainCamera = Camera.main;
    }
    private void Update()
    {
        Vector2 screenPosition;

        // Checking if the touch or mouse is on a UI element
        if (IsPointerOverUIObject()) return;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // for mobile
            screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null)
        {
            // for window
            screenPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            // No input available
            return;
        }

        // Check if the screenPosition is within the defined bottom-right area
        if (screenPosition.x < Screen.width * (1 - screenWidth) || screenPosition.y > Screen.height * screenHeight)
        {
            return; // Ignore input outside the bottom-right area
        }

        // Calculate the aiming position
        Ray ray = mmainCamera.ScreenPointToRay(screenPosition);
        transform.position = ray.GetPoint(10);
    }

    // Helper method to check if pointer is over UI
    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Touchscreen.current?.primaryTouch.position.ReadValue() ?? Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
