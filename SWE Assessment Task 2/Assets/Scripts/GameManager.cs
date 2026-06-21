using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class Data {

    public static Dictionary<int, List<int>> rooms = new Dictionary<int, List<int>> {
        {1,  new List<int> { 6,  2,  3 } },
        {2,  new List<int> { 5,  1,  4 } },
        {3,  new List<int> { 19, 7,  1 } },
        {4,  new List<int> { 7,  8,  2 } },
        {5,  new List<int> { 2,  9,  10} },
        {6,  new List<int> { 1,  10, 20} },
        {7,  new List<int> { 4,  3,  11} },
        {8,  new List<int> { 12, 4,  9 } },
        {9,  new List<int> { 13, 5,  8 } },
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

    // Enemies: Health | Damage | Attack Speed | Attack Range | Speed
    public static Dictionary<int, List<float>> enemies = new Dictionary<int, List<float>> {
        {1, new List<float> { 30, 1,   1f,   1.1f, 2 } }, // Slime
        {2, new List<float> { 25, 1,   0.8f, 1.4f, 3 } }, // Bat
        {3, new List<float> { 160,1,   1.5f, 2f,   3 } }, // The Wumpus
    };
}

[Serializable]
public class EnemyGroup {
    public int roomID;
    public int enemyCount;
    public List<int> enemyIDs = new List<int>();
    public bool isBossRoom = false;
}

[Serializable]
public class Trap {
    public int roomID;
    public int damage;
}

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    // ---------------------------------------------------------
    // REFERENCES
    // ---------------------------------------------------------

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject baseEnemy;
    [SerializeField] private GameObject baseBoss;

    [SerializeField] private GameObject[] obstacles;
    [SerializeField] private Sprite[] enemySprites;

    [Header("UI")]
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject arrowUI;

    [SerializeField] private GameObject blackBackground;
    [SerializeField] private GameObject redOverlay;

    [SerializeField] private TextMeshPro[] arrowText;
    [SerializeField] private TextMeshProUGUI roomText;
    [SerializeField] private TextMeshProUGUI clearText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI endScreenText;

    [SerializeField] private Image dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private GameObject deathOverlay;

    [Header("World")]
    [SerializeField] private Vector2[] caveSpawnPositions;
    [SerializeField] private float timer;
    public int currentRoom;

    [Header("Enemy Groups")]
    [SerializeField] private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
    [SerializeField] private Dictionary<int, int> groupEnemyCounts = new Dictionary<int, int>();
    public HashSet<int> clearedGroups = new HashSet<int>();

    [Header("Traps")]
    [SerializeField] private GameObject trapPrefab;
    [SerializeField] private List<Trap> traps = new List<Trap>();
    [SerializeField] private Coroutine trapDamageRoutine;

    [Header("State")]
    [SerializeField] private bool toggleMap;
    [SerializeField] private bool checkingRooms;

    [SerializeField] private bool gameEnd;

    [Header("Internal")]
    [SerializeField] private System.Random rng = new System.Random();
    [SerializeField] private Image blackImage;
    [SerializeField] private Animator anim;

    [Header("Sprites")]
    [SerializeField] private Sprite normalCave;
    [SerializeField] private Sprite trapCave;

    // ---------------------------------------------------------
    // LIFECYCLE
    // ---------------------------------------------------------

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start() {
        anim = GetComponent<Animator>();
        blackImage = blackBackground.GetComponent<Image>();

        SpawnEnemies();
        SpawnTraps();

        blackBackground.SetActive(true);
        StartCoroutine(Fade(1f, 0f, 5f));
    }

    private void Update() {
        map.SetActive(toggleMap);

        if (Input.GetKeyDown(KeyCode.M))
            toggleMap = !toggleMap;

        if (Input.GetKeyDown(KeyCode.N))
            StartCoroutine(End(timer));

        if (Input.GetKeyDown(KeyCode.Tab) && !checkingRooms)
            RoomCheck();


        if (clearedGroups.Count == 1 && !gameEnd) {
            gameEnd = true;

            StopAllCoroutines();
            StartCoroutine(End(timer));
        }

        SetUI();
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
    }

    // ---------------------------------------------------------
    // ENEMY SPAWNING
    // ---------------------------------------------------------

    private void SpawnEnemies() {
        GenerateRandomEnemyGroups();
        SpawnBoss();

        foreach (EnemyGroup group in enemyGroups) {
            if (group.isBossRoom)
                continue;

            for (int i = 0; i < group.enemyCount; i++) {
                int id = group.enemyIDs[i];

                if (Data.enemies[id].Count < 5) {
                    Debug.LogError($"Enemy ID {id} has incomplete data!");
                    continue;
                }

                GameObject enemyObj = Instantiate(baseEnemy);
                Enemies e = enemyObj.GetComponent<Enemies>();

                e.currentRoom = group.roomID;
                e.groupID = group.roomID;

                e.maxHealth      = Data.enemies[id][0];
                e.damage         = Data.enemies[id][1];
                e.attackCooldown = Data.enemies[id][2];
                e.attackRange    = Data.enemies[id][3];
                e.moveSpeed      = Data.enemies[id][4];

                SpriteRenderer sr = enemyObj.GetComponent<SpriteRenderer>();
                sr.sprite = enemySprites[id - 1];

                if (!groupEnemyCounts.ContainsKey(group.roomID))
                    groupEnemyCounts[group.roomID] = 0;

                groupEnemyCounts[group.roomID]++;
            }
        }
    }

    private void GenerateRandomEnemyGroups() {
        enemyGroups.Clear();

        HashSet<int> usedRooms = new HashSet<int>();

        for (int g = 0; g < 4; g++) {
            EnemyGroup group = new EnemyGroup();

            int room;
            do {
                room = rng.Next(2, 21); // 2–20
            } while (usedRooms.Contains(room));

            usedRooms.Add(room);
            group.roomID = room;

            group.enemyCount = rng.Next(3, 7); // 3–6 enemies

            for (int i = 0; i < group.enemyCount; i++) {
                int id = rng.Next(1, 3); // enemy IDs 1–2
                group.enemyIDs.Add(id);
            }

            enemyGroups.Add(group);
        }

        // This hashset is built but not used further; kept to preserve behaviour
        HashSet<int> enemyRooms = new HashSet<int>();
        foreach (EnemyGroup g in enemyGroups)
            enemyRooms.Add(g.roomID);
    }

    private void SpawnTraps() {
        traps.Clear();

        HashSet<int> enemyRooms = new HashSet<int>();
        foreach (EnemyGroup g in enemyGroups)
            enemyRooms.Add(g.roomID);

        HashSet<int> usedTrapRooms = new HashSet<int>();

        for (int i = 0; i < 2; i++) {
            Trap t = new Trap();

            int room;
            do {
                room = rng.Next(2, 21); // 2–20
            } while (enemyRooms.Contains(room) || usedTrapRooms.Contains(room));

            usedTrapRooms.Add(room);
            t.roomID = room;
            t.damage = 2;

            traps.Add(t);

            Instantiate(trapPrefab);
        }
    }

    private void SpawnBoss() {
        List<int> occupied = GetEnemyRooms();
        occupied.AddRange(GetTrapRooms());

        int bossRoom;
        do {
            bossRoom = rng.Next(2, 21); // 2–20
        } while (occupied.Contains(bossRoom));

        GameObject bossObj = Instantiate(baseBoss);
        bossObj.SetActive(false);

        Enemies boss = bossObj.GetComponent<Enemies>();
        int id = 3;

        boss.currentRoom   = bossRoom;
        boss.groupID       = bossRoom;
        boss.maxHealth     = Data.enemies[id][0];
        boss.damage        = Data.enemies[id][1];
        boss.attackCooldown= Data.enemies[id][2];
        boss.attackRange   = Data.enemies[id][3];
        boss.moveSpeed     = Data.enemies[id][4];

        bossObj.SetActive(true);
        bossObj.transform.position = new Vector2(0, 0);

        enemyGroups.Add(new EnemyGroup {
            roomID     = bossRoom,
            enemyCount = 1,
            enemyIDs   = new List<int> { 3 },
            isBossRoom = true
        });

        if (!groupEnemyCounts.ContainsKey(bossRoom))
            groupEnemyCounts[bossRoom] = 1;
    }

    public void OnEnemyKilled(Enemies enemy) {
        int group = enemy.groupID;

        groupEnemyCounts[group]--;

        if (clearedGroups.Contains(group))
            return;

        if (groupEnemyCounts[group] <= 0)
            clearedGroups.Add(group);
    }

    // ---------------------------------------------------------
    // PLAYER MOVEMENT
    // ---------------------------------------------------------

    public IEnumerator MovePlayer(int arrowNum) {
        yield return StartCoroutine(FadeTransition(arrowNum));
        yield return StartCoroutine(CheckForTrapDamage());
    }

    private IEnumerator CheckForTrapDamage() {
        bool inTrapRoom = false;

        foreach (Trap t in traps) {
            if (t.roomID == currentRoom) {
                inTrapRoom = true;

                yield return StartCoroutine(Dialogue(
                    "*beep*\n\nSCANNING...   SCANNING...   SCANNING... \n" +
                    "YOU ARE IN A TRAPPED ROOM! NOXIOUS GAS DETECTED! OH... AND ALSO, SPIKES!\n" +
                    "LEAVE AS SOON AS POSSIBLE! LEAVE AS SOON AS POSSIBLE!\n\n*beep*"));

                if (trapDamageRoutine == null)
                    trapDamageRoutine = StartCoroutine(TrapDamageOverTime(t.damage));
            }
        }

        if (!inTrapRoom && trapDamageRoutine != null) {
            StopCoroutine(trapDamageRoutine);
            trapDamageRoutine = null;
        }

        player.movementLock = false;
    }

    private IEnumerator TrapDamageOverTime(int damage) {
        int stack = 1;

        while (true) {
            player.TakeDamage(damage * stack);
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

    private void RoomCheck() {
        checkingRooms = true;

        List<int> enemyRooms = GetEnemyRooms();
        List<int> trapRooms  = GetTrapRooms();

        int enemyCount = 0;
        int trapCount  = 0;

        foreach (int room in enemyRooms)
            if (Data.rooms[currentRoom].Contains(room))
                enemyCount++;

        foreach (int room in trapRooms)
            if (Data.rooms[currentRoom].Contains(room))
                trapCount++;

        List<string> parts = new List<string>();

        if (enemyCount == 1)      parts.Add("AN ENEMY GROUP");
        else if (enemyCount > 1)  parts.Add("ENEMY GROUPS");

        if (trapCount == 1)       parts.Add("A TRAP");
        else if (trapCount > 1)   parts.Add("TRAPS");

        string adjacents = parts.Count == 0 ? "NOTHING" : string.Join(" AND ", parts);

        StartCoroutine(Dialogue(
            "*beep*\n\nSCANNING...   SCANNING...   SCANNING...\n" +
            $"{adjacents} DETECTED IN ADJACENT ROOMS!\n\n*beep*"));
    }

    // ---------------------------------------------------------
    // DIALOGUE
    // ---------------------------------------------------------

    private IEnumerator Dialogue(string text) {
        player.movementLock = true;

        player.anim.SetTrigger("listen");
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(TypeWrite(dialogueText, text));

        player.anim.SetTrigger("stopListen");
        yield return new WaitForSeconds(3f);

        player.movementLock = false;
        checkingRooms = false;
    }

    private IEnumerator TypeWrite(TextMeshProUGUI box, string line) {
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

    private IEnumerator FadeTransition(int arrowNum) {
        blackBackground.SetActive(true);
        player.inputLock = true;

        yield return StartCoroutine(Fade(0f, 1f, 1.5f));

        SetUI();

        player.inputLock = false;
        player.movementLock = true;

        currentRoom = Data.rooms[currentRoom][arrowNum];
        player.transform.position = caveSpawnPositions[arrowNum];

        CheckRoomType();

        yield return StartCoroutine(Fade(1f, 0f, 1.5f));

        blackBackground.SetActive(false);
    }

    private IEnumerator Fade(float start, float end, float duration) {
        float t = 0f;

        blackImage.color = new Color(0, 0, 0, start);

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

        clearText.text = $"Enemy Rooms Cleared: {clearedGroups.Count} / 5";
        timerText.text = TimeSpan.FromSeconds(Math.Floor(timer)).ToString();

        redOverlay.SetActive(GetTrapRooms().Contains(currentRoom));
    }

    // ---------------------------------------------------------
    // END GAME
    // ---------------------------------------------------------

    private IEnumerator End(float timer) {
        blackBackground.SetActive(true);
        yield return Fade(0f, 1f, 1.5f);

        float damageTaken = player.GetDamageTaken();
        float score = (float)Math.Round((1200 - timer) * (15 - damageTaken), 0);


        endScreenText.enabled = true;

        endScreenText.text = $"Final Time: {TimeSpan.FromSeconds(Math.Floor(timer))}\n";

        yield return new WaitForSeconds(1.5f);

        endScreenText.text += $"Damage Taken: {player.GetDamageTaken()}\n";

        yield return new WaitForSeconds(3f);

        endScreenText.text += $"\n\nFinal Score: {score} points.\nRank: ";

        endScreenText.text += score switch {
            > 18000              => "T. For Tenna. How on earth did you get this high?",
            > 16000 and <= 18000 => "S+. Fantastic job!",
            > 14000 and <= 16000 => "S. Amazing job!",
            > 11000 and <= 14000 => "A. Good job!",
            > 8000 and <= 11000  => "B. Pretty good!",
            > 5000 and <= 8000   => "C. You did alright!",
            > 2000 and <= 5000   => "D. Maybe next time...",
            > 0 and <= 2000      => "E. Lock in buddy.",
            <= 0                 => "F. Your taking too long",
            _                    => "bro?",
        };
    }

    // ---------------------------------------------------------
    // OTHER
    // ---------------------------------------------------------

    public void Die() {
        toggleMap = false;
        deathOverlay.SetActive(true);
    }

    public void Reset() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToTitle() {
        SceneManager.LoadScene("Title");
    }
}
