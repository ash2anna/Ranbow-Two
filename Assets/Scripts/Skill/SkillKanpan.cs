using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKanpan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AI"))
        {
            other.GetComponent<FSM>().GetDamage(50);
            transform.parent.gameObject.SetActive(false);
        }
    }
}
