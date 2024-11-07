using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosition : MonoBehaviour
{
    public Camera mainCamera;
    private void Update()
    {
      Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycast))
        {
            transform.position = raycast.point; 
        }

    }
}
