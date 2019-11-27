/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows grabbing and throwing of objects with the Climbable component on them.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Climber : MonoBehaviour
{
    public ClimberController climberController;
    public CharacterController m_characterController;
    // Grip trigger thresholds for picking up objects, with some hysteresis.
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    // // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
    // // Also used for ranking grab targets in case of multiple candidates.
    // [SerializeField]
    // protected Transform m_gripTransform = null;

    // [SerializeField]
    // protected Collider[] m_grabVolumes = null;

    // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    protected OVRInput.Controller m_controller;

    [SerializeField]
    protected Transform m_parentTransform;

    // protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
    protected Climbable m_grabbedObj = null;
    private Climbable m_potentialGrabbable = null;
    // protected Vector3 m_grabbedObjectPosOff;
    // protected Quaternion m_grabbedObjectRotOff;
    // protected Dictionary<Climbable, int> m_grabCandidates = new Dictionary<Climbable, int>();
    // protected bool operatingWithoutOVRCameraRig = true;

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public Climbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if (m_parentTransform == null)
        {
            if (gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }
    }


    void Update()
    {
        OnUpdatedAnchors();
    }

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on
    // your hands or held objects, you may wish to switch to parenting.
    public virtual void OnUpdatedAnchors()
    {
        Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
        Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + handPos);
        Quaternion destRot = m_parentTransform.rotation * handRot * m_anchorOffsetRotation;
        GetComponent<Rigidbody>().MovePosition(destPos);
        GetComponent<Rigidbody>().MoveRotation(destRot);

        if (m_grabbedObj != null)
        {
            m_characterController.Move(transform.position - destPos);
        }

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

        float prevFlex = m_prevFlex;
        // Update values from inputs
        m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

        CheckForGrabOrRelease(prevFlex);
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
        Climbable grabbable = otherCollider.GetComponent<Climbable>() ?? otherCollider.GetComponentInParent<Climbable>();
        if (grabbable == null) return;
        m_potentialGrabbable = grabbable;
    }

    void OnTriggerExit(Collider otherCollider)
    {
        Climbable grabbable = otherCollider.GetComponent<Climbable>() ?? otherCollider.GetComponentInParent<Climbable>();
        if (grabbable == null) return;
        m_potentialGrabbable = null;
    }

    protected void CheckForGrabOrRelease(float prevFlex)
    {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            if (m_potentialGrabbable != null)
            {
                m_grabbedObj = m_potentialGrabbable;
                climberController.AddClimber(this);
            }
            // GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            m_grabbedObj = null;
            climberController.RemoveClimber(this);
            // GrabEnd();
        }
    }
}
