using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHealth : MonoBehaviour,IGetDamage
{
    private int health;

    void Start()
    {
        health = 1;
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            transform.position = new Vector3(0, -3, 0);
            //gameObject.SetActive(false);
        }
    }

}
