using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusEffect { None, Stunned, Dazed }
public enum BuffEffect { None, Strenght, Dodge }
public class Player : MonoBehaviour {
    [Header("Player stats")]
    [SerializeField] int health;
    public int Health {
        get { return health; }
        set { health = value; }
    }
    [SerializeField] int maxHp;
    public int MaxHp {
        get { return maxHp; }
        set { maxHp = value; }
    }
    [SerializeField] int block;
    public int Block {
        get { return block; }
    }
    [SerializeField] int maxAP;
    public int MaxAP {
        get { return maxAP; }
    }
    [SerializeField] int aP;
    public int AP {
        get { return aP; }
        set { aP = value; }
    }
    [SerializeField] TextMeshProUGUI aPCounter;
    [SerializeField] StatusEffect statusEffect;
    [SerializeField] BuffEffect[] buffs;
    public BuffEffect[] Buffs {
        get { return buffs; }
    }

    [Header("Health & block related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;
    [SerializeField] Image blockImg;
    [SerializeField] TextMeshProUGUI blockTxt;
    [SerializeField] CanvasGroup blockIconGrp;

    [Header("Buff & debuff related")]
    [SerializeField] CanvasGroup strIconGgp;
    [SerializeField] TextMeshProUGUI strDurTxt;
    [SerializeField] int strDuration;
    [SerializeField] float strDmgMultiplier;
    public float StrDmgMultiplier {
        get { return strDmgMultiplier; }
    }

    [Header("Deck related")]
    [SerializeField] int deckSize; //Not used anywhere atm. Maybe even remove at some point
    public int DeckSize {
        get { return deckSize; }
    }
    [SerializeField] int drawAmount;
    public int DrawAmount {
        get { return drawAmount; }
        set { drawAmount = value; }
    }

    GameManager gM;

    void Start() {
        UpdateHealthInfo();
        UpdateActionPointCounter();
        gM = FindObjectOfType<GameManager>();
    }

    public void TakeDamage(int damage) {
        int remainingDmg = 0;

        //First check if player has block, reduce that  first before health
        switch (block) {
            case > 0:
            //Further check if damage is more than current block, calculate remaining damage.
            if (damage > block) {
                remainingDmg = damage - block;
                ResetBlock();
                //Then use remaining damage for health removal
                health -= remainingDmg;
            }
            else if (damage == block) {
                ResetBlock();
            }
            else ReduceBlock(damage);
            break;

            default:
            health -= damage;
            break;
        }
        StartCoroutine(AnimateHealthBarDmg(30f));
    }

    public void TakeHeal(int amount) {
        health += amount;
        switch (gameObject.activeInHierarchy) {
            case true:
            StartCoroutine(AnimateHealthBarHealth(30f));
            break;

            default:
            Debug.Log("Player object not active. Skipping health bar animation");
            break;
        }
    }

    public void GainBlock(int amount) {
        switch (block) {
            case <= 0:
            StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 0f, 1f));
            break;
        }
        block += amount;
        UpdateBlockCounter();
    }

    void ReduceBlock(int amount) {
        block -= amount;
        UpdateBlockCounter();
    }

    IEnumerator FadeIcon(CanvasGroup group, float duration, float startValue, float endValue) {
        float timer = 0f;

        while (timer < duration) {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            yield return group.alpha = Mathf.Lerp(startValue, endValue, t);
        }
        group.alpha = endValue;
    }

    public void ResetBlock() {
        StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 1f, 0f));
        block = 0;
        UpdateBlockCounter();
    }

    public void ReduceAP(int actionCost) {
        aP -= actionCost;
        UpdateActionPointCounter();
    }

    public void RecoverAP(int amount) {
        aP += amount;
        UpdateActionPointCounter();
    }

    public void ResetAP() {
        aP = maxAP;
        UpdateActionPointCounter();
    }

    public void ResetBuffs() {
        for (int i = 0; i < buffs.Length; i++) {
            buffs[i] = BuffEffect.None;
        }
        strDuration = 0;
        strDurTxt.text = strDuration.ToString();
        StartCoroutine(FadeIcon(strIconGgp, 0.3f, 1f, 0f));

        //Add rest of the buff resets later...
    }

    public void GainBuff(BuffType buffType, int duration) {
        switch (buffType) {
            case BuffType.Strenght:
            if (strDuration == 0) StartCoroutine(FadeIcon(strIconGgp, 0.3f, 0f, 1f));
            buffs[0] = BuffEffect.Strenght;
            strDuration += duration;
            strDurTxt.text = strDuration.ToString();
            break;
        }
    }

    public void UpdateBuffDuration(BuffEffect buffEffect) {
        switch (buffEffect) {
            case BuffEffect.Strenght:
            strDuration -= 1;
            strDurTxt.text = strDuration.ToString();
            if (strDuration == 0) {
                StartCoroutine(FadeIcon(strIconGgp, 0.3f, 1f, 0f));
                buffs[0] = BuffEffect.None;
            }
            break;
        }
    }

    void UpdateActionPointCounter() {
        aPCounter.text = $"{aP}/{maxAP}";
    }

    void UpdateBlockCounter() {
        blockTxt.text = block.ToString();
    }

    void UpdateHealthInfo() {
        //Health bar values
        healthBar.maxValue = maxHp;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();
    }

    public void Die() { //Later for juicying add death animation or like different sprite
        gM.StartCoroutine(gM.GameOver());
    }

    IEnumerator AnimateHealthBarDmg(float animSpeed) {
        if (health <= 0) health = 0;

        while (healthBar.value > health) {
            //yield return new WaitForSeconds(Time.deltaTime);
            healthBar.value -= animSpeed * Time.deltaTime;
            healtBarNumber.text = Mathf.Round(healthBar.value).ToString();
            yield return null;
        }
        //Make sure value is same as the health
        healthBar.value = health;
        Debug.Log($"{gameObject.name} health bar anim ended");
    }

    IEnumerator AnimateHealthBarHealth(float animSpeed) {
        if (health >= maxHp) health = maxHp;

        while (healthBar.value < health) {
            healthBar.value += animSpeed * Time.deltaTime;
            healtBarNumber.text = Mathf.Round(healthBar.value).ToString();
            yield return null;
        }
        //Make sure value is same as the health
        healthBar.value = health;
        Debug.Log($"{gameObject.name} health bar anim ended");
    }
}
