using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { None, Dodge, Strenght }
public enum DebuffType { None, Stun, Weakness }
[CreateAssetMenu(fileName = "Damage Card", menuName = "Scriptable Objects/Damage Card", order = 1)]
public class CardData : ScriptableObject {
    public string cardName;
    public string description;
    public Sprite cardBackground;
    public Color backgroundColor;
    public Sprite cardImage;
    public Sprite[] cardBorders;
    public int damage, healAmount, blockAmount;
    public int playCost;

    [Header("What can the card do?")]
    public bool draw;
    public bool dealDamage;
    public bool heal;
    public bool block;
    public bool buff;
    public BuffType buffType;
    public bool debuff;
    public DebuffType debuffType;
}
