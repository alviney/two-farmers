using UnityEngine;
using UnityEngine.Events;

public class Trigger : OVRGrabbable
{
    public UnityEvent onTriggerPull;
    public UnityEvent onTriggerReleased;
    // Trigger thresholds for picking up objects, with some hysteresis.
    public float triggerBegin = 0.55f;
    public float triggerEnd = 0.35f;
    // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    protected OVRInput.Controller m_controller;
    protected float m_prevTriggerFlex;

    private void Update()
    {
        float prevTriggerFlex = m_prevTriggerFlex;

        // Update values from inputs
        m_prevTriggerFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, m_controller);

        CheckForTriggerOrRelease(prevTriggerFlex);
    }
    private void CheckForTriggerOrRelease(float prevFlex)
    {
        if ((m_prevTriggerFlex >= triggerBegin) && (prevFlex < triggerBegin))
        {
            TriggerBegin();
        }
        else if ((m_prevTriggerFlex <= triggerEnd) && (prevFlex > triggerEnd))
        {
            TriggerEnd();
        }
    }

    public void TriggerBegin()
    {
        if (m_grabbedBy != null)
        {
            onTriggerPull.Invoke();
        }
    }

    public void TriggerEnd()
    {
        if (m_grabbedBy != null)
        {
            onTriggerReleased.Invoke();
        }
    }
}
