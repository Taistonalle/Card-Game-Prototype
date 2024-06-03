using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscardPile : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new();
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    [SerializeField] TextMeshProUGUI counterTxt;

    GameManager gM;
    PlayerCardPile pCP;

    void Start() {
        gM = FindObjectOfType<GameManager>();
        pCP = FindObjectOfType<PlayerCardPile>();
    }

    public void AddCardIntoDiscardPile(GameObject card) {
        cards.Add(card);
        StartCoroutine(MoveCardToDiscardPile(card, 5f));
    }

    public void RemoveCard(GameObject card) {
        cards.Remove(card);
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
        counterTxt.text = cardCount.ToString();
    }

    public void ClearDiscardPile() {
        foreach (GameObject card in cards) Destroy(card);
        cards.Clear();
        UpdateCounter();
    }

    IEnumerator MoveCardToDiscardPile(GameObject card, float moveSpeed) {
        float timer = 0f;
        while (card.transform.position != transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            //Scale card to be smaller as it moves to pile
            card.transform.localScale = new Vector3(card.transform.localScale.x - Time.deltaTime, card.transform.localScale.y - Time.deltaTime);
            yield return card.transform.position = Vector2.Lerp(card.transform.position, transform.position, moveSpeed * Time.deltaTime);
        }
        //Hide card and set discard pile tranform as it's parent
        card.SetActive(false);
        card.transform.SetParent(transform, true);
        UpdateCounter();
        card.transform.localScale = new Vector3(1f, 1f);
        card.GetComponent<DamageCard>().Clicked = false; //Set Card clicked bool back to false. -> Hovering "animations" work again
    }

    public IEnumerator MoveCardToPlayerCardPile(GameObject card, float moveSpeed) {
        //Temporarily set card to be visible
        card.SetActive(true);

        //Remove card from the discard pile
        RemoveCard(card);

        float timer = 0f;
        while (card.transform.position != pCP.transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            //Scale card to be smaller as it moves to pile
            card.transform.localScale = new Vector3(card.transform.localScale.x - Time.deltaTime, card.transform.localScale.y - Time.deltaTime);
            yield return card.transform.position = Vector2.Lerp(card.transform.position, pCP.transform.position, moveSpeed * Time.deltaTime);
        }
        //Hide card and set pile tranform as it's parent
        card.SetActive(false);
        card.transform.SetParent(pCP.transform, true);
        pCP.AddCardIntoCardPile(card);
        pCP.UpdateCounter();
        card.transform.localScale = new Vector3(1f, 1f);
    }
}
