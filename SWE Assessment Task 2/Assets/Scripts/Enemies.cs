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
    public PlayerController playerController;

    public GameObject damageEffect;

    [Header("Information")]
    public string enemyName;

    public int groupID;

    public bool visible;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;

    public Rigidbody2D rb;
    private PolygonCollider2D col;
    public SpriteRenderer sr;

    private Vector2 moveDirection;
    private float directionTimer;

    [Header("Violence")]
    public float aggroRange = 5f;       // distance to start chasing
    public float deaggroRange = 7f;     // distance to stop chasing
    public float attackRange = 0.5f;    // distance to attack
    public float attackCooldown = 1f;

    public float damage = 1;

    private bool isAggro = false;
    private float attackTimer = 0f;

    public bool isCharging = false;

    public Transform player;

    [Header("Health")]
    public float maxHealth;
    public float currentHealth;

    public GameObject healthBar;
    public GameObject bar;
    public Image fill;

    public Vector2 offset;

    [Header("Location")]
    public int currentRoom;


    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        bar = Instantiate(healthBar, transform);
        bar.transform.localPosition = offset;

        fill = bar.transform.Find("Fill").gameObject.GetComponent<Image>();
    }

    protected virtual void Start() {
        PickNewDirection();

        currentHealth = maxHealth;
    }

    protected virtual void FixedUpdate() {
        visible = (gManager.currentRoom == currentRoom);

        if (visible && currentHealth > 0) {
            Visible(true);

            float distToPlayer = Vector2.Distance(transform.position, player.position);

            // Aggro logic
            if (!isAggro && distToPlayer <= aggroRange)
                isAggro = true;

            if (isAggro && distToPlayer >= deaggroRange)
                isAggro = false;

            if (isAggro) {
                ChasePlayer();
            } else {
                Wander();
            }

            attackTimer += Time.fixedDeltaTime;
            if (isAggro && distToPlayer <= attackRange && attackTimer >= attackCooldown) {
                AttackPlayer();
                attackTimer = 0f;
            }
        } else {
            Visible(false);
        }
    }

    void Wander() {
        directionTimer += Time.fixedDeltaTime;
        if (directionTimer >= changeDirectionTime)
            PickNewDirection();

        if (!isCharging)
            rb.linearVelocity = moveDirection * moveSpeed;
    }

    void ChasePlayer() {
        Vector2 dir = (player.position - transform.position).normalized;
        
        if (!isCharging)
            rb.linearVelocity = dir * moveSpeed;
    }

    void PickNewDirection() {
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        directionTimer = 0f;
    }

    protected void AttackPlayer() {
        playerController.TakeDamage(damage);
    }

    void Visible(bool state) {
        sr.enabled = state;
        bar.SetActive(state);
    }

    public IEnumerator TakeDamage(int amount) {
        if (visible) {
            currentHealth -= amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            fill.fillAmount = (float)currentHealth / maxHealth;

            Instantiate(damageEffect, transform.position, Quaternion.identity);

            if (currentHealth <= 0) {
                Die();
                yield break;
            }

            sr.color = new Color32(255, 137, 137, 255);
            yield return new WaitForSeconds(0.15f);
            sr.color = new Color32(137, 137, 137, 255);
        }
    }

    public void Die() {
        GameManager.Instance.OnEnemyKilled(this);
        Visible(false);
    }

    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}