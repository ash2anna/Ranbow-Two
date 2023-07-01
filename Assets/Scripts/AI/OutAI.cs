using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OutAI : MonoBehaviour
{
    public GameObject Player;
    public GameObject patrolPos;

    public BreakerFactorySO breakerFactory;
    public DetecterFactorySO detecterFactory;

    //生成选项
    public int breakNum;
    public int detecNum;
    public bool isOpen;
    private float timer;
    private float timerBreaker;
    private float timerDetecter;


    public void CreatBreaker()
    {
        FSM fsm = breakerFactory.Creat();
        fsm.transform.parent = transform.GetChild(1);
        fsm.GetComponent<NavMeshAgent>().gameObject.SetActive(false);
        fsm.transform.localPosition = new Vector3(0, 0, 0);
        fsm.GetComponent<NavMeshAgent>().gameObject.SetActive(true);
        fsm.parameter.Player = Player;
        fsm.parameter.totalManager = GetComponent<OutAI>();

        //for(int i = 0; i < transform.GetChild(0).childCount; i++)
        //{
        //    fsm.GetComponent<FSM>().parameter.patrolPoints[i] = transform.GetChild(0).GetChild(i).transform;
        //}
        for (int i = 0; i < patrolPos.transform.childCount; i++)
        {
            fsm.GetComponent<FSM>().parameter.patrolPoints[i] = patrolPos.transform.GetChild(i).transform;
        }
    }

    public void CreatDetecter()
    {
        FSM fsm = detecterFactory.Creat();
        fsm.transform.parent = transform.GetChild(2);
        fsm.GetComponent<NavMeshAgent>().gameObject.SetActive(false);
        fsm.transform.localPosition = new Vector3(0, 0, 0);
        fsm.GetComponent<NavMeshAgent>().gameObject.SetActive(true);
        fsm.parameter.Player = Player;
        fsm.parameter.totalManager = GetComponent<OutAI>();

        //for (int i = 0; i < transform.GetChild(0).childCount; i++)
        //{
        //    fsm.GetComponent<FSM>().parameter.patrolPoints[i] = transform.GetChild(0).GetChild(i).transform;
        //}
        for (int i = 0; i < patrolPos.transform.childCount; i++)
        {
            fsm.GetComponent<FSM>().parameter.patrolPoints[i] = patrolPos.transform.GetChild(i).transform;
        }
    }

    //通报全体玩家位置
    public void GetPlayerPosition(Vector3 pos)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < transform.GetChild(0).childCount; j++)
            {
                transform.GetChild(i).GetChild(j).GetComponent<FSM>().parameter.targetPos = pos;
            }
        }
    }

    private void Start()
    {
        if (!isOpen)
            return;
        //CreatBreaker();
        //CreatDetecter();
    }

    private void Update()
    {
        if (Time.time - timerBreaker > Mathf.Clamp(Random.Range(12 - Time.time / 60, 16 - Time.time / 60), 1, 16)) 
        {
            CreatBreaker();
            timerBreaker = Time.time;
        }

        if (Time.time - timerDetecter > Mathf.Clamp(Random.Range(24 - Time.time / 60, 32 - Time.time / 60), 2, 32))
        {
            CreatDetecter();
            timerDetecter = Time.time;
        }
    }
}
