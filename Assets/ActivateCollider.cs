using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCollider : MonoBehaviour
{
    public HeroWeapon weapon;
    private void Start()
    {
        weapon = GetComponentInChildren<HeroWeapon>();
        weapon.DeactivateWeapon();
    }
    public void ActivateCol()
    {
        weapon.ActivateWeapon();
    }
    public void DectivateCol()
    {
        weapon.DeactivateWeapon();
    }
}
