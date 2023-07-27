using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string name = "None"; // Name
    public string attackName = "Do"; // Shoot / Stab / Explode / Etc.

    public AudioClip attackSound;
    public AudioClip missSound;
    public AudioClip reloadSound;
    public AudioClip failedReloadSound;

    public int weaponType = 0; //Determines Injury Types in Injury Manager

    public float DAM = 0;       // Damage
    public float ACC = 0;       // Accuracy Adder

    public float burst = 1;     // # of attack per turn // - bullets
    public float projectilePerShot = 1;

    public bool isMelee = false;
    public bool usesAmmo = false;
    public float currAmmo = 0;
    public float maxAmmo = 0;

    public Weapon(Weapon weapon)
    {
        this.name = weapon.name;
        this.attackName = weapon.attackName;
        this.weaponType = weapon.weaponType;
        this.DAM = weapon.DAM;
        this.ACC = weapon.ACC;
        this.burst = weapon.burst;
        this.projectilePerShot = weapon.projectilePerShot;
        this.isMelee = weapon.isMelee;
        this.usesAmmo = weapon.usesAmmo;
        this.currAmmo = weapon.currAmmo;
        this.maxAmmo = weapon.maxAmmo;
    }
}