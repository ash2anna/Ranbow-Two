using UnityEngine;

[CreateAssetMenu(fileName = "BreakerFactory", menuName = "Factory/BreakerFactory")]
public class BreakerFactorySO : FactorySO<FSM>
{
    public FSM aiPrefab;

    public override FSM Creat()
    {
        return Instantiate(aiPrefab);
    }
}
