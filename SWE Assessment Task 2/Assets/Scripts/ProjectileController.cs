using UnityEngine;

public class ProjectileController : MonoBehaviour {
    public float speed = 5f;
    public float lifetime = 3f;
    public bool destroySelf = true;

    private Vector2 direction;
    private Rigidbody2D rb;
    private PlayerController player;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        if (destroySelf)
            Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && player != null)
        {
            player.TakeDamage(1);   // non‑coroutine wrapper is safest
            Destroy(gameObject);    // kill projectile immediately on hit
        }
    }
}
