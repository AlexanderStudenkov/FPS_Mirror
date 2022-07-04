using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlatformButton : NetworkBehaviour
{
    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;

    [SerializeField]
    private float speed = 0.7f;


    [SerializeField]
    bool moveMode = true;
    
    
    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerScript>() != null)
            moveMode = !moveMode;
    }

    
    public void SetPoints(Transform start, Transform end)
    {
        startPoint = start;
        endPoint = end;
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            if (moveMode)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, endPoint.position, speed * Time.fixedDeltaTime);
            }
            else
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, startPoint.position, speed * Time.fixedDeltaTime);
            }
        }
        
    }
}
