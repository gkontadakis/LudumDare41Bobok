using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SlotManager : MonoBehaviour
{
    public int SizeX;
    public int SizeY;

    public int BoxRowMinRemoveNum = 3;

    private int _expectedSlots;

    private GameObject[,] _slotLevel;

    private HashSet<GameObject> _boxesToDestroy;

    // Use this for initialization
    void Start ()
	{

    }

    void Awake()
    {
    }

    void OnValidate()
    {
        StartCoroutine(OnValidateDelayed());
    }

    IEnumerator OnValidateDelayed()
    {
        yield return new WaitForEndOfFrame();

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        
        _slotLevel = new GameObject[SizeX, SizeY];
        GameObject slot = Resources.Load("Slot") as GameObject;
        for (var i = 0; i < SizeX; i++)
        {
            for (var j = 0; j < SizeY; j++)
            {
                _slotLevel[i, j] = Instantiate(slot, transform.position + Vector3.right * i + Vector3.up * j, Quaternion.identity, transform);
            }
        }

        _boxesToDestroy = new HashSet<GameObject>();
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
        foreach (Transform slot in transform)
        {
            if (slot.childCount != 0)
            {
                Vector3 indxs = slot.localPosition;
                int x = (int) indxs.x;
                int y = (int) indxs.y;

                //Check previous first

                int minX = x - 1;
                bool slotPrevious = IsSlotOccupied(minX, y);
                while (slotPrevious)
                {
                    --minX;
                    slotPrevious = IsSlotOccupied(minX, y);
                }

                //Then next

                int maxX = x + 1;
                bool slotNext = IsSlotOccupied(maxX, y);
                while (slotNext)
                {
                    ++maxX;
                    slotNext = IsSlotOccupied(maxX, y);
                }

                if(maxX - minX - 1 >= BoxRowMinRemoveNum)

                for (var i = minX + 1; i < maxX; i++)
                {
                    for (var j = _slotLevel[i, y].transform.childCount - 1 ; j >=0 ; j--)
                    {
                        if (!_boxesToDestroy.Contains(_slotLevel[i, y].transform.GetChild(j).gameObject))
                        {
                            _boxesToDestroy.Add(_slotLevel[i, y].transform.GetChild(j).gameObject);
                        }

                    }
                }
            }
        }

        foreach (var box in _boxesToDestroy)
        {
            box.GetComponent<BoxBehaviour>().MarkForDestroy();
        }
        _boxesToDestroy.Clear();
    }

    private bool IsSlotOccupied(int i, int j)
    {
        if (i >= 0 && i < SizeX && j >= 0 & j < SizeY && _slotLevel[i, j].transform.childCount != 0)
        {
            return true;
        }

        return false;
    }
}
