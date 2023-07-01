using UnityEngine;

public class AimSight : MonoBehaviour
{
    public Material aimSight;
    public Material aimDefault;

    private MeshRenderer ren;

    void Start()
    {
        ren = GetComponent<MeshRenderer>();
    }

    public void ChangeAimSight(bool isSight)
    {
        ren.material = isSight ? aimSight : aimDefault;
    }
}
