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

    public PlayerController player;
    public Enemies enemy;

    public GameObject[] obstacles;
    public GameObject baseEnemy;

    public GameObject notebookUI;
    public GameObject arrowUI;
    public GameObject uiArea;
    
    public GameObject playerSprite;

    public GameObject blackBackground;

    public TextMeshPro[] arrowText;
    public TextMeshProUGUI roomText;

    public Vector2[] cavePositions;

    public Image dialogueBox;
    public TextMeshProUGUI dialogueText;

    public bool toggleNotes;

    private System.Random random = new System.Random();

    public Animator animator;


    void Start() {
        SetUI();

        for (int i = 0; i < 5; i++) {
            GameObject enemy1 = Instantiate(baseEnemy);
            Enemies enemies = enemy1.GetComponent<Enemies>();

            int randomNum = 0;

            while (true) {
                randomNum = random.Next(2, 21);

                if (!Data.enemyLocations.Contains(randomNum) || !Data.rooms[1].Contains(randomNum)) {
                    enemies.currentRoom = randomNum;
                    break;
                }
            }

            if (i == 0 || i == 1)
                randomNum = random.Next(0, 2);
            if (i == 2 || i == 3)
                randomNum = random.Next(12, 14);
            if (i == 4)
                randomNum = random.Next(6, 7);

            enemies.enemyName = Data.obstacles[randomNum][0].ToString();
            enemies.damage = int.Parse(Data.obstacles[randomNum][3].ToString());
            enemies.flavourText = Data.obstacles[randomNum][4].ToString();

            enemy1.name = enemies.enemyName;
        }
    }


    void Update() {
        notebookUI.SetActive(toggleNotes);  // Toggles the notebook and HUD visibility
        arrowUI.SetActive(!toggleNotes);

        if (Input.GetKeyDown(KeyCode.Tab))  // Keybind to hide the Notebook
            toggleNotes = !toggleNotes;

        if (Input.GetKeyDown(KeyCode.L))
        StartCoroutine(dialogue("d"));
    }


    public void MovePlayer(int arrowNum) {  // code for moving the player (will be more later trust me)
        player.currentRoom = Data.rooms[player.currentRoom][arrowNum];
        StartCoroutine(fadeOut(arrowNum));  // fades in black background
    }


    IEnumerator dialogue(string text) {
        player.movementLock = true;
        
        player.anim.SetTrigger("listen");
        yield return new WaitForSeconds(2);
        StartCoroutine(typeWrite(dialogueText, text));
        yield return new WaitForSeconds(text.Length * 0.05f + 3f);
        player.anim.SetTrigger("stopListen");

        yield return new WaitForSeconds(2.8f);

        player.movementLock = false;
    }


    IEnumerator fadeOut(int arrowNum) {  // fades in black background
        Image blackImage = blackBackground.GetComponent<Image>();
        blackBackground.SetActive(true);

        for (int i = 1; i <= 50; i++) {  // increases the alpha value slowly
            Color tempColor = blackImage.color;
            tempColor.a += 0.02f;
            blackImage.color = tempColor;
            
            yield return new WaitForSeconds(0.01f);
        }

        SetUI();

        yield return new WaitForSeconds(0.3f);

        player.transform.position = cavePositions[arrowNum];

        for (int i = 1; i <= 50; i++) {  // reduces the alpha value slowly
            Color tempColor = blackImage.color;
            tempColor.a -= 0.02f;
            blackImage.color = tempColor;
            
            yield return new WaitForSeconds(0.01f);
        }

        blackBackground.SetActive(false);
    }


    IEnumerator typeWrite(TextMeshProUGUI textBox, string line) {  // typewriter text effect
        textBox.enabled = true;
        dialogueBox.enabled = true;

        textBox.text = line;
        textBox.ForceMeshUpdate();  // forces mesh update to remove issues with character count

        int totalCharacters = textBox.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++) {
            textBox.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.05f);  // waits 0.05s between inputs
        }

        yield return new WaitForSeconds(2f);  // waits an extra 2s

        textBox.enabled = false;
        dialogueBox.enabled = false;
    }

    public void SetUI() {
        for (int i = 0; i < 3; i++) {  // Display for arrows to go to different rooms
            arrowText[i].text = $"Room {Data.rooms[player.currentRoom][i]}";
        } 

        roomText.text = $"You are in Room {player.currentRoom}, Floor {Data.floor}";  // The current room/floor text
    }
}
