using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour {
    [Header("Stats")]
    public float health;
    public float maxHealth;
    public float speed;

    [Header("UI")]
    public Image hpFill;
    public TextMeshProUGUI hpText;

    [Header("Melee Attack")]
    public float attackRange = 1.5f;
    public float attackAngle = 45f;
    public float attackDamage = 2f;
    public float attackCooldown = 0.4f;

    private float attackTimer;


    [Header("State")]
    public bool movementLock = false;
    public bool inputLock = false;

    private Rigidbody2D rb;
    public Animator anim;
    private Vector2 input;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update() {
        ReadInput();
        UpdateAnimation();
        UpdateUI();

        attackTimer += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && attackTimer >= attackCooldown) {
            attackTimer = 0f;
            PerformAttack();
        }
    }

    void FixedUpdate() {
        if (!movementLock)
            rb.linearVelocity = input * speed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    void PerformAttack() {
        // Get mouse world position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - transform.position).normalized;

        // Find all enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D hit in hits) {
            Enemies enemy = hit.GetComponent<Enemies>();
            if (enemy == null) continue;

            // Direction from player to enemy
            Vector2 toEnemy = (enemy.transform.position - transform.position).normalized;

            // Check if enemy is within attack angle
            float angle = Vector2.Angle(attackDir, toEnemy);

            if (angle <= attackAngle) {
                enemy.TakeDamage(attackDamage);
            }
        }

        // Optional: play attack animation
        // anim.SetTrigger("attack");
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
        hpFill.fillAmount = health / maxHealth;
        hpText.text = $"{health} / {maxHealth}";
    }

    //void OnDrawGizmos() {
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(Vector3.zero, 6f);
    //}

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - transform.position).normalized;

        Vector3 left = Quaternion.Euler(0, 0, attackAngle) * attackDir;
        Vector3 right = Quaternion.Euler(0, 0, -attackAngle) * attackDir;

        Gizmos.DrawLine(transform.position, transform.position + left * attackRange);
        Gizmos.DrawLine(transform.position, transform.position + right * attackRange);
    }
}
