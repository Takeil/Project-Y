using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isDefeated = false;
    [HideInInspector] public bool cantPhysicallyFight = false;
    [HideInInspector] public bool cantMelee = false;
    [HideInInspector] public bool reasoned = false;

    public string FName;
    public string LName;

    public float maxHP = 0;
    public float currHP = 0;

    public float maxWill = 100;
    public float currWill = 100;

    public int gender; // 0 = male, 1 = female;

    public BodyParts[] bodyParts;
    public Statistics stats;

    public Weapon[] weapons = new Weapon[3];// = new Weapon[] { new Weapon(1), new Weapon(2), new Weapon(3) }; // 0 melee, 1 secondary, 2 primary;
    public int equipedWeapon = 0;

    public GameObject body;
    public GameObject portrait;

    [HideInInspector] public Color skinColor;
    [HideInInspector] public Color hairColor;
    [HideInInspector] public Color shirtColor;

    //useless variables
    [HideInInspector] public int age;
    [HideInInspector] public string birthday, bloodTpye;

    public void RecolorPortrait(GameObject _portrait)
    {
        foreach(Transform child in _portrait.transform)
        {
            if (child.name.Equals("Hair"))
            {
                child.GetComponent<SpriteRenderer>().color = hairColor;
            }
            if (child.name.Equals("Body"))
            {
                child.GetComponent<SpriteRenderer>().color = skinColor;
            }
            if (child.name.Equals("Shirt"))
            {
                child.GetComponent<SpriteRenderer>().color = shirtColor;
            }
        }
    }

    public void CalculateHP()
    {
        currHP = 0f;
        maxHP = 0f;
        foreach (BodyParts part in bodyParts)
        {
            currHP += part.HP;
            maxHP += part.maxHP;
        }
    }

    public void SetAll(Unit u)
    {
        bodyParts = u.bodyParts;
        currHP = u.currHP;
        currWill = u.currWill;
        equipedWeapon = u.equipedWeapon;
        isAlive = u.isAlive;
        isDefeated = u.isDefeated;
        cantPhysicallyFight = u.cantPhysicallyFight;

        cantMelee = u.cantMelee;
        reasoned = u.reasoned;
    }

    public void SetEverything(Unit u)
    {
        this.FName = u.FName;
        this.LName = u.LName;

        this.maxHP = u.maxHP;
        this.maxWill = u.maxWill;

        this.gender = u.gender;
        this.stats = u.stats;

        this.weapons = u.weapons;
        this.body = u.body;

        this.age = u.age;
        this.birthday = u.birthday;
        this.bloodTpye = u.bloodTpye;


        this.portrait = u.portrait;

        this.skinColor = u.skinColor;
        this.hairColor = u.hairColor;
        this.shirtColor = u.shirtColor;
    
        SetAll(u);
    }
}

[System.Serializable]
public class Statistics
{
    public int DEX; //Speed / Dodge %
    public int ACC; //Accuracy - Gun
}