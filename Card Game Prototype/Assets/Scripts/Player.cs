using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CustomAudioManager;

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
        set { maxAP = value; }
    }
    [SerializeField] int aP;
    public int AP {
        get { return aP; }
        set { aP = value; }
    }
    [SerializeField] TextMeshProUGUI aPCounter;
    [SerializeField] StatusEffect statusEffect;
    public StatusEffect StatusEffect {
        get { return statusEffect; }
        set { statusEffect = value; }
    }
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
    [SerializeField] CanvasGroup stunIconGgp;
    [SerializeField] CanvasGroup strIconGgp;
    [SerializeField] TextMeshProUGUI strDurTxt;
    [SerializeField] int strDuration;
    [SerializeField] float strDmgMultiplier;
    public float StrDmgMultiplier {
        get { return strDmgMultiplier; }
    }

    [Header("Damage flash related")]
    [SerializeField] Image playerImg;
    [ColorUsage(true, true)]
    [SerializeField] Color flashColor = Color.white;
    [SerializeField] float flashLenght = 0.2f;
    Coroutine dmgFlashRoutine;
    Coroutine dmgShake;

    [Header("Deck related")]
    //[SerializeField] int deckSize; //Not used anywhere atm. Maybe even remove at some point
    //public int DeckSize {
    //    get { return deckSize; }
    //}
    [SerializeField] int drawAmount;
    public int DrawAmount {
        get { return drawAmount; }
        set { drawAmount = value; }
    }

    GameManager gM;
    StatusBar statusBar;

    void Start() {
        UpdateHealthInfo();
        UpdateActionPointCounter();
        gM = FindObjectOfType<GameManager>();
        statusBar = FindObjectOfType<StatusBar>();
    }

    public void TakeDamage(int damage) {
        int remainingDmg = 0;

        //First check if player has block, reduce that  first before health
        switch (block) {
            case > 0:
            //Further check if damage is more than current block, calculate remaining damage.
            if (damage > block) {
                gM.TotalDmgBlocked += block;
                remainingDmg = damage - block;
                ResetBlock();
                //Then use remaining damage for health removal
                health -= remainingDmg;
                AudioManager.PlayDamageSound();
                gM.TotalDmgBlocked += remainingDmg;
            }
            else if (damage == block) {
                ResetBlock();
                AudioManager.PlayBlockDamagedSound();
                gM.TotalDmgBlocked += damage;
            }
            else {
                ReduceBlock(damage);
                AudioManager.PlayBlockDamagedSound();
                gM.TotalDmgBlocked += damage;
            }
            break;

            default:
            health -= damage;
            AudioManager.PlayDamageSound();
            gM.TotalDmgTaken += damage;
            break;
        }
        statusBar.UpdateHealthTxt();
        CallDamageFlash();
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
        statusBar.UpdateHealthTxt();
        gM.TotalHealAmount += amount;
    }

    public void GainBlock(int amount) {
        switch (block) {
            case <= 0:
            StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 0f, 1f));
            break;
        }
        block += amount;
        AudioManager.PlayBlockUpSound();
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
        AudioManager.PlayApRecoverySound();
        aP += amount;
        UpdateActionPointCounter();
        gM.TotalApRecovered += amount;
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
        AudioManager.PlayBuffSound();
    }

    public void GainDebuff(StatusEffect debuff) {
        switch (debuff) {
            case StatusEffect.Stunned:
            statusEffect = StatusEffect.Stunned;
            StartCoroutine(FadeIcon(stunIconGgp, 0.3f, 0f, 1f));
            AudioManager.PlayStunSound();
            break;
        }
    }

    public void ResetDebuff(StatusEffect debuff) {
        switch (debuff) {
            case StatusEffect.Stunned:
            statusEffect = StatusEffect.None;
            StartCoroutine(FadeIcon(stunIconGgp, 0.3f, 1f, 0f));
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

    public void UpdateHealthInfo() {
        if (health > maxHp) health = maxHp;

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

    #region Flash functions
    void CallDamageFlash() {
        dmgFlashRoutine = StartCoroutine(DamageFlasher());
        dmgShake = StartCoroutine(ShakePlayer(0.05f, 0.15f));
    }

    IEnumerator ShakePlayer(float shakeLenght, float shakeStr) {
        Vector2 startPos = transform.position;

        float currentMoveAmount = 0f;
        float elapsedTime = 0f;

        //First shake to left
        while (elapsedTime < shakeLenght) {
            elapsedTime += Time.deltaTime;

            currentMoveAmount = Mathf.Lerp(startPos.x, startPos.x - shakeStr, elapsedTime / (shakeLenght / 3));
            SetMoveAmount(currentMoveAmount);

            yield return null;
        }

        //Second shake to right
        elapsedTime = 0f;
        while (elapsedTime < shakeLenght) {
            elapsedTime += Time.deltaTime;

            currentMoveAmount = Mathf.Lerp(startPos.x - shakeStr, startPos.x + shakeStr, elapsedTime / (shakeLenght / 3));
            SetMoveAmount(currentMoveAmount);

            yield return null;
        }

        //Third shake back starting pos
        elapsedTime = 0f;
        while (elapsedTime < shakeLenght) {
            elapsedTime += Time.deltaTime;

            currentMoveAmount = Mathf.Lerp(startPos.x + shakeStr, startPos.x, elapsedTime / (shakeLenght / 3));
            SetMoveAmount(currentMoveAmount);

            yield return null;
        }
    }

    IEnumerator DamageFlasher() {
        //Set color
        SetFlashColor();

        //Lerp flash amount
        float currentFlashAmount = 0f;
        float elapsedTime = 0f;

        while (elapsedTime < flashLenght) {
            //Iterate elapsed time
            elapsedTime += Time.deltaTime;

            //Lerp the flash amount
            currentFlashAmount = Mathf.Lerp(1f, 0f, elapsedTime / flashLenght);
            SetFlashAmount(currentFlashAmount);

            yield return null;
        }
    }

    void SetFlashColor() {
        playerImg.material.SetColor("_FlashColor", flashColor);
    }

    private void SetFlashAmount(float amount) {
        playerImg.material.SetFloat("_FlashAmount", amount);
    }

    void SetMoveAmount(float amount) {
        transform.position = new Vector2(amount, transform.position.y);
    }
    //void SetMainTex() {
    //    cardImage.material.SetTexture("_MainTex", enemyData.enemyArt.texture);
    //}
    #endregion
}