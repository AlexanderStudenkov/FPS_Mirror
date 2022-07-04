using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : NetworkBehaviour
{
    [SerializeField]
    private Transform platformStart;
    [SerializeField]
    private Transform platformEnd;

    [SerializeField]
    GameObject platformPrefab;
    public override void OnStartServer()
    {
        GameObject platform = Instantiate(platformPrefab, platformStart.position, Quaternion.identity);

        var controller = platform.GetComponent<PlatformButton>();
        controller.SetPoints(platformStart, platformEnd);

        NetworkServer.Spawn(platform);

    } 
}
