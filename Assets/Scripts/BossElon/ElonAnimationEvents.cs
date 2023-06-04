using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElonAnimationEvents : MonoBehaviour
{
    private ElonController controller;
    // Start is called before the first frame update
    void Start()
    {
        var parent = transform.parent.gameObject;
        controller = parent.GetComponent<ElonController>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShootXRockets()
    {
        controller.XRocketsPhase2();
    }
}
