using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Health,
    Speed,
    Damage
}

public class Pickup : MonoBehaviour
{
    public PickupType type;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (type)
        {
            case PickupType.Health:
            {
                // set player health to 100
                Debug.Log("picked up health");
                break;
            }
            case PickupType.Speed:
            {
                // set speed
                break;
            }
            case PickupType.Damage:
            {
                // set damage
                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }

        Destroy(gameObject);
    }
}