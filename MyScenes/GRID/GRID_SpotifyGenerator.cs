using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GRID_SpotifyGenerator : MonoBehaviour
{
    private int numberOfObjects;
    private MySpotify.SpotifyData[] myObjects;

    private float scaleOfQuads = .15f;
    private float hoveringOffset = .25f;
    private float radius = .5f;
    private float angularRadianSpacing = .5f;

    public void processSpotifyPlaylistData(List<string> playlistIDs, List<string> playlistNames, List<Texture2D> playlistTextures)
    {
        string objectType = "playlist";
        GenerateObjects(playlistIDs, playlistNames, playlistTextures, objectType);
        Spawn();
    }

    private void GenerateObjects(List<string> ids, List<string> names, List<Texture2D> textures, string objectType)
    {
        numberOfObjects = ids.Count;
        myObjects = new MySpotify.SpotifyData[numberOfObjects];

        for (int i = 0; i < numberOfObjects; i++)
        {

            GameObject go = Instantiate(GameObject.Find("myObject"));
            go.transform.localScale = new Vector3(scaleOfQuads, scaleOfQuads, 1f);
            go.GetComponent<MeshRenderer>().material.mainTexture = textures[0];
            go.GetComponentInChildren<TextMesh>().text = names[0];

            int idInCircle;
            if (i % 2 == 0)
                idInCircle = i / 2;
            else
                idInCircle = -1 * (i / 2 + 1);

            MySpotify.SpotifyData myObject = new MySpotify.SpotifyData(go, ids[0], names[0], objectType, idInCircle, textures[0]);
            go.GetComponent<GRID_TouchableCircleObject>().spotifyData = myObject;

            myObjects[i] = myObject;

            ids.RemoveAt(0);
            names.RemoveAt(0);
            textures.RemoveAt(0);
        }
    }

    private void Spawn()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {

            float angle = myObjects[i].idInCircle * angularRadianSpacing;

            Vector3 outVector = -Mathf.Sin(angle) * this.transform.right + Mathf.Cos(angle) * this.transform.forward;
            Vector3 targetPosition = outVector * radius + transform.position + hoveringOffset * -transform.up;
            Quaternion targetRotation = Quaternion.LookRotation(outVector, transform.up);

            myObjects[i].GO.transform.position = targetPosition;
            myObjects[i].GO.transform.rotation = targetRotation;
            myObjects[i].GO.transform.parent = this.transform;
        }
    }
}