using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum StateType
{
    Idle,Patrol,Chase,React,Attack,Hit,Death
}
public enum AIType
{
    Breaker,Buster,Detecter,Supptor,Controler
}

[Serializable]
public class Parameter
{
    public int health;
    public float speed;
    public float idleTime;
    public float attackTime;
    public int damage;
    public int getdamage;
    public bool getHit;
    public bool InWifi;
    public Transform target;
    public Vector3 targetPos;
    public Transform[] patrolPoints;
    public Transform[] chasePoints;

    public GameObject Player;
    public OutAI totalManager;

}

public class FSM : MonoBehaviour,IGetDamage
{
    public Parameter parameter;
    private IState currentState;
    public AIType type;

    public NavMeshAgent nav;

    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Hit, new HitState(this));
        states.Add(StateType.Death, new DeathState(this));

        TransState(StateType.Idle);
    }

    void Update()
    {
        currentState.OnUpdate();
    }

    //�л�״̬
    public void TransState(StateType type)
    {
        if (currentState != null)
            currentState.OnExit();
        currentState = states[type];
        currentState.OnEnter();
    }

    /// <summary>
    /// wifiӰ��
    /// </summary>
    /// <param name="isIn">�Ƿ���WiFi��Χ��</param>
    public void InWifi(bool isIn)
    {
        parameter.InWifi = isIn;
    }

    /// <summary>
    /// �յ��˺�
    /// </summary>
    /// <param name="damage">�˺�ֵ</param>
    public void GetDamage(int damage)
    {
        parameter.getdamage = damage;
        parameter.getHit = true;
    }

    //����
    public void Dead()
    {
        Destroy(this.gameObject);
    }

    
}
