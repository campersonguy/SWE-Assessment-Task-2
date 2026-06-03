using UnityEngine;

public class HitboxDetector : MonoBehaviour {
    
    public int id;

    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D other) {
        gameManager.MovePlayer(id);
    }
}