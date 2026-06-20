using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Boss : Enemies {

    [Header("Boss Stats")]
    [SerializeField] private float baseDamage;

    [Header("Charge Attack")]
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeCooldown = 4f;
    private float chargeTimer;

    [SerializeField] private float chargeWindupTime = 0.4f;
    [SerializeField] private Color chargeGlowColor = Color.white;

    [Header("Projectile Attack")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileCooldown = 3f;
    [SerializeField] private int projectileCount = 5;
    private float projectileTimer;

    [Header("Spawn Delay")]
    [SerializeField] private float spawnDelay = 5f;
    private float spawnTimer = 0f;
    private bool isSpawning = true;

    protected override void Awake() {
        base.Awake();
        baseDamage = damage;
    }

    protected override void Start() {
        base.Start();
        spawnTimer = spawnDelay;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        // If not visible or dead, reset state and stop attacks
        if (!visible || currentHealth <= 0) {
            isSpawning = true;
            chargeTimer = 8f;
            return;
        }

        // Handle spawn delay (boss intro)
        if (isSpawning) {
            spawnTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;

            if (spawnTimer <= 0f)
                isSpawning = false;

            return;
        }

        // Scale damage based on cleared rooms
        damage = Mathf.Floor(baseDamage + (4 - gManager.clearedGroups.Count));

        // Update attack timers
        chargeTimer -= Time.fixedDeltaTime;
        projectileTimer -= Time.fixedDeltaTime;

        // Charge attack
        if (chargeTimer <= 0f) {
            ChargeAttack();
            chargeTimer = chargeCooldown;
        }

        // Projectile burst
        if (projectileTimer <= 0f) {
            ProjectileBurst();
            projectileTimer = projectileCooldown;
        }
    }

    // ---------------------------------------------------------
    // CHARGE ATTACK
    // ---------------------------------------------------------

    private void ChargeAttack() {
        if (player == null)
            return;

        StartCoroutine(ChargeAttackRoutine());
    }

    private IEnumerator ChargeAttackRoutine() {

        isCharging = true;
        rb.linearVelocity = Vector2.zero;

        Color originalColor = sr.color;
        sr.color = chargeGlowColor;

        yield return new WaitForSeconds(chargeWindupTime);

        sr.color = originalColor;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.AddForce(direction * chargeSpeed, ForceMode2D.Impulse);

        yield return new WaitForSeconds(2f);

        isCharging = false;
    }

    // ---------------------------------------------------------
    // PROJECTILE BURST
    // ---------------------------------------------------------

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
    }

    // ---------------------------------------------------------
    // DEBUG
    // ---------------------------------------------------------

    protected override void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
    }
}
