using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum StatusEffect { None, Stunned, Dazed }
public class Player : MonoBehaviour {
    [Header("Player stats")]
    [SerializeField][Range(0, 100)] int health;
    public int Health {
        get { return health; }
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

    [Header("Health bar related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;

    [Header("Deck related")]
    //Maybe add list later that has cards player uses or has collected etc. 
    [SerializeField] int deckSize;
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
        //Health bar values
        healthBar.maxValue = health;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();

        UpdateActionPointCounter();
        gM = FindObjectOfType<GameManager>();
    }

    public void TakeDamage(int damage) {
        health -= damage;
        StartCoroutine(AnimateHealthBar(30f));
    }

    public void ReduceAP(int actionCost) {
        aP -= actionCost;
        UpdateActionPointCounter();
    }

    public void ResetAP() {
        aP = maxAP;
        UpdateActionPointCounter();
    }

    void UpdateActionPointCounter() {
        aPCounter.text = $"{aP}/{maxAP}";
    }

    public void Die() { //Later for juicying add death animation or like different sprite
        gM.StartCoroutine(gM.GameOver());
    }

    IEnumerator AnimateHealthBar(float animSpeed) {
        while (healthBar.value >= health) {
            yield return new WaitForSeconds(Time.deltaTime);
            healthBar.value -= animSpeed * Time.deltaTime;
            healtBarNumber.text = Mathf.Round(healthBar.value).ToString();
        }
        //Make sure value is same as the health
        healthBar.value = health;
        Debug.Log($"{gameObject.name} health bar anim ended");
    }
}
