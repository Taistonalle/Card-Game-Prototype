using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathProgression : MonoBehaviour {
    [Header("Legend icons")]
    [SerializeField] Sprite[] icons;

    [Header("Path buttons")]
    [SerializeField] List<Button> pathButtons = new();

    [Header("Path line renderers")]
    [SerializeField] LineRenderer[] paths;

    int pathPoint = 0; //Used to determine what point does path linerender draw for TravelledLine
    public int PathPoint {
        get { return pathPoint; }
        set { pathPoint = value; }
    }

    GameManager gM;

    void Start() {
        AssignButtonIcons();
        gM = FindObjectOfType<GameManager>();
    }

    void AssignButtonIcons() {
        foreach (Button button in pathButtons) {
            button.image.sprite = icons[0];
        }
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
        foreach (Button button in pathButtons) {
            randomIndex = Random.Range(0, gM.EnemyDatas.Length);
            Debug.Log($"Random index roll: {randomIndex} for {button.name}. {gM.EnemyDatas[randomIndex].enemyName}");
            button.onClick.AddListener(delegate { gM.Enemy.AssignDataValues(gM.EnemyDatas[randomIndex]); });
        }
    }

    public void SelectRandomEnemyData(Button button) { //Button press function. Used in PathButtons
        int rand = Random.Range(0, gM.EnemyDatas.Length);

        Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.EnemyDatas[rand].enemyName}");
        gM.Enemy.AssignDataValues(gM.EnemyDatas[rand]);
    }
}
