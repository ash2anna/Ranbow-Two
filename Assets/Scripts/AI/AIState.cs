using UnityEngine;
using StarterAssets;

public abstract class AIState : IState
{
    public FSM manager;
    public Parameter parameter;

    protected Vector3 dir;
    private Ray ray;
    private RaycastHit hit;
    private LayerMask mask;

    protected bool isFound;

    public AIState(FSM manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public abstract void OnEnter();
    public virtual void OnUpdate()
    {
        if (parameter.getHit)
        {
            manager.TransState(StateType.Hit);
        }

        if (manager.type == AIType.Detecter && parameter.InWifi)
        {
            return;
        }
        //else if (manager.type == AIType.Detecter && !parameter.InWifi)
        //{
        //    manager.nav.enabled = true;
        //}

        dir = parameter.Player.transform.position - manager.transform.position + new Vector3(0, 0.8f, 0);
        ray = new Ray(manager.transform.position, dir);
        mask = ~((1 << 15) | (1 << 16));

        if (Physics.Raycast(ray, out hit, 30f, mask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                parameter.targetPos = parameter.Player.transform.position;
                isFound = true;
            }
            else
            {
                isFound = false;
            }
        }
    }
    public abstract void OnExit();

}


//����
public class IdleState : AIState
{
    private float timer;

    public IdleState(FSM manager):base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
        timer = 0;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        timer += Time.deltaTime;
        if(timer >= parameter.idleTime)
        {
            manager.TransState(StateType.Patrol);
        }
    }
    public override void OnExit()
    {
        timer = 0;
    }
}

//Ѳ��
public class PatrolState : AIState
{
    private int posID;
    

    public PatrolState(FSM manager) : base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (manager.type == AIType.Detecter && parameter.InWifi)
        {
            manager.nav.enabled = false;
            return;
        }
        else if (manager.type == AIType.Detecter && !parameter.InWifi)
        {
            manager.nav.enabled = true;
        }

        manager.nav.SetDestination(parameter.patrolPoints[posID].position);

        if (isFound)
        {
            manager.TransState(StateType.Chase);
        }

        if (Vector3.Distance(manager.transform.position, parameter.patrolPoints[posID].position) < 2f)
        {
            manager.TransState(StateType.Idle);
        }

    }
    public override void OnExit()
    {
        posID++;
        if (posID >= parameter.patrolPoints.Length)
            posID = 0;
    }

}

//׷��
public class ChaseState : AIState
{
    public ChaseState(FSM manager) : base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (manager.type == AIType.Detecter && parameter.InWifi)
        {
            manager.nav.enabled = false;
            return;
        }
        else if (manager.type == AIType.Detecter && !parameter.InWifi)
        {
            manager.nav.enabled = true;
        }

        switch (manager.type)
        {
            case AIType.Breaker: BreakerChase(); break;
            case AIType.Detecter: DetecterChase(); break;
            default: break;
        }


        if (Vector3.Distance(manager.transform.position, parameter.targetPos) < 2f && !isFound)
        {
            isFound = false;
            manager.TransState(StateType.Idle);
        }
    }
    public override void OnExit()
    {
    }

    //�ƻ���׷��
    void BreakerChase()
    {
        manager.nav.SetDestination(parameter.targetPos - dir.normalized * 1.5f);

        //���ֲ��Ҿ����㹻����ת������״̬
        if (Vector3.Distance(manager.transform.position, parameter.Player.transform.position) <= 2f && isFound)
        {
            manager.TransState(StateType.Attack);
        }
    }

    //̽����׷��
    void DetecterChase()
    {
        //���ֲ��Ҿ����Զ�����ƶ�һ������
        if (isFound && Vector3.Distance(manager.transform.position, parameter.Player.transform.position) > 10f)
        {
            manager.nav.SetDestination(parameter.targetPos - dir.normalized * 10f);
        }
        //���ֲ��Ҿ����㹻����������ͨ��ȫ��
        else if (isFound && Vector3.Distance(manager.transform.position, parameter.Player.transform.position) < 10f)
        {
            //manager.nav.SetDestination(parameter.target.position - dir.normalized * 1.5f);
            parameter.totalManager.GetPlayerPosition(parameter.Player.transform.position);
        }
        //δ���֡����ƶ�������ֵص�
        else if (!isFound)
        {
            manager.nav.SetDestination(parameter.targetPos - dir.normalized * 1.5f);
        }
    }
}


//����
public class AttackState : AIState
{
    private float timer;

    public AttackState(FSM manager) : base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
        if (manager.type == AIType.Supptor || manager.type == AIType.Detecter) manager.TransState(StateType.Chase);
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        if (Vector3.Distance(manager.transform.position, parameter.Player.transform.position) > 2f || !isFound)
        {
            manager.TransState(StateType.Chase);
        }

        timer += Time.deltaTime;
        if (timer >= parameter.attackTime)
        {
            parameter.Player.GetComponent<FirstPersonController>().GetDamage(parameter.damage);
            timer = 0;
        }
    }
    public override void OnExit()
    {
        timer = 0;
    }
}


//�ܻ�
public class HitState : AIState
{
    public HitState(FSM manager):base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
        parameter.health -= parameter.getdamage;
    }

    public override void OnUpdate()
    {
        if (parameter.health <= 0)
        {
            manager.TransState(StateType.Death);
        }
        else
        {
            parameter.targetPos = parameter.Player.transform.position;
            manager.TransState(StateType.Chase);
        }
    }

    public override void OnExit()
    {
        parameter.getHit = false;
    }
}


//����
public class DeathState : AIState
{
    public DeathState(FSM manager) : base(manager)
    {
        this.manager = manager;
        this.parameter = manager.parameter;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate()
    {
        manager.Dead();
    }

    public override void OnExit()
    {
    }

    
}