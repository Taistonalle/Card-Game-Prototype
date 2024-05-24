using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy", order = 1)]
public class DataEnemy : ScriptableObject {
    public string enemyName;
    public int health;
    public Sprite enemyArt;
}
