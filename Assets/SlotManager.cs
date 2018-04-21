using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class SlotManager : MonoBehaviour
{
    public int SizeX;
    public int SizeY;

    private int _expectedSlots;

	// Use this for initialization
	void Start ()
	{

    }

    void Awake()
    {
    }

    void OnValidate()
    {
        //StartCoroutine(OnValidateDelayed());
    }

    IEnumerator OnValidateDelayed()
    {
        yield return new WaitForEndOfFrame();
        /*
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        */
        /*
        while (transform.childCount != 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        */
        GameObject slot = Resources.Load("Slot") as GameObject;
        Instantiate(slot, transform);
    }

    // Update is called once per frame
    void Update ()
	{
		
	}

    public void SetExpectedSlots(int numOfSlotsExpected)
    {
        _expectedSlots = numOfSlotsExpected;
    }

    public void NotifySlotManager()
    {
        --_expectedSlots;
        if (_expectedSlots == 0)
        {
            UpdateSlots();
        }
    }

    private void UpdateSlots()
    {
    }
}
