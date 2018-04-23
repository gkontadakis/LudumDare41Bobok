using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    public Vector3 SpawnLocation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().PlayerSpawnLocation = SpawnLocation;
        }
    }
}
