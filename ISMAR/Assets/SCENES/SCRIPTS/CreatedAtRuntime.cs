using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatedAtRuntime : MonoBehaviour {

	public void Play()
    {

        if (GameObject.FindWithTag("Grabbed") == null)
            return;
        InteractibleObject_Helix io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject_Helix>(); 
        io.Play(); 
    }

    public void Stop()
    {

        if (GameObject.FindWithTag("Grabbed") == null)
            return;
        InteractibleObject_Helix io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject_Helix>();
        io.StopPlay();
    }

    public void ReturnToHelix()
    {

        if (GameObject.FindWithTag("Grabbed") == null)
            return;
        InteractibleObject_Helix io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject_Helix>();
        io.ReturnToHelix(); 
    }

    public void SaveSong()
    {
        if (GameObject.FindWithTag("Grabbed") == null)
            return;
        InteractibleObject_Helix io = GameObject.FindWithTag("Grabbed").GetComponent<InteractibleObject_Helix>();
        io.SaveSong(); 
    }
}

