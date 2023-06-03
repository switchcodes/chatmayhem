using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{

    private float speed=1f;
    private Rigidbody2D rb;
    private float lifeTime = 5f;

    private Vector3 dir = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag==("Ground"))
        {
            gameObject.SetActive(false);
        }else if (other.transform.tag=="Enemy")
        {
            other.gameObject.SetActive(false);
        }
    }

    public void ShootFireBall(Vector3 dir, float speed)
    {
        this.speed = speed;
        this.dir = dir;
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.velocity= dir * speed;
    }
}