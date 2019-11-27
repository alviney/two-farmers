using System.Collections.Generic;
using UnityEngine;

public class ClimberController : MonoBehaviour
{
    private OVRPlayerController m_ovrCharacterController;
    List<Climber> climbers = new List<Climber>();
    private float originalGravityModifier = 1f;

    private void Start()
    {
        m_ovrCharacterController = GetComponent<OVRPlayerController>();

        originalGravityModifier = m_ovrCharacterController.GravityModifier;
    }

    public void AddClimber(Climber climber)
    {
        if (!climbers.Contains(climber))
        {
            climbers.Add(climber);
        }

        m_ovrCharacterController.GravityModifier = 0;
    }

    public void RemoveClimber(Climber climber)
    {
        if (climbers.Contains(climber))
        {
            climbers.Remove(climber);
        }

        if (climbers.Count == 0)
        {
            m_ovrCharacterController.GravityModifier = originalGravityModifier;

        }
    }
}
