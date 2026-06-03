using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Enemies : MonoBehaviour {

    public GameManager gManager;
    
    public string enemyName;
    public string flavourText;

    public float damage;

    public float currentRoom;


    public void PlayerCheck() {

    }

    void DamagePlayer() {
        
    }

}

public class Bosses : Enemies {

}
