using UnityEngine;

public class HitboxDetector : MonoBehaviour {
    
    [SerializeField] private int id;

    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name == "Player")
            StartCoroutine(gameManager.MovePlayer(id));
    }
}