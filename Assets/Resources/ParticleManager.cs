using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private Dictionary<string, GameObject> _gameObjects;    //particle name maps to game object (name)

	// Use this for initialization
	void Awake () {
	    _gameObjects = new Dictionary<string, GameObject>();
	    foreach (Transform child in transform)
	    {
	        _gameObjects.Add(child.name, GameObject.Find(GetObjectNameFromParticleName(child.name)));
	    }
    }

    string GetObjectNameFromParticleName(string particleName)
    {
        return particleName.Replace("Particle_", "");
    }

    GameObject GetGameObject(string particleName)
    {
        GameObject result = null;
        if (_gameObjects.TryGetValue(particleName, out result) && result != null)
            return result;

        var objName = GetObjectNameFromParticleName(particleName);
        _gameObjects[particleName] = GameObject.Find(objName);  // No Add it throws exception we want to ovewrite value with newly spawned Player GameObject !!!

        return _gameObjects[particleName];
    }

    // Update is called once per frame
	void Update () {
	    foreach (Transform child in transform)
	    {
	        if (child.GetComponent<ParticleSystem>().isPlaying)
	        {
	            child.position = GetGameObject(child.name).transform.GetChild(0).position;
	        }
	    }
    }

    public ParticleSystem GetParticleSystem(string objName)
    {
        return transform.Find("Particle_" + objName).GetComponent<ParticleSystem>();
    }
}
