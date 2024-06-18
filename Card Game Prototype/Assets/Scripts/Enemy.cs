using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour/*, IPointerDownHandler*/ {
    [SerializeField] DataEnemy enemyData;
    public DataEnemy EnemyData {
        get { return enemyData; }
    }
    [Header("Enemy base info. Updates from DataEnemy")]
    [SerializeField] string enemyName;
    [SerializeField] int health;
    [SerializeField] Image cardImage;

    [Space(10f)]
    [Header("Other infos")]
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] StatusEffect statusEffect;
    [SerializeField] int lastActionRange;

    [Header("Health bar & block related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;
    [SerializeField] Image blockImg;
    [SerializeField] TextMeshProUGUI blockTxt;
    [SerializeField] CanvasGroup blockIconGrp;
    [SerializeField] int block;
    public int Block {
        get { return block; }
    }

    [Header("Indicator related")]
    [SerializeField] TextMeshProUGUI valueTxt;
    [SerializeField] Sprite[] iconSprites;
    [SerializeField] Image iconImg;
    [SerializeField] int damage;
    [SerializeField] int plannedBlock;

    [Header("Canvas related")]
    [SerializeField] GameObject combatCanvas;
    public GameObject CombatCanvas {
        get { return combatCanvas; }
    }
    [SerializeField] GameObject pathCanvas;
    public GameObject PathCanvas {
        get { return pathCanvas; }
    }
    [SerializeField] GameObject rewardCanvas;
    public GameObject RewardCanvas {
        get { return rewardCanvas; }
    }
    [SerializeField] GameObject winRewardCanvas;
    public GameObject WinRewardCanvas {
        get { return rewardCanvas; }
    }
    [SerializeField] GameObject cardCraftCanvas;
    public GameObject CardCraftCanvas {
        get { return cardCraftCanvas; }
    }

    GameManager gM;

    void Start() {
        gM = FindObjectOfType<GameManager>();
    }

    public void SetNewData(DataEnemy data) {
        enemyData = data;
    }

    public void AssignDataValues(DataEnemy data) {
        enemyData = data;

        //Assing DataEnemy variables to this object. So Scriptable object's values don't change
        enemyName = data.enemyName;
        health = data.health;
        cardImage.sprite = data.enemyArt;

        //Health bar values
        healthBar.maxValue = health;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();

        //Name
        nameTxt.text = enemyName;
    }

    public void TakeDamage(int damage) {
        int remainingDmg = 0;

        //First check if enemy has block, reduce that  first before health
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
        StartCoroutine(AnimateHealthBar(30f));
    }

    IEnumerator GainBlock(int amount) {
        switch (block) {
            case <= 0:
            yield return StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 0f, 1f));
            break;
        }
        block += amount;
        UpdateBlockCounter();
        gM.StartCoroutine(gM.BeginNewTurn());
    }

    void ReduceBlock(int amount) {
        block -= amount;
        UpdateBlockCounter();
    }

    public void ResetBlock() {
        StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 1f, 0f));
        block = 0;
        UpdateBlockCounter();
    }

    void UpdateBlockCounter() {
        blockTxt.text = block.ToString();
    }

    void Die() {
        Debug.Log($"{enemyName} died! Activating reward view");

        if (enemyData.bossEnemy) winRewardCanvas.SetActive(true);
        else if (enemyData.miniBoss) {
            cardCraftCanvas.SetActive(true);
            combatCanvas.SetActive(false);
        }
        else StartCoroutine(ActivateRewardView());
        /*
        switch (enemyData.bossEnemy) {
            case true:
            winRewardCanvas.SetActive(true);
            break;

            default:
            StartCoroutine(ActivateRewardView());
            break;
        }
        */
    }

    void UpdateIndicator() {
        switch (enemyData.action) {
            case PlannedAction.Attack:
            valueTxt.text = damage.ToString();
            iconImg.sprite = iconSprites[0];
            break;

            case PlannedAction.Debuff:
            valueTxt.text = "";
            iconImg.sprite = iconSprites[1];
            break;

            case PlannedAction.Block:
            valueTxt.text = plannedBlock.ToString();
            iconImg.sprite = iconSprites[2];
            break;

            case PlannedAction.AttackAndBlock:
            valueTxt.text = damage.ToString();
            iconImg.sprite = iconSprites[3];
            break;
        }
    }

    public void PlanNextAction(int aRange) {
        int actionRange = Random.Range(0, aRange + 1);
        //int actionRange = 2;

        switch (actionRange) {
            case 0:
            enemyData.action = PlannedAction.Attack;
            damage = Random.Range(enemyData.minDamage, enemyData.maxDamage + 1); // +1 because, max last digit is exclusive
            break;

            case 1:
            enemyData.action = PlannedAction.Block;
            plannedBlock = Random.Range(enemyData.minBlock, enemyData.maxBlock + 1);
            break;

            case 2:
            enemyData.action = PlannedAction.AttackAndBlock;
            plannedBlock = (int)(Random.Range(enemyData.minBlock, enemyData.maxBlock + 1) * 0.5f);
            damage = (int)(Random.Range(enemyData.minDamage, EnemyData.maxDamage + 1) * 0.5f);
            break;

            case 3:
            enemyData.action = PlannedAction.Debuff;
            break;
        }
        UpdateIndicator();
        Debug.Log($"Planned enemy action: {enemyData.action}");
    }

    public void DoPlannedAction() {
        switch (enemyData.action) {
            case PlannedAction.Attack:
            StartCoroutine(DealDamage());
            break;

            case PlannedAction.Debuff:
            Debug.Log("Implement debuff mechanic");
            gM.StartCoroutine(gM.BeginNewTurn());
            break;

            case PlannedAction.Block:
            StartCoroutine(GainBlock(plannedBlock));
            break;

            case PlannedAction.AttackAndBlock:
            StartCoroutine(DealDamageAndBlock(plannedBlock));
            break;
        }
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

    IEnumerator ActivateRewardView() {
        rewardCanvas.SetActive(true);
        yield return null; //Wait for next frame

        //Find all preview cards
        CardPreview[] cards = FindObjectsOfType<CardPreview>();

        //Set random new data for the cards
        foreach (CardPreview card in cards) card.AssingNewData();
    }

    IEnumerator AnimateHealthBar(float animSpeed) {
        if (health <= 0) health = 0;

        while (healthBar.value > health) {
            healthBar.value -= animSpeed * Time.deltaTime;
            healtBarNumber.text = Mathf.Round(healthBar.value).ToString();
            yield return null; //Wait for next frame
        }
        //Make sure value is same as the health
        healthBar.value = health;
        Debug.Log($"{gameObject.name} health bar anim ended");

        //Enemy dies -> change view.
        if (health <= 0) {
            Die();
        }
        else {
            Debug.Log($"{enemyName} is still alive with {health} health");
        }
    }

    public IEnumerator DealDamage() {
        Player player = FindObjectOfType<Player>();

        //TO DO: Add little animation for dealing damage to player
        yield return new WaitForSeconds(1f);
        player.TakeDamage(damage);
        Debug.Log($"{enemyName} dealt: {damage} damage");
        yield return new WaitForSeconds(3f); //Placeholder timewise. Maybe add time lenght of animation later or something similar
        switch (player.Health) {
            case 0:
            player.Die();
            break;

            default:
            gM.StartCoroutine(gM.BeginNewTurn());
            break;
        }
    }

    IEnumerator DealDamageAndBlock(int blockAmount) {
        Player player = FindObjectOfType<Player>();

        //TO DO: Add little animation for dealing damage to player
        yield return new WaitForSeconds(1f);
        player.TakeDamage(damage);
        Debug.Log($"{enemyName} dealt: {damage} damage");

        switch (block) {
            case <= 0:
            yield return StartCoroutine(FadeIcon(blockIconGrp, 0.3f, 0f, 1f));
            break;
        }
        block += blockAmount;
        UpdateBlockCounter();

        yield return new WaitForSeconds(3f); //Placeholder timewise. Maybe add time lenght of animation later or something similar
        switch (player.Health) {
            case 0:
            player.Die();
            break;

            default:
            gM.StartCoroutine(gM.BeginNewTurn());
            break;
        }
    }
}