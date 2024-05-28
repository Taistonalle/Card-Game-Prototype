using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { PlayerTurn, EnemyTurn, AfterCombat, Shop }
public class GameManager : MonoBehaviour {
    [SerializeField] GameState gameState; //Use this later to check what player can do. For example: Not play cards in enemy turn etc.
    public GameState GameState {
        get { return gameState; }
    }
    [SerializeField] float cardMoveRoutineMaxTime;
    public float CardMoveRoutineMaxTime {
        get { return cardMoveRoutineMaxTime; }
    }

    [Header("Card prefabs")]
    [SerializeField] GameObject[] dmgCards;

    [Header("End turn button")]

    [SerializeField] TextMeshProUGUI endTurnButtonTxt;
    public TextMeshProUGUI EndTurnButtonTxt {
        get { return endTurnButtonTxt; }
    }

    BoxCollider2D cardDropArea;
    public BoxCollider2D CardDropArea {
        get { return cardDropArea; }
    }
    PlayerCardPile pCP;
    DiscardPile dP;

    void Start() {
        cardDropArea = GameObject.Find("CardDropArea").GetComponent<BoxCollider2D>();
        pCP = FindObjectOfType<PlayerCardPile>();
        dP = FindObjectOfType<DiscardPile>();
        StartCoroutine(InstantiateCardCopy(dmgCards[0]));
    }

    IEnumerator InstantiateCardCopy(GameObject card) { //proto start initiation
        //Shuffle deck cards into PlayerCardPile
        for (int i = 0; i < 10; i++) {
            yield return new WaitForSeconds(0.2f);
            pCP.AddCardIntoCardPile(Instantiate(card, pCP.transform.position, Quaternion.identity));
            pCP.UpdateCounter();
        }

        //Remove all cards and add them into hand
        for (int i = pCP.Cards.Count - 1; i >= 0; i--) {
            yield return new WaitForSeconds(0.2f);
            pCP.StartCoroutine(pCP.MoveCardToPlayerHand(pCP.Cards[i], 5f));
            pCP.RemoveCard(pCP.Cards[i]);
        }
    }

    //Call this function from End turn button
    public void StartEndTurn() {
        endTurnButtonTxt.text = "Enemy turn";
        StartCoroutine(EndTurn());
    }

    public IEnumerator BeginNewTurn() {
        endTurnButtonTxt.text = "Player turn";
        gameState = GameState.PlayerTurn;
        //Placeholder thing to do
        //Check if PlayerCardPile is empty. Yes -> empty discard pile into PlayerCardPile. NOTE no actual check yet, implement later.
        for (int i = dP.Cards.Count - 1; i >= 0; i--) {
            yield return new WaitForSeconds(0.2f);
            dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[i], 5f));
        }
    }

    IEnumerator EndTurn() {
        gameState = GameState.EnemyTurn;
        //Call enemy function stuff
        Enemy enemy = FindObjectOfType<Enemy>(); //Note to future self. This way works only for one enemy at a time.
        yield return enemy.StartCoroutine(enemy.DealDamage(enemy.EnemyData.damage));
    }
}
