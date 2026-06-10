using UnityEngine;

public class ProjectileController : MonoBehaviour {
    
    public int id;

    public GameManager gameManager;

    public Rigidbody2D rb;

    void Update() {
        rb.linearVelocity = new Vector2(0, -10);
    }
}