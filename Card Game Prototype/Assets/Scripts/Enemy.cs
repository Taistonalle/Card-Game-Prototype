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

    [Space(10f)]
    [Header("Other infos")]
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] StatusEffect statusEffect;

    [Header("Health bar related")]
    [SerializeField] Slider healthBar;
    [SerializeField] TextMeshProUGUI healtBarNumber;

    Image cardImage;
    GameManager gM;

    void Start() {
        gM = FindObjectOfType<GameManager>();
        cardImage = GetComponent<Image>();

        //Assing DataEnemy variables to this object. So Scriptable object's values don't change
        enemyName = enemyData.enemyName;
        health = enemyData.health;
        cardImage.sprite = enemyData.enemyArt;

        //Health bar values
        healthBar.maxValue = health;
        healthBar.value = health;
        healtBarNumber.text = health.ToString();

        //Name
        nameTxt.text = enemyName;
    }

    /*
    //When enemy sprite is clicked, update it to player as current target. Assigned, but not used atm at all
    public void OnPointerDown(PointerEventData eventData) {
        FindObjectOfType<Player>().UpdateTarget(gameObject);
    }
    */

    public void TakeDamage(int damage) {
        health -= damage;
        StartCoroutine(AnimateHealthBar(30f));
    }

    //Instant healt bar update, not used atm
    void UpdateHealthBar(float value) {
        healthBar.value = value;
        healtBarNumber.text = health.ToString();
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

    public IEnumerator DealDamage(int damage) {
        Player player = FindObjectOfType<Player>();
        //TO DO: Add little animation for dealing damage to player
        yield return new WaitForSeconds(1f);
        player.TakeDamage(damage);
        yield return new WaitForSeconds(3f); //Placeholder timewise. Maybe add time lenght of animation later or something similar
        gM.StartCoroutine(gM.BeginNewTurn());
    }
}
