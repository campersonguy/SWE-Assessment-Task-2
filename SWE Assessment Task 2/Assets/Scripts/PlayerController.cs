using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour {
    [Header("Stats")]
    public float health;
    public float maxHealth;
    public float speed;

    [Header("UI")]
    public Image hpFill;
    public TextMeshProUGUI hpText;

    [Header("Combat")]
    public GameObject projectile;

    [Header("State")]
    public int currentRoom;
    
    public bool movementLock = false;

    private Rigidbody2D rb;
    public Animator anim;
    private Vector2 input;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update() {
        ReadInput();
        UpdateAnimation();
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.F))
            Instantiate(projectile, transform.position, Quaternion.identity);
    }

    void FixedUpdate() {
        if (!movementLock)
            rb.linearVelocity = input * speed;
        else
            rb.linearVelocity = Vector2.zero;
    }

    private void ReadInput() {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    private void UpdateAnimation() {
        if (input == Vector2.zero) {
            anim.SetInteger("walkDir", 0);
            return;
        }

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
            anim.SetInteger("walkDir", input.x > 0 ? 4 : 2);
        }
        else {
            anim.SetInteger("walkDir", input.y > 0 ? 3 : 1);
        }
    }

    private void UpdateUI() {
        hpFill.fillAmount = health / maxHealth;
        hpText.text = $"{health} / {maxHealth}";
    }

    //void OnDrawGizmos() {
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(Vector3.zero, 8f);
    //}

}
