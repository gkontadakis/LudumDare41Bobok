using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxBehaviour : MonoBehaviour
{
    private Dictionary<string, int> _tagFrequencies;

    private HashSet<Collider> _overlappingSlots;

    public bool IsActiveBox { get; set; }

    public bool OverlapsSlots
    {
        get { return _overlappingSlots.Count != 0; }
    }

    public bool OnGround
    {
        get
        {
            return _tagFrequencies != null && _tagFrequencies.ContainsKey("Ground") && _tagFrequencies["Ground"] != 0;
        }
    }

    private bool _placeAnimStarted;

    // Use this for initialization
    void Start()
    {
        _tagFrequencies = new Dictionary<string, int>();
        _overlappingSlots = new HashSet<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActiveBox)
        {
            Color color = GetComponent<Renderer>().material.GetColor("_Color");
            color.a = Mathf.PingPong(Time.time, 1); // 0.5f * (Mathf.Cos(2 * Mathf.PI * Time.realtimeSinceStartup) + 1);
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        else if(_placeAnimStarted)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 2.5f * Time.smoothDeltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 2.5f * Time.smoothDeltaTime);
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.1f && Mathf.Abs(transform.localRotation.eulerAngles.z) < 0.1f)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = transform.localRotation;
                _placeAnimStarted = false;
            }
        }
    }

    public void SetActiveBox(bool isActiveBox)
    {
        //tag =  isActiveBox ? "Player" : "Untagged";
        IsActiveBox = isActiveBox;
        if (!isActiveBox)
        {
            Color color = GetComponent<Renderer>().material.GetColor("_Color");
            color.a = 1;
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }

    }

    void OnCollisionEnter(Collision col)
    {
        if (_tagFrequencies.ContainsKey(col.gameObject.tag))
        {
            _tagFrequencies[col.gameObject.tag] += 1;
        }
        else
        {
            _tagFrequencies[col.gameObject.tag] = 1;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (_tagFrequencies.ContainsKey(col.gameObject.tag))
        {
            _tagFrequencies[col.gameObject.tag] -= 1;
        }
        else
        {
            _tagFrequencies[col.gameObject.tag] = 0;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Slot")
        {
            _overlappingSlots.Add(col);
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (tag == "Slot")
        {
            _overlappingSlots.Remove(col);
        }
    }

    public Collider AddToSlot()
    {
        Collider minSlot = null;
        if (OverlapsSlots)
        {
            minSlot = _overlappingSlots.First();
            var minDistance = Vector3.Distance(transform.position, minSlot.ClosestPoint(transform.position));
            foreach (var slot in _overlappingSlots)
            {
                var currentDistance = Vector3.Distance(transform.position, slot.ClosestPoint(transform.position));
                if (currentDistance < minDistance)
                {
                    currentDistance = minDistance;
                    minSlot = slot;
                }
            }

            // We have slot here add to parent and center / rotate at once (TODO: Add anim)
            transform.parent = minSlot.transform;
            _placeAnimStarted = true;
        }

        return minSlot;
    }

}
