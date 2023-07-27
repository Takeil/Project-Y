using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InjuryManager : MonoBehaviour
{
    [SerializeField] Injury[] injuries;

    [SerializeField] Injury[] permanentInjuries = new Injury[2];

    [SerializeField] BattleSystem battleSystem;
    [SerializeField] SoundManager soundManager;

    [SerializeField] AudioClip treatedSound;
    [SerializeField] AudioClip failedTreatedSound;

    List<Injury> tempInjury = new List<Injury>();
    BodyParts Cut(BodyParts bp)
    {
        tempInjury = new List<Injury>();
        BodyParts bodyPart = bp;
        bodyPart.injuries = tempInjury.ToArray();

        tempInjury.Add(new Injury(permanentInjuries[0]));
        bodyPart.injuries = tempInjury.ToArray();
        return bodyPart;
    }

    BodyParts PatchedCut(BodyParts bp)
    {
        tempInjury = new List<Injury>();
        BodyParts bodyPart = bp;
        bodyPart.injuries = tempInjury.ToArray();

        tempInjury.Add(new Injury(permanentInjuries[1]));
        bodyPart.injuries = tempInjury.ToArray();
        return bodyPart;
    }

    BodyParts RemoveBodyPart(BodyParts bp)
    {
        tempInjury = new List<Injury>();
        BodyParts bodyParts = bp;
        bodyParts.injuries = tempInjury.ToArray();
        bodyParts.HP = 0;
        bodyParts.isDestroyed = true;
        return bodyParts;
    }

    BodyParts[] RemoveChildren(BodyParts[] bodyPart, int index)
    {
        if (bodyPart[index].parentBelow)
        {
            //Debug.Log("Avulsion on: " + bodyPart[index].partName + ". Also Removed " + bodyPart[index+1].partName);
            bodyPart[index+1] = RemoveBodyPart(bodyPart[index+1]);
            RemoveChildren(bodyPart, index + 1);
        }
        if (bodyPart[index].parentAbove)
        {
            bodyPart[index - 1] = RemoveBodyPart(bodyPart[index - 1]);
            RemoveChildren(bodyPart, index - 1);
        }

        return bodyPart;
    }

    public Unit DealInjury(Unit u, Unit p)
    {
        Unit unit = u, unit2 = p;
        int selectedPart;
        BodyParts[] bodyPart = u.bodyParts;

        int ACC = unit2.stats.ACC;
        int injuryType = unit2.weapons[unit2.equipedWeapon].weaponType;

        int loopBreaker = 0;

        bool isBurst = false;
        List<string> destroyedParts = new List<string>();
        bool destroyedBurst = false;
        int hits = 0;

        // Lethality Probability Calculator
        // (1) Get the sum of all injuries in injury manager; add them in a list.
        float sum = 0f;
        List<float> prob = new List<float>();
        List<Injury> injury = new List<Injury>();

        for (int i = 0; i < injuries.Length; i++)
        {
            if (injuries[i].injuryType == injuryType)
            {
                injury.Add(new Injury(injuries[i]));
                sum += injuries[i].partDamage;
            }
        }

        // Order by part damage
        // (2) Order the injuries by lethality (partDamage).
        injury = injury.OrderBy(e => e.partDamage).ToList();

        // (3) Compute the probability of each injury by dividing the damage to the sum plus the previous chance.
        for (int i = 0; i < injury.Count; i++)
        {
            if (i != 0)
                prob.Add(injury[i].partDamage / sum + prob[i - 1]);
            else
                prob.Add(injury[i].partDamage / sum);
        }

        // Reverse the list
        // (4) Reverse the list to get the final list of injuries according to its lethality.
        injury.Reverse();

        if (unit2.weapons[unit2.equipedWeapon].burst > 1 || unit2.weapons[unit2.equipedWeapon].projectilePerShot > 1)
            isBurst = true;

        for (int j = 0; j < unit2.weapons[unit2.equipedWeapon].burst; j++)
        {
            for (int k = 0; k < unit2.weapons[unit2.equipedWeapon].projectilePerShot; k++)
            {
                while (true)
                {
                    selectedPart = Random.Range(0, u.bodyParts.Length);

                    // Check if part is already gone
                    if (bodyPart[selectedPart].isDestroyed || bodyPart[selectedPart].HP <= 0)
                    {
                        //Debug.Log("Tried Hitting A Destroyed Body Part");
                        loopBreaker++;
                    }
                    else
                    {
                        break;
                    }

                    if (loopBreaker > 20)
                    {
                        if (!isBurst)
                            battleSystem.SetDialogue(unit2.FName + " tried hitting " + unit.FName + "'s " + bodyPart[selectedPart].partName + ", but it's already gone");
                        return unit;
                    }
                }

                //Roll if it hits or nahh
                float roll = (Random.Range(0, 1000) / 1000f) - ((ACC / (10) + unit2.weapons[unit2.equipedWeapon].ACC / 10));
                //Lethality according to Accuracy

                // If it hits
                if (ACC > Random.Range(0, 10))
                // Deal the damage
                {
                    if (unit.stats.DEX > Random.Range(0, 20))
                    {
                        if (!isBurst)
                        {
                            battleSystem.SetDialogue(unit.FName + " dodged " + unit2.FName + "'s attack!");
                            battleSystem.weaponManager.PlayMissSound(unit.weapons[unit.equipedWeapon]);
                        }
                    }

                    else
                    {
                        for (int i = 0; i < prob.Count; i++)
                        {
                            if (prob[i] > roll)
                            {
                                bodyPart[selectedPart].tookDamage = true;

                                //Add the Injury to the Body Part
                                tempInjury = new List<Injury>(bodyPart[selectedPart].injuries);
                                tempInjury.Add(new Injury(injury[i]));

                                bodyPart[selectedPart].injuries = tempInjury.ToArray();
                                bodyPart[selectedPart] = new BodyParts(bodyPart[selectedPart]);

                                //Start Timer
                                bodyPart[selectedPart].injuries[bodyPart[selectedPart].injuries.Length - 1].timeLeft
                                    = bodyPart[selectedPart].injuries[bodyPart[selectedPart].injuries.Length - 1].remainingTime;

                                //bodyPart[selectedPart].HP -= injury[i].partDamage;

                                //Lose Will and Deal Weapon Damage
                                unit.currWill -= injury[i].willDamage;
                                unit.currHP -= unit2.weapons[unit2.equipedWeapon].DAM;

                                if (!isBurst)
                                { 
                                    battleSystem.SetDialogue(unit2.FName + " injured " + unit.FName + "'s " + bodyPart[selectedPart].partName.ToLower());// + "!\nDealt: " + injury[i].injuryName);
                                    battleSystem.weaponManager.PlayAttackSound(unit.weapons[unit.equipedWeapon]);
                                }
                                else
                                    hits++;

                                //Compute Body Part
                                bodyPart = BodyPartInjuryComputation(bodyPart);

                                // Remove Body Part if it's at Zero
                                if (bodyPart[selectedPart].HP <= 0)
                                {
                                    if (!isBurst)
                                    {
                                        battleSystem.SetDialogue(unit2.FName + " destroyed " + unit.FName + "'s " + bodyPart[selectedPart].partName.ToLower() + "!");
                                    }
                                    else
                                    {
                                        destroyedParts.Add(bodyPart[selectedPart].partName);
                                        destroyedBurst = true;
                                    }

                                    if (bodyPart[selectedPart].deathWhenDestroyed)
                                    {
                                        unit.currHP = 0;
                                    }

                                    bodyPart[selectedPart].HP = 0;
                                    bodyPart[selectedPart] = Cut(bodyPart[selectedPart]);
                                    RemoveChildren(bodyPart, selectedPart);
                                }
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (!isBurst)
                    { 
                        battleSystem.SetDialogue(unit2.FName + " missed!"); 
                        battleSystem.weaponManager.PlayMissSound(unit.weapons[unit.equipedWeapon]);
                    }

                    //Compute Body Part
                    bodyPart = BodyPartInjuryComputation(bodyPart);
                }
            }
        }

        if (isBurst)
        {
            if (hits > 0)
            { 
                battleSystem.SetDialogue(unit2.FName + " hit " + unit.FName + " " + hits + " times!");
                battleSystem.weaponManager.PlayAttackSound(unit.weapons[unit.equipedWeapon]);
            }
            else
            { 
                battleSystem.SetDialogue(unit2.FName + " missed!");
                battleSystem.weaponManager.PlayMissSound(unit.weapons[unit.equipedWeapon]);
            }

            if (destroyedBurst)
            {
                string str = unit2.FName + " destroyed " + unit.FName + "'s ";

                for (int i = 0; i < destroyedParts.Count; i++)
                {
                    if (destroyedParts.Count <= 1)
                        str += destroyedParts[i] + "!";
                    else
                    { 
                        if (i < destroyedParts.Count - 1)
                            str += destroyedParts[i] + ", ";
                        else
                            str += "and " + destroyedParts[i] + "!";
                    }
                }

                battleSystem.SetDialogue(str);
            }
        }

        return unit;
    }

    public BodyParts[] BodyPartInjuryComputation(BodyParts[] bp)
    {
        BodyParts[] bodyPart = bp;

        foreach(BodyParts part in bodyPart)
        {
            float totalDMG = 0;
            foreach(Injury injury in part.injuries)
            {
                // Calculate part damage though this equation:
                // (Time_Left_To_Regenerate / Total_Time_To_Regenerate * Part_Damage)

                if (injury.remainingTime != 0)
                    totalDMG += Mathf.Round((((float)injury.timeLeft / (float)injury.remainingTime)) * injury.partDamage);
                else
                    totalDMG = 999999;

                //totalDMG += injury.partDamage;
            }
            part.HP = part.maxHP - totalDMG;

            if (part.HP <= 0)
                part.HP = 0;
        }

        return bodyPart;
    }

    public BodyParts[] RandomTend(BodyParts[] bp, int numToTend, string name)
    {
        BodyParts[] bodyParts = bp;
        int treated = 0;

        //Loop numToTend
        for (int i = 0; i < numToTend; i++)
        {
            //Get Random Body Part
            int selected = Random.Range(0, bodyParts.Length);

            //Check if it has Injury
            if (bodyParts[selected].injuries.Length > 0)
            {
                //Get Random Injury
                int injurySel = Random.Range(0, bodyParts[selected].injuries.Length);

                //Check if it's not tended
                if (!bodyParts[selected].injuries[injurySel].isTended)
                {
                    //Check if its Cut
                    if (bodyParts[selected].injuries[0].injuryName.Contains(permanentInjuries[0].injuryName))
                    {
                        bodyParts[selected] = PatchedCut(bodyParts[selected]);
                        treated++;
                    }

                    //Treat The Injury
                    else if (!bodyParts[selected].injuries[0].injuryName.Contains(permanentInjuries[1].injuryName))
                    {
                        // Check if its removed ^^^
                        // if it is, it doesent really need tending, so you can just skip it

                        bodyParts[selected].injuries[injurySel] = Tend(bodyParts[selected].injuries[injurySel]);

                        bodyParts[selected] = new BodyParts(bodyParts[selected]);
                        //Debug.Log("Treated: " + bodyParts[selected].injuries[injurySel].injuryName + " in " + bodyParts[selected].partName);

                        treated++;
                    }
                }
            }
        }

        bodyParts = BodyPartInjuryComputation(bodyParts);
        if (treated > 0)
        {
            battleSystem.SetDialogue(name + " treated " + treated + " of their injuries");
            soundManager.PlaySound(treatedSound);
        }
        else
        { 
            battleSystem.SetDialogue(name + " failed to treat their injuries");
            soundManager.PlaySound(failedTreatedSound);
        }

        //Return
        return bodyParts;
    }

    public BodyParts[] TendAll(BodyParts[] bp)
    {
        BodyParts[] bodyParts = bp;

        //Go through all body parts
        for(int j = 0; j < bodyParts.Length; j++)
        {
            for (int i = 0; i < bodyParts[j].injuries.Length; i++)
            { 
                if (bodyParts[j].injuries[0].injuryName.Contains(permanentInjuries[0].injuryName))
                {
                    bodyParts[j] = PatchedCut(bodyParts[i]);
                }

                else if (!bodyParts[j].injuries[0].injuryName.Contains(permanentInjuries[1].injuryName))
                {
                    bodyParts[j].injuries[i] = Tend(bodyParts[j].injuries[i]);
                }
            }
        }

        bodyParts = BodyPartInjuryComputation(bodyParts);
        return bodyParts;
    }

    public BodyParts[] Regenerate(BodyParts[] bp)
    {
        BodyParts[] bodyParts = bp;

        tempInjury = new List<Injury>();
        for (int j = 0; j < bodyParts.Length; j++)
        {
            for (int i = 0 ; i < bodyParts[j].injuries.Length; i++)
            {
                if (bodyParts[j].injuries[i].injuryType != 999)
                {
                    bodyParts[j].injuries[i].timeLeft--;

                    if (bodyParts[j].injuries[i].timeLeft <= 0)
                    {
                        tempInjury = new List<Injury>(bodyParts[j].injuries);
                        tempInjury.Remove(bodyParts[j].injuries[i]);

                        bodyParts[j].injuries = tempInjury.ToArray();
                        bodyParts[j] = new BodyParts(bodyParts[j]);
                    }
                }
            }
        }
        return bodyParts;
    }

    public Injury Tend(Injury I)
    {
        Injury injury = I;

        injury.isTended = true;
        injury.timeLeft /= 2;

        return injury;
    }

    public bool ImportantPartsGone(Unit unit)
    {
        foreach(BodyParts bodypart in unit.bodyParts)
        {
            if (bodypart.deathWhenDestroyed && (bodypart.isDestroyed || bodypart.HP <= 0))
            {
                return true;
            }
        }
        return false;
    }

    public Unit BloodLoos(Unit u)
    {
        Unit unit = u;

        foreach (BodyParts bodyPart in unit.bodyParts)
        {
            foreach (Injury injury in bodyPart.injuries)
            {
                if (!injury.isTended)
                { 
                    unit.currHP -= injury.damagePerTurn;
                }
            }
        }
        return unit;
    }
}
