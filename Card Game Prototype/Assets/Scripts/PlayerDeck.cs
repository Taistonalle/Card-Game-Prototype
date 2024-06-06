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
    [SerializeField] List<GameObject> originalDeck = new();
    public List<GameObject> OriginalDeck {
        get { return originalDeck; }
        set { originalDeck = value; }
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
        //Loop through the Card list and compare data. Remove the prefab on first match
        for (int i = 0; i < cards.Count; i++) {
            if (cards[i].GetComponent<Card>().CardData == card.GetComponent<Card>().CardData) {
                Debug.Log($"Removing: {cards[i].name}");
                cards.Remove(cards[i]);
                break;
            }
        }
        UpdateCounter();
    }

    public void ResetDeck() {
        cards.Clear();
        UpdateCounter();
    }

    public void RetryResetDeck() {
        cards = new List<GameObject>(originalDeck); //clone the original back to cards
        UpdateCounter();
        /*
        for (int i = cardCount - 1; i >= maxStartCardAmount; i--) {
            Debug.Log($"Removing card from deck: {cards[i].name}");
            cards.RemoveAt(i);
            UpdateCounter();
            Debug.Log($"Cards left in deck after removal: {cardCount}");
        }
        */
    }

    void UpdateCounter() {
        cardCount = cards.Count;
    }
}
