using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class Enemies : MonoBehaviour {

    [Header("References")]
    public GameManager gManager;
    public PlayerController player;

    [Header("Info")]
    public string enemyName;
    public string flavourText;
    public int damage = 1;

    [Header("Health")]
    public float maxHealth = 10f;
    public float currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;
    public float aggroRange = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float directionTimer;

    [Header("Attack")]
    public float attackRange = 1.2f;
    public float attackCooldown = 1f;

    private float attackTimer;

    [Header("Shooting")]
    public bool canShoot = false;

    public GameObject projectilePrefab;
    public float shootInterval = 9f;
    public float projectileSpeed = 5f;
    public int projectileCount = 4;

    private float shootTimer;

    [Header("Location")]
    public int currentRoom;
    private Vector2 spawnPosition;

    [Header("Components")]
    private SpriteRenderer sr;
    private Collider2D col;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private Image healthFill;
    private Transform healthBar;
    public Vector3 healthBarOffset = new Vector3(0, 1.2f, 0);


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = gManager.player;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        currentHealth = maxHealth;

        GameObject hb = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
        healthBar = hb.transform;
        healthFill = hb.transform.Find("Fill").GetComponent<Image>();

        spawnPosition = transform.position; // store original spawn
        PickNewDirection();

        SetVisible(gManager.currentRoom == currentRoom);
    }

    void Update() {
        bool shouldBeVisible = gManager.currentRoom == currentRoom;

        if (sr.enabled != shouldBeVisible) {
            SetVisible(shouldBeVisible);

            if (!shouldBeVisible)
                ResetEnemy();
        }

        // Follow enemy
        healthBar.position = transform.position + healthBarOffset;

        // Update fill
        healthFill.fillAmount = currentHealth / maxHealth;

        // Hide when enemy is invisible
        healthBar.gameObject.SetActive(sr.enabled);
    }

    void FixedUpdate() {
        if (!sr.enabled)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Attack if close enough
        if (distanceToPlayer <= attackRange) {
            TryAttack();
            rb.linearVelocity = Vector2.zero; // stop moving while attacking
            return;
        }

        // Otherwise move normally
        if (distanceToPlayer <= aggroRange)
            AggroMove();
        else
            WanderMove();

        HandleShooting();
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------

    void AggroMove() {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = dir * moveSpeed * 1.5f; // faster when aggro
    }

    void WanderMove() {
        directionTimer += Time.fixedDeltaTime;

        if (directionTimer >= changeDirectionTime)
            PickNewDirection();

        rb.linearVelocity = moveDirection * moveSpeed;
    }

    void PickNewDirection() {
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        directionTimer = 0f;
    }

    // ---------------------------------------------------------
    // COMBAT
    // ---------------------------------------------------------

    void HandleShooting() {
        if (!canShoot)
            return;

        shootTimer += Time.fixedDeltaTime;

        if (shootTimer >= shootInterval) {
            ShootProjectile();
            shootTimer = 0f;
        }
    }

    void ShootProjectile() {
        for (int i = 0; i < projectileCount; i++) {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            ProjectileController p = proj.GetComponent<ProjectileController>();
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;

            if (p != null)
                p.SetDirection(randomDir);
        }
    }

    void TryAttack() {
        attackTimer += Time.fixedDeltaTime;

        if (attackTimer >= attackCooldown)
        {
            PerformAttack();
            attackTimer = 0f;
        }
    }

    void PerformAttack() {
        // Deal damage to the player
        player.health -= damage;

        // Optional: play an animation
        // anim.SetTrigger("attack");

        // Optional: knockback
        // Vector2 knockDir = (player.transform.position - transform.position).normalized;
        // player.rb.AddForce(knockDir * 5f, ForceMode2D.Impulse);
    }

    public void TakeDamage(float amount) {
        currentHealth -= amount;

        if (currentHealth <= 0)
            Die();
    }

    void Die() {
        Destroy(gameObject);
    }

    // ---------------------------------------------------------
    // RESET
    // ---------------------------------------------------------

    void ResetEnemy() {
        rb.linearVelocity = Vector2.zero;
        transform.position = spawnPosition;
        PickNewDirection();
        shootTimer = 0f;

        currentHealth = maxHealth;
        healthFill.fillAmount = 1f;
    }

    // ---------------------------------------------------------
    // VISIBILITY
    // ---------------------------------------------------------

    void SetVisible(bool state) {
        sr.enabled = state;
        col.enabled = state;
    }
}
