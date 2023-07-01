using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Climb : MonoBehaviour
{
    public GameObject Player;

    private void OnTriggerEnter(Collider other)
    {
        Vector3 direction = Player.transform.position - transform.position;
        float dot = Vector3.Dot(direction.normalized, transform.forward);
        Vector3 target = Player.transform.position += dot < 0 ? transform.forward * 3 : -transform.forward * 3;
        Player.GetComponent<FirstPersonController>().climbTargetPosition = target;

        Player.GetComponent<FirstPersonController>().canClimb = true;
    }
    private void OnTriggerExit(Collider other)
    {
        Player.GetComponent<FirstPersonController>().canClimb = false;
    }
}
