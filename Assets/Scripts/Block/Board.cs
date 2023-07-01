using UnityEngine;

public class Board : MonoBehaviour
{
    public bool startBuild;
    public bool isForward;

    public int heightNum = 20;
    public int widthNum = 3;

    private float unitWidth = 0.4f;
    private float unitHeight = 0.05f;

    public int hp=0;

    void Start()
    {
        unitWidth = 1f / widthNum;
        unitHeight = 1f / heightNum;
        if(startBuild) ReBuild(isForward);
    }

    /// <summary>
    /// 木板门整体收到伤害
    /// </summary>
    /// <param name="damage">伤害值</param>
    public void GetDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            for(int i = transform.childCount - 1; i >= 0; i--)
            {
                transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;
                transform.GetChild(i).GetComponent<BoardUnit>().isLink = false;
                //transform.GetChild(i).parent = null;
            }
        }
    }

    /// <summary>
    /// 重新建造木板
    /// </summary>
    /// <param name="forward">木板门的朝向</param>
    public void ReBuild(bool forward)
    {
        hp = 100;
        GameObject unit = new GameObject();
        for (int i = 2; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                unit = Instantiate(Resources.Load<GameObject>("Block/boardunit"));
                unit.transform.parent = transform;
                unit.transform.GetComponent<Rigidbody>().isKinematic = true;
                unit.transform.localScale = new Vector3(unitWidth, unitHeight, 0.02f);
                unit.transform.localRotation = transform.parent.parent.localRotation;
                unit.transform.localPosition = new Vector3(j * unitWidth - 0.5f + unitWidth / 2f, i * unitHeight - 0.5f, forward ? 0.15f:-0.15f);
            }
        }
    }

}
