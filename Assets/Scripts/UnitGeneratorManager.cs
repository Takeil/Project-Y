using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitGeneratorManager : MonoBehaviour
{
    [SerializeField] GameObject GeneratedInfoDisplay;
    [SerializeField] GameObject PlayerCrew;
    [SerializeField] GameObject InfoScreen;
    [SerializeField] Button continueBtn;

    public void SetToGeneratedDisplay(Unit unit)
    {
        foreach (Transform child in GeneratedInfoDisplay.transform.GetChild(0).transform)
            Destroy(child.gameObject);

        Text[] text = null;
        string gender = "";
        if (unit.gender == 0)
            gender = "Male";
        else
            gender = "Female";

        GameObject portrait = Instantiate(unit.portrait, GeneratedInfoDisplay.transform.GetChild(0).transform);
        unit.RecolorPortrait(portrait);

        portrait.transform.localScale = new Vector3(20, 20, 20);
        portrait.transform.localPosition = new Vector3(0, -57, 0);

        text = GeneratedInfoDisplay.GetComponentsInChildren<Text>();

        text[0].text = unit.FName + " " + unit.LName;

        text[1].text = "Accuracy: " + unit.stats.ACC + "\n" +
                       "Dexterity: " + unit.stats.DEX;

        text[2].text = "Gender: " + gender + "\n" +
                       "Age: " + unit.age;

        text[3].text = "Birthday: " + unit.birthday + "\n" +
                       "Blood Type: " + unit.bloodTpye;
    }
    public void SetToRewardGeneratedDisplay(Unit unit)
    {
        foreach (Transform child in GeneratedInfoDisplay.transform.GetChild(0).transform)
            Destroy(child.gameObject);

        Text[] text = null;

        GameObject portrait = Instantiate(unit.portrait, GeneratedInfoDisplay.transform.GetChild(0).transform);
        unit.RecolorPortrait(portrait);

        portrait.transform.localScale = new Vector3(20, 20, 20);
        portrait.transform.localPosition = new Vector3(0, -57, 0);

        text = GeneratedInfoDisplay.GetComponentsInChildren<Text>();

        text[0].text = unit.FName + " " + unit.LName;

        text[1].text = "Accuracy: " + unit.stats.ACC + "\n" +
                       "Dexterity: " + unit.stats.DEX;

        text[2].text = "Status:\n";
        if (!unit.isAlive)
        {
            text[2].text += "<color=#ff0000>[DEAD]</color>";
        }
        else if (unit.cantPhysicallyFight)
        {
            text[2].text += "<color=#ffff00>[INCAPABLE]</color>";
        }
        else if (unit.cantMelee)
        {
            text[2].text += "<color=#ffff00>[CAN'T MELEE]</color>";
        }
        else
        {
            text[2].text += "<color=#00ff00>[ALIVE]</color>";
        }

        text[3].text = "Weapons:\n";

        for (int j = 0; j < unit.weapons.Length; j++)
        {
            if (unit.weapons[j].name != "None")
                text[3].text += unit.weapons[j].name + "; ";
        }
    }

    void ToggleContinue()
    {
        bool setTrue = true;
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();
            if (unit.FName == "-----")
            {
                setTrue = false;
                break;
            }
        }
        continueBtn.interactable = setTrue;
    }
    
    public void SetRewardScreens()
    {
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            foreach (Transform child in InfoScreen.transform.GetChild(i).transform.GetChild(0).transform)
                Destroy(child.gameObject);

            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();
            Text[] text = InfoScreen.transform.GetChild(i).GetComponentsInChildren<Text>(); ;

            if (unit.FName != "-----")
            {
                GameObject portrait = Instantiate(unit.portrait, InfoScreen.transform.GetChild(i).transform.GetChild(0).transform);
                unit.RecolorPortrait(portrait);

                portrait.transform.localScale = new Vector3(20, 20, 20);
                portrait.transform.localPosition = new Vector3(0, -57, 0);

                text[0].text = unit.FName + " " + unit.LName;

                text[1].text = "Accuracy: " + unit.stats.ACC + "\n" +
                               "Dexterity: " + unit.stats.DEX;

                text[2].text = "Status:\n";
                if (!unit.isAlive)
                {
                    text[2].text += "<color=#ff0000>[DEAD]</color>";
                }
                else if (unit.cantPhysicallyFight)
                {
                    text[2].text += "<color=#ffff00>[INCAPABLE]</color>";
                }
                else if (unit.cantMelee)
                {
                    text[2].text += "<color=#ffff00>[CAN'T MELEE]</color>";
                }
                else
                {
                    text[2].text += "<color=#00ff00>[ALIVE]</color>";
                }

                text[3].text = "Weapons:\n";

                for (int j = 0; j < unit.weapons.Length; j++)
                {
                    if (unit.weapons[j].name != "None")
                        text[3].text += unit.weapons[j].name + "; ";
                }
            }
            else
            {
                text[0].text = unit.FName; text[1].text = unit.FName; text[2].text = unit.FName; text[3].text = unit.FName;
            }
        }
        ToggleContinue();
    }
    
    public void SetAllScreens()
    {
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            foreach (Transform child in InfoScreen.transform.GetChild(i).transform.GetChild(0).transform)
                Destroy(child.gameObject);

            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();
            Text[] text = InfoScreen.transform.GetChild(i).GetComponentsInChildren<Text>(); ;

            if (unit.FName != "-----")
            {
                string gender = "";
                if (unit.gender == 0)
                    gender = "Male";
                else
                    gender = "Female";

                GameObject portrait = Instantiate(unit.portrait, InfoScreen.transform.GetChild(i).transform.GetChild(0).transform);
                unit.RecolorPortrait(portrait);

                portrait.transform.localScale = new Vector3(20, 20, 20);
                portrait.transform.localPosition = new Vector3(0, -57, 0);

                text[0].text = unit.FName + " " + unit.LName;

                text[1].text = "Accuracy: " + unit.stats.ACC + "\n" +
                               "Dexterity: " + unit.stats.DEX;

                text[2].text = "Gender: " + gender + "\n" +
                               "Age: " + unit.age;

                text[3].text = "Birthday: " + unit.birthday + "\n" +
                               "Blood Type: " + unit.bloodTpye;
            }
            else
            {
                text[0].text = unit.FName; text[1].text = unit.FName; text[2].text = unit.FName; text[3].text = unit.FName;
            }
        }
        ToggleContinue();
    }
}