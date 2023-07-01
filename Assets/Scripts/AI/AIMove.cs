using UnityEngine;
using UnityEngine.AI;

public class AIMove : MonoBehaviour
{
    private NavMeshAgent nav;
    //public Transform trans;
    public Vector3 trans;
    private float time;
    
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Time.time - time > 1f)
        {
            CheckPos();
            time = Time.time;
        }
        
    }

    private void CheckPos()
    {
        trans = new Vector3(transform.position.x + 1f, transform.position.y, transform.position.z);
        nav.SetDestination(trans);
        //nav.SamplePathPosition()
    }
}
