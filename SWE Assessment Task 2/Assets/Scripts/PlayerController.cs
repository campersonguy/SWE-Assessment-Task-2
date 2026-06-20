using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Camera cam;

    [Header("Stats")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth;
    [SerializeField] private float speed;

    [Header("UI")]
    [SerializeField] private Image fill;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private Image attackFill;
    [SerializeField] private GameObject attackBar;

    [SerializeField] private Image dashFill;
    [SerializeField] private GameObject dashBar;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackAngle = 60f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.4f;

    private float attackTimer = 0f;

    [Header("Sword")]
    [SerializeField] private Transform sword;
    [SerializeField] private float radiusX = 1.5f;
    [SerializeField] private float radiusY = 0.8f;

    [Header("Sword Jab")]
    [SerializeField] private float jabDistance = 0.5f;
    [SerializeField] private float jabSpeed = 12f;

    private float jabAmount = 0f;
    private bool isJabbing = false;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private float dashCooldownTimer = 0f;
    private Vector2 dashDirection;

    [Header("State")]
    public bool movementLock = false;
    public bool inputLock = false;

    public Animator anim;

    private Vector2 input;


    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cam = Camera.main;

        currentHealth = maxHealth;
    }

    private void Update() {
        ReadInput();
        UpdateAnimation();
        UpdateUI();

        attackTimer += Time.deltaTime;
        dashCooldownTimer += Time.deltaTime;

        bool inCombatRoom = gameManager.GetEnemyRooms().Contains(gameManager.currentRoom);

        if (inCombatRoom) {
            sword.gameObject.SetActive(true);

            if (Input.GetMouseButtonDown(0) && attackTimer >= attackCooldown) {
                Attack();
                attackTimer = 0f;
            }

            UpdateSwordPosition();
        }
        else {
            sword.gameObject.SetActive(false);
            attackTimer = attackCooldown;
        }

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTimer >= dashCooldown)
            TryDash();
    }

    private void FixedUpdate() {
        if (isDashing) {
            rb.linearVelocity = dashDirection * dashSpeed;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f) {
                isDashing = false;
                movementLock = false;
            }
            return;
        }

        rb.linearVelocity = (!movementLock) ? input * speed : Vector2.zero;
    }

    // ---------------------------------------------------------
    // INPUT
    // ---------------------------------------------------------

    private void ReadInput() {
        if (!inputLock && !movementLock) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
        else {
            input = Vector2.zero;
        }
    }

    // ---------------------------------------------------------
    // ANIMATION
    // ---------------------------------------------------------

    private void UpdateAnimation() {
        if (movementLock || input == Vector2.zero) {
            anim.SetInteger("walkDir", 0);
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            anim.SetInteger("walkDir", input.x > 0 ? 4 : 2);
        else
            anim.SetInteger("walkDir", input.y > 0 ? 3 : 1);
    }

    // ---------------------------------------------------------
    // UI
    // ---------------------------------------------------------

    private void UpdateUI() {
        fill.fillAmount = currentHealth / maxHealth;
        hpText.text = $"{currentHealth} / {maxHealth}";

        attackFill.fillAmount = attackTimer / attackCooldown;
        attackBar.SetActive(attackFill.fillAmount < 1f);

        dashFill.fillAmount = dashCooldownTimer / dashCooldown;
        dashBar.SetActive(dashFill.fillAmount < 1f);
    }

    // ---------------------------------------------------------
    // ATTACK
    // ---------------------------------------------------------

    private void Attack() {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - transform.position).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        HashSet<Enemies> damaged = new HashSet<Enemies>();

        foreach (Collider2D hit in hits) {
            Enemies enemy = hit.GetComponent<Enemies>();
            if (enemy == null || damaged.Contains(enemy))
                continue;

            damaged.Add(enemy);

            Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(attackDir, toEnemy);

            if (angle <= attackAngle * 0.5f)
                StartCoroutine(enemy.TakeDamage(attackDamage));
        }

        StartJab();
    }

    // ---------------------------------------------------------
    // DAMAGE
    // ---------------------------------------------------------

    public void TakeDamage(float amount) {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);

        if (currentHealth <= 0) {
            gameManager.Die();
            movementLock = true;
        }

        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash() {
        sr.color = new Color32(255, 137, 137, 255);
        yield return new WaitForSeconds(0.15f);
        sr.color = new Color32(137, 137, 137, 255);
    }

    // ---------------------------------------------------------
    // SWORD POSITIONING
    // ---------------------------------------------------------

    private void UpdateSwordPosition() {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x);

        float x = Mathf.Cos(angle) * radiusX;
        float y = Mathf.Sin(angle) * radiusY;

        Vector3 basePos = new Vector3(x, y, 0f);

        Vector3 jabOffset = new Vector3(
            Mathf.Cos(angle) * jabAmount,
            Mathf.Sin(angle) * jabAmount,
            0f
        );

        sword.localPosition = basePos + jabOffset;
        sword.localRotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);

        if (isJabbing) {
            jabAmount = Mathf.MoveTowards(jabAmount, 0f, jabSpeed * Time.deltaTime);
            if (jabAmount <= 0f)
                isJabbing = false;
        }
    }

    private void StartJab() {
        isJabbing = true;
        jabAmount = jabDistance;
    }

    // ---------------------------------------------------------
    // DASH
    // ---------------------------------------------------------

    private void TryDash() {
        if (input == Vector2.zero || movementLock || inputLock)
            return;

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = 0f;

        dashDirection = input.normalized;
        movementLock = true;
    }

    // ---------------------------------------------------------
    // DEBUG
    // ---------------------------------------------------------

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
