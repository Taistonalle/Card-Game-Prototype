using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage Card", menuName = "Scriptable Objects/Damage Card", order = 1)]
public class DataDamageCard : ScriptableObject {
    public string cardName;
    public string description;
    public Sprite cardArt;
    public int damage;
    [Tooltip("How much it costs to play the card")] public int playCost;
}
