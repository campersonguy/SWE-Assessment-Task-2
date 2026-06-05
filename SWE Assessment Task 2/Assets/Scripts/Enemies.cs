using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Enemies : MonoBehaviour {

    public GameManager gManager;

    public GameObject projectile;
    
    public string enemyName;
    public string flavourText;

    public int damage;
    public int projectileCount;

    public int currentRoom;


    public void PlayerCheck() {

    }

    // IEnumerator Shoot() {

    // }

    void DamagePlayer() {
        
    }

}

public class Bosses : Enemies {

}
