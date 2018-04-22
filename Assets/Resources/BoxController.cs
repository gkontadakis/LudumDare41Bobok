using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BoxController : MonoBehaviour
{

    public int CurrentBox = 0;

    public float ForceStrength = 1;

    private List<BoxBehaviour> _boxBehaviours;

    // Use this for initialization
    void Start()
    {
        _boxBehaviours = new List<BoxBehaviour>();
        for (var i = 0; i < transform.childCount; i++)
        {
            _boxBehaviours.Add(transform.GetChild(i).GetComponent<BoxBehaviour>());
        }

        _boxBehaviours[CurrentBox].SetActiveBox(true);
    }

    void ChangeActiveBox(int newActiveBox)
    {
        _boxBehaviours[CurrentBox].SetActiveBox(false);
        CurrentBox = newActiveBox;
        _boxBehaviours[CurrentBox].SetActiveBox(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (tag != "Player") return;

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.F1))
        {
            ChangeActiveBox(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.F2))
        {
            ChangeActiveBox(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.F3))
        {
            ChangeActiveBox(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.F4))
        {
            ChangeActiveBox(3);
        }

        // Calculate OnGround For Children
        var onGround = false;
        foreach (var boxBehaviour in _boxBehaviours)
        {
            onGround |= boxBehaviour.OnGround;
            if (onGround) break;
        }

        if (onGround)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                _boxBehaviours[CurrentBox].GetComponent<Rigidbody>()
                    .AddForce(Vector3.right * ForceStrength, ForceMode.Impulse);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                _boxBehaviours[CurrentBox].GetComponent<Rigidbody>()
                    .AddForce(Vector3.left * ForceStrength, ForceMode.Impulse);
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                _boxBehaviours[CurrentBox].GetComponent<Rigidbody>()
                    .AddForce(2.5f * Vector3.up * ForceStrength, ForceMode.Impulse);    // More force Up
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                _boxBehaviours[CurrentBox].GetComponent<Rigidbody>()
                    .AddForce(Vector3.down * ForceStrength, ForceMode.Impulse);
            }
        }

        //If at least one overlapping slot place then try placing
        var atLeastOneOverlap = false;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (var boxBehaviour in _boxBehaviours)
            {
                atLeastOneOverlap |= boxBehaviour.OverlapsSlots;
                if (atLeastOneOverlap) break;
            }

            if (atLeastOneOverlap)
            {
                StopActiveTetra();
                AddToSlots();
            }
        }
    }

    void StopActiveTetra()
    {
        tag = "Untagged";
        _boxBehaviours[CurrentBox].SetActiveBox(false);
        //CurrentBox = 0;

        foreach (var boxBehaviour in _boxBehaviours)
        {
            if (boxBehaviour.OverlapsSlots)
            {
                boxBehaviour.GetComponent<Rigidbody>().isKinematic = true; // Set to kinematic if slot found.
            }
            else
            {
                boxBehaviour.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None; // Otherwise leave it in game level to obscure player unfreeze axis to have some depth
            }

            boxBehaviour.tag = "Ground"; // Make it Ground

            CharacterJoint[] joints = boxBehaviour.GetComponents<CharacterJoint>();
            foreach (var joint in joints)
            {
                Destroy(joint); // Destroy all joint constraints
            }
        }

        transform.DetachChildren(); //Remove every box from the parent (TODO: Or Maybe Change Parent?)
    }

    void AddToSlots()
    {
        int expectedSlots = 0;
        Collider foundCollider = null;
        foreach (var boxBehaviour in _boxBehaviours)
        {
            var slotCollider = boxBehaviour.AddToSlot();
            if (slotCollider != null)
            {
                ++expectedSlots;
                foundCollider = slotCollider;
            }
        }

        if (foundCollider != null)
        {
            foundCollider.transform.parent.GetComponent<SlotManager>().SetExpectedSlots(expectedSlots); //Notify SlotManager in what to expect
        }

        SpawnAndDestroy();
    }

    public void SpawnAndDestroy()
    {
        GameObject player = Resources.Load("Player") as GameObject;
        Instantiate(player);
        Destroy(gameObject);
    }
}
