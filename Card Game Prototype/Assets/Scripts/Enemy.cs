using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Health bar related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;

    [Header("Indicator related")]
    [SerializeField] TextMeshProUGUI dmgTxt;
    [SerializeField] Sprite[] iconSprites;
    [SerializeField] Image iconImg;
    [SerializeField] int damage;

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
        health -= damage;
        StartCoroutine(AnimateHealthBar(30f));
    }



    void Die() {
        Debug.Log($"{enemyName} died! Activating reward view");

        switch (enemyData.bossEnemy) {
            case true:
            //Placeholder thing to do
            winRewardCanvas.SetActive(true);
            break;

            default:
            StartCoroutine(ActivateRewardView());
            break;
        }
    }

    void UpdateIndicator() {
        switch (enemyData.action) {
            case PlannedAction.Attack:
            dmgTxt.text = damage.ToString();
            break;
        }
    }

    public void PlanNextAction() {
        int actionRange = Random.Range(0, System.Enum.GetNames(typeof(PlannedAction)).Length + 1); // used later
        Debug.Log($"Enum num: {actionRange}");

        //Switch using action range to set PlannedAction enum...

        //Placeholder
        if (enemyData.action == PlannedAction.Attack) {
            //Randomise enemy damage from min damage to max damage range
            damage = Random.Range(enemyData.minDamage, enemyData.maxDamage + 1); // +1 because, max last digit is exclusive
            UpdateIndicator();
        }
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
            //yield return new WaitForSeconds(Time.deltaTime);
            healthBar.value -= animSpeed * Time.deltaTime;
            healtBarNumber.text = Mathf.Round(healthBar.value).ToString();
            yield return null; //Wait for next frame
        }
        //Make sure value is same as the health
        healthBar.value = health;
        Debug.Log($"{gameObject.name} health bar anim ended");

        //Enemy dies -> change view. ----IMPLEMENT REWARD LATER----
        if (health <= 0) {
            Die();
            //combatCanvas.SetActive(false);
            //pathCanvas.SetActive(true);
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
}