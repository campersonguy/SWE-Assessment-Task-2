using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Enemies : MonoBehaviour {

    [Header("References")]
    [SerializeField] protected GameManager gManager;
    [SerializeField] protected PlayerController playerController;

    [SerializeField] private GameObject damageEffect;

    [Header("Information")]
    public int groupID;
    public bool visible;

    [Header("Movement")]
    public float moveSpeed = 2f;
    [SerializeField] private float changeDirectionTime = 2f;

    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] private PolygonCollider2D col;
    [SerializeField] protected SpriteRenderer sr;

    private Vector2 moveDirection;
    private float directionTimer;

    [Header("Aggro + Attacking")]
    [SerializeField] private float aggroRange = 5f;
    public float attackRange = 0.5f;
    public float attackCooldown = 1f;

    public float damage = 1;

    private bool isAggro = false;
    private float attackTimer = 0f;

    [SerializeField] protected bool isCharging = false;
    [SerializeField] protected Transform player;

    [Header("Health")]
    public float maxHealth;
    [SerializeField] protected float currentHealth;

    [SerializeField] private GameObject healthBar;
    private GameObject bar;
    private Image fill;

    [SerializeField] private bool dead = false;

    [SerializeField] private Vector2 offset;

    [Header("Location")]
    public int currentRoom;


    // ---------------------------------------------------------
    // INITIALISATION
    // ---------------------------------------------------------

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        bar = Instantiate(healthBar, transform);
        bar.transform.localPosition = offset;

        fill = bar.transform.Find("Fill").GetComponent<Image>();
    }

    protected virtual void Start() {
        PickNewDirection();
        currentHealth = maxHealth;
    }

    // ---------------------------------------------------------
    // FIXED UPDATE (AI + MOVEMENT)
    // ---------------------------------------------------------

    protected virtual void FixedUpdate() {

        visible = (gManager.currentRoom == currentRoom);

        // Early exit if not visible or dead
        if (!visible || currentHealth <= 0) {
            Visible(false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Visible(true);

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Smooth aggro logic with hysteresis
        if (!isAggro && distToPlayer <= aggroRange)
            isAggro = true;
        else if (isAggro && distToPlayer >= aggroRange + 1f)
            isAggro = false;

        // Movement
        if (isAggro)
            ChasePlayerSmooth();
        else
            WanderSmooth();

        // Attacking
        attackTimer += Time.fixedDeltaTime;
        if (isAggro && distToPlayer <= attackRange && attackTimer >= attackCooldown) {
            AttackPlayer();
            attackTimer = 0f;
        }
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------

    private void WanderSmooth() {
        directionTimer += Time.fixedDeltaTime;
        if (directionTimer >= changeDirectionTime)
            PickNewDirection();

        if (!isCharging) {
            Vector2 targetVel = moveDirection * moveSpeed;
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, 0.1f);
        }
    }

    private void ChasePlayerSmooth() {
        if (isCharging)
            return;

        Vector2 dir = (player.position - transform.position).normalized;
        Vector2 targetVel = dir * moveSpeed;

        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVel, 0.15f);
    }

    private void PickNewDirection() {
        moveDirection = Random.insideUnitCircle.normalized;
        directionTimer = 0f;
    }

    // ---------------------------------------------------------
    // COMBAT
    // ---------------------------------------------------------

    protected void AttackPlayer() {
        playerController.TakeDamage(damage);
    }

    // ---------------------------------------------------------
    // VISIBILITY
    // ---------------------------------------------------------

    void Visible(bool state) {
        sr.enabled = state;
        bar.SetActive(state);
    }

    // ---------------------------------------------------------
    // DAMAGE + DEATH
    // ---------------------------------------------------------

    public IEnumerator TakeDamage(int amount) {
        if (!visible)
            yield break;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        fill.fillAmount = currentHealth / maxHealth;

        Instantiate(damageEffect, transform.position, Quaternion.identity);

        if (currentHealth <= 0) {
            Die();
            yield break;
        }

        sr.color = new Color32(255, 137, 137, 255);
        yield return new WaitForSeconds(0.15f);
        sr.color = new Color32(137, 137, 137, 255);
    }

    public void Die() {
        if (dead)
            return;

        dead = true;
        GameManager.Instance.OnEnemyKilled(this);
        Visible(false);
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
