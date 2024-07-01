using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardCrafting : MonoBehaviour {
    [SerializeField] CardPreview previewCard;
    //[SerializeField] CardData[] craftedScriptables;
    [SerializeField] GameObject[] craftedCardPrefabs;
    public GameObject[] CraftedCardPrefabs { get {  return craftedCardPrefabs; } }
    [SerializeField] int prefabIndex;
    public int PrefabIndex { //Modified when pressing miniboss point in path view. Should be fixed now :)
        get { return prefabIndex; }
        set { prefabIndex = value; }
    }
    [SerializeField] List<GameObject> storedComponents = new();
    public List<GameObject> StoredComponents { get { return storedComponents; } }
    [SerializeField] GameObject compParent;
    public GameObject CompParent { get { return compParent; } }

    [Header("Component related")]
    [SerializeField] Button[] compButtons;
    [SerializeField] int componentAmount;
    [SerializeField] int maxComponentAmount;
    [SerializeField] int drawAmount;
    [SerializeField] int dmgAmount;
    [SerializeField] int blockAmount;
    [SerializeField] int healAmount;
    [SerializeField] int buffDuration;
    [SerializeField] bool[] typeAdded;
    [SerializeField] int typeAmount;

    [Header("Canvases")]
    [SerializeField] GameObject pathCanvas;
    //[SerializeField] GameObject combatCanvas;
    [SerializeField] GameObject craftCanvas;

    PlayerDeck deck;

    void Start() {
        deck = FindObjectOfType<PlayerDeck>();
        //if (FindObjectsOfType<CardCrafting>().Length > 1) {
        //    Debug.Log($"Destroying extra {gameObject.name}");
        //    Destroy(gameObject);
        //}
        //else DontDestroyOnLoad(gameObject);
        //DelegateCompButtons();
    }

    void DelegateCompButtons() {
        compButtons[0].onClick.AddListener(delegate { DrawComponent(); });
        compButtons[1].onClick.AddListener(delegate { DamageComponent(); });
        compButtons[2].onClick.AddListener(delegate { BlockComponent(); });
        compButtons[3].onClick.AddListener(delegate { HealComponent(); });
        compButtons[4].onClick.AddListener(delegate { BuffComponent(); });
    }

    bool CheckTypeAmount(bool type) {
        //Is type already added? Yes, return false and continue
        if (type) return false;

        //Ensure counter is zero before new check
        typeAmount = 0;

        //Check the amount of true states in typeAdded
        foreach (bool comp in typeAdded) {
            if (comp) typeAmount += 1;
        }
        
        //Return true if amount is 3
        if (typeAmount == 3) return true;
        else return false;
    }

    void DrawComponent() {
        if (componentAmount == 5 || CheckTypeAmount(typeAdded[0])) return;
        componentAmount += 1;
        typeAdded[0] = true;

        previewCard.CardData.playCost += 1;
        previewCard.CardData.drawAmount += drawAmount;
        previewCard.CardData.draw = true;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    void DamageComponent() {
        if (componentAmount == 5 || CheckTypeAmount(typeAdded[1])) return;
        componentAmount += 1;
        typeAdded[1] = true;

        previewCard.CardData.playCost += 1;
        previewCard.CardData.damage += dmgAmount;
        previewCard.CardData.dealDamage = true;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    void BlockComponent() {
        if (componentAmount == 5 || CheckTypeAmount(typeAdded[2])) return;
        componentAmount += 1;
        typeAdded[2] = true;

        previewCard.CardData.playCost += 1;
        previewCard.CardData.blockAmount += blockAmount;
        previewCard.CardData.block = true;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    void HealComponent() {
        if (componentAmount == 5 || CheckTypeAmount(typeAdded[3])) return;
        componentAmount += 1;
        typeAdded[3] = true;

        previewCard.CardData.playCost += 1;
        previewCard.CardData.healAmount += healAmount;
        previewCard.CardData.heal = true;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    void BuffComponent() {
        if (componentAmount == 5 || CheckTypeAmount(typeAdded[4])) return;
        componentAmount += 1;
        typeAdded[4] = true;

        previewCard.CardData.playCost += 1;
        previewCard.CardData.buffDuration += buffDuration;
        previewCard.CardData.buff = true;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
             previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    public void IncreaseSliderSize() { 
        RectTransform rectTransform = compParent.GetComponent<RectTransform>();
        rectTransform.sizeDelta += new Vector2(100f, 0f);
    }

    #region Button functions
    public void ResetComponents() { 
        craftedCardPrefabs[prefabIndex].GetComponent<Card>().ComponentAmount = 0;
        typeAmount = 0;
        for (int i = 0; i < typeAdded.Length;  i++) typeAdded[i] = false;

        //Damage
        previewCard.CardData.damage = 0;
        previewCard.CardData.dealDamage = false;

        //Block
        previewCard.CardData.blockAmount = 0;
        previewCard.CardData.block = false;

        //Draw
        previewCard.CardData.drawAmount = 0;
        previewCard.CardData.draw = false;

        //Heal
        previewCard.CardData.healAmount = 0;
        previewCard.CardData.heal = false;

        //Buff
        previewCard.CardData.buffDuration = 0;
        previewCard.CardData.buff = false;
        previewCard.CardData.buffType = BuffType.None;

        //Debuff
        previewCard.CardData.debuffDuration = 0;
        previewCard.CardData.debuff = false;
        previewCard.CardData.debuffType = DebuffType.None;

        //Recover
        previewCard.CardData.aPRecoverAmount = 0;
        previewCard.CardData.recoverAp = false;

        previewCard.CardData.playCost = 0;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    public void AddCard() { 
        PlayerDeck deck = FindObjectOfType<PlayerDeck>();

        switch (componentAmount) {
            case 0:
            Debug.Log("No components added yet!");
            break;

            default:
            deck.RewardAddCard(craftedCardPrefabs[prefabIndex]);
            craftCanvas.SetActive(false);
            pathCanvas.SetActive(true);
            break;
        }
    }

    /*
    public void SetupCrafting(int index) {
        prefabIndex = index;
        previewCard.CardData = craftedScriptables[index];
        ResetComponents();
    }
    */

    public void AddComponentValues(CraftComponent compData) { //This gets delegated into the component button from GameManager
        if (craftedCardPrefabs[prefabIndex].GetComponent<Card>().ComponentAmount == maxComponentAmount) return;
        craftedCardPrefabs[prefabIndex].GetComponent<Card>().ComponentAmount += 1;

        //Bools
        if (compData.Draw) previewCard.CardData.draw = compData.Draw;
        if (compData.DealDamage) previewCard.CardData.dealDamage = compData.DealDamage;
        if (compData.Heal) previewCard.CardData.heal = compData.Heal;
        if (compData.Block) previewCard.CardData.block = compData.Block;
        //if (compData.RecoverAp) previewCard.CardData.recoverAp = compData.RecoverAp;
        if (compData.Buff) previewCard.CardData.buff = compData.Buff;
        if (compData.Debuff) previewCard.CardData.debuff = compData.Debuff;

        //Values
        previewCard.CardData.drawAmount += compData.DrawAmount;
        previewCard.CardData.damage += compData.Damage;
        previewCard.CardData.healAmount += compData.HealAmount;
        previewCard.CardData.blockAmount += compData.BlockAmount;
        //previewCard.CardData.aPRecoverAmount += compData.APRecoverAmount;
        previewCard.CardData.playCost += compData.PlayCost;
        previewCard.CardData.buffDuration += compData.BuffDuration;
        if (previewCard.CardData.buffType == BuffType.None) previewCard.CardData.buffType = compData.BuffType;
        previewCard.CardData.debuffDuration += compData.DebuffDuration;
        if (previewCard.CardData.debuffType == DebuffType.None) previewCard.CardData.debuffType = compData.DebuffType;

        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
             previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    public void PreviousCard() {
        if (prefabIndex == 0) return;
        prefabIndex--;

        previewCard.CardData = craftedCardPrefabs[prefabIndex].GetComponent<Card>().CardData;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }

    public void NextCard() {
        if (prefabIndex == deck.CraftCardCount - 1) return;
        prefabIndex++;

        previewCard.CardData = craftedCardPrefabs[prefabIndex].GetComponent<Card>().CardData;
        previewCard.CS.CardSetup(previewCard.Background, previewCard.Borders[0], previewCard.Borders[1],
            previewCard.CardImage, previewCard.PlayCostText, previewCard.NameText, previewCard.DescriptionText, previewCard.CardData);
    }
    #endregion
}