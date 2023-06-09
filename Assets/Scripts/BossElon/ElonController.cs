using BossElon;
using UnityEngine;

public class ElonController : MonoBehaviour
{
    public Node[] nodes;
     public int currentNode;
    public Transform player;
    public float speed;
    private float distance;
    public float detectionRange;
    private Vector2 direction;
    public GameObject xRocket;
    public Vector3 currentNodePosition;
    // Start is called before the first frame update
    void Start()
    {
        nodes = GetComponentsInChildren<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("elon update");
        var playerPos = player.position;
        var position = transform.position;
        distance = Vector2.Distance(position, playerPos);
        direction = playerPos - position;
        direction.Normalize();
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.InverseTransformPoint(player.transform.position);
        Debug.Log("elon update 2");
        // rotate front towards player
        //if (!playerDetected()) return;
        var transformLocalScale = transform.localScale;
        transformLocalScale.y = playerOnLeft() ? 1 : -1;
        transform.localScale = transformLocalScale;
       transform.rotation = Quaternion.Euler(0, 0, angle + 180 + Random.Range(-20, 20));
        Debug.Log("elon update 3");
        Debug.Log("nodes" + nodes.Length);
        if (currentNode < nodes.Length - 1) { 
            Debug.Log("current node" + currentNode);
            transform.position = Vector2.MoveTowards(position, currentNodePosition, speed * Time.deltaTime);
            currentNode++;
            checkNode();
        }
        
        
        XRockets();

    }

    void checkNode()
    {
        currentNodePosition = nodes[currentNode].transform.position;
    }

    [SerializeField] Animator animator;

    [Header("ROCKETS")]
    [SerializeField] private float xRocketsSpeed = 5f;

    [SerializeField] private float xRocketsMaxCooldown = 0.5f;
    [SerializeField] private Transform xRocketsSpawnPoint;
    private float xRocketsCurrentCooldown;
    private void XRockets()
    {
        xRocketsCurrentCooldown -= Time.deltaTime;
        if (xRocketsCurrentCooldown > 0)
        {
            return;
        }
        XRocketsPhase2();
        animator.SetTrigger("XRocket");
        xRocketsCurrentCooldown = xRocketsMaxCooldown;
    }

    public void XRocketsPhase2()
    {
        Vector3 worldPos = player.position;
        Vector3 dir = (worldPos - transform.position).normalized;
        GameObject rocket = Instantiate(xRocket, xRocketsSpawnPoint.position, Quaternion.identity);
        rocket.GetComponent<XRockets>().ShootXRockets(dir, xRocketsSpeed);
    }
    bool playerOnLeft() { return direction.x < 0; }
    bool playerOnRight() { return direction.x > 0; }


    bool playerDetected() { return distance < detectionRange; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Fire")) return;
        collision.gameObject.GetComponent<ElonHealth>().GetDamaged();
    }
}
