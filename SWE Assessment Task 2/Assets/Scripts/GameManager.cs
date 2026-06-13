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

        // Enemies        |  Name             |  Minimum Floor     |  Health           |  Damage          |  Speed             |  Flavour Text
        {0,  new ArrayList {"Slime",            1,                  20,                 1,                  2,                  "I sense !" } },
        {1,  new ArrayList {"Bat",              1,                  25,                 1,                  3,                  "I sense fluttering nearby!"} },
        {2,  new ArrayList {"",                 99} },
        {3,  new ArrayList {"",                 99} },
        {4,  new ArrayList {"",                 99} },
        {5,  new ArrayList {"",                 99} },

        // Bosses         |  Name             |  Minimum Floor     |  Health           |  Damage          |  Speed             |  Flavour Text
        {6,  new ArrayList {"The Wumpus",       1,                  60,                 2,                  1,                  "I smell a wumpus!" } },
        {7,  new ArrayList {"",                 3,                  2,                  2,                  1,                  "I smell!" } },
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


[System.Serializable] public class EnemyGroup {
    public int roomID;
    public int enemyCount;
    public List<int> enemyIDs = new List<int>();
    public List<Vector2> spawnOffsets = new List<Vector2>();
}


[System.Serializable] public class Trap {
    public int roomID;
    public int damage;
    public Vector2 offset;
}


public class GameManager : MonoBehaviour {

    [Header("References")]
    public PlayerController player;
    public GameObject baseEnemy;
    public GameObject[] obstacles;

    public Sprite[] enemySprites;

    [Header("UI")]
    public GameObject notebookUI;
    public GameObject arrowUI;

    public GameObject blackBackground;
    public GameObject redOverlay;

    public TextMeshPro[] arrowText;
    public TextMeshProUGUI roomText;

    public Image dialogueBox;
    public TextMeshProUGUI dialogueText;

    [Header("World")]
    public Vector2[] cavePositions;

    public int currentRoom;

    [Header("Enemy Groups")] public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

    [Header("Traps")]
    public GameObject trapPrefab;
    public List<Trap> traps = new List<Trap>();

    private Coroutine trapDamageRoutine;

    [Header("State")]
    public bool toggleNotes;

    private System.Random rng = new System.Random();
    private Image blackImage;
    private Animator anim;

    [Header("Sprites")]
    public Sprite normalCave;
    public Sprite trapCave;

    void Start() {
        anim = GetComponent<Animator>();
        blackImage = blackBackground.GetComponent<Image>();

        SetUI();

        SpawnEnemies();
        SpawnTraps();
    }

    void Update() {
        notebookUI.SetActive(toggleNotes);
        arrowUI.SetActive(!toggleNotes);

        if (Input.GetKeyDown(KeyCode.Escape))
            toggleNotes = !toggleNotes;

        if (Input.GetKeyDown(KeyCode.Tab))
            RoomCheck();
    }

    // ---------------------------------------------------------
    // ENEMY SPAWNING
    // ---------------------------------------------------------

    private void SpawnEnemies() {
        GenerateRandomEnemyGroups();

        foreach (EnemyGroup group in enemyGroups) {
            for (int i = 0; i < group.enemyCount; i++) {
                GameObject enemyObj = Instantiate(baseEnemy);
                Enemies e = enemyObj.GetComponent<Enemies>();

                int id = group.enemyIDs[i];

                if (Data.obstacles[id].Count < 6) {
                    Debug.LogError($"Enemy ID {id} has incomplete data!");
                    continue;
                }

                e.currentRoom = group.roomID;
                e.enemyName = Data.obstacles[id][0].ToString();
                e.maxHealth = int.Parse(Data.obstacles[id][2].ToString());
                e.damage = int.Parse(Data.obstacles[id][3].ToString());
                e.moveSpeed = int.Parse(Data.obstacles[id][4].ToString());
                e.flavourText = Data.obstacles[id][5].ToString();

                if (enemySprites[id] != null)
                    enemyObj.GetComponent<SpriteRenderer>().sprite = enemySprites[id];
            }
        }
    }

    private int GetEnemyID(int index) {
        // Your special-case logic preserved
        return index switch {
            0 or 1 => rng.Next(0, 2),
            2 => rng.Next(6, 7),
            _ => rng.Next(0, 2),
        };
    }

    private void GenerateRandomEnemyGroups() {
        enemyGroups.Clear();

        HashSet<int> usedRooms = new HashSet<int>();

        for (int g = 0; g < 4; g++) {
            EnemyGroup group = new EnemyGroup();

            // Pick a unique room
            int room;
            do {
                room = rng.Next(2, 21); // 2–20
            } while (usedRooms.Contains(room));

            usedRooms.Add(room);
            group.roomID = room;

            // Random number of enemies (3–6)
            group.enemyCount = rng.Next(3, 7);

            for (int i = 0; i < group.enemyCount; i++) {
                int id = rng.Next(0, 2); // enemy IDs 0–1
                group.enemyIDs.Add(id);

                float x = UnityEngine.Random.Range(-1.5f, 1.5f);
                float y = UnityEngine.Random.Range(-1.0f, 1.0f);
                group.spawnOffsets.Add(new Vector2(x, y));
            }

            enemyGroups.Add(group);
        }

        HashSet<int> enemyRooms = new HashSet<int>();

        foreach (EnemyGroup g in enemyGroups) {
            enemyRooms.Add(g.roomID);
        }
    }

    private void SpawnTraps() {
        traps.Clear();

        // Collect enemy rooms so traps avoid them
        HashSet<int> enemyRooms = new HashSet<int>();
        foreach (EnemyGroup g in enemyGroups)
            enemyRooms.Add(g.roomID);

        HashSet<int> usedTrapRooms = new HashSet<int>();

        for (int i = 0; i < 2; i++) {
            Trap t = new Trap();

            int room;
            do {
                room = rng.Next(2, 21); // 2–20
            }
            while (enemyRooms.Contains(room) || usedTrapRooms.Contains(room));

            usedTrapRooms.Add(room);
            t.roomID = room;

            t.damage = 2;

            float x = UnityEngine.Random.Range(-0.5f, 0.5f);
            float y = UnityEngine.Random.Range(-0.5f, 0.5f);
            t.offset = new Vector2(x, y);

            traps.Add(t);

            GameObject trapObj = Instantiate(trapPrefab);
        }
    }

    // ---------------------------------------------------------
    // PLAYER MOVEMENT
    // ---------------------------------------------------------

    public IEnumerator MovePlayer(int arrowNum) {
        currentRoom = Data.rooms[currentRoom][arrowNum];
        yield return StartCoroutine(FadeTransition(arrowNum));

        yield return StartCoroutine(CheckForTrapDamage());
    }

    private IEnumerator CheckForTrapDamage() {
        bool inTrapRoom = false;

        foreach (Trap t in traps) {
            if (t.roomID == currentRoom) {
                inTrapRoom = true;

                yield return StartCoroutine(Dialogue($"*beep*\n\nSCANNING...   SCANNING...   SCANNING... \nYOU ARE IN A TRAPPED ROOM! NOXIOUS GAS DETECTED! OH... AND ALSO, SPIKES!\nLEAVE AS SOON AS POSSIBLE! LEAVE AS SOON AS POSSIBLE!\n\n*beep*"));

                // Start DoT if not already running
                if (trapDamageRoutine == null)
                    trapDamageRoutine = StartCoroutine(TrapDamageOverTime(t.damage));
            }
        }

        // If we left the trap room, stop the DoT
        if (!inTrapRoom && trapDamageRoutine != null) {
            StopCoroutine(trapDamageRoutine);
            trapDamageRoutine = null;
        }
    }

    private IEnumerator TrapDamageOverTime(int damage) {
        int stack = 1;

        while (true) {
            player.StartCoroutine(player.TakeDamage(damage * stack));
            stack++;
            yield return new WaitForSeconds(4f);
        }
    }

    // ---------------------------------------------------------
    // ROOM CHECKS
    // ---------------------------------------------------------

    private void CheckRoomType() {
        if (GetTrapRooms().Contains(currentRoom))
            GetComponent<SpriteRenderer>().sprite = trapCave;
        else
            GetComponent<SpriteRenderer>().sprite = normalCave;
    }

    public List<int> GetEnemyRooms() {
        List<int> rooms = new List<int>();

        foreach (EnemyGroup g in enemyGroups)
            rooms.Add(g.roomID);

        return rooms;
    }

    public List<int> GetTrapRooms() {
        List<int> rooms = new List<int>();

        foreach (Trap t in traps)
            rooms.Add(t.roomID);

        return rooms;
    }

    void RoomCheck() {
        List<int> enemyRooms = GetEnemyRooms();
        List<int> trapRooms = GetTrapRooms();

        int enemyCount = 0;
        int trapCount = 0;

        // Count adjacent enemy rooms
        foreach (int room in enemyRooms)
            if (Data.rooms[currentRoom].Contains(room))
                enemyCount++;

        // Count adjacent trap rooms
        foreach (int room in trapRooms)
            if (Data.rooms[currentRoom].Contains(room))
                trapCount++;

        // Build the message
        List<string> parts = new List<string>();

        if (enemyCount == 1) parts.Add("AN ENEMY GROUP");
        else if (enemyCount > 1) parts.Add("ENEMY GROUPS");

        if (trapCount == 1) parts.Add("A TRAP");
        else if (trapCount > 1) parts.Add("TRAPS");

        string adjacents;

        if (parts.Count == 0)
            adjacents = "NOTHING";
        else
            adjacents = string.Join(" AND ", parts);

        StartCoroutine(Dialogue($"*beep*\n\nSCANNING...   SCANNING...   SCANNING...\n" +$"{adjacents} DETECTED IN ADJACENT ROOMS!\n\n*beep*"));
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
        player.inputLock = true;

        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        SetUI();

        player.inputLock = false;
        player.transform.position = cavePositions[arrowNum];

        CheckRoomType();

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

        redOverlay.SetActive(GetTrapRooms().Contains(currentRoom));
    }
}