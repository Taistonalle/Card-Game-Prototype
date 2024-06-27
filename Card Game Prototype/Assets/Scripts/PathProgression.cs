using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathProgression : MonoBehaviour {
    [Header("Legend icons")]
    [SerializeField] Sprite[] icons;

    [Header("Path buttons")]
    [SerializeField] List<Button> enemyPathButtons = new();
    [SerializeField] List<Button> eventPathButtons = new();
    [SerializeField] List<Button> restPathButtons = new();

    [Header("Path line renderers")]
    [SerializeField] LineRenderer[] paths;

    [Header("Canvas references")]
    [SerializeField] GameObject combatCanvas;
    [SerializeField] GameObject restCanvas;
    [SerializeField] GameObject craftCanvas;

    [Header("Other values")]
    [SerializeField][Tooltip("Time it takes at start for view to scroll from top to bottom")] float viewScrollTime;

    int pathPoint = 0; //Used to determine what point does path linerender draw for TravelledLine
    public int PathPoint {
        get { return pathPoint; }
        set { pathPoint = value; }
    }

    [Header("FindObjectOfType workaround")] //For objects that are not active at start
    [SerializeField] Player player;
    [SerializeField] PathEvent pEvent;
    [SerializeField] GameObject deckButton;
    //FadeCanvas fadeCanvas;
    GameManager gM;

    void Start() {
        FindPathButtons();
        gM = FindObjectOfType<GameManager>();
        //fadeCanvas = FindObjectOfType<FadeCanvas>();
        ResetScriptables();
        StartCoroutine(MoveContentView());
    }

    void FindPathButtons() {
        GameObject[] enemyButtons = GameObject.FindGameObjectsWithTag("PathButton_Enemy"); //Side note: Tags are assigned manually in inspector
        GameObject[] eventButtons = GameObject.FindGameObjectsWithTag("PathButton_Event");
        GameObject[] restButtons = GameObject.FindGameObjectsWithTag("PathButton_Rest");

        foreach (GameObject button in enemyButtons) enemyPathButtons.Add(button.GetComponent<Button>());
        foreach (GameObject button in eventButtons) eventPathButtons.Add(button.GetComponent<Button>());
        foreach (GameObject button in restButtons) restPathButtons.Add(button.GetComponent<Button>());

        AssignButtonIcons();
        AssignEnemyButtons();
        AssignRestButtons();
        AssignEventButtons();
    }

    void AssignButtonIcons() {
        foreach (Button button in enemyPathButtons) button.image.sprite = icons[0];
        foreach (Button button in eventPathButtons) button.image.sprite = icons[1];
        foreach (Button button in restPathButtons) button.image.sprite = icons[2];
    }

    void AssignEnemyButtons() {
        foreach (Button button in enemyPathButtons) button.onClick.AddListener(delegate {
            combatCanvas.SetActive(true);
            gameObject.SetActive(false);
            deckButton.SetActive(false);
            gM.StartCombat();
            //fadeCanvas.StartCoroutine(fadeCanvas.Fade());
        });
    }

    void AssignRestButtons() {
        foreach (Button button in restPathButtons) button.onClick.AddListener(delegate {
            ActivateRestView();
            //fadeCanvas.StartCoroutine(fadeCanvas.Fade());
        });
    }

    void AssignEventButtons() {
        foreach (Button button in eventPathButtons) button.onClick.AddListener(delegate {
            pEvent.ActivatePathView();
            //fadeCanvas.StartCoroutine(fadeCanvas.Fade());
        });
    }

    public void DrawPath(int pIndex) {
        //Add new empty point for TravelledLine linerender
        paths[3].positionCount += 1;

        //Select the lastly added point & draw line based on paths index & pathpoint
        paths[3].SetPosition(paths[3].positionCount - 1, paths[pIndex].GetPosition(pathPoint));
    }

    public void PlaceEnemiesOnPath() { //Not used, did not work as intended. All buttons had the same enemy in the end
        int randomIndex = 0;

        //Add button listener for every button in the paths, that assign a random enemy data to enemy
        foreach (Button button in enemyPathButtons) {
            randomIndex = Random.Range(0, gM.TierOneEnemyDatas.Length);
            Debug.Log($"Random index roll: {randomIndex} for {button.name}. {gM.TierOneEnemyDatas[randomIndex].enemyName}");
            button.onClick.AddListener(delegate { gM.Enemy.AssignDataValues(gM.TierOneEnemyDatas[randomIndex]); });
        }
    }

    #region Button functions
    public void SelectRandomEnemyData(Button button) { //Button press function. Used in PathButtons
        //int rand = Random.Range(0, gM.EnemyDatas.Length);
        int rand = 0;

        //After a button is pressed it also modifies the path point int. Therefore it can be used here to check what range of enemies can spawn on pressed button
        switch (pathPoint) {
            case < 5:
            Debug.Log($"PathPoint: {pathPoint} below below 5. Tier 1 enemy");
            rand = Random.Range(0, gM.TierOneEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.TierOneEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.TierOneEnemyDatas[rand]);
            break;

            case 5:
            Debug.Log($"PathPoint: {pathPoint}. MiniBoss time!");
            rand = Random.Range(0, gM.MiniBossEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.MiniBossEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.MiniBossEnemyDatas[rand]);
            break;

            case < 10:
            Debug.Log($"PathPoint: {pathPoint} below below 10. Tier 2 enemy");
            rand = Random.Range(0, gM.TierTwoEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.TierTwoEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.TierTwoEnemyDatas[rand]);
            break;

            case 10:
            Debug.Log($"PathPoint: {pathPoint}. MiniBoss time!");
            rand = Random.Range(0, gM.MiniBossEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.MiniBossEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.MiniBossEnemyDatas[rand]);
            break;

            case < 15:
            Debug.Log($"PathPoint: {pathPoint} below below 15. Tier 3 enemy");
            rand = Random.Range(0, gM.TierThreeEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.TierThreeEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.TierThreeEnemyDatas[rand]);
            break;

            case 15:
            Debug.Log($"PathPoint: {pathPoint}. MiniBoss time!");
            rand = Random.Range(0, gM.MiniBossEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.MiniBossEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.MiniBossEnemyDatas[rand]);
            break;

            case < 20:
            Debug.Log($"PathPoint: {pathPoint} below below 20. Tier 4 enemy");
            rand = Random.Range(0, gM.TierFourEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.TierFourEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.TierFourEnemyDatas[rand]);
            break;

            case 20:
            Debug.Log($"PathPoint: {pathPoint}. BOSS time! Selectin boss enemy from boss data");
            rand = Random.Range(0, gM.BossEnemyDatas.Length);
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.BossEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.BossEnemyDatas[rand]);
            break;
        }

        ////Second "same" switch to check data sheet to use at different points
        //switch (pathPoint) {
        //    case 20:
        //    Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.BossEnemyDatas[rand].enemyName}");
        //    gM.Enemy.AssignDataValues(gM.BossEnemyDatas[rand]);
        //    break;

        //    default:
        //    Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.TierOneEnemyDatas[rand].enemyName}");
        //    gM.Enemy.AssignDataValues(gM.TierOneEnemyDatas[rand]);
        //    break;
        //}
    }

    //----Used in Rest canvas buttons----
    public void Rest() {
        float restHeal = player.MaxHp * 0.3f;
        float healResult = player.Health + restHeal;

        if (healResult > player.MaxHp) {
            player.Health = player.MaxHp;
        }
        else {
            player.TakeHeal((int)restHeal);
        }
    }

    public void Pray() {
        player.MaxHp += 10;
        float restHeal = player.MaxHp * 0.1f;
        float healResult = player.Health + restHeal;

        if (healResult > player.MaxHp) {
            player.Health = player.MaxHp;
        }
        else {
            player.TakeHeal((int)restHeal);
        }
    }
    #endregion

    void ActivateRestView() {
        gameObject.SetActive(false);
        restCanvas.SetActive(true);
    }

    void ResetScriptables() {
        List<CardData> craftedDatas = new List<CardData>(Resources.LoadAll<CardData>("Crafted cards"));
        foreach (CardData data in craftedDatas) {
            //Bools
            data.draw = false;
            data.dealDamage = false;
            data.heal = false;
            data.block = false;
            data.recoverAp = false;
            data.buff = false;
            data.debuff = false;

            //Values
            data.drawAmount = 0;
            data.damage = 0;
            data.healAmount = 0;
            data.blockAmount = 0;
            data.aPRecoverAmount = 0;
            data.playCost = 0;
            data.buffDuration = 0;
            data.buffType = BuffType.None;
            data.debuffDuration = 0;
            data.debuffType = DebuffType.None;
        }

        GameObject[] prefabs = craftCanvas.GetComponent<CardCrafting>().CraftedCardPrefabs;
        foreach (GameObject prefab in prefabs) prefab.GetComponent<Card>().ComponentAmount = 0;
    }

    //Automatic scrolling for the start, seeing the whole path and realize you can scroll it (hopefully).
    IEnumerator MoveContentView() {
        ScrollRect scrollRect = FindObjectOfType<ScrollRect>();

        //float scrollDuration = 10f;
        float timer = 0f;
        float startValue = scrollRect.verticalNormalizedPosition;
        float endValue = 0f;

        while (timer < viewScrollTime) {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / viewScrollTime);
            yield return scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, endValue, t);
        }

        scrollRect.verticalNormalizedPosition = endValue;

        //Finally activate crafting view canvas
        craftCanvas.SetActive(true);
    }
}
