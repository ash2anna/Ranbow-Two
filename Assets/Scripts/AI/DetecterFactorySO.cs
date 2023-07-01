using UnityEngine;

[CreateAssetMenu(fileName = "DetecterFactory", menuName = "Factory/DetecterFactory")]
public class DetecterFactorySO : FactorySO<FSM>
{
    public FSM aiPrefab;

    public override FSM Creat()
    {
        return Instantiate(aiPrefab);
    }
}

