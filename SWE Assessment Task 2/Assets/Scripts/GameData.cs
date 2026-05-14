using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public static class Data {

    static Dictionary<int, List<int>> rooms = new Dictionary<int, List<int>> {
        {1, new List<int>  { 3,  2,  6  } },
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

    static Dictionary<int, ArrayList> things = new Dictionary<int, ArrayList> {

        // Enemies        |  Name             |  Minimum Floor    |  Spawn Weight     |  Damage           |  Flavour Text
        {0, new ArrayList {"The Boing",        1,                  1,                  1,                  "" } },
        {1, new ArrayList {} },
        {2, new ArrayList {} },
        {3, new ArrayList {} },
        {4, new ArrayList {} },
        {5, new ArrayList {} },

        // Bosses         |  Name             |  Minimum Floor    |  Spawn Weight     |  Flavour Text
        {0, new ArrayList {"The Wumpus",       1,                  1,                  "I smell a wumpus!" } },
        {1, new ArrayList {"The Bunga Booga",  3,                  2,                  "I smell hot dogs!" } },
        {2, new ArrayList {""   } },
        {3, new ArrayList {} },
        {4, new ArrayList {} },
        {5, new ArrayList {} },

        // Bosses         |  Name             |  Minimum Floor    |  Spawn Weight     |  Flavour Text
        {0, new ArrayList {"The Wumpus",       1,                  1,                  "I smell a wumpus!" } },
        {1, new ArrayList {} },
        {2, new ArrayList {""   } },
        {3, new ArrayList {} },
        {4, new ArrayList {} },
        {5, new ArrayList {} },
    };

    // static int currentRoom = 1;
    // static int floor = 1;
}


public class GameData : MonoBehaviour {

    public GameObject notebookUI;
    public GameObject uiArea;

    public bool toggleNotes;


    void Update() {
        if (toggleNotes)
            notebookUI.SetActive(true);
        if (!toggleNotes)
            notebookUI.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Escape))
            toggleNotes = !toggleNotes;
    }
}
