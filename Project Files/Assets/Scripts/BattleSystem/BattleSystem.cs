using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST, WAIT}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] GameObject playerCrew;
    [SerializeField] GameObject enemyCrew;

    [SerializeField] Transform playerStand;
    [SerializeField] Transform enemyStand;

    [SerializeField] Text dialogueText;

    [SerializeField] InfoScreen playerInfo;
    [SerializeField] InfoScreen enemyInfo;

    [SerializeField] InjuryManager injuryManager;

    [HideInInspector] public Unit playerUnit;
    [HideInInspector] public Unit enemyUnit;

    [SerializeField] Button[] buttons;
    [SerializeField] GameObject[] switchUnitButtons;
    [SerializeField] GameObject[] switchUnitInfos;
    [SerializeField] GameObject enemyInfoInSwitch;

    [SerializeField] GameObject[] switchWeaponButtons;
    [SerializeField] GameObject reloadBtn;
    [SerializeField] GameObject[] weaponInfos;

    [SerializeField] UnityEvent onStartOfBattle;

    [SerializeField] float textSpeed = 2f;

    [SerializeField] UnityEvent onWin;
    [SerializeField] UnityEvent onLose;
    [SerializeField] UnityEvent onSurrender;


    [HideInInspector] public int currPlayer = 0;
    int currEnemy = 0;

    public BattleState state;

    //Remove when done testing
    [SerializeField] UnitGenerator unitGen;

    [SerializeField] Text attackBtnText;
    public WeaponManager weaponManager;

    [SerializeField] SoundManager soundManager;
    [SerializeField] AudioClip swiitchSound;
    [SerializeField] AudioClip defeatedSound;
    [SerializeField] AudioClip switchWeaponSound;
    [SerializeField] AudioClip blandishSound;
    [SerializeField] AudioClip surrenderSound;

    public void StartBattle()
    {
        state = BattleState.START;
        onStartOfBattle.Invoke();
        currEnemy = 0;
        //unitGen.EnemyCrewGenerator();

        toggleControl(false);
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        playerUnit = SpawnUnit(playerCrew, currPlayer, playerStand);
        enemyUnit = SpawnUnit(enemyCrew, currEnemy, enemyStand);

        playerInfo.SetHUD(playerUnit);
        enemyInfo.SetHUD(enemyUnit);

        ChangeUI();

        //Should I add player crew name generator? Maybe...
        SetDialogue(playerCrew.name + " VS " + enemyCrew.name);
        yield return new WaitForSeconds(textSpeed);
        SetDialogue("3");
        yield return new WaitForSeconds(1);
        SetDialogue("2");
        yield return new WaitForSeconds(1);
        SetDialogue("1");
        yield return new WaitForSeconds(1);
        SetDialogue("BEGIN!");
        yield return new WaitForSeconds(textSpeed);

        state = BattleState.START;

        // Add Determiner who gets to go first;
        if (Random.Range(0, 2) == 1)
            PlayerTurn();
        else
            EnemyTurn();
    }

    public void SetDialogue(string str)
    {
        dialogueText.text = str;
    }

    void Bleed()
    {
        playerUnit = injuryManager.BloodLoos(playerUnit);
        enemyUnit = injuryManager.BloodLoos(enemyUnit);
        
        PlayerCrewUnitCheck();

        ChangeUI();

        playerUnit = StatusCheck(playerUnit);
        enemyUnit = StatusCheck(enemyUnit);

        enemyCrew.transform.GetChild(currEnemy).GetComponent<Unit>().SetAll(enemyUnit);
        playerCrew.transform.GetChild(currPlayer).GetComponent<Unit>().SetAll(playerUnit);
    }

    void UpdateAllVal()
    {

        playerUnit = StatusCheck(playerUnit);
        enemyUnit = StatusCheck(enemyUnit);

        playerUnit.bodyParts = injuryManager.Regenerate(playerUnit.bodyParts);
        enemyUnit.bodyParts = injuryManager.Regenerate(enemyUnit.bodyParts);

        enemyCrew.transform.GetChild(currEnemy).GetComponent<Unit>().SetAll(enemyUnit);
        playerCrew.transform.GetChild(currPlayer).GetComponent<Unit>().SetAll(playerUnit);

        Bleed();

        if (playerUnit.isDefeated || !playerUnit.isAlive)
            StartCoroutine(Defeated(playerUnit));

        if (enemyUnit.isDefeated || !enemyUnit.isAlive)
            StartCoroutine(Defeated(enemyUnit));
    }

    Unit SpawnUnit(GameObject crew, int index, Transform stand)
    {
        foreach (Transform child in stand)
            GameObject.Destroy(child.gameObject);

        GameObject GO = Instantiate(crew.transform.GetChild(index).gameObject, stand);

        Unit u = GO.GetComponent<Unit>();
        GameObject portrait = Instantiate(u.portrait, stand);
        u.RecolorPortrait(portrait);

        portrait.transform.localScale = new Vector3(50, 50, 50);

        return GO.GetComponent<Unit>();
    }

    public void RestoreAllGOCharacters()
    {
        playerUnit = SpawnUnit(playerCrew, currPlayer, playerStand);
        enemyUnit = SpawnUnit(enemyCrew, currEnemy, enemyStand);

        playerInfo.SetHUD(playerUnit);
        enemyInfo.SetHUD(enemyUnit);
    }

    public void DeleteAllGOCharacters()
    {
        foreach (Transform child in playerStand)
            GameObject.Destroy(child.gameObject);

        foreach (Transform child in enemyStand)
            GameObject.Destroy(child.gameObject);

        playerInfo.ClearBody();
        enemyInfo.ClearBody();
    }

    public void ClearUnit(Transform stand)
    {
        foreach (Transform child in stand)
            Destroy(child.gameObject);
    }

    Unit Reload(Unit u)
    {
        Unit unit = u;
        if (DexCheck(unit.stats.DEX))
        {
            unit.weapons[unit.equipedWeapon].currAmmo = unit.weapons[unit.equipedWeapon].maxAmmo;
            SetDialogue(unit.FName + " reloaded!");
            weaponManager.PlayReloadSound(playerUnit.weapons[playerUnit.equipedWeapon]);
        }
        else
        {
            SetDialogue(unit.FName + " failed to reload!");
            weaponManager.PlayFailedReloadSound(playerUnit.weapons[playerUnit.equipedWeapon]);
        }
        return unit;
    }

    IEnumerator PlayerAttack()
    {

        if (playerUnit.weapons[playerUnit.equipedWeapon].usesAmmo)
        {
            if (playerUnit.weapons[playerUnit.equipedWeapon].currAmmo > 0)
            {
                //Shoot
                playerUnit.weapons[playerUnit.equipedWeapon].currAmmo -= playerUnit.weapons[playerUnit.equipedWeapon].burst;
                enemyUnit = injuryManager.DealInjury(enemyUnit, playerUnit);
            }
            else
            {
                //Reload
                playerUnit = Reload(playerUnit);
            }
        }
        else
        {
            enemyUnit = injuryManager.DealInjury(enemyUnit, playerUnit);
        }

        toggleControl(false);
        UpdateAllVal();

        yield return new WaitForSeconds(textSpeed);

        if (!playerUnit.isAlive || playerUnit.isDefeated)
            PlayerTurn();
        else
            EnemyTurn();
    }

    IEnumerator PlayerHeal()
    {
        //check if need tending
        bool needsTending = false;
        foreach (BodyParts bodyPart in playerUnit.bodyParts)
        {
            if (bodyPart.injuries.Length >= 1)
            {
                foreach (Injury injury in bodyPart.injuries)
                {
                    if (!injury.isTended)
                    {
                        if (injury.injuryName != "Removed")
                        {
                            needsTending = true;
                            break;
                        }
                    }
                }
                if (needsTending)
                    break;
            }
        }

        if (needsTending)
        {
            playerUnit.bodyParts = injuryManager.RandomTend(playerUnit.bodyParts, 20, playerUnit.FName);

            toggleControl(false);
            UpdateAllVal();

            yield return new WaitForSeconds(textSpeed);

            if (!playerUnit.isAlive || playerUnit.isDefeated)
                PlayerTurn();
            else
                EnemyTurn();
        }

        else
        {
            SetDialogue( playerUnit.FName + " does not need tending");
            toggleControl(false);

            yield return new WaitForSeconds(textSpeed);
            PlayerTurn();
        }
    }

    IEnumerator PlayerReload()
    {
        Reload(playerUnit);

        toggleControl(false);
        UpdateAllVal();

        yield return new WaitForSeconds(textSpeed);

        if (!playerUnit.isAlive || playerUnit.isDefeated)
            PlayerTurn();
        else
            EnemyTurn();
    }

    public void PrePlayerWeaponSwitch()
    {
        //set name
        Text[] text = new Text[2];
        Button button = null;
        string info = null;

        for (int i = 0; i < playerUnit.weapons.Length; i++)
        {
            button = switchWeaponButtons[i].transform.GetComponentInChildren<Button>();
            text = switchWeaponButtons[i].transform.GetComponentsInChildren<Text>();

            text[0].text = "";
            button.interactable = true;

            if (i == playerUnit.equipedWeapon)
            {
                button.interactable = false;
                text[0].text = "(E) ";
            }

            text[0].text += playerUnit.weapons[i].name;

            info = "DAM: " + playerUnit.weapons[i].DAM + "\n" + 
                   "ACC: " + playerUnit.weapons[i].ACC + "\n" +
                   "BURST: " + playerUnit.weapons[i].burst + "\n" +
                   "AMMO: ";

            if (playerUnit.weapons[i].usesAmmo)
            {
                info += "(" + playerUnit.weapons[i].currAmmo + "/" + playerUnit.weapons[i].maxAmmo + ")";
            }
            else
            { 
                info += "NONE"; 
            }

            //text = switchWeaponButtons[i].transform.GetComponentInChildren<Text>();
            text[1].text = info;
        }

        //Set Info Screen
        for (int i = 0; i < weaponInfos.Length; i++)
        {
            button = weaponInfos[i].transform.GetComponentInChildren<Button>();
            text = weaponInfos[i].transform.GetComponentsInChildren<Text>();

            text[0].text = "";
            text[2].text = "";
            button.interactable = true;

            if (i == playerUnit.equipedWeapon)
            {
                button.interactable = false;
                text[2].text = "(EQUIPPED)";
            }

            text[0].text += playerUnit.weapons[i].name;

            info = "DAM: " + playerUnit.weapons[i].DAM + "\n" +
                   "ACC: " + playerUnit.weapons[i].ACC + "\n" +
                   "BURST: " + playerUnit.weapons[i].burst + "\n" +
                   "AMMO: ";

            if (playerUnit.weapons[i].usesAmmo)
            {
                info += "(" + playerUnit.weapons[i].currAmmo + "/" + playerUnit.weapons[i].maxAmmo + ")";
            }
            else
            {
                info += "NONE";
            }

            text[1].text = info;
        }

        if (playerUnit.weapons[playerUnit.equipedWeapon].currAmmo == playerUnit.weapons[playerUnit.equipedWeapon].maxAmmo ||
            playerUnit.weapons[playerUnit.equipedWeapon].isMelee)
        {
            reloadBtn.gameObject.GetComponent<Button>().interactable = false;
        }
        else
        {
            reloadBtn.gameObject.GetComponent<Button>().interactable = true;
        }

        CheckCurrentPlayer();
    }

    IEnumerator PlayerWeaponSwitch(int index)
    {
        playerUnit.equipedWeapon = index;
        SetDialogue(playerUnit.FName + " equipped " + playerUnit.weapons[playerUnit.equipedWeapon].name);
        soundManager.PlaySound(switchWeaponSound);
        
        ChangeUI();
        state = BattleState.WAIT;
        toggleControl(false);

        yield return new WaitForSeconds(textSpeed);
        PlayerTurn();
    }

    IEnumerator PlayerUnitSwitch(int index)
    {
        if (currPlayer == index)
        {
            SetDialogue(playerUnit.FName + " is already in battle!");
            toggleControl(false);

            yield return new WaitForSeconds(textSpeed);
            PlayerTurn();
        }

        else
        {
            currPlayer = index;
            playerUnit = SpawnUnit(playerCrew, currPlayer, playerStand);
            SetDialogue(playerUnit.FName + " stepped in!");
            soundManager.PlaySound(swiitchSound);

            toggleControl(false);
            //UpdateAllVal();
            ChangeUI();

            yield return new WaitForSeconds(textSpeed);

            if (!playerUnit.isAlive || playerUnit.isDefeated)
                PlayerTurn();
            else
                EnemyTurn();
        }
    }

    public void PrePlayerUnitSwitch()
    {
        Text[] text = new Text[2]; // = switchUnitButtons[i].transform.GetChild(i);
        Button button = null;
        Slider slider = null;

        //Show Enemies
        text = enemyInfoInSwitch.transform.GetComponentsInChildren<Text>();
        text[0].text = "ENEMIES:";

        for (int i = 0; i < enemyCrew.transform.childCount; i++)
        {
            Unit unit = enemyCrew.transform.GetChild(i).GetComponent<Unit>();
            text[0].text += "\n";

            if (!unit.isAlive || unit.isDefeated)
                text[0].text += "[X] ";

            else
                text[0].text += "[ ] ";

            text[0].text += unit.FName + " " + unit.LName;
        }

        for (int i = 0; i < playerCrew.transform.childCount; i++)
        {
            //text = switchUnitButtons[i].transform.GetComponentInChildren<Text>();//GetChild(0).GetComponent<Text>();

            text = switchUnitButtons[i].transform.GetComponentsInChildren<Text>();
            button = switchUnitButtons[i].transform.GetComponentInChildren<Button>();
            slider = switchUnitButtons[i].transform.GetComponentInChildren<Slider>();

            text[0].text = "N/A";
            button.interactable = false;
            slider.maxValue = 1;
            slider.value = 0;

            //set text to name
            text[0].text = playerCrew.transform.GetChild(i).GetComponent<Unit>().FName.ToUpper();

            if (!playerCrew.transform.GetChild(i).GetComponent<Unit>().isAlive)//)
            {
                text[0].text += "\n<color=#ff0000>[DEAD]</color>";
            }
            else if (playerCrew.transform.GetChild(i).GetComponent<Unit>().isDefeated)
            {
                text[0].text += "\n<color=#ff0000>[UNSTABLE]</color>";
            }
            else
            {
                if (playerCrew.transform.GetChild(i).GetComponent<Unit>().cantPhysicallyFight)
                { 
                    text[0].text += "\n<color=#ffff00>[INCAPABLE]</color>";
                }
                else if (playerCrew.transform.GetChild(i).GetComponent<Unit>().cantMelee)
                {
                    text[0].text += "\n<color=#ffff00>[CAN'T MELEE]</color>";
                }
                else if (i == currPlayer)
                {
                    text[0].text += "\n[SELECTED]";
                }
                else
                {
                    text[0].text += "\n<color=#00ff00>[ALIVE]</color>";
                }

                button.interactable = true;
                slider.maxValue = playerCrew.transform.GetChild(i).GetComponent<Unit>().maxHP;
                slider.value = playerCrew.transform.GetChild(i).GetComponent<Unit>().currHP;

                //this is too specific.
                Image sliderFill = slider.gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
                sliderFill.color = Color.Lerp(Color.red, Color.green, slider.value / slider.maxValue);
            }

            text[1].text = "ACC: " + playerCrew.transform.GetChild(i).GetComponent<Unit>().stats.ACC + "\n" +
                           "DEX: " + playerCrew.transform.GetChild(i).GetComponent<Unit>().stats.DEX;

            if (currPlayer == i)
                button.interactable = false;
        }

        Slider[] sliders = new Slider[2];
        for (int i = 0; i < playerCrew.transform.childCount; i++)
        {
            //text = switchUnitButtons[i].transform.GetComponentInChildren<Text>();//GetChild(0).GetComponent<Text>();

            text = switchUnitInfos[i].transform.GetComponentsInChildren<Text>();
            button = switchUnitInfos[i].transform.GetComponentInChildren<Button>();
            sliders = switchUnitInfos[i].transform.GetComponentsInChildren<Slider>();

            text[0].text = "N/A";
            button.interactable = false;
            sliders[0].maxValue = 1;
            sliders[0].value = 0;
            sliders[1].maxValue = 1;
            sliders[1].value = 0;

            //set text to name
            text[0].text = playerCrew.transform.GetChild(i).GetComponent<Unit>().FName.ToUpper() + "\n" +
                           playerCrew.transform.GetChild(i).GetComponent<Unit>().LName.ToUpper();

            if (!playerCrew.transform.GetChild(i).GetComponent<Unit>().isAlive)
            {
                text[2].text = "<color=#ff0000>[DEAD]</color>";
            }
            else if (playerCrew.transform.GetChild(i).GetComponent<Unit>().isDefeated)
            {
                text[2].text = "<color=#ff0000>[UNSTABLE]</color>";
            }
            else
            {
                text[2].text = "";
                button.interactable = true;

                if (i == currPlayer)
                {
                    text[2].text = "[IN BATTLE]";
                    button.interactable = false;
                }

                sliders[0].maxValue = playerCrew.transform.GetChild(i).GetComponent<Unit>().maxHP;
                sliders[0].value = playerCrew.transform.GetChild(i).GetComponent<Unit>().currHP;

                sliders[1].maxValue = playerCrew.transform.GetChild(i).GetComponent<Unit>().maxWill;
                sliders[1].value = playerCrew.transform.GetChild(i).GetComponent<Unit>().currWill;

                //this is too specific.
                //HP
                Image sliderFill = sliders[0].gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
                sliderFill.color = Color.Lerp(Color.red, Color.white, sliders[0].value / sliders[0].maxValue);

                //Will
                sliderFill = sliders[1].gameObject.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
                sliderFill.color = Color.Lerp(Color.red, Color.white, sliders[1].value / sliders[1].maxValue);
            }

            text[3].text = "Weapons:\n" + 
                playerCrew.transform.GetChild(i).GetComponent<Unit>().weapons[0].name + "\n" +
                playerCrew.transform.GetChild(i).GetComponent<Unit>().weapons[1].name + "\n" +
                playerCrew.transform.GetChild(i).GetComponent<Unit>().weapons[2].name;

            text[1].text = "ACC: " + playerCrew.transform.GetChild(i).GetComponent<Unit>().stats.ACC + " " +
                           "DEX: " + playerCrew.transform.GetChild(i).GetComponent<Unit>().stats.DEX;
        }

        CheckCurrentPlayer();
    }

    IEnumerator EnemyAttack()
    {
        if (enemyUnit.weapons[enemyUnit.equipedWeapon].usesAmmo)
        {
            if (enemyUnit.weapons[enemyUnit.equipedWeapon].currAmmo > 0)
            {
                //Shoot
                enemyUnit.weapons[enemyUnit.equipedWeapon].currAmmo -= enemyUnit.weapons[enemyUnit.equipedWeapon].burst;
                playerUnit = injuryManager.DealInjury(playerUnit, enemyUnit);
            }
            else
            {
                enemyUnit = Reload(enemyUnit);
            }
        }
        else
        {
            playerUnit = injuryManager.DealInjury(playerUnit, enemyUnit);
        }

        UpdateAllVal();
        yield return new WaitForSeconds(textSpeed);

        if (!enemyUnit.isAlive || enemyUnit.isDefeated)
            StartCoroutine(EnemySwitch());
        else
            PlayerTurn();
    }

    IEnumerator EnemySwitch()
    {
        currEnemy++;
        if (currEnemy >= enemyCrew.transform.childCount)
        {
            currEnemy = 0;
        }

        //Switch Player
        enemyUnit = SpawnUnit(enemyCrew, currEnemy, enemyStand);
        SetDialogue(enemyUnit.FName + " stepped in!");
        soundManager.PlaySound(swiitchSound);

        //UpdateAllVal();
        ChangeUI();

        yield return new WaitForSeconds(textSpeed);

        PlayerTurn();
    }

    IEnumerator EnemyHeal()
    {
        enemyUnit.bodyParts = injuryManager.RandomTend(enemyUnit.bodyParts, 20, enemyUnit.FName);

        toggleControl(false);
        UpdateAllVal();

        yield return new WaitForSeconds(textSpeed);

        if (!enemyUnit.isAlive || enemyUnit.isDefeated)
            StartCoroutine(EnemySwitch());
        else
            PlayerTurn();
    }

    void PlayerCrewUnitCheck()
    {
        for (int i = 0; i < playerCrew.transform.childCount; i++)
        {
            Unit unit = playerCrew.transform.GetChild(i).GetComponent<Unit>();
            unit = StatusCheck(unit);

            playerCrew.transform.GetChild(i).GetComponent<Unit>().SetAll(unit);
        }
    }

    Unit StatusCheck(Unit u)
    {
        Unit unit = u;
        if (unit.isAlive || !unit.isDefeated)
        {
            if (unit.currHP <= 0)
            {
                unit.currHP = 0;
                unit.isAlive = false;
            }
            else if (unit.currWill <= 0)
            {
                unit.currWill = 0;
                unit.isDefeated = true;
            }
        }

        int used = 0, usedDest = 0;
        foreach(BodyParts bodyParts in unit.bodyParts)
        {
            if (bodyParts.usedForAttacking)
                used++;
            if ((bodyParts.isDestroyed && bodyParts.usedForAttacking) || ( bodyParts.HP <= 0 && bodyParts.usedForAttacking))
                usedDest++;
        }

        if (used == usedDest)
            unit.cantPhysicallyFight = true;

        used = 0; usedDest = 0;
        foreach (BodyParts bodyParts in unit.bodyParts)
        {
            if (bodyParts.usedForMovement)
                used++;
            if ((bodyParts.isDestroyed && bodyParts.usedForMovement) || (bodyParts.HP <= 0 && bodyParts.usedForMovement))
                usedDest++;
        }

        if (used == usedDest)
            unit.cantMelee = true;

        return unit;
    }

    IEnumerator Defeated(Unit unit)
    {
        SetDialogue(unit.FName + " is defeated!");
        soundManager.PlaySound(defeatedSound);
        yield return new WaitForSeconds(textSpeed);
        
        if (!CrewFighting(playerCrew))
        {
            Lose();
            StopAllCoroutines();
        }
        else if (!CrewFighting(enemyCrew))
        {
            Win();
            StopAllCoroutines();
        }
    }

    bool CrewFighting(GameObject GO)
    {
        float defeated = 0;
        for (int i = 0; i < GO.transform.childCount; i++)
        {
            if (GO.transform.GetChild(i).GetComponent<Unit>().isDefeated ||
               !GO.transform.GetChild(i).GetComponent<Unit>().isAlive)
            {
                defeated++;
            }
        }
        if (defeated >= GO.transform.childCount)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool DexCheck(float val)
    {
        int i = Random.Range(0, 11);
        if (val >= i)
        { 
           return true;
        }
        else
        { 
            return false;
        }
    }

    public void toggleControl(bool enable)
    {
        //Stop player input;
        foreach (Button button in buttons)
            button.interactable = enable;
    }

    IEnumerator EnemyReason()
    {
        SetDialogue(enemyUnit.FName + ": \"Please... I don't want to die...\"");
        yield return new WaitForSeconds(textSpeed);
        PlayerTurn();
    }

    IEnumerator SkipTurn()
    {
        toggleControl(false);
        SetDialogue("Skipped Turn");
        UpdateAllVal();
        yield return new WaitForSeconds(textSpeed);

        if (!playerUnit.isAlive || playerUnit.isDefeated)
            PlayerTurn();
        else
            EnemyTurn();
    }

    void PlayerTurn()
    {
        //Bleed();
        dialogueText.text = "Choose an action:";

        state = BattleState.PLAYERTURN;
        toggleControl(true);
        CheckCurrentPlayer();
    }

    void EnemyTurn()
    {
        state = BattleState.ENEMYTURN;

        float bleedingLvl = 0;
        foreach (BodyParts bodyPart in enemyUnit.bodyParts)
        {
            foreach (Injury injury in bodyPart.injuries)
            {
                if (!injury.isTended)
                    bleedingLvl += injury.damagePerTurn;
            }
        }

        if (!enemyUnit.isAlive || enemyUnit.isDefeated)
            StartCoroutine(EnemySwitch());

        else if ((enemyUnit.cantPhysicallyFight || (enemyUnit.cantMelee && enemyUnit.weapons[enemyUnit.equipedWeapon].isMelee)))
        {
            if (!enemyUnit.reasoned)
            {
                StartCoroutine(EnemyReason());
                enemyUnit.reasoned = true;
            }
            else
            {
                enemyUnit.isDefeated = true;
                StartCoroutine(EnemySwitch());
            }
        }

        else if (bleedingLvl >= Random.Range(25, 100))
            StartCoroutine(EnemyHeal());

        else
            StartCoroutine(EnemyAttack());
    }

    void ChangeUI()
    {
        // Change UI
        attackBtnText.text = playerUnit.weapons[playerUnit.equipedWeapon].attackName.ToUpper();
        if (playerUnit.weapons[playerUnit.equipedWeapon].usesAmmo)
        {
            if (playerUnit.weapons[playerUnit.equipedWeapon].currAmmo <= 0)
                attackBtnText.text = "RELOAD";
        }

        playerInfo.SetHUD(playerUnit);
        enemyInfo.SetHUD(enemyUnit);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //if (CheckCurrentPlayer())
            StartCoroutine(PlayerAttack());
    }

    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //if (CheckCurrentPlayer())
            StartCoroutine(PlayerHeal());
    }

    public void OnSwitchWeapon(int index)
    {
        if (state != BattleState.PLAYERTURN)
            return;

        //if (CheckCurrentPlayer())
            StartCoroutine(PlayerWeaponSwitch(index));
    }

    public void OnSwitchUnit(int index)
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerUnitSwitch(index));
    }

    public void OnReload()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(PlayerReload());
    }

    public void OnSkipButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;

        StartCoroutine(SkipTurn());
    }

    void CheckCurrentPlayer()
    {
        //bool F = false;
        if (!playerUnit.isAlive)
        {
            toggleControl(false);
            buttons[4].interactable = true;
            buttons[7].interactable = true;

        }

        else if (playerUnit.isDefeated)
        {
            toggleControl(false);
            buttons[4].interactable = true;
            buttons[7].interactable = true;
        }

        else if (playerUnit.cantPhysicallyFight)
        {
            toggleControl(false);
            buttons[3].interactable = true;
            buttons[4].interactable = true;
            buttons[5].interactable = true;
            buttons[6].interactable = true;
            buttons[7].interactable = true;
        }
        else if (playerUnit.cantMelee)
        {
            toggleControl(true);
            if (playerUnit.weapons[playerUnit.equipedWeapon].isMelee)
                buttons[0].interactable = false;
        }

        CheckCurrentEnemy();
    }

    void CheckCurrentEnemy()
    {
        if (!enemyUnit.isAlive || enemyUnit.isDefeated)
        {
            buttons[3].interactable = false;
            buttons[5].interactable = false;
        }
    }

    public void Surrender()
    {
        soundManager.PlaySound(surrenderSound);
        toggleControl(false);
        DeleteAllGOCharacters();
        onSurrender.Invoke();
    }

    public void Reason()
    {
        soundManager.PlaySound(blandishSound);
        StartCoroutine(reason());
    }

    IEnumerator reason()
    {
        toggleControl(false);

        //if enemy agrees
        if (1 == Random.Range(0, 1000))
        {
            SetDialogue(playerUnit.FName + ": \"Can you back down?\"");
            yield return new WaitForSeconds(textSpeed);
            SetDialogue(enemyUnit.FName + ": \"Okay... Sure...\"");
            yield return new WaitForSeconds(textSpeed);
            Win();
        }

        //if not
        else
        {
        SetDialogue(playerUnit.FName + ": \"Can you back down?\"");
        yield return new WaitForSeconds(textSpeed);
        SetDialogue(enemyUnit.FName + ": \"No...\"");
        yield return new WaitForSeconds(textSpeed);
        EnemyTurn();
        }
    }

    void Win()
    {
        SetDialogue("YOU WON");
        toggleControl(false);
        DeleteAllGOCharacters();
        unitGen.ClearEnemyCrew();
        onWin.Invoke();
    }

    public void Lose()
    {
        SetDialogue("GAME OVER");
        toggleControl(false);
        DeleteAllGOCharacters();
        unitGen.ClearEnemyCrew();
        unitGen.enemies = 0;
        onLose.Invoke();
    }
}
