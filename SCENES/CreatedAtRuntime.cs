using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatedAtRuntime : MonoBehaviour {

	public void Play()
    {
        InteractibleObject io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject>(); 
        io.Play(); 
    }

    public void Stop()
    {
        InteractibleObject io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject>();
        io.StopPlay();
    }

    public void ReturnToHelix()
    {
        InteractibleObject io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject>();
        io.ReturnToHelix(); 
    }

    public void SaveSong()
    {
        InteractibleObject io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject>();
        io.SaveSong(); 
    }
}

