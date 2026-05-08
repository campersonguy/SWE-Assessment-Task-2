using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameData : MonoBehaviour {

    public GameObject notebookUI;
    public GameObject uiArea;

    public bool toggleNotes;

    void Start() {
        uiArea.SetActive(false);   
    }

    void Update() {
        if (toggleNotes)
            notebookUI.SetActive(true);
        if (!toggleNotes)
            notebookUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
            toggleNotes = !toggleNotes;
    }
}
