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
    [SerializeField] private float chargeSpeed = 10f;
    [SerializeField] private float chargeCooldown = 4f;

    [SerializeField] private float chargeTimer;

    [Header("Charge Telegraph")]
    [SerializeField] private float chargeWindupTime = 0.4f;
    [SerializeField] private Color chargeGlowColor = Color.white;

    //[Header("Stomp Attack")]
    //[SerializeField] private float stompCooldown = 6f;
    //[SerializeField] private float stompRadius = 3f;

    //[SerializeField] private float stompTimer;

    [Header("Projectile Attack")]
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float projectileCooldown = 3f;
    [SerializeField] private int projectileCount = 5;
    [SerializeField] private float projectileTimer;

    [Header("Spawn Delay")]
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private float spawnTimer = 0f;
    [SerializeField] private bool isSpawning = true;


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
        //stompTimer -= Time.fixedDeltaTime;
        projectileTimer -= Time.fixedDeltaTime;

        if (chargeTimer <= 0f) {
            ChargeAttack();
            chargeTimer = chargeCooldown;
        }

        //if (stompTimer <= 0f) {
        //    StompAttack();
        //    stompTimer = stompCooldown;
        //}

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

    //private void StompAttack() {
    //    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stompRadius);

    //    foreach (Collider2D hit in hits) {
    //        if (hit.CompareTag("Player"))
    //            AttackPlayer();
    //    }

        //Debug.Log("Boss used stomp Attack");
    //E}

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
        //Gizmos.DrawWireSphere(transform.position, stompRadius); // stomp AoE
    }
}

