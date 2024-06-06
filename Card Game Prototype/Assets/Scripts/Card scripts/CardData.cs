using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType { None, Dodge, Strenght }
public enum DebuffType { None, Stun, Weakness }
[CreateAssetMenu(fileName = "Damage Card", menuName = "Scriptable Objects/Card", order = 1)]
public class CardData : ScriptableObject {
    [Header("Card visuals")]
    public string cardName;
    [TextArea(3, 5)]
    public string description;
    public Sprite cardBackground;
    public Color backgroundColor;
    public Sprite cardImage;
    public Sprite[] cardBorders;

    [Header("Card values")]
    public int drawAmount;
    public int damage;
    public int healAmount;
    public int blockAmount;
    public int aPRecoverAmount;
    public int playCost;

    [Header("What can the card do?")]
    public bool draw;
    public bool dealDamage;
    public bool heal;
    public bool block;
    public bool recoverAp;
    public bool buff;
    public BuffType buffType;
    public bool debuff;
    public DebuffType debuffType;
}
