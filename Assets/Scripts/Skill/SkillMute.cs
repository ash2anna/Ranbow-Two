using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMute : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            other.GetComponent<FSM>().InWifi(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            other.GetComponent<FSM>().InWifi(false);
        }
    }
}
