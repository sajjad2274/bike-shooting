using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Unity.Netcode;
using Unity.VisualScripting;
public class WeaponActivation : NetworkBehaviour
{
    [SerializeField] private TwoBoneIKConstraint rightHandIK;
    [SerializeField] private MultiAimConstraint headAiming;
    [SerializeField] private MultiAimConstraint rightArmAiming;
    [SerializeField] private MultiAimConstraint rightForearmAiming;
    [SerializeField] private MultiAimConstraint rightHandAiming;
    [SerializeField] private RigBuilder rigbuilder;
    [SerializeField] private GameObject weapon;
    [SerializeField] private Transform weaponTransform;
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
        if (!IsOwner) return;
        isHavingWeapon = !isHavingWeapon;
        if (isHavingWeapon)
        {
            WeaponServerRpc();
        }
        else
        {
            DeactivateWeapon();
        }
    }
    [ServerRpc]
    public void WeaponServerRpc()
    {
        // GameObject weaponInstance = Instantiate(weapon, weaponTransform.position, weaponTransform.rotation);

        // Make the weapon a networked object
        //  weaponInstance.GetComponent<NetworkObject>().Spawn();
        weapon.SetActive(true);
        rightHandIK.weight = 0;
        headAiming.weight = 1;
        rightHandAiming.weight = 1;
        rightArmAiming.weight = 1;
        rightForearmAiming.weight = 1;
        ShooterController.shootingInstance.SettingLayer(1);
        // WeaponClientRpc();
    }
    [ClientRpc]
    public void WeaponClientRpc()
    {

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

}


//using System;
//using UnityEngine;
//using Unity.Netcode;
//using UnityEngine.Animations.Rigging;

//public class WeaponActivation : NetworkBehaviour
//{
//    [SerializeField] private TwoBoneIKConstraint rightHandIK;
//    [SerializeField] private MultiAimConstraint headAiming;
//    [SerializeField] private MultiAimConstraint rightArmAiming;
//    [SerializeField] private MultiAimConstraint rightForearmAiming;
//    [SerializeField] private MultiAimConstraint rightHandAiming;
//    [SerializeField] private RigBuilder rigBuilder;
//    [SerializeField] private GameObject weapon;


//    public NetworkVariable<bool> isHavingWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//    public static WeaponActivation weaponInstance;

//    private void Start()
//    {
//        weaponInstance = this;
//        isHavingWeapon.OnValueChanged += OnWeaponStateChanged;
//    }

//    private void Update()
//    {
//        weapon.SetActive(isHavingWeapon.Value);
//    }

//    public void ActivateWeapon()
//    {
//        if (!IsOwner) return;
//        ToggleWeaponStateServerRpc(!isHavingWeapon.Value);
//    }

//    public void OnWeaponStateChanged(bool previousValue, bool newValue)
//    {
//        if (newValue)
//        {
//            EnableWeapon();
//        }
//        else
//        {
//            DisableWeapon();
//        }
//    }

//    [ServerRpc]
//    private void ToggleWeaponStateServerRpc(bool newWeaponState)
//    {
//        isHavingWeapon.Value = newWeaponState;
//    }

//    private void EnableWeapon()
//    {
//        weapon.SetActive(true);
//        rightHandIK.weight = 0;
//        headAiming.weight = 1;
//        rightHandAiming.weight = 1;
//        rightArmAiming.weight = 1;
//        rightForearmAiming.weight = 1;

//        if (ShooterController.shootingInstance != null)
//        {
//            ShooterController.shootingInstance.SettingLayer(1);
//        }
//    }

//    private void DisableWeapon()
//    {
//        weapon.SetActive(false);
//        rightHandIK.weight = 1;
//        headAiming.weight = 0;
//        rightHandAiming.weight = 0;
//        rightArmAiming.weight = 0;
//        rightForearmAiming.weight = 0;

//        if (ShooterController.shootingInstance != null)
//        {
//            ShooterController.shootingInstance.SettingLayer(0);
//        }
//    }

//    private void OnDestroy()
//    {
//        isHavingWeapon.OnValueChanged -= OnWeaponStateChanged;
//    }
//}