using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    public GameObject uiSkill;

    private void SwitchMain()
    {
        SwitchWeapon(1);
    }

    private void SwitchSide()
    {
        SwitchWeapon(2);
    }

    //���
    public void UpdateHealth(int hp)
    {
        transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<TextMeshProUGUI>().text = hp.ToString();
        transform.GetChild(0).GetChild(4).GetComponent<Slider>().value = hp;
    }

    //����
    public void SwitchWeapon(int e)
    {
        for(int i = 0; i < 3; i++)
        {
            transform.GetChild(0).GetChild(i).GetChild(0).GetComponent<Image>().color = new Color32(0, 0, 0, 255);
            transform.GetChild(0).GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
            transform.GetChild(0).GetChild(i).GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 255);
            transform.GetChild(0).GetChild(i).transform.localScale = new Vector3(0.98f, 0.98f);
        }
        transform.GetChild(0).GetChild(e).GetChild(0).GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        transform.GetChild(0).GetChild(e).GetChild(1).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        transform.GetChild(0).GetChild(e).GetChild(2).GetComponent<TextMeshProUGUI>().color = new Color32(0, 0, 0, 255);
        transform.GetChild(0).GetChild(e).transform.localScale = new Vector3(1.02f, 1.02f);
    }

    /// <summary>
    /// ���µ�ҩ
    /// </summary>
    /// <param name="i">��ҩ��</param>
    public void UpdateAmmoMainCurrent(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = i.ToString();
    }
    public void UpdateAmmoMainLeaft(int i)
    {
        transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>().text = i.ToString();
    }
    public void UpdateAmmoSideCurrent(int i)
    {
        transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = i.ToString();
    }
    public void UpdateAmmoSideLeaft(int i)
    {
        transform.GetChild(0).GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>().text = i.ToString();
    }


    /// <summary>
    /// ���¼���
    /// </summary>
    /// <param name="i">����ID</param>
    public void UpdateSkillChoice(int i)
    {

    }

    public void UpdateSkillCurrent(int i)
    {

    }

    /// <summary>
    /// ���¼�������
    /// </summary>
    /// <param name="i">���ܱ��</param>
    /// <param name="num">��������</param>
    public void UpdateSkillNum(int i,int num)
    {
        uiSkill.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = num.ToString();
    }

    /// <summary>
    /// ���»ظ�����
    /// </summary>
    /// <param name="num">����</param>
    public void UpdateHealNum(int num)
    {
        transform.GetChild(0).GetChild(3).GetChild(1).GetComponent<TextMeshProUGUI>().text = num.ToString();
    }
}
