using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new();
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }
    [SerializeField] int maxStartCardAmount;
    public int MaxStartCardAmount {
        get { return maxStartCardAmount; }
    }

    [Header("All of the card prefabs")] //Used to pass prefab copies for cards list
    [SerializeField] GameObject[] cardPrefabs; 
    public GameObject[] CardPrefabs {
        get { return cardPrefabs; }
    }

    void Start() {
        if (FindObjectsOfType<PlayerDeck>().Length > 1) {
            Debug.Log($"Destroying extra {gameObject.name}");
            Destroy(gameObject);
        }
        else DontDestroyOnLoad(gameObject);
    }

    public void RewardAddCard(GameObject card) {
        cards.Add(card);
        UpdateCounter();
    }

    public void MenuAddCard(GameObject card) {
        if (cardCount >= maxStartCardAmount) {
            Debug.Log("Deck full");
            return;
        }
        else {
            cards.Add(card);
            UpdateCounter();
        }
    }

    public void RemoveCard(GameObject card) {
        cards.Remove(card);
        UpdateCounter();
    }

    public void ResetDeck() {
        cards.Clear();
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
    }
}
