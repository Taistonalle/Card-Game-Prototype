using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IPointerDownHandler {
    [SerializeField] DataEnemy enemyData;
    [Header("Enemy base info. Updates from DataEnemy")]
    [SerializeField] string enemyName;
    [SerializeField] int health;

    [Space(10f)]
    [Header("Other infos")]
    [SerializeField] StatusEffect statusEffect;

    [Header("Health bar related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;

    Image cardImage;

    void Start() {
        cardImage = GetComponent<Image>();

        //Assing DataEnemy variables to this. So Scriptable object's values don't change
        enemyName = enemyData.enemyName;
        health = enemyData.health;
        cardImage.sprite = enemyData.enemyArt;

        //Health bar values
        healthBar.maxValue = health;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();
    }

    //When enemy sprite is clicked, update it to player as current target. Assigned, but not used atm at all
    public void OnPointerDown(PointerEventData eventData) {
        FindObjectOfType<Player>().UpdateTarget(gameObject);
    }

    public void TakeDamage(int damage) {
        health -= damage;
        UpdateHealthBar(health);
    }

    void UpdateHealthBar(float value) {
        healthBar.value = value;
        healtBarNumber.text = health.ToString();
    }
}
