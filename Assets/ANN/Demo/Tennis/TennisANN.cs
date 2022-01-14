using UnityEngine;

public class TennisANN : MonoBehaviour
{
    public TennisControl TC;
    private Transform T;
    public TennisBall Ball;
    private Transform B;
    private Rigidbody BallRB;
    public Transform Enemy;

    public Perceptron PCT = new Perceptron();                           // create perceptron

    public bool Crash = false;
    public float Life = 0;
    public int Goals = 0;

    void Start()
    {
        B = Ball.transform;
        BallRB = B.gameObject.GetComponent<Rigidbody>();
        T = TC.transform;
        int[] Layer = new int[2];                                       // create array of hiden layers
        Layer[0] = 9;
        Layer[1] = 9;
        PCT.CreatePerceptron(1, false, true, 3, Layer, 1);              // create perceptron
        PerceptronInterface PI = gameObject.AddComponent<PerceptronInterface>();
        PI.PCT = PCT;
    }

    void Update()
    {
        PCT.Input[0] = T.position.y / 6F;
        //PCT.Input[1] = B.position.x / 12F;
        //PCT.Input[1] = B.position.y / 7F;
        PCT.Input[1] = Vector3.Angle(TC.transform.right, B.position - TC.transform.position) * Mathf.Sign(B.position.y - TC.transform.position.y) / 180F;
        PCT.Input[2] = Vector3.Angle(-Vector3.right, BallRB.velocity.normalized) * Mathf.Sign(BallRB.velocity.y) / 180F;
        //PCT.Input[4] = Enemy.position.y / 6F;
        PCT.Solution();
        TC.Move = PCT.Output[0];

        if (Crash)
        {
            Goals = 0;
            TC.Poitns = 0;
            Life = 0;
            Crash = false;
        }
        if (TC.Poitns > 0)
        {
            TC.Poitns = 0;
            Life++;
            Goals++;
        }
        Life += Time.deltaTime;
        if (TC.Poitns < 0 || Goals > 100)
            Crash = true;
    }
}
