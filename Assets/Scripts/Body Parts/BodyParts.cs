using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BodyParts
{
    public string partName;
    public float HP;
    public float maxHP;
    public bool parentBelow;
    public bool parentAbove;
    [HideInInspector] public bool isDestroyed = false;
    public bool deathWhenDestroyed = false;
    public Injury[] injuries;
    public bool usedForAttacking = false;
    public bool usedForMovement = false;
    [HideInInspector] public bool tookDamage = false;

    public BodyParts (BodyParts bodyParts)
    {
        this.partName = bodyParts.partName;
        this.HP = bodyParts.HP;
        this.maxHP = bodyParts.maxHP;
        this.parentBelow = bodyParts.parentBelow;
        this.parentAbove = bodyParts.parentAbove;
        this.isDestroyed = bodyParts.isDestroyed;
        this.deathWhenDestroyed = bodyParts.deathWhenDestroyed;
        this.injuries = bodyParts.injuries;
        this.usedForAttacking = bodyParts.usedForAttacking;
        this.usedForMovement = bodyParts.usedForMovement;
        this.tookDamage = bodyParts.tookDamage;
    }
}
