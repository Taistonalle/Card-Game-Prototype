using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[Serializable]
public struct CardSlots {
    public GameObject[] slots; //Make sure these are same size
    public bool[] slotsInUse;
}

public class PlayerHand : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new(); //Used to manage card removal, adding etc
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }
    [SerializeField] CardSlots cardSlots; //Related to visual side, where cards are seen
    public CardSlots CardSlots {
        get { return cardSlots; }
    }

    GameManager gM;

    private void Start() {
        gM = FindObjectOfType<GameManager>();
    }

    public void AddCardIntoHand(GameObject card) {
        cards.Add(card);
        UpdateCounter();
        AssignCardSlot(card);
    }

    public void RemoveCardFromHand(GameObject card) {
        cardSlots.slotsInUse[card.GetComponent<DamageCard>().SlotIndex] = false; //Deactivate slot, so a new card can be assigned to that slot. Works only if card is DamageCard
        cards.Remove(card);
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
    }

    void AssignCardSlot(GameObject card) {
        //Search slots for empty slot
        for (int i = 0; i < cardSlots.slots.Length; i++) {
            //Found slot, assing card pos and exit loop
            if (!cardSlots.slotsInUse[i]) {
                //Think how to make next to lines better, instead of using specifically just DamageCard
                card.GetComponent<DamageCard>().SlotPos = cardSlots.slots[i].transform.position;
                card.GetComponent<DamageCard>().SlotIndex = i;
                StartCoroutine(MoveCardToCorrectSlot(card, i, 5f));
                cardSlots.slotsInUse[i] = true;
                break;
            }
        }
    }

    void RearrangeHand() {
        //
    }

    public IEnumerator DrawCards(int amount) {
        PlayerCardPile cardPile = FindObjectOfType<PlayerCardPile>();

        for (int i = 0; i < amount; i++) {
            int slotId = 0;
            //Look for unused slot and use that slot index for coroutine
            for (int k = 0; k < cardSlots.slotsInUse.Length; k++) {
                if (!cardSlots.slotsInUse[k]) {
                    slotId = k;
                    break;
                }
            }
            cardPile.StartCoroutine(cardPile.MoveCardToPlayerHand(cardPile.Cards[0], 5f, slotId)); //Draw top card & move it to unused slot
            cardPile.RemoveCard(cardPile.Cards[0]);
            Debug.Log($"Loop: {i}, slotId: {slotId}, slotUsed: {cardSlots.slotsInUse[slotId]}"); // Debugging 
            yield return new WaitForSeconds(gM.CardMoveRoutineMaxTime); //NOTE: Time cannot be too low, otherwise the slot assignment might overlap
        }
    }

    IEnumerator MoveCardToCorrectSlot(GameObject card, int slotIndex, float moveSpeed) {
        float timer = 0f;
        while (card.transform.position != cardSlots.slots[slotIndex].transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            yield return card.transform.position = Vector2.Lerp(card.transform.position, cardSlots.slots[slotIndex].transform.position, moveSpeed * Time.deltaTime);
        }
    }
}
