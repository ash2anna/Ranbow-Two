using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int heightNum=16;
    public int widthNum=16;

    private float unitWidth=0.1f;
    private float unitHeight=0.1f;

    private GameObject[,] unitArray;
    private bool[,] isSearched;
    private bool[,] isBroken;
    private bool[] isLink = new bool[4];

    void Start()
    {
        heightNum = (int)(transform.localScale.y * 10);
        widthNum = (int)(transform.localScale.x * 10);

        Build();
    }

    //生成木墙
    public void Build()
    {
        GetComponent<MeshRenderer>().enabled = false;

        GameObject unit = new GameObject();
        unitArray = new GameObject[heightNum, widthNum];
        isSearched = new bool[heightNum, widthNum];
        isBroken = new bool[heightNum, widthNum];

        for (int i = 0; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                unit = Instantiate(Resources.Load<GameObject>("Block/wallunit"));
                unit.transform.parent = transform;
                unit.transform.GetComponent<Rigidbody>().isKinematic = true;
                unit.transform.localPosition = new Vector3(j * unitHeight / transform.localScale.x - 0.5f, i * unitWidth / transform.localScale.y - 0.5f, 0);

                unit.transform.GetComponent<WallUnit>().uid = i * widthNum + j;
                unitArray[i, j] = unit;
            }
        }
    }

    /// <summary>
    /// 检查四方是否脱离
    /// </summary>
    /// <param name="uid">四方中心的编号</param>
    public void CheckBroken(int uid)
    {
        int x = uid / widthNum;
        int y = uid % widthNum;
        

        for (int i = 0; i < 4; i++)
        {
            isLink[i] = false;
        }
        isBroken[x, y] = true;

        for (int i = 0; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                isSearched[i, j] = false;
            }
        }
        DFS(x - 1, y, 0);
        for (int i = 0; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                isSearched[i, j] = false;
            }
        }
        DFS(x + 1, y, 1);
        for (int i = 0; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                isSearched[i, j] = false;
            }
        }
        DFS(x, y - 1, 2);
        for (int i = 0; i < heightNum; i++)
        {
            for (int j = 0; j < widthNum; j++)
            {
                isSearched[i, j] = false;
            }
        }
        DFS(x, y + 1, 3);

        for (int i = 0; i < 4; i++)
        {
            if (!isLink[i])
            {
                switch (i)
                {
                    case 0: Broken(x - 1, y);break;
                    case 1: Broken(x + 1, y); break;
                    case 2: Broken(x, y - 1); break;
                    case 3: Broken(x, y + 1); break;
                    default:break;
                }
            }
        }
    }

    /// <summary>
    /// 深度优先搜索是否与边界相连
    /// </summary>
    /// <param name="x">坐标</param>
    /// <param name="y">坐标</param>
    /// <param name="part">脱落部分</param>
    private void DFS(int x,int y,int part)
    {
        if (x < 0 || y < 0 || x > heightNum - 1 || y > widthNum - 1) return;
        if (isSearched[x, y]) return;
        if (isBroken[x, y]) {
            return;
        }

        if (x == 0 || y == 0 || x == heightNum - 1 || y == widthNum - 1)
        {
            if (!isBroken[x, y]) isLink[part] = true;
            return;
        }

        isSearched[x, y] = true;

        DFS(x - 1, y, part);
        if (isLink[part]) return;
        DFS(x + 1, y, part);
        if (isLink[part]) return;
        DFS(x, y - 1, part);
        if (isLink[part]) return;
        DFS(x, y + 1, part);
        if (isLink[part]) return;
    }

    //递归破坏其他方块
    private void Broken(int x, int y)
    {
        if (x <= 0 || y <= 0 || x >= heightNum - 1 || y >= widthNum - 1) return;
        if (isBroken[x, y]) return;

        unitArray[x,y].GetComponent<Rigidbody>().isKinematic = false;
        isBroken[x, y] = true;

        Broken(x - 1, y);
        Broken(x + 1, y);
        Broken(x, y - 1);
        Broken(x, y + 1);
    }

    //加固墙
    public void Strength()
    {

    }
}
