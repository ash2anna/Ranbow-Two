using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkill : MonoBehaviour
{
    public KanpanScriptableSO eddFactory;
    public MuteScriptableSO wifiFactory;

    /// <summary>
    /// ����-���ù���
    /// </summary>
    /// <param name="tag">�ж���������Ŀ���Ƿ����ſ�</param>
    /// <param name="pos">�ſ�λ��</param>
    /// <param name="height">���׷��ø߶�</param>
    /// <param name="front">���׷���ǰ��</param>
    /// <param name="left">���׷�������</param>
    public void CreatEDD(string tag, Transform pos, float height,Vector3 front, bool left)
    {
        if (tag.Equals("DoorFrame"))
        {
            ItemHealth edd = eddFactory.Creat(14f,left);
            edd.transform.position = new Vector3(0, height, 0);
            edd.transform.eulerAngles = pos.parent.parent.eulerAngles + new Vector3(0, -90, 0);
            edd.transform.position = new Vector3(pos.position.x, height, pos.position.z) + front * 0.2f;
        }
        else if (tag.Equals("WindowFrame"))
        {
            ItemHealth edd = eddFactory.Creat(14f, left);
            edd.transform.position = new Vector3(0, height, 0);
            edd.transform.eulerAngles = pos.parent.parent.eulerAngles + new Vector3(0, -90, 0);
            edd.transform.position = new Vector3(pos.position.x, height, pos.position.z) + front * 0.2f;
        }
        else if (tag.Equals("DoorWideFrame"))
        {
            ItemHealth edd = eddFactory.Creat(28f, left);
            edd.transform.position = new Vector3(0, height, 0);
            edd.transform.eulerAngles = pos.parent.parent.eulerAngles + new Vector3(0, -90, 0);
            edd.transform.position = new Vector3(pos.position.x, height, pos.position.z) + front * 0.2f;
        }
    }

    public void CreatWifi()
    {
        ItemHealth wifi = wifiFactory.Creat();
        wifi.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
    }
}
