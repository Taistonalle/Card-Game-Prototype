using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Event", menuName = "Scriptable Objects/Event", order = 1)]
public class EventData : ScriptableObject {
    public string eventName;
    [TextArea(5, 10)]
    public string eventDescription;
    public Sprite eventImgSprite;

    [Header("Choice texts")]
    public string choiceHeader;
    public string choiceOneHeaderTxt;
    [TextArea(2,5)]
    public string choiceOneDescriptionTxt;
    public string choiceTwoHeaderTxt;
    [TextArea(2, 5)]
    public string choiceTwoDescriptionTxt;
    public string choiceThreeHeaderTxt;
    [TextArea(2, 5)]
    public string choiceThreeDescriptionTxt;

    [Header("Choice result")] //Not one for three, idea is just for the third option to always ignore the event -> No result
    [TextArea(2, 5)]
    public string choiceOneGood;
    [TextArea(2,5)]
    public string choiceOneBad;
    [TextArea(2,5)]
    public string choiceTwoGood;
    [TextArea(2,5)]
    public string choiceTwoBad;
}
