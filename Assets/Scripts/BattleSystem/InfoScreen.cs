using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Slider HPSlider;
    [SerializeField] Slider willSlider;

    [SerializeField] GameObject body;
    [SerializeField] Text status;
    [SerializeField] Text injuryText;
    [SerializeField] Text armory;

    [SerializeField] GameObject hit;
    [SerializeField] GameObject bloodHit;

    BodyParts[] oldParts;
    public void SetHUD(Unit unit)
    {
        nameText.text = unit.FName;

        nameText.text += " " + unit.LName;

        HPSlider.maxValue = unit.maxHP;

        willSlider.maxValue = unit.maxWill;

        //clear body children
        ClearBody();

        //add child with colors depending on the hp of the body part
        //Spawn Body
        GameObject bodyGO = Instantiate(unit.body, body.transform);
        bodyGO.transform.localPosition = new Vector3(0,-170,0);
        bodyGO.transform.localScale = new Vector3(17.5f, 17.5f, 17.5f);

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

                    //Explode particles if it took damage
                    if (bodyPart.tookDamage)
                    {
                        GameObject _hit = Instantiate(hit, child.transform);
                        //_hit.transform.localPosition = new Vector3(0, 0, -1);

                        _hit.GetComponent<ParticleSystem>().Emit(1);

                        foreach (Injury injury in bodyPart.injuries)
                        {
                            //if will start bleeding
                            if (injury.damagePerTurn >= 1)
                            {
                                //Blood SPLAT
                                GameObject _bloodHit = Instantiate(bloodHit, child.transform);

                                _bloodHit.GetComponent<ParticleSystem>().Emit(20);
                            }
                        }

                        if (bodyPart.isDestroyed || bodyPart.HP <= 0)
                        {
                            //BODY PART GOT DESTORYED
                            _hit.GetComponent<ParticleSystem>().Emit(1);
                        }
                        bodyPart.tookDamage = false;

                    }
                }
            }
        }

        UpdateAllInfo(unit.currHP, unit.currWill, unit);
    }

    public void ClearBody()
    {
        foreach (Transform child in body.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void UpdateAllInfo(float Hp, float Will, Unit unit)
    {
        SetHP(Hp);
        SetWill(Will);
        SetBodyDetails(unit);
        SetArmory(unit);
        SetInjuriesDetail(unit);
    }

    public void SetHP (float HP)
    { HPSlider.value = HP; }

    public void SetWill (float Will)
    { willSlider.value = Will; }


    public void SetBodyDetails(Unit unit)
    {

        //Body Details
        status.text = "Body: ";
        float healthPct = unit.currHP/unit.maxHP;

        if (healthPct == 1)
            status.text += "<color=#00ff00>Healthy</color>";

        else if (healthPct < 1f && healthPct >= 0.8f)
            status.text += "<color=#55ff00>Slightly Injured</color>";

        else if (healthPct < 0.8f && healthPct >= 0.6f)
            status.text += "<color=#aaff00>Moderately Injured</color>";

        else if (healthPct < 0.6f && healthPct >= 0.4f)
            status.text += "<color=#ffff00>Seriously Injured</color>";

        else if (healthPct < 0.4f && healthPct >= 0.2f)
            status.text += "<color=#ffaa00>Severely Injured</color>";

        else if (healthPct < 0.2f && healthPct > 0)
            status.text += "<color=#ff5500>Critically Injured</color>";

        else if (healthPct <= 0)
            status.text += "<color=#ff0000>Dead</color>";

        //Mental Details
        status.text += "\nMental: ";
        float willPct = unit.currWill / unit.maxWill;

        if (willPct == 1)
            status.text += "<color=#00ff00>Normal</color>";

        else if (willPct < 1f && willPct >= 0.8f)
            status.text += "<color=#55ff00>Tense</color>";

        else if (willPct < 0.8f && willPct >= 0.6f)
            status.text += "<color=#aaff00>Uncomfortable</color>";

        else if (willPct < 0.6f && willPct >= 0.4f)
            status.text += "<color=#ffff00>Distracted</color>";

        else if (willPct < 0.4f && willPct >= 0.2f)
            status.text += "<color=#ffaa00>Distressed</color>";

        else if (willPct < 0.2f && willPct > 0)
            status.text += "<color=#ff5500>Severe Stress</color>";

        else if (willPct <= 0)
            status.text += "<color=#ff0000>Broken</color>";
    }

    void SetInjuriesDetail(Unit unit)
    {
        injuryText.text = "";
        //Show Injuries and Body Part HP
        for (int i = 0; i < unit.bodyParts.Length; i++)
        {
            if (unit.bodyParts[i].HP != unit.bodyParts[i].maxHP && !unit.bodyParts[i].isDestroyed)
            {
                Color color = Color.Lerp(Color.red, Color.yellow, unit.bodyParts[i].HP / unit.bodyParts[i].maxHP);
                injuryText.text += "<color=#"+ ColorUtility.ToHtmlStringRGB(color) + ">" + unit.bodyParts[i].partName + "</color>";
                injuryText.text += " (" + unit.bodyParts[i].HP + "/" + unit.bodyParts[i].maxHP + ")";

                Dictionary<string, int> printedInjuries = new Dictionary<string, int>();
                List<string> injuries = new List<string>();

                Dictionary<string, int> printedTreatedInjuries = new Dictionary<string, int>();
                List<string> treatedInjuries = new List<string>();

                for (int j = 0; j < unit.bodyParts[i].injuries.Length; j++)
                {
                    // count how many one injury they have;
                    if (!unit.bodyParts[i].injuries[j].isTended)
                        injuries.Add(unit.bodyParts[i].injuries[j].injuryName);

                    // count how many tended there are;
                    else
                        treatedInjuries.Add(unit.bodyParts[i].injuries[j].injuryName);
                }

                // Put these in a function
                PrintInuries(printedInjuries, injuries, "");
                PrintInuries(printedTreatedInjuries, treatedInjuries, "(Tended)");

                bool isBleeding = false;
                foreach (Injury inju in unit.bodyParts[i].injuries)
                {
                    if (!inju.isTended && inju.damagePerTurn > 1)
                    {
                        isBleeding = true;
                        break;
                    }
                }
                if (isBleeding)
                    injuryText.text += "\n- <color=#ff0000>Bleeding</color>";

                injuryText.text += "\n";
            }
        }
    }

    void PrintInuries(Dictionary<string, int> printedInjuries, List<string> injuries, string additionalString)
    {
        foreach (string injury in injuries)
        {
            if (!printedInjuries.ContainsKey(injury))
                printedInjuries.Add(injury, 1);
            else
            {
                int count = 0;
                printedInjuries.TryGetValue(injury, out count);
                printedInjuries.Remove(injury);
                printedInjuries.Add(injury, count + 1);
            }
        }

        // print them all with a "(xN)" multiplier at once;
        foreach (KeyValuePair<string, int> entry in printedInjuries)
        {
            injuryText.text += "\n" + "- " + entry.Key + " " + additionalString;
            if (entry.Value > 1)
                injuryText.text += " (x" + entry.Value + ")";
        }
    }

    public void SetArmory(Unit unit)
    {
        //armory.text = "Armor:\nNone";
        
        armory.text = "Equipped: ";
        for (int i = 0; i < unit.weapons.Length; i++)
        {
            if (unit.equipedWeapon == i)
            {
                armory.text += unit.weapons[i].name;
                if (unit.weapons[i].usesAmmo)
                {
                    armory.text += " (" + unit.weapons[i].currAmmo + "/" + unit.weapons[i].maxAmmo + ")";
                }
            }
        }
    }
}