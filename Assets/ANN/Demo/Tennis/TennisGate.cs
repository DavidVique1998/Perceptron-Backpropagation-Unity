using UnityEngine;

public class TennisGate : MonoBehaviour
{
    public TennisControl TC1;
    public TennisControl TC2;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<TennisBall>().Goal = true;
        TC1.Poitns++;
        TC2.Poitns--;
    }
}
