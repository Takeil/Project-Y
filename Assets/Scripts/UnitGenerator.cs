using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitGenerator : MonoBehaviour
{
    [SerializeField] GameObject FemaleTemplate;
    [SerializeField] GameObject MaleTemplate;

    [SerializeField] GameObject PlayerCrew;
    [SerializeField] GameObject EnemyCrew;

    [SerializeField] Unit unitTemplate;

    [SerializeField] TextAsset Male, Female, Surname;
    string AllName;
    List<string> Names;

    [SerializeField] Vector2 MinMaxStats;
    [SerializeField] WeaponManager weaponManager;

    [SerializeField] UnitGeneratorManager ugm;
    [SerializeField] UnitGeneratorManager ugm2;

    private System.Random gen = new System.Random();

    public void GenerateLegacy()
    {
        for (int i = 0; i < 2; i++)
        {
            GameObject person;

            //Gender Randomizer
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                //Female
                person = Instantiate(FemaleTemplate, transform);
                AllName = Female.text;
            }
            else
            {
                //Male
                person = Instantiate(MaleTemplate, transform);
                AllName = Male.text;
            }

            Unit unit = person.GetComponent<Unit>();

            unit.CalculateHP();

            //Name Generator
            Names = new List<string>();
            Names.AddRange(AllName.Split("\n"[0]));

            unit.FName = Names[UnityEngine.Random.Range(0, Names.Count)];

            AllName = Surname.text;
            Names = new List<string>();
            Names.AddRange(AllName.Split("\n"[0]));

            unit.LName = Names[UnityEngine.Random.Range(0, Names.Count)];

            person.name = unit.FName + " " + unit.LName;

            //Stats Generator
            unit.stats.DEX = UnityEngine.Random.Range((int)MinMaxStats.x, (int)MinMaxStats.y) + 1;
            unit.stats.ACC = UnityEngine.Random.Range((int)MinMaxStats.x, (int)MinMaxStats.y) + 1;

            //Useless Info Generator
            unit.age = UnityEngine.Random.Range(19, 26);

            unit.birthday = RandomDate();

            int BT = UnityEngine.Random.Range(0, 4) + 1;
            switch (BT)
            {
                case 1:
                    unit.bloodTpye = "A";
                    break;
                case 2:
                    unit.bloodTpye = "B";
                    break;
                case 3:
                    unit.bloodTpye = "AB";
                    break;
                case 4:
                    unit.bloodTpye = "O";
                    break;
            }
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                unit.bloodTpye += "+";
            }
            else
            {
                unit.bloodTpye += "-";
            }

            //Weapon Giver (None to all weapons)
            for (int j = 0; j < unit.weapons.Length; j++)
            {
                unit.weapons[j] = new Weapon(weaponManager.weapons[UnityEngine.Random.Range(0, weaponManager.weapons.Length)]);
            }

            if (PlayerCrew.transform.childCount < 1)
            {
                person.transform.SetParent(PlayerCrew.transform);
            }
            else if (EnemyCrew.transform.childCount < 1)
            {
                person.transform.SetParent(EnemyCrew.transform);
            }
        }
    }

    public void Generate(bool withWeapon)
    {
        if (transform.childCount >= 1)
            Destroy(transform.GetChild(0).gameObject);

        GameObject person;

        //Gender Randomizer
        if (UnityEngine.Random.Range(0, 2) == 1)
        { 
            //Female
            person = Instantiate(FemaleTemplate, transform);
            AllName = Female.text;
        }
        else
        { 
            //Male
            person = Instantiate(MaleTemplate, transform); 
            AllName = Male.text;
        }

        Unit unit = person.GetComponent<Unit>();

        unit.CalculateHP();

        //Name Generator
        Names = new List<string>();
        Names.AddRange(AllName.Split("\n"[0]));

        unit.FName = Names[UnityEngine.Random.Range(0, Names.Count)];

        AllName = Surname.text;
        Names = new List<string>();
        Names.AddRange(AllName.Split("\n"[0]));

        unit.LName = Names[UnityEngine.Random.Range(0, Names.Count)];

        person.name = unit.FName + " " + unit.LName;

        //Portrait Color Generator
        Color darkest = new Color(0, 0, 0), brightest = new Color(0, 0, 0);

        darkest = new Color(0.1f, 0.1f, 0.1f);

        //Hair
        int num = UnityEngine.Random.Range(0, 4);
        switch (num)
        {
            //Black to Red
            case 0:
                brightest = new Color(251f / 255f, 231f / 255f, 161f / 255f);
                break;
            //Black to Blonde
            case 1:
                brightest = new Color(124f / 255f, 10f / 255f, 2f / 255f);
                break;
            //Black to Orange
            case 2:
                brightest = new Color(241f / 255f, 181f / 255f, 130f / 255f);
                break;
            //Black to Random
            case 3:
                brightest = new Color((float)UnityEngine.Random.Range(0, 255)/255f,
                                      (float)UnityEngine.Random.Range(0, 255)/ 255f,
                                      (float)UnityEngine.Random.Range(0, 255)/ 255f);
                break;
        }

        unit.hairColor = Color.Lerp(darkest, brightest, ((float)(UnityEngine.Random.Range(1, 100))/100f));

        //Skin color range
        darkest = new Color(141f / 255f, 85f/255f, 36f / 255f);
        brightest = new Color(255f / 255f, 219f / 255f, 172f / 255f);

        unit.skinColor = Color.Lerp(darkest,brightest, ((float)(UnityEngine.Random.Range(1, 100)) / 100f));

        //Shirt
        darkest = new Color(0, 0, 0);
        brightest = new Color(UnityEngine.Random.Range(0, 2),
                              UnityEngine.Random.Range(0, 2),
                              UnityEngine.Random.Range(0, 2));

        unit.shirtColor = Color.Lerp(darkest, brightest, ((float)(UnityEngine.Random.Range(1, 100)) / 100f));


        //Stats Generator
        unit.stats.DEX = UnityEngine.Random.Range((int)MinMaxStats.x, (int)MinMaxStats.y) + 1;
        unit.stats.ACC = UnityEngine.Random.Range((int)MinMaxStats.x, (int)MinMaxStats.y) + 1;

        //Useless Info Generator
        unit.age = UnityEngine.Random.Range(19,26);

        unit.birthday = RandomDate();

        int BT = UnityEngine.Random.Range(0, 4) + 1;
        switch (BT)
        {
            case 1:
                unit.bloodTpye = "A";
                break;
            case 2:
                unit.bloodTpye = "B";
                break;
            case 3:
                unit.bloodTpye = "AB";
                break;
            case 4:
                unit.bloodTpye = "O";
                break;
        }
        if (UnityEngine.Random.Range(0, 2) == 1)
        {
            unit.bloodTpye += "+";
        }
        else
        {
            unit.bloodTpye += "-";
        }

        //Weapon Giver (None to all weapons)
        for (int j = 0; j < unit.weapons.Length; j++)
        {
            unit.weapons[j] = new Weapon(weaponManager.weapons[0]);
        }

        if (withWeapon)
        {
            unit.weapons[0] = new Weapon(weaponManager.weapons[UnityEngine.Random.Range(1, weaponManager.weapons.Length)]);
            ugm.SetToRewardGeneratedDisplay(unit);
            ugm2.SetToRewardGeneratedDisplay(unit);
        }
        else
        {
            ugm.SetToGeneratedDisplay(unit);
            ugm2.SetToGeneratedDisplay(unit);
        }
    }

    public void ResetPlayerCrew()
    {
        foreach (Transform child in PlayerCrew.transform)
        {
            GameObject GO = child.gameObject;
            GO.name = "Unit";
            GO.GetComponentInChildren<Unit>().SetEverything(unitTemplate);
        }

        Generate(false);
    }

    public void SetPlayerCharacter(int index)
    {
        Unit unit = transform.GetComponentInChildren<Unit>();
        GameObject GO = PlayerCrew.transform.GetChild(index).gameObject;
        GO.name = index + " - " + unit.FName + " " + unit.LName;
        GO.GetComponentInChildren<Unit>().SetEverything(unit);
    }

    DateTime RandomDay()
    {
        DateTime start = new DateTime(1995, 1, 1);
        int range = (DateTime.Today - start).Days;
        return start.AddDays(gen.Next(range));
    }

    string RandomDate()
    {
        DateTime DT = RandomDay();
        return (DT.Day + "/" + DT.Month);
    }

    public void ClearEnemyCrew()
    {
        //clear
        foreach (Transform child in EnemyCrew.transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    [HideInInspector] public int enemies = 0;

    int GenerateDifficulty()
    {
        int highestDiff = 0;
        int lowestDiff = 0;

        int finalVal = 0;
        //Get Values
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();

            if (unit.stats.ACC > highestDiff)
                highestDiff = unit.stats.ACC;
            if (unit.stats.DEX > highestDiff)
                highestDiff = unit.stats.DEX;

            if (unit.stats.ACC < lowestDiff)
                lowestDiff = unit.stats.ACC;
            if (unit.stats.DEX < lowestDiff)
                lowestDiff = unit.stats.DEX;
        }

        // Depending on the difficulty set
        switch(PlayerPrefs.GetInt("Difficulty"))
        {
            case 1:
                //fair
                finalVal = UnityEngine.Random.Range(lowestDiff, highestDiff);
                break;
            case 2:
                //unfairly fair
                finalVal = UnityEngine.Random.Range(highestDiff - 2, highestDiff + 2);
                break;
            case 3:
                //unfair
                finalVal = highestDiff;
                break;
        }

        if (finalVal > 10)
            finalVal = 10;

        if (finalVal < 1)
            finalVal = 1;

        return finalVal;
    }

    public void EnemyCrewGenerator()
    {
        ClearEnemyCrew();
        
        //increase enemies for added difficulty
        enemies++;

        //Max out to 5 enemies
        if (enemies > 5)
        { 
            enemies = UnityEngine.Random.Range(1,6); 
        }

        //generate difficulty according to player units
        GenerateDifficulty();

        for (int i = 0; i < enemies; i++)
        {
            GameObject person;

            //Gender Randomizer
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                //Female
                person = Instantiate(FemaleTemplate, transform);
                AllName = Female.text;
            }
            else
            {
                //Male
                person = Instantiate(MaleTemplate, transform);
                AllName = Male.text;
            }

            Unit unit = person.GetComponent<Unit>();

            unit.CalculateHP();

            //Name Generator
            Names = new List<string>();
            Names.AddRange(AllName.Split("\n"[0]));

            unit.FName = Names[UnityEngine.Random.Range(0, Names.Count)];

            AllName = Surname.text;
            Names = new List<string>();
            Names.AddRange(AllName.Split("\n"[0]));

            unit.LName = Names[UnityEngine.Random.Range(0, Names.Count)];

            person.name = unit.FName + " " + unit.LName;

            //Portrait Color Generator
            Color darkest = new Color(0, 0, 0), brightest = new Color(0, 0, 0);

            darkest = new Color(0.1f, 0.1f, 0.1f);

            //Hair
            int num = UnityEngine.Random.Range(0, 4);
            switch (num)
            {
                //Black to Red
                case 0:
                    brightest = new Color(251f / 255f, 231f / 255f, 161f / 255f);
                    break;
                //Black to Blonde
                case 1:
                    brightest = new Color(124f / 255f, 10f / 255f, 2f / 255f);
                    break;
                //Black to Orange
                case 2:
                    brightest = new Color(241f / 255f, 181f / 255f, 130f / 255f);
                    break;
                //Black to Random
                case 3:
                    brightest = new Color((float)UnityEngine.Random.Range(0, 255) / 255f,
                                          (float)UnityEngine.Random.Range(0, 255) / 255f,
                                          (float)UnityEngine.Random.Range(0, 255) / 255f);
                    break;
            }

            unit.hairColor = Color.Lerp(darkest, brightest, ((float)(UnityEngine.Random.Range(1, 100)) / 100f));

            //Skin color range
            darkest = new Color(141f / 255f, 85f / 255f, 36f / 255f);
            brightest = new Color(255f / 255f, 219f / 255f, 172f / 255f);

            unit.skinColor = Color.Lerp(darkest, brightest, ((float)(UnityEngine.Random.Range(1, 100)) / 100f));

            //Shirt
            darkest = new Color(0, 0, 0);
            brightest = new Color(UnityEngine.Random.Range(0,2), 
                                  UnityEngine.Random.Range(0, 2), 
                                  UnityEngine.Random.Range(0, 2));

            unit.shirtColor = Color.Lerp(darkest, brightest, ((float)(UnityEngine.Random.Range(1, 100)) / 100f));

            //Stats Generator
            unit.stats.DEX = GenerateDifficulty();
            unit.stats.ACC = GenerateDifficulty();

            //Weapon Giver
            for (int j = 0; j < unit.weapons.Length; j++)
            {
                unit.weapons[j] = new Weapon(weaponManager.weapons[UnityEngine.Random.Range(1, weaponManager.weapons.Length)]);
            }

            person.transform.SetParent(EnemyCrew.transform);
        }
    }
}
