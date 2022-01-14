using UnityEngine;

public class TennisControl : MonoBehaviour
{
    public float Speed = 1;
    public float Move;
    private float M;
    public float X;
    public TennisBall TB;
    public int Poitns = 0;

    void Start()
    {
        X = transform.position.x;
        transform.position = new Vector3(X, 0, 0);
        Poitns = 0;
        if (Speed > 10)
            Speed = 10;
    }

    void Update()
    {
        if (TB.Goal)
            transform.position = new Vector3(X, 0, 0);

        if (Mathf.Abs(Move) > 1 / 3F)
            M = Mathf.Lerp(M, Mathf.Sign(Move), 0.1F);
        else
            M = 0;
        transform.Translate(0, M * Speed * 5F * Time.deltaTime, 0);

        if (transform.position.y > 6)
            transform.position = new Vector3(X, 6, 0);
        else if (transform.position.y < -6)
            transform.position = new Vector3(X, -6, 0);
    }
}
