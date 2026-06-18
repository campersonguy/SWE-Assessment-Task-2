using UnityEngine;

public class ProjectileController : MonoBehaviour {
    [SerializeField] private float speed = 5f;
    [SerializeField] private float lifetime = 3f;

    [SerializeField] private Vector2 direction;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerController player;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate() {
        rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 dir) {
        direction = dir.normalized;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && player != null) {
            player.TakeDamage(1);   // non‑coroutine wrapper is safest
            Destroy(gameObject);    // kill projectile immediately on hit
        }
    }
}
