using UnityEngine;

public class TennisBot : MonoBehaviour
{
    public Transform Ball;
    private float Move;
    public TennisControl TC;

    void Update()
    {
        Move = Ball.position.y - transform.position.y;
        if (Move < -1)
            Move = -1;
        else if (Move > 1)
            Move = 1;

        TC.Move = Move;
    }
}