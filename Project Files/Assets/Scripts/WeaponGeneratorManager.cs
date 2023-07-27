using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponGeneratorManager : MonoBehaviour
{
    [SerializeField] WeaponManager weaponManager;
    [SerializeField] Button[] addWeaponButton;
    [SerializeField] Button generateButton;
    [SerializeField] GameObject weaponGenerated;
    [SerializeField] GameObject playerCrew;

    [SerializeField] GameObject infoScreen;
    public int generationsLeft = 0;
    int totalGensLeft;

    public void SetGenerations(int total)
    {
        generationsLeft = total;
        totalGensLeft = total; 
        
        foreach (Button btns in addWeaponButton)
        {
            btns.interactable = true;
        }

        UnitWeaponCheck();
        SetWeaponsDisplay();
        GenerateWeapon(); 
    }

    Weapon weapon;

    public void GenerateWeapon()
    {
        weapon = weaponManager.weaponGenerator();
        SetGeneratedWeapon(weapon);
        UnitWeaponCheck();
    }

    public void ContinueCheck()
    {
        generateButton.interactable = true;
        if (generationsLeft <= 0)
        { 
            generateButton.interactable = false; 
            foreach(Button btns in addWeaponButton)
            {
                btns.interactable = false;
            }
        }
    }

    void SetGeneratedWeapon(Weapon weapon)
    {
        for (int i = 0; i < infoScreen.transform.childCount; i++)
        {
            Unit unit = playerCrew.transform.GetChild(i).gameObject.GetComponent<Unit>();
            Text[] text = weaponGenerated.GetComponentsInChildren<Text>();

            text[0].text = "WEAPONS LEFT TO ASSIGN\n" + generationsLeft + "/" + totalGensLeft;

            text[1].text = weapon.name;
        }
    }

    void UnitWeaponCheck()
    {
        for (int i = 0; i < playerCrew.transform.childCount; i++)
        {
            addWeaponButton[i].interactable = false;
            Unit unit = playerCrew.transform.GetChild(i).gameObject.GetComponent<Unit>();
            addWeaponButton[i].gameObject.GetComponentInChildren<Text>().text = "<b>Give to\n" + unit.FName+"</b>";

            if (unit.isAlive)
            {
                for (int j = 0; j < unit.weapons.Length; j++)
                {
                    if (unit.weapons[j].name == "None")
                    {
                        addWeaponButton[i].interactable = true;
                    }
                }
            }
        }
        ContinueCheck();
    }

    public void SetWeapon(int index)
    {
        Unit unit = playerCrew.transform.GetChild(index).gameObject.GetComponent<Unit>();

        for (int i = 0; i < unit.weapons.Length; i++)
        {
            if (unit.weapons[i].name == "None")
            {
                unit.weapons[i] = weapon; 
                generationsLeft--;
                break;
            }
        }

        SetWeaponsDisplay();
        GenerateWeapon();
    }

    void SetWeaponsDisplay()
    {
        for (int i = 0; i < infoScreen.transform.childCount; i++)
        {
            Unit unit = playerCrew.transform.GetChild(i).gameObject.GetComponent<Unit>();
            Text[] text = infoScreen.transform.GetChild(i).GetComponentsInChildren<Text>();

            text[0].text = unit.FName + " " + unit.LName + " | A: " + unit.stats.ACC + " D: " + unit.stats.DEX;

            text[1].text = unit.weapons[0].name;
            text[2].text = unit.weapons[1].name;
            text[3].text = unit.weapons[2].name;

            for (int j = 0; j < unit.weapons.Length; j++)
            {
                if (unit.weapons[j].name == "None")
                {
                    infoScreen.transform.GetChild(i).transform.GetChild(j+2).GetComponent<Image>().color = new Color(0, 0, 0);
                }
                else
                {
                    infoScreen.transform.GetChild(i).transform.GetChild(j + 2).GetComponent<Image>().color = new Color(.2f, .2f, .2f);
                }
            }

        }
    }
}
