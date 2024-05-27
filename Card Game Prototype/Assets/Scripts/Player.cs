using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum StatusEffect { None, Stunned, Dazed }
public class Player : MonoBehaviour {
    [Header("Player stats")]
    [SerializeField][Range(0, 100)] int health;
    [SerializeField] int actionPoints;
    public int ActionPoints {
        get { return actionPoints; }
    }
    [SerializeField] StatusEffect statusEffect;

    [Header("Health bar related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;

    [Header("Deck related")]
    [SerializeField] int deckSize;
    public int DeckSize {
        get { return deckSize; }
    }

    [Header("Hand related")]
    [SerializeField] int handSize;
    public int HandSize {
        get { return handSize; }
    }

    [Header("Target related")]
    [SerializeField] GameObject selectedTarget; //Does nothing atm, maybe even remove later or handle some other way targeting

    void Start() {
        //Health bar values
        healthBar.maxValue = health;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();
    }

    public void UpdateTarget(GameObject newTarget) { //Also not really used atm
        selectedTarget = newTarget;
    }

    public void TakeDamage(int damage) {
        health -= damage;
        StartCoroutine(AnimateHealthBar(10f));
    }

    public void ReduceAP(int actionCost) {
        actionPoints -= actionCost;
    }

    public void ResetAP() {
        actionPoints = 4; //Placeholder hardcoded number
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
