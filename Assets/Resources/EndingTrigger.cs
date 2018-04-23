using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{

    private bool _isFired;

    public void ResetTrigger()
    {
        _isFired = false;
    }

    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player" && !_isFired)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().StartEndingSequence();
            _isFired = true;
        }
    }
}
