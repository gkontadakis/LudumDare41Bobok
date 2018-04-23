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

    private bool _playDestroyAnim;

    public void MarkForDestroy()
    {
        _playDestroyAnim = true;
    }

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
            color.a = Mathf.PingPong(Time.time, 1);
            GetComponent<Renderer>().material.SetColor("_Color", color);
        }
        else if(_placeAnimStarted)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, 5.0f * Time.smoothDeltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, 5.5f * Time.smoothDeltaTime);
            if (Vector3.Distance(transform.localPosition, Vector3.zero) < 0.05f)    // TODO proper check Z angle of quaternion  
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                _placeAnimStarted = false;

                transform.parent.parent.GetComponent<SlotManager>().NotifySlotManager();
            }
        }
        else if (_playDestroyAnim)
        {
            Color color = GetComponent<Renderer>().material.GetColor("_Color");
            color.a = Mathf.PingPong(2.5f * Time.time, 1);
            GetComponent<Renderer>().material.SetColor("_Color", color);

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 5.5f * Time.smoothDeltaTime);
            if (Vector3.Distance(transform.localScale, Vector3.zero) < 0.1f)
            {
                _playDestroyAnim = false;
                Destroy(gameObject);
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

        if (col.tag == "Slot")
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
            var minDistance = Vector3.Distance(transform.position, minSlot.transform.position); //minSlot.ClosestPoint(transform.position)
            foreach (var slot in _overlappingSlots)
            {
                var currentDistance = Vector3.Distance(transform.position, slot.transform.position); //slot.ClosestPoint(transform.position)
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
