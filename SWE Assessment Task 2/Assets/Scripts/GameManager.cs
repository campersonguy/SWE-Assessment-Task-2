using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;


public static class Data {

    public static Dictionary<int, List<int>> rooms = new Dictionary<int, List<int>> {
        {1, new List<int>  { 2,  3,  6  } },
        {2, new List<int>  { 1,  4,  5  } },
        {3, new List<int>  { 1,  7,  19 } },
        {4, new List<int>  { 2,  7,  8  } },
        {5, new List<int>  { 2,  9,  10 } },
        {6, new List<int>  { 1,  10, 20 } },
        {7, new List<int>  { 3,  4,  11 } },
        {8, new List<int>  { 4,  9,  12 } },
        {9, new List<int>  { 8,  5,  13 } },
        {10, new List<int> { 5,  6,  14 } },
        {11, new List<int> { 7,  12, 16 } },
        {12, new List<int> { 8,  11, 15 } },
        {13, new List<int> { 9,  15, 14 } },
        {14, new List<int> { 10, 15, 18 } },
        {15, new List<int> { 12, 13, 17 } },
        {16, new List<int> { 11, 17, 19 } },
        {17, new List<int> { 15, 16, 18 } },
        {18, new List<int> { 14, 17, 20 } },
        {19, new List<int> { 3,  16, 20 } },
        {20, new List<int> { 6,  18, 19 } },  // extremely unoptimised fix ts later
    };

    public static Dictionary<int, ArrayList> things = new Dictionary<int, ArrayList> {

        // Enemies        |  Name             |  Minimum Floor    |  Spawn Weight     |  Damage           |  Flavour Text
        {0, new ArrayList {"The Boing",        1,                  1,                  1,                  "" } },
        {1, new ArrayList {} },
        {2, new ArrayList {} },
        {3, new ArrayList {} },
        {4, new ArrayList {} },
        {5, new ArrayList {} },

        // Bosses         |  Name             |  Minimum Floor    |  Spawn Weight     |  Flavour Text
        {6, new ArrayList {"The Wumpus",       1,                  1,                  "I smell a wumpus!" } },
        {7, new ArrayList {"The Bunga Booga",  3,                  2,                  "I smell hot dogs!" } },
        {8, new ArrayList {""   } },
        {9, new ArrayList {} },
        {10, new ArrayList {} },
        {11, new ArrayList {} },

        // Bosses         |  Name             |  Minimum Floor    |  Spawn Weight     |  Flavour Text
        {12, new ArrayList {"The Wumpus",       1,                  1,                  "I smell a wumpus!" } },
        {13, new ArrayList {} },
        {14, new ArrayList {""   } },
        {15, new ArrayList {} },
        {16, new ArrayList {} },
        {17, new ArrayList {} },
    };

    public static int currentRoom = 1;
    public static int floor = 1;

    public static List<int> enemyLocations = new List<int> { 0, 0, 0, 0, 0, 0 };

}


public class GameManager : MonoBehaviour {

    public GameObject notebookUI;
    public GameObject arrowUI;
    public GameObject uiArea;

    public GameObject blackBackground;

    public TextMeshProUGUI[] arrowText;
    public TextMeshProUGUI roomText;
    public TextMeshProUGUI transitionText;

    public bool toggleNotes;

    private System.Random random = new System.Random();


    void Start() {
        SetUI();

        for (int i = 0; i < 6; i++) {
            int randomNum = random.Next(2, 21);

            Data.enemyLocations[i] = randomNum;
            Debug.Log(Data.enemyLocations[i]);
        }
    }


    void Update() {
        notebookUI.SetActive(toggleNotes);  // Toggles the notebook and HUD visibility
        arrowUI.SetActive(!toggleNotes);

        if (Input.GetKeyDown(KeyCode.Escape))  // Keybind to hide the Notebook
            toggleNotes = !toggleNotes;
    }


    public void MovePlayer(int arrowNum) {  // code for moving the player (will be more later trust me)
        Data.currentRoom = Data.rooms[Data.currentRoom][arrowNum];
        StartCoroutine(fadeOut());  // fades in black background
    }


    IEnumerator fadeOut() {  // fades in black background
        Image blackImage = blackBackground.GetComponent<Image>();
        blackBackground.SetActive(true);

        for (int i = 1; i <= 25; i++) {  // increases the alpha value slowly
            Color tempColor = blackImage.color;
            tempColor.a += 0.04f;
            blackImage.color = tempColor;
            
            yield return null;
        }

        SetUI();

        string line = "";

        List<int> adjacentRooms = Data.rooms[Data.currentRoom];
        List<int> enemyRooms = Data.enemyLocations;

        for (int i = 0; i < 6; i++) {
            if (adjacentRooms.Contains(enemyRooms[i])) {
                line += "\nI sense an enemy nearby!";
                
                if (i >= 3 && i < 5)
                    line += "\nI sense a hazard nearby!";

                if (i == 5)
                    line += "\nI sense a boss nearby!";
            }

            if (Data.currentRoom == enemyRooms[i])
                line += "\nBro you are COOKED...";
        }

        StartCoroutine(typeWrite(transitionText, line));
        yield return new WaitForSeconds(line.Length * 0.05f + 2f);  // waits the time for the text to type plus 3s

        for (int i = 1; i <= 25; i++) {  // reduces the alpha value slowly
            Color tempColor = blackImage.color;
            tempColor.a -= 0.04f;
            blackImage.color = tempColor;
            
            yield return null;
        }

        blackBackground.SetActive(false);
    }


    IEnumerator typeWrite(TextMeshProUGUI textBox, string line) {  // typewriter text effect
        textBox.enabled = true;

        textBox.text = line;
        textBox.ForceMeshUpdate();  // forces mesh update to remove issues with character count

        int totalCharacters = textBox.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++) {
            textBox.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.05f);  // waits 0.05s between inputs
        }

        yield return new WaitForSeconds(2f);  // waits an extra 2s

        textBox.enabled = false;
    }

    public void SetUI() {
        for (int i = 0; i < 3; i++) {  // Display for arrows to go to different rooms
            arrowText[i].text = $"Travel to room {Data.rooms[Data.currentRoom][i]}";
        } 

        roomText.text = $"You are in Room {Data.currentRoom}, Floor {Data.floor}";  // The current room/floor text
    }
}
