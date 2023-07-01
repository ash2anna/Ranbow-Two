using UnityEngine;

[CreateAssetMenu(fileName = "KanpanFactory", menuName = "Factory/KanpanFactory")]
public class KanpanScriptableSO : FactorySO<ItemHealth>
{
    public ItemHealth skillPrefab;

    public override ItemHealth Creat()
    {
        return Instantiate(skillPrefab);
    }

    /// <summary>
    /// 生成诡雷
    /// </summary>
    /// <param name="width">触发器宽度</param>
    /// <param name="left">触发器中心</param>
    /// <returns></returns>
    public ItemHealth Creat(float width, bool left)
    {
        ItemHealth edd = Instantiate(skillPrefab);
        //edd.transform.localScale = new Vector3(0.2f, 0.3f, 0.1f);
        edd.transform.GetChild(0).GetComponent<BoxCollider>().size = new Vector3(1, 1, width);
        edd.transform.GetChild(0).GetComponent<BoxCollider>().center = new Vector3(0, 0, (left ? -width : width) / 2);
        return edd;
    }
}