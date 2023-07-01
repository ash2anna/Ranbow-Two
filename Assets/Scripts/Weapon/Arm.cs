using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Arm : MonoBehaviour
{
    public enum ArmType {
        attack,heal
    }
    public FirstPersonController player;
    private ArmType armType;
    public int preWeapon;
    private Animator animator;


    void OnEnable()
    {
        animator = GetComponent<Animator>();

        for (int i = 2; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        armType = player.armType;
        preWeapon = player.switchCurrent;

        switch (armType)
        {
            case ArmType.attack: transform.GetChild(3).gameObject.SetActive(true); animator.SetTrigger("attack"); break;
            case ArmType.heal: transform.GetChild(2).gameObject.SetActive(true); animator.SetTrigger("heal"); break;
            default: End(); break;
        }
    }

    //帧事件：结束近战攻击
    public void AttackEnd()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        LayerMask mask = ~(1 << 15);

        if (Physics.Raycast(ray, out RaycastHit hit, 1.5f, mask))
        {
            Debug.DrawLine(new Vector3(Screen.width / 2f, Screen.height / 2f, 0), hit.point, Color.red);
            if (hit.collider.CompareTag("WallUnit"))
            {
                hit.transform.GetComponent<WallUnit>().GetDamage();
            }
            if (hit.collider.CompareTag("BoardUnit"))
            {
                if (hit.transform.parent.GetComponent<Board>().hp > 0)
                {
                    hit.transform.GetComponent<BoardUnit>().GetDamage(40);
                }
            }
        }
    }

    //帧事件：结束治疗
    public void HealEnd()
    {
        player.GetDamage(-50);
    }

    //帧事件：动画播放完成
    public void End()
    {
        transform.parent.GetChild(preWeapon).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
