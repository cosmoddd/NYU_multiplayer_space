using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class holdpoint : NetworkBehaviour
{
    public float y;
    public float speed;
    public GameObject holdPoint;

    // Start is called before the first frame update
    void Start()
    {
        y = Random.Range(3, 9);
        holdPoint = this.gameObject;
        this.transform.Rotate(0, 0, 0, Space.Self);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0f, y + speed * Time.deltaTime, 0f);
    }
}
