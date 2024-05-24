using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatusEffect { None, Stunned, Dazed }
public class Player : MonoBehaviour {
    [Header("Player stats")]
    [SerializeField][Range(0, 100)] int health;
    [SerializeField] int actionPoints;
    public int ActionPoints {
        get { return actionPoints; }
    }
    [SerializeField] StatusEffect statusEffect;

    [Header("Target related")]
    [SerializeField] GameObject selectedTarget; //Does nothing atm, maybe even remove later or handle some other way targeting

    void Start() {

    }

    public void UpdateTarget(GameObject newTarget) {
        selectedTarget = newTarget;
    }

    public void ReduceAP(int actionCost) {
        actionPoints -= actionCost;
    }

}
