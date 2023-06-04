using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChase : MonoBehaviour
{

    public Transform player;
    public float speed;
    private float distance;
    public float detectionRange;
    public float despawnRange = 25;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var playerPos = player.position;
        var position = transform.position;
        distance = Vector2.Distance(position, playerPos);
        
        if (distance > despawnRange)
        {
            Destroy(gameObject);
        }

        direction = playerPos - position;
        direction.Normalize();
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.InverseTransformPoint(player.transform.position);
        
        // rotate front towards player
        if (!playerDetected()) return;
        var transformLocalScale = transform.localScale;
        transformLocalScale.y = playerOnLeft() ? 1 : -1;
        transform.localScale = transformLocalScale;
        transform.rotation = Quaternion.Euler(0, 0, angle + 180 + Random.Range(-20, 20));
        transform.position = Vector2.MoveTowards(position, playerPos + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5)), speed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        var playerPos = player.position;
        var position = transform.position;
        distance = Vector2.Distance(position, playerPos);

        direction = playerPos - position;
        direction.Normalize();
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(position, despawnRange);
    }

    bool playerOnLeft() { return direction.x < 0; }
    bool playerOnRight() { return direction.x > 0; }


    bool playerDetected() {return distance < detectionRange;}
}
