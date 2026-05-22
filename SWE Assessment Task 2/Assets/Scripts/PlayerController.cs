using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class PlayerController : MonoBehaviour {

    public float speed;

    public Animator anim;
    
    Rigidbody2D rb;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        rb.linearVelocity = new Vector2(moveX * speed, moveY * speed);

        if (moveX == 0 && moveY == 0)
            anim.SetInteger("walkDir", 0);

        if (Input.GetAxis("Vertical") > 0)
            anim.SetInteger("walkDir", 3);
        if (Input.GetAxis("Vertical") < 0)
            anim.SetInteger("walkDir", 1);
        if (Input.GetAxis("Horizontal") > 0)
            anim.SetInteger("walkDir", 4);
        if (Input.GetAxis("Horizontal") < 0)
            anim.SetInteger("walkDir", 2);
    }
}
