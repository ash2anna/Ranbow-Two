using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSkill : MonoBehaviour
{
    public KanpanScriptableSO eddFactory;
    public MuteScriptableSO wifiFactory;

    /// <summary>
    /// 技能-放置诡雷
    /// </summary>
    /// <param name="tag">判断视线命中目标是否是门框</param>
    /// <param name="pos">门框位置</param>
    /// <param name="height">诡雷放置高度</param>
    /// <param name="front">诡雷放置前后</param>
    /// <param name="left">诡雷放置左右</param>
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
