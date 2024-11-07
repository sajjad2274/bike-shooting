using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
public class WeaponActivation : MonoBehaviour
{
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private MultiAimConstraint headAiming;
    [SerializeField] private MultiAimConstraint rightArmAiming;
    [SerializeField] private MultiAimConstraint rightForearmAiming;
    [SerializeField] private MultiAimConstraint rightHandAiming;
    [SerializeField] private RigBuilder rigbuilder;
    [SerializeField] private GameObject weapon;
    public bool isHavingWeapon;
    public static WeaponActivation weaponInstance;

    private void Start()
    {
        weaponInstance = this;
    }
    private void Update()
    {
        if (isHavingWeapon == false)
        {
            weapon.SetActive(false);
        }
    }
    public void ActivateWeapon()
    {
        isHavingWeapon = !isHavingWeapon;
        if (isHavingWeapon)
        {
            weapon.SetActive(true);
            rightHandIK.weight = 0;
            headAiming.weight = 1;
            rightHandAiming.weight = 1;
            rightArmAiming.weight = 1;
            rightForearmAiming.weight = 1;
            ShooterController.shootingInstance.SettingLayer(1);
        }
        else
        {
            DeactivateWeapon();
        }
    }
    public void DeactivateWeapon()
    {
        weapon.SetActive(false);
        rightHandIK.weight = 1;
        headAiming.weight = 0;
        rightHandAiming.weight = 0;
        rightArmAiming.weight = 0;
        rightForearmAiming.weight = 0;
        ShooterController.shootingInstance.SettingLayer(0);
    }
    //private IEnumerator SmoothRigWeight()
    //{
    //    aimHandRig.weight = 0;
    //    yield return new WaitForEndOfFrame(); // Ensure one frame passes before re-enabling
    //    aimHandRig.weight = 1;
    //}
    /*public void ChangeTarget(Transform newTarget)
    {
      
        if (rigbuilder != null)
        {
            rigbuilder.Build();
        }
        aimHandRig.data.target = newTarget;
       
    }*/
}
