using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour {
    [Header("Stats")]
    public float currentHealth;
    public float maxHealth;
    public float speed;

    [Header("UI")]
    public Image fill;
    public TextMeshProUGUI hpText;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackAngle = 60f;   // degrees of arc in front of player
    public int attackDamage = 1;
    public float attackCooldown = 0.4f;

    private float attackTimer = 0f;

    [Header("State")]
    public bool movementLock = false;
    public bool inputLock = false;

    public Animator anim;

    private Rigidbody2D rb;
    private Vector2 input;
    private SpriteRenderer sr;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;
    }

    void Update() {
        ReadInput();
        UpdateAnimation();
        UpdateUI();

        attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && attackTimer >= attackCooldown) {
            Attack();
            attackTimer = 0f;
        }
    }

    void FixedUpdate() {
        if (!movementLock)
            rb.linearVelocity = input * speed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void ReadInput() {
        if (!inputLock) {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }
    }

    private void UpdateAnimation() {
        if (input == Vector2.zero) {
            anim.SetInteger("walkDir", 0);
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
            anim.SetInteger("walkDir", input.x > 0 ? 4 : 2);
        }
        else {
            anim.SetInteger("walkDir", input.y > 0 ? 3 : 1);
        }
    }

    private void UpdateUI() {
        fill.fillAmount = currentHealth / maxHealth;
        hpText.text = $"{currentHealth} / {maxHealth}";
    }

    private void Attack() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - transform.position).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        HashSet<Enemies> damaged = new HashSet<Enemies>();

        foreach (Collider2D hit in hits) {
            Enemies enemy = hit.GetComponent<Enemies>();
            if (enemy == null) continue;

            if (damaged.Contains(enemy)) continue; // prevent double hits
            damaged.Add(enemy);

            Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector2.Angle(attackDir, toEnemy);

            if (angle <= attackAngle * 0.5f) {
                StartCoroutine(enemy.TakeDamage(attackDamage));
            }
        }
    }

    public IEnumerator TakeDamage(int amount) {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        sr.color = new Color32(255, 137, 137, 255);
        yield return new WaitForSeconds(0.15f);
        sr.color = new Color32(137, 137, 137, 255);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
