using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MercuryMessaging; 

public class PlaySong : MonoBehaviour {

    MmRelayNode _myRelayNode;
    bool play = true; 
    string msg = ""; 

    void Start()
    {
        _myRelayNode = gameObject.AddComponent<MmRelayNode>();
    }

    void OnTriggerEnter (Collider other)
    {
        if (play)
        {
            msg = "Play";
        }

        if (!play)
        {
            msg = "Stop"; 
        }

        play = !play;
        _myRelayNode.MmInvoke(MmMethod.MessageString, msg, 
            new MmMetadataBlock(MmLevelFilter.Parent, MmActiveFilter.All));
        Debug.Log("Message Sent"); 
    }
}