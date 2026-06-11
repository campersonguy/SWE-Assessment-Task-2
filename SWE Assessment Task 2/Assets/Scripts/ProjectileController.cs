using UnityEngine;

public class ProjectileController : MonoBehaviour {

    public PlayerController player;
    
    public float speed = 5f;
    public float lifetime = 3f;

    public bool destroySelf = true;

    private Vector2 direction;
    private Rigidbody2D rb;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        if (destroySelf)
            Destroy(gameObject, lifetime);
    }

    void FixedUpdate() {
        rb.linearVelocity = direction * speed;
    }

    public void SetDirection(Vector2 dir) {
        direction = dir.normalized;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player")
            player.health -= 1;
    }
}
