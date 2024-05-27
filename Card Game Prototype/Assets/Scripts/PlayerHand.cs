using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new();
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }

    public void AddCardIntoHand(GameObject card) {
        cards.Add(card);
        UpdateCounter();
    }

   public void RemoveCardFromHand(GameObject card) {
        cards.Remove(card);
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
    }
}
