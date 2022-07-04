using UnityEngine;
using Mirror;

public class FireBall : NetworkBehaviour
{
    public float destroyAfter = 2;
    public Rigidbody rigidBody;
    public float force = 1000;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfter);
    }

    void Start()
    {
        rigidBody.AddForce(transform.forward * force);
    }

    
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider co) => DestroySelf();
}

