using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using BossElon;

public class AnimationEvents : MonoBehaviour
{
    // Start is called before the first frame update
    
    private PlayerController controller;
    void Start()
    {
        var parent = transform.parent.gameObject;
        controller = parent.GetComponent<PlayerController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void ShootFireBall()
    {
        Debug.Log("Shoot Fireball");
        controller.FirePhase2();
    }
}
