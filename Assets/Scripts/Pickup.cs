using System;
using System.Collections;
using System.Collections.Generic;
using Player;
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

    private Health _health;
    private PlayerController _playerController;
    
    private void Awake()
    {
        
    }

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
        if (!other.CompareTag("Player")) return;
        player = other.gameObject;
        _health = player.GetComponent<Health>();
        _playerController = player.GetComponent<PlayerController>();
        switch (type)
        {
            case PickupType.Health:
            {
                // set player health to 100
                Debug.Log("picked up health");
                _health.Heal();
                break;
            }
            case PickupType.Speed:
            {
                // set speed
                Debug.Log("picked up speed");
                _playerController.IncreaseSpeed();
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