using UnityEngine;

public class BoardUnit : MonoBehaviour
{
    public bool isLink = true;
    public int Force = 1000;

    public void GetDamage(int damage)
    {
        if (isLink)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            transform.parent.GetComponent<Board>().GetDamage(damage);
            isLink = false;
        }
        GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * Force);
    }
}
