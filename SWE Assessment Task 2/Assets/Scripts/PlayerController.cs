using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour {

    public float health;
    public float maxHealth;
    public float speed;

    public Image hpFill;
    public TextMeshProUGUI hpText;

    public Animator anim;
    
    Rigidbody2D rb;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            StartCoroutine(listen());
        }

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(moveX * speed, moveY * speed);

        if (moveX == 0 && moveY == 0)
            anim.SetInteger("walkDir", 0);

        if (moveY > 0)
            anim.SetInteger("walkDir", 3);
        if (moveY < 0)
            anim.SetInteger("walkDir", 1);
        if (moveX > 0)
            anim.SetInteger("walkDir", 4);
        if (moveX < 0)
            anim.SetInteger("walkDir", 2);
        
        hpFill.fillAmount = health / maxHealth;
        hpText.text = $"{health} / {maxHealth}";
    }

    IEnumerator listen() {
        anim.SetTrigger("listen");

        yield return new WaitForSeconds(5f);

        anim.SetTrigger("stopListen");
    }
}
