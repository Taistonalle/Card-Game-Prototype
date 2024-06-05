using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerCardPile : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new();
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }
    [SerializeField] TextMeshProUGUI counterTxt;
    [SerializeField] bool movingRoutineRunning;
    public bool MovingRoutineRunning {
        get { return movingRoutineRunning; }
    }

    GameManager gM;
    PlayerHand hand;

    void Start() {
        gM = FindObjectOfType<GameManager>();
        hand = FindObjectOfType<PlayerHand>();
        UpdateCounter();
    }
   
    public void AddCardIntoCardPile(GameObject card) {
        cards.Add(card);
    }

    public void RemoveCard(GameObject card) {
        cards.Remove(card);
        UpdateCounter();
    }

    public void UpdateCounter() {
        cardCount = cards.Count;
        counterTxt.text = cardCount.ToString();
    }

    public void ClearPlayerCardPile() {
        foreach (GameObject card in cards) Destroy(card);
        cards.Clear();
        UpdateCounter();
    }

    public IEnumerator MoveCardToPlayerHand(GameObject card, int slotIndex) {
        //Set card to visible
        card.SetActive(true);
        card.transform.SetParent(hand.CardSlots.slots[slotIndex].transform, true);

        hand.AddCardIntoHand(card); //This also does the moving for the card
        yield return null;

        /*
        float timer = 0f;
        while (card.transform.position != hand.transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            yield return card.transform.position = Vector2.Lerp(card.transform.position, hand.transform.position, moveSpeed * Time.deltaTime);
        }
        */
    }

    /*
    IEnumerator MoveCardToPile(GameObject card, float moveSpeed) { // not used atm
        //Temporarily set card to be visible
        card.SetActive(true);

        float timer = 0f;
        while (card.transform.position != transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            //Scale card to be smaller as it moves to pile
            card.transform.localScale = new Vector3(card.transform.localScale.x - Time.deltaTime, card.transform.localScale.y - Time.deltaTime);
            yield return card.transform.position = Vector2.Lerp(card.transform.position, transform.position, moveSpeed * Time.deltaTime);
        }
        //Hide card and set pile tranform as it's parent
        card.SetActive(false);
        card.transform.SetParent(transform, true);
        UpdateCounter();
        card.transform.localScale = new Vector3(1f, 1f);
    }
    */
}
