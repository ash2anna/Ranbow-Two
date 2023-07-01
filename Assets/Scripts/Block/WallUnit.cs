using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUnit : MonoBehaviour
{
    public int uid;
    public int Force=1000;

    public void GetDamage()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * Force);
        transform.parent.GetComponent<Wall>().CheckBroken(uid);
        transform.gameObject.SetActive(false);
    }
}
