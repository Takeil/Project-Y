using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Injury
{
    public string injuryName;

    //Type of Weapon that will cause the Injury
    public int injuryType;
    
    public float damagePerTurn; //Bleed Rate Per Turn

    //One time for Will and BodyPart
    public float willDamage;
    public float partDamage;

    [HideInInspector]
    public bool isTended = false;

    //Time until recovery
    public int remainingTime;

    [HideInInspector]
    public int timeLeft;

    public Injury(Injury injury)
    {
        this.injuryName = injury.injuryName;
        this.injuryType = injury.injuryType;
        this.damagePerTurn = injury.damagePerTurn;
        this.willDamage = injury.willDamage;
        this.partDamage = injury.partDamage;
        this.isTended = injury.isTended;
        this.remainingTime = injury.remainingTime;
        this.timeLeft = injury.timeLeft;
    }
}