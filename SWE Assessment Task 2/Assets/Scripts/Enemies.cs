using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Enemies : MonoBehaviour {

    [Header("importanc")]
    public GameManager gManager;

    public string enemyName;
    public string flavourText;

    public int damage = 1;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float changeDirectionTime = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float directionTimer;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public float shootInterval = 3f;
    public float projectileSpeed = 5f;
    public float projectileCount = 4;

    private float shootTimer;

    [Header("Location")]
    public int currentRoom;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        PickNewDirection();
    }

    void FixedUpdate() {
        // Movement timer
        directionTimer += Time.fixedDeltaTime;
        if (directionTimer >= changeDirectionTime) {
            PickNewDirection();
        }

        // Apply movement
        rb.linearVelocity = moveDirection * moveSpeed;

        // Shooting timer
        shootTimer += Time.fixedDeltaTime;
        if (shootTimer >= shootInterval) {
            ShootProjectile();
            shootTimer = 0f;
        }
    }

    void PickNewDirection() {
        moveDirection = UnityEngine.Random.insideUnitCircle.normalized;
        directionTimer = 0f;
    }

    void ShootProjectile() {
        for (int i = 0; i < projectileCount; i++) {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            ProjectileController p = proj.GetComponent<ProjectileController>();
            Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;

            if (p != null)
                p.SetDirection(randomDir); // or any direction you want
        }
    }
}

public class Bosses : Enemies {

}
