using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class Boss : Enemies {

    [Header("Boss Stats")]
    [SerializeField] private float baseDamage;

    [Header("Charge Attack")]
    public float chargeSpeed = 10f;
    public float chargeCooldown = 4f;

    private float chargeTimer;

    [Header("Charge Telegraph")]
    public float chargeWindupTime = 0.4f;
    public Color chargeGlowColor = Color.white;

    [Header("Shockwave Attack")]
    public float shockwaveCooldown = 6f;
    public float shockwaveRadius = 3f;
    public int shockwaveDamage = 10;

    private float shockwaveTimer;

    [Header("Projectile Attack")]
    public GameObject projectilePrefab;

    public float projectileCooldown = 3f;
    public float projectileSpeed = 6f;
    public int projectileCount = 5;
    private float projectileTimer;

    [Header("Spawn Delay")]
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float spawnTimer = 0f;
    public bool isSpawning = true;


    protected override void Awake() {
        base.Awake(); // IMPORTANT

        baseDamage = damage;
    }

    protected override void Start() {
        base.Start(); // IMPORTANT
        spawnTimer = spawnDelay;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        // Boss is still in spawn delay
        if (isSpawning && visible) {
            spawnTimer -= Time.fixedDeltaTime;

            // Freeze movement
            rb.linearVelocity = Vector2.zero;

            if (spawnTimer <= 0f)
                isSpawning = false;

            return; // stop attacks too
        }

        damage = (int)Math.Floor(baseDamage + (4 - gManager.clearedGroups.Count));

        // Prevent attacks if not in current room
        if (!visible || currentHealth <= 0) {
            isSpawning = true;
            chargeTimer = 8f;
            return;
        }

        chargeTimer -= Time.fixedDeltaTime;
        shockwaveTimer -= Time.fixedDeltaTime;
        projectileTimer -= Time.fixedDeltaTime;

        if (chargeTimer <= 0f) {
            ChargeAttack();
            chargeTimer = chargeCooldown;
        }

        if (shockwaveTimer <= 0f) {
            ShockwaveAttack();
            shockwaveTimer = shockwaveCooldown;
        }

        if (projectileTimer <= 0f) {
            ProjectileBurst();
            projectileTimer = projectileCooldown;
        }
    }

    private void ChargeAttack() {
        if (player == null)
            return;
        
        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine() {

        isCharging = true;

        // Stop normal movement during windup
        rb.linearVelocity = Vector2.zero;

        // Save original color
        Color originalColor = sr.color;

        // Glow
        sr.color = chargeGlowColor;

        // Wait for windup
        yield return new WaitForSeconds(chargeWindupTime);

        // Restore color
        sr.color = originalColor;

        // Perform the actual charge
        Vector2 direction = (player.position - transform.position).normalized;
        rb.AddForce(direction * chargeSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2); // Charge duration

        isCharging = false;

        // Debug.Log("Boss performed CHARGE attack");
    }

    private void ShockwaveAttack() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius);

        foreach (Collider2D hit in hits) {
            if (hit.CompareTag("Player"))
                AttackPlayer();
        }

        //Debug.Log("Boss used Shockwave Attack");
    }

    private void ProjectileBurst() {
        float angleStep = 360f / projectileCount;
        float angle = 0f;

        for (int i = 0; i < projectileCount; i++) {
            float dirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float dirY = Mathf.Sin(angle * Mathf.Deg2Rad);
            Vector2 direction = new Vector2(dirX, dirY);

            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<ProjectileController>().SetDirection(direction);

            angle += angleStep;
        }

        //Debug.Log("Boss used Projectile Burst");
    }

    protected override void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius); // shockwave AoE
    }
}

