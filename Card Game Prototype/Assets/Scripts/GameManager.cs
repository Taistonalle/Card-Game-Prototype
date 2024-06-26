using CustomAudioManager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { PathView, PlayerTurn, EnemyTurn, AfterCombat, Shop, GameOver }
public class GameManager : MonoBehaviour {
    [SerializeField] GameState gameState; //Use this to check what player can do. For example: Not play cards in enemy turn etc.
    public GameState GameState {
        get { return gameState; }
    }
    [SerializeField] float cardMoveRoutineMaxTime;
    public float CardMoveRoutineMaxTime {
        get { return cardMoveRoutineMaxTime; }
    }
    [SerializeField] List<GameObject> deckCopy;

    [Header("End turn button")]
    [SerializeField] TextMeshProUGUI endTurnButtonTxt;
    public TextMeshProUGUI EndTurnButtonTxt {
        get { return endTurnButtonTxt; }
    }

    [Header("Game over box")]
    [SerializeField] GameObject gOBox;

    [Header("Card drop area")]
    [SerializeField] BoxCollider2D cardDropArea;
    public BoxCollider2D CardDropArea {
        get { return cardDropArea; }
    }

    [Header("Enemy related")]
    [SerializeField] Enemy enemy;
    public Enemy Enemy {
        get { return enemy; }
    }
    [SerializeField] DataEnemy[] tierOneEnemyDatas;
    public DataEnemy[] TierOneEnemyDatas {
        get { return tierOneEnemyDatas; }
    }
    [SerializeField] DataEnemy[] tierTwoEnemyDatas;
    public DataEnemy[] TierTwoEnemyDatas {
        get { return tierTwoEnemyDatas; }
    }
    [SerializeField] DataEnemy[] tierThreeEnemyDatas;
    public DataEnemy[] TierThreeEnemyDatas {
        get { return tierThreeEnemyDatas; }
    }
    [SerializeField] DataEnemy[] tierFourEnemyDatas;
    public DataEnemy[] TierFourEnemyDatas {
        get { return tierFourEnemyDatas; }
    }
    [SerializeField] DataEnemy[] miniBossEnemyDatas;
    public DataEnemy[] MiniBossEnemyDatas {
        get { return miniBossEnemyDatas; }
    }
    [SerializeField] DataEnemy[] bossEnemyDatas;
    public DataEnemy[] BossEnemyDatas {
        get { return bossEnemyDatas; }
    }

    [Header("Player related")]
    [SerializeField] Player player;
    [SerializeField] PlayerHand hand;
    [SerializeField] PlayerCardPile pCP;
    [SerializeField] DiscardPile dP;
    PlayerDeck deck;
    StatusBar statusBar;

    [Header("Crafting related")]
    [SerializeField] CardCrafting cardCrafting;
    public CardCrafting CardCrafting { get { return cardCrafting; } }

    void Start() {
        deck = FindObjectOfType<PlayerDeck>();
        statusBar = FindObjectOfType<StatusBar>();
    }

    IEnumerator InstantiateCardCopy(GameObject card) { //proto start initiation
        //Shuffle deck cards into PlayerCardPile. Deck part is still yet to be implemented!
        for (int i = 0; i < 10; i++) {
            yield return new WaitForSeconds(0.2f);
            GameObject newCard = Instantiate(card, pCP.transform, false);
            pCP.AddCardIntoCardPile(newCard);
            pCP.UpdateCounter();
        }

        //Remove all cards and add them into hand
        int revOrder = 0;
        for (int i = pCP.Cards.Count - 1; i >= 0; i--) {
            yield return new WaitForSeconds(0.2f);
            pCP.StartCoroutine(pCP.MoveCardToPlayerHand(pCP.Cards[i], revOrder));
            pCP.RemoveCard(pCP.Cards[i]);
            revOrder++;
        }
    }

    IEnumerator InstantiateDeckCards() {
        //Add copied deck cards as instiated cards to PlayerCardPile
        foreach (GameObject card in deckCopy) {
            GameObject copiedCard = Instantiate(card, pCP.transform, false);
            pCP.AddCardIntoCardPile(copiedCard);
            pCP.UpdateCounter();
        }
        pCP.ShufflePile(); // loop hapens first, no waiting needed :)

        yield return new WaitUntil(() => hand.isActiveAndEnabled);
        hand.StartCoroutine(hand.DrawCards(player.DrawAmount));
        /*
        //Add fixed amount of cards to hand
        int revOrder = 0;
        for (int i = pCP.CardCount -1; i >= player.DrawAmount; i--) {
            yield return new WaitForSeconds(0.2f);
            pCP.StartCoroutine(pCP.MoveCardToPlayerHand(pCP.Cards[i], 5f, revOrder));
            pCP.RemoveCard(pCP.Cards[i]);
            revOrder++;
        }
        */
    }

    #region Button functions
    public void StartEndTurn() {
        switch (gameState) {
            case GameState.PlayerTurn:
            endTurnButtonTxt.text = "Enemy turn";
            StartCoroutine(EndTurn());
            break;
        }
    }

    public void Retry() {
        deck.RetryResetDeck();
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu() {
        deck.ResetDeck();
        SceneManager.LoadScene(0);
    }

    public void StartCombat() {
        gameState = GameState.PlayerTurn;

        hand.ClearHand();
        pCP.ClearPlayerCardPile();
        dP.ClearDiscardPile();
        player.ResetAP();
        if (player.Block > 0) player.ResetBlock();
        if (player.Buffs[0] != BuffEffect.None || player.Buffs[1] != BuffEffect.None) player.ResetBuffs(); // Do check better later...
        player.UpdateHealthInfo();
        enemy.AtkCounter = enemy.EnemyData.atksBeforeOtherAction;
        enemy.PlanNextAction();
        CopyDeckForUsage();
        StartCoroutine(InstantiateDeckCards());

    }

    public void PickRewardButton(Button button) {
        //Get data from the picked card
        var cardData = button.gameObject.GetComponentInParent<CardPreview>().CardData;

        //Loop through card prefabs to compare data. If same -> Add prefab to deck
        for (int i = 0; i < deck.CardPrefabs.Length; i++) {
            if (deck.CardPrefabs[i].GetComponent<Card>().CardData == cardData) {
                Debug.Log($"{deck.CardPrefabs[i].name} matched with {cardData}. Adding {deck.CardPrefabs[i].name} to deck");
                deck.RewardAddCard(deck.CardPrefabs[i]);
                break;
            }
            else {
                Debug.Log($"No match found for {cardData}!");
            }
        }
        enemy.CombatCanvas.SetActive(false);
        enemy.RewardCanvas.SetActive(false);
        enemy.PathCanvas.SetActive(true);
        statusBar.UpdateDeckCountTxt();
    }

    public void PickComponentButton(GameObject component) {
        //Simply add the picked component into CardCrafting component list
        cardCrafting.StoredComponents.Add(Instantiate(component, cardCrafting.CompParent.transform));

        //And activate/deactivate needed canvases
        enemy.CombatCanvas.SetActive(false);
        enemy.RewardCanvas.SetActive(false);
        enemy.PathCanvas.SetActive(true);
    }

    public void SkipRewardButton() {
        enemy.CombatCanvas.SetActive(false);
        enemy.RewardCanvas.SetActive(false);
        enemy.PathCanvas.SetActive(true);
    }
    #endregion

    //void FindEverything() {
    //    player = FindObjectOfType<Player>();
    //    hand = FindObjectOfType<PlayerHand>();
    //    deck = FindObjectOfType<PlayerDeck>();
    //    pCP = FindObjectOfType<PlayerCardPile>();
    //    dP = FindObjectOfType<DiscardPile>();
    //}

    void CopyDeckForUsage() {
        deckCopy = deck.Cards;
    }

    public IEnumerator BeginNewTurn() {
        //Check if PlayerCardPile is empty. Yes -> empty discard pile into PlayerCardPile.
        if (pCP.CardCount == 0) {
            for (int i = dP.CardCount - 1; i >= 0; i--) {
                yield return new WaitForSeconds(0.2f);
                AudioManager.PlayCardSound();
                dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[i], 5f)); //Keep like this, not yield return. Affects speed as well.
            }
            yield return new WaitForSeconds(CardMoveRoutineMaxTime); //Wait for the cards to be moved before drawing
            pCP.ShufflePile();
            //yield return new WaitUntil(() => dP.CardCount == 0); Seems to mess with intervals as well
        }

        //Player checks and functions
        //Buff checks
        if (player.Buffs[0] == BuffEffect.Strenght) player.UpdateBuffDuration(BuffEffect.Strenght);
        //More buff checks later as more get added...
        if (player.Block > 0) player.ResetBlock();
        player.ResetAP();
        //Check if player can draw a card
        if (hand.CardCount < 10 && pCP.Cards.Count > 0) {
            Debug.Log("Drawing card(s)");
            hand.StartCoroutine(hand.DrawCards(player.DrawAmount));
        }

        yield return new WaitUntil(() => !hand.Drawing);
        
        //Is player debuffed?
        switch (player.StatusEffect) {
            case StatusEffect.Stunned:
            Debug.Log($"Player is {player.StatusEffect}! Ending turn automatically");
            player.ResetDebuff(StatusEffect.Stunned);
            enemy.PlanNextAction();
            StartCoroutine(EndTurn());
            break;

            //No
            default:
            endTurnButtonTxt.text = "End turn";
            gameState = GameState.PlayerTurn;

            //Show next enemy action
            enemy.PlanNextAction();
            break;
        }
    }

    IEnumerator EndTurn() {
        gameState = GameState.EnemyTurn;

        //Empty player hand to discard pile
        for (int i = hand.CardCount; i > 0; i--) {
            yield return new WaitForSeconds(0.2f); //Tiny delay between each card
            dP.AddCardIntoDiscardPile(hand.Cards[0]); //NOTE: This can be index zero always because, list updates on card removal therefore removing first card always works
            hand.RemoveCardFromHand(hand.Cards[0]);
        }

        //Check if enemy still has block, reset it before it's new action
        if (enemy.Block > 0) enemy.ResetBlock();

        //Is enemy debuffed?
        switch (enemy.StatusEffect) {
            case StatusEffect.Stunned:
            Debug.Log($"Enemy is {enemy.StatusEffect}! Ending enemy turn automatically");
            enemy.ResetDebuff(StatusEffect.Stunned);
            yield return new WaitForSeconds(1f); //Small delay before new turn
            StartCoroutine(BeginNewTurn());
            break;

            //No
            default:
            enemy.DoPlannedAction();
            break;
        }
    }

    public IEnumerator GameOver() {
        yield return gameState = GameState.GameOver;

        //Activate/make game over message box visible. Add fancier fade in effect later?
        gOBox.SetActive(true);
        AudioManager.PlayGameOverSound();
    }
}
