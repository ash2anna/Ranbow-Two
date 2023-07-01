using UnityEngine;

[CreateAssetMenu(fileName = "MuteFactory", menuName = "Factory/MuteFactory")]
public class MuteScriptableSO : FactorySO<ItemHealth>
{
    public ItemHealth skillPrefab;

    public override ItemHealth Creat()
    {
        return Instantiate(skillPrefab);
    }
}
