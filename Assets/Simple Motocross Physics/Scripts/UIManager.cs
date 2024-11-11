using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    public void Fire()
    {
        ShooterController.shootingInstance.Fire();
    }
    public void Weapon()
    {
       //  if (IsOwner) return;

        WeaponActivation.weaponInstance.ActivateWeapon();
    }
   
   
}
