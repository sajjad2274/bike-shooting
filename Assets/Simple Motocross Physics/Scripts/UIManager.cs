using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
   public void Fire()
    {
        
        ShooterController.shootingInstance.Fire();
    }
    public void Weapon()
    {

        WeaponActivation.weaponInstance.ActivateWeapon();
    }
}
