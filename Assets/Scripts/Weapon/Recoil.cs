using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    public Vector2 recoil;
    public Vector2 current;
    public Vector2 addSpeed = new Vector2(1, 0.75f);
    public Vector2 subSpeed = new Vector2(5, 5f);


    private void Update()
    {
        if (recoil.x != 0 || recoil.y != 0)
        {
            recoil.x = Mathf.MoveTowards(recoil.x, 0, subSpeed.x * Time.deltaTime);
            recoil.y = Mathf.MoveTowards(recoil.y, 0, subSpeed.y * Time.deltaTime);

            transform.localEulerAngles = new Vector3(-recoil.y, recoil.x, 0);
        }
    }

    public void AddRecoil(int ammoNum, int xDir)
    {
        recoil.x += xDir * addSpeed.x;
        recoil.y += addSpeed.y;
    }
}
