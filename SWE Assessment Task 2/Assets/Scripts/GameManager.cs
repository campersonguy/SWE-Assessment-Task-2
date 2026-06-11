using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;


public static class Data {

    public static Dictionary<int, List<int>> rooms = new Dictionary<int, List<int>> {
        {1, new List<int>  { 6,  2,  3 } },
        {2, new List<int>  { 5,  1,  4 } },
        {3, new List<int>  { 19, 7,  1 } },
        {4, new List<int>  { 7,  8,  2 } },
        {5, new List<int>  { 2,  9,  10} },
        {6, new List<int>  { 1,  10, 20} },
        {7, new List<int>  { 4,  3,  11} },
        {8, new List<int>  { 12, 4,  9 } },
        {9, new List<int>  { 13, 5,  8 } },
        {10, new List<int> { 14, 6,  5 } },
        {11, new List<int> { 16, 12, 7 } },
        {12, new List<int> { 8,  11, 15} },
        {13, new List<int> { 9,  15, 14} },
        {14, new List<int> { 10, 18, 13} },
        {15, new List<int> { 17, 13, 12} },
        {16, new List<int> { 11, 17, 19} },
        {17, new List<int> { 15, 16, 18} },
        {18, new List<int> { 20, 14, 17} },
        {19, new List<int> { 3,  20, 16} },
        {20, new List<int> { 18, 19, 6 } },
    };

    public static Dictionary<int, ArrayList> obstacles = new Dictionary<int, ArrayList> {

        // Enemies        |  Name             |  Minimum Floor    |  Spawn Weight     |  Damage           |  Flavour Text
        {0,  new ArrayList {"Goblin",           1,                  1,                  1,                  "I sense !" } },
        {1,  new ArrayList {"Bat",              1,                  1,                  1,                  "I sense !"} },
        {2,  new ArrayList {"",                 99} },
        {3,  new ArrayList {"",                 99} },
        {4,  new ArrayList {"",                 99} },
        {5,  new ArrayList {"",                 99} },

        // Bosses         |  Name             |  Minimum Floor    |  Spawn Weight     |  Damage           |  Flavour Text
        {6,  new ArrayList {"The Wumpus",       1,                  1,                  99,                 "I smell a wumpus!" } },
        {7,  new ArrayList {"",                 3,                  2,                  99,                 "I smell!" } },
        {8,  new ArrayList {"",                 99} },
        {9,  new ArrayList {"",                 99} },
        {10, new ArrayList {"",                 99} },
        {11, new ArrayList {"",                 99} },

        // Hazards         |  Name             |  Minimum Floor    |  Spawn Weight     |  Damage           |  Flavour Text
        {12, new ArrayList {"Bottomless Pit",   1,                  1,                  99,                 "I feel a draft!" } },
        {13, new ArrayList {"Superbats",        1,                  1,                  99,                 "I feel fluttering nearby!"} },
        {14, new ArrayList {"",                 99   } },
        {15, new ArrayList {"",                 99} },
        {16, new ArrayList {"",                 99} },
        {17, new ArrayList {"",                 99} },
    };

    public static Dictionary<int, string> damageText = new Dictionary<int, string> {
        {0,  "The Boing bounced into you! You took 1 damage.\nThe Boing bounced away..." },
        {1,  "" },
        {6,  "The Wumpus bumped you! You took 4 damage.\nThe Wumpus is on the move..."},
        {7,  ""},
        {12, "You fell into a bottomless pit! You took all of the damage.\nYou died! HAHHAHAAH LOSER"},
        {13, "You were carried away by Superbats! You suck IDIOT"},
    };

    public static int floor = 1;

    public static List<int> enemies = new List<int> {};
    public static List<int> enemyLocations = new List<int> {};
}


public class GameManager : MonoBehaviour {

    [Header("References")]
    public PlayerController player;
    public GameObject baseEnemy;
    public GameObject[] obstacles;

    [Header("UI")]
    public GameObject notebookUI;
    public GameObject arrowUI;
    public GameObject uiArea;
    public GameObject blackBackground;

    public TextMeshPro[] arrowText;
    public TextMeshProUGUI roomText;

    public Image dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("World")]
    public Vector2[] cavePositions;

    public int currentRoom;

    [Header("State")]
    public bool toggleNotes;

    private System.Random rng = new System.Random();
    private Image blackImage;
    private Animator anim;

    void Start() {
        anim = GetComponent<Animator>();
        blackImage = blackBackground.GetComponent<Image>();

        SetUI();
        SpawnEnemies();
    }

    void Update() {
        notebookUI.SetActive(toggleNotes);
        arrowUI.SetActive(!toggleNotes);

        if (Input.GetKeyDown(KeyCode.Tab))
            toggleNotes = !toggleNotes;

        if (Input.GetKeyDown(KeyCode.L))
            StartCoroutine(Dialogue("d"));
    }

    // ---------------------------------------------------------
    // ENEMY SPAWNING
    // ---------------------------------------------------------

    private void SpawnEnemies() {
        for (int i = 0; i < 5; i++) {
            GameObject enemyObj = Instantiate(baseEnemy);
            Enemies e = enemyObj.GetComponent<Enemies>();

            int id = GetEnemyID(i);
            
            e.currentRoom = rng.Next(1, 21);

            e.enemyName = Data.obstacles[id][0].ToString();
            e.damage = int.Parse(Data.obstacles[id][3].ToString());
            e.flavourText = Data.obstacles[id][4].ToString();

            enemyObj.name = e.enemyName;
        }
    }

    private int GetEnemyID(int index) {
        // Your special-case logic preserved
        return index switch {
            0 or 1 => rng.Next(0, 2),
            2 or 3 => rng.Next(12, 14),
            4 => rng.Next(6, 7),
            _ => rng.Next(2, 21)
        };
    }

    // ---------------------------------------------------------
    // PLAYER MOVEMENT
    // ---------------------------------------------------------

    public void MovePlayer(int arrowNum) {
        currentRoom = Data.rooms[currentRoom][arrowNum];
        StartCoroutine(FadeTransition(arrowNum));
    }

    // ---------------------------------------------------------
    // DIALOGUE
    // ---------------------------------------------------------

    IEnumerator Dialogue(string text) {
        player.movementLock = true;

        player.anim.SetTrigger("listen");
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(TypeWrite(dialogueText, text));

        player.anim.SetTrigger("stopListen");
        yield return new WaitForSeconds(2.8f);

        player.movementLock = false;
    }

    IEnumerator TypeWrite(TextMeshProUGUI box, string line) {
        dialogueBox.enabled = true;
        box.enabled = true;

        box.text = line;
        box.ForceMeshUpdate();

        int total = box.textInfo.characterCount;

        for (int i = 0; i <= total; i++) {
            box.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(2f);

        box.enabled = false;
        dialogueBox.enabled = false;
    }

    // ---------------------------------------------------------
    // FADE TRANSITION
    // ---------------------------------------------------------

    IEnumerator FadeTransition(int arrowNum) {
        blackBackground.SetActive(true);

        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        SetUI();

        player.transform.position = cavePositions[arrowNum];

        yield return StartCoroutine(Fade(1f, 0f, 1.5f));

        blackBackground.SetActive(false);
    }

    IEnumerator Fade(float start, float end, float duration) {
        float t = 0f;

        while (t < duration) {
            float a = Mathf.Lerp(start, end, t / duration);
            blackImage.color = new Color(0, 0, 0, a);
            t += Time.deltaTime;
            yield return null;
        }

        blackImage.color = new Color(0, 0, 0, end);
    }

    // ---------------------------------------------------------
    // UI
    // ---------------------------------------------------------

    public void SetUI() {
        for (int i = 0; i < 3; i++)
            arrowText[i].text = $"Room {Data.rooms[currentRoom][i]}";

        roomText.text = $"You are in Room {currentRoom}, Floor {Data.floor}";
    }
}