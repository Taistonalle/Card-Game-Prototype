using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VersionNumberCanvas : MonoBehaviour {
    [SerializeField] TextMeshProUGUI versionTxt;

    void Start() {
        versionTxt.text = $"Build version: {Application.version}";

        if (FindObjectsOfType<VersionNumberCanvas>().Length > 1) {
            Debug.Log($"Destroying extra {gameObject.name}");
            Destroy(gameObject);
        }
        else DontDestroyOnLoad(gameObject);
    }
}
