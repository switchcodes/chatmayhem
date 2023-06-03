using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    private float distance;
    public float detectionRange;
    private Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);

        direction = player.transform.position - transform.position;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 playerDirection =
            transform.InverseTransformPoint(player.transform.position);

        if (playerDetected() && playerOnLeft())
        {
            
            var scale = transform.localScale;
            if(scale.y > 0) {
                scale.y *= -1;
                transform.localScale = scale;
            }
          
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);

        }else if (playerDetected() && playerOnRight()) {
            var scale = transform.localScale;
            if (scale.y < 0)
            {
                scale.y *= -1;
                transform.localScale = scale;
            }
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);


        }
    }

    bool playerOnLeft() { return direction.x < 0; }
    bool playerOnRight() { return direction.x > 0; }


    bool playerDetected() {return distance < detectionRange;}
}
