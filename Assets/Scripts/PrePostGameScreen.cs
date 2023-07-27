using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrePostGameScreen : MonoBehaviour
{
    [SerializeField] GameObject InfoScreen;
    [SerializeField] GameObject General;
    [SerializeField] GameObject PlayerCrew;
    [SerializeField] InjuryManager injuryManager;

    [SerializeField] Button[] selectUnit;
    [SerializeField] Button continueBtn;
    [SerializeField] BattleSystem battleSystem;

    public void changeUnit(int index)
    {
        battleSystem.currPlayer = index;
        CheckUnits();
    }

    void CheckUnits()
    {
        Unit[] unit = PlayerCrew.transform.GetComponentsInChildren<Unit>();
        int dead = 0;

        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            Text text = selectUnit[i].GetComponentInChildren<Text>();

            //Check if bodyparts are gone and is now dead;
            if (!unit[i].isAlive || unit[i].currHP <= 0 || injuryManager.ImportantPartsGone(unit[i]))
            {
                selectUnit[i].interactable = false;
                unit[i].isAlive = false;
                text.text = "[DEAD]";
                dead++;
            }
            else if (i == battleSystem.currPlayer)
            {
                selectUnit[i].interactable = false;
                text.text = "SELECTED";
            }
            else
            {
                selectUnit[i].interactable = true;
                text.text = "SELECT";
            }
            unit[i].SetAll(unit[i]);
        }

        if (unit[battleSystem.currPlayer].isAlive)
            continueBtn.interactable = true;
        else
            continueBtn.interactable = false;

        if (dead == PlayerCrew.transform.childCount)
        {
            battleSystem.Lose();
        }
    }

    public void AddStats()
    {
        CheckUnits();
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();
            
            if (Random.Range(0,2) == 1)
                unit.stats.ACC += Random.Range(1, 3);

            else
                unit.stats.DEX += Random.Range(1, 3);

            if (unit.stats.ACC > 10)
                unit.stats.ACC = 10;
            if (unit.stats.DEX > 10)
                unit.stats.DEX = 10;
        }
    }

    public void SetScreen()
    {
        CheckUnits();
        for (int i = 0; i < PlayerCrew.transform.childCount; i++)
        {
            Unit unit = PlayerCrew.transform.GetChild(i).GetComponent<Unit>();

            //ask if its still alive
            if (unit.isAlive && unit.currHP > 0)
            {
                //RESET VARIABLES
                unit.bodyParts = injuryManager.TendAll(unit.bodyParts);

                unit.isDefeated = false;
                unit.currWill = 100;

                unit.CalculateHP();
            }

            // INFO SCREEN
            foreach (Transform child in InfoScreen.transform.GetChild(i).transform.GetChild(0).transform)
                    Destroy(child.gameObject);

            Text[] text = InfoScreen.transform.GetChild(i).GetComponentsInChildren<Text>();
            Image image = InfoScreen.transform.GetChild(i).transform.GetChild(0).GetComponent<Image>();

            if (unit.FName != "-----")
            {
                GameObject portrait = Instantiate(unit.portrait, InfoScreen.transform.GetChild(i).transform.GetChild(0).transform);

                unit.RecolorPortrait(portrait);

                portrait.transform.localScale = new Vector3(20, 20, 20);
                portrait.transform.localPosition = new Vector3(0, -57, 0);

                text[0].text = unit.FName + " " + unit.LName;

                text[1].text = "Accuracy: " + unit.stats.ACC + "\n" +
                                "Dexterity: " + unit.stats.DEX;

                text[2].text = "Weapons:";

                for (int j = 0; j < unit.weapons.Length; j++)
                {
                    if (unit.weapons[j].name != "None")
                        text[2].text += " " + unit.weapons[j].name + ";";
                }
            }

            //LOWER PART
            text = General.transform.GetChild(i).GetComponentsInChildren<Text>();

            text[0].text = unit.FName;

            if (!unit.isAlive)
            {
                text[1].text = "<color=#ff0000>[DEAD]</color>";
                image.color = new Color(150f / 255f, 0, 0);
            }
            else if (unit.cantPhysicallyFight)
            {
                text[1].text = "<color=#ffff00>[INCAPABLE]</color>";
                image.color = new Color(150f / 255f, 150f / 255f, 0);
            }
            else if (unit.cantMelee)
            {
                text[1].text = "<color=#ffff00>[CAN'T MELEE]</color>";
                image.color = new Color(150f / 255f, 150f / 255f, 0);
            }
            else
            {
                text[1].text = "<color=#00ff00>[ALIVE]</color>";
                image.color = new Color(0, 150f / 255f, 0);
            }

            foreach (Transform child in General.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }

            GameObject bodyGO = Instantiate(unit.body, General.transform.GetChild(i).transform.GetChild(0).transform.GetChild(0));
            bodyGO.transform.localPosition = new Vector3(0, -50, 0);
            bodyGO.transform.localScale = new Vector3(8, 8, 8);
            foreach (Transform child in bodyGO.transform)
            {
                //Find same name
                foreach (BodyParts bodyPart in unit.bodyParts)
                {
                    //Debug.Log(bodyPart.partName + " " + child.name);
                    if (bodyPart.partName.Equals(child.name))
                    {
                        child.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.red, Color.white, bodyPart.HP / bodyPart.maxHP);
                        if (bodyPart.HP <= 0 || bodyPart.isDestroyed)
                            child.GetComponent<SpriteRenderer>().color = Color.clear;
                    }
                }
            }
        }
    }
}
