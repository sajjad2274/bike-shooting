using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrossHairTarget : MonoBehaviour
{
    //  Camera maincamera;
    public Transform target;
    Ray ray;
    RaycastHit hitInfo;
    private void Start()
    {
     //   maincamera = Camera.main;
    }
    private void Update()
    {
        ray.origin = target.transform.position;
        ray.direction = target.transform.forward;
        
        Physics.Raycast(ray, out hitInfo);
        
    }
}
