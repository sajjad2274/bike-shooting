using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShooterController : MonoBehaviour
{
    //[SerializeField] private Button fireButton;
    [SerializeField] private ParticleSystem muzleFlash;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform crossHairTarget;
    [SerializeField] private TrailRenderer bullettracer;
    public Animator anim;
    Ray ray;
    RaycastHit hitInfo;
    public static ShooterController shootingInstance;

    private void Start()
    {
        shootingInstance = this;
    }
    public void Fire()
    {
        if (WeaponActivation.weaponInstance.isHavingWeapon == true)
        {
            Startfiring();
        }
        
    }
    public void SettingLayer( int layerIndex)
    {
        for (int i = 0; i < anim.layerCount; i++)
        {
            anim.SetLayerWeight(i, 0);
        }
        Debug.Log("Layer setted");
        // Set the specified layer's weight to 1 (active)
        anim.SetLayerWeight(layerIndex, 1);
    }
    public void Startfiring ()
    {
       
        muzleFlash.Emit(1);
        ray.origin = raycastOrigin.position;
        ray.direction = raycastOrigin.forward; // it give direction from raycast origin position towards crosshair destination 
        var tracer = Instantiate(bullettracer, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);
        if (Physics.Raycast(ray,out hitInfo))
        {
           // Debug.DrawLine(ray.origin, hitInfo.point,Color.red, 2.0f);
            hitEffect.transform.position = hitInfo.point;

            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);
            tracer.transform.position=hitInfo.point;
          //  Debug.Log(hitInfo);
        }

    }
   }
