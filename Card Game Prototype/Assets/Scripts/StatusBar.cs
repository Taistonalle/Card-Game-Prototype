using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusBar : MonoBehaviour {
    [SerializeField] PlayerDeck deck;
    [SerializeField] Player player;
    [SerializeField] GameObject helpBox;

    [Header("Text components")]
    [SerializeField] TextMeshProUGUI deckCountTxt;
    [SerializeField] TextMeshProUGUI playerHealthTxt;

    void Start() {
        deck = FindObjectOfType<PlayerDeck>();
        UpdateDeckCountTxt();
        UpdateHealthTxt();
    }

    public void UpdateHealthTxt() {
        playerHealthTxt.text = $"Health: {player.Health}/{player.MaxHp}";
    }

    public void UpdateDeckCountTxt() {
        deckCountTxt.text = $"Total cards: {deck.CardCount}";
    }

    public void ToggleHelpBox() {
        switch (helpBox.activeSelf) {
            case false:
            helpBox.SetActive(true);
            break;

            case true:
            helpBox.SetActive(false);
            break;
        }
    }
}
