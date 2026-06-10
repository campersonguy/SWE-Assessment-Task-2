using UnityEngine;

public class ProjectileController : MonoBehaviour {

    public float speed = 5f;
    public float lifetime = 3f;

    public bool destroySelf = true;

    private Vector2 direction;
    private Rigidbody2D rb;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate() {
        rb.linearVelocity = direction * speed;
    }

    // Call this right after instantiating the projectile
    public void SetDirection(Vector2 dir) {
        direction = dir.normalized;
    }
}
