using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Weapon[] weapons;
    public SoundManager soundManager;

    public AudioClip defAttackSound;
    public AudioClip defMissSound;
    public AudioClip defReloadSound;
    public AudioClip defFailedReloadSound;

    public Weapon weaponGenerator()
    {
        Weapon weapon = weapons[Random.Range(1, weapons.Length)];

        return weapon;
    }

    public void PlayAttackSound(Weapon weap)
    {
        if (weap.attackSound)
            soundManager.PlaySound(weap.attackSound);
        else
            soundManager.PlaySound(defAttackSound);
    }

    public void PlayMissSound(Weapon weap)
    {
        if (weap.missSound)
            soundManager.PlaySound(weap.missSound);
        else
            soundManager.PlaySound(defMissSound);
    }

    public void PlayReloadSound(Weapon weap)
    {
        if (weap.reloadSound)
            soundManager.PlaySound(weap.reloadSound);
        else
            soundManager.PlaySound(defReloadSound);
    }

    public void PlayFailedReloadSound(Weapon weap)
    {
        if (weap.failedReloadSound)
            soundManager.PlaySound(weap.failedReloadSound);
        else
            soundManager.PlaySound(defFailedReloadSound);
    }
}
