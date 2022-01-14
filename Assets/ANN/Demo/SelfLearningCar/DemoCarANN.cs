using UnityEngine;

public class DemoCarANN : MonoBehaviour
{
    public DemoCar Car;

    public Perceptron PCT = new Perceptron();                           // create perceptron

    public bool ByAxisControl = true;

    public bool ReloadANN = false;
    public bool ReloadedANN = false;

    public bool Crash = false;
    public float Distance = 0;

    void Start()
    {
        int[] Layer = new int[1];                                       // create array of hiden layers
        Layer[0] = 45;
        PCT.CreatePerceptron(1, false, false, 9, Layer, 2);             // create perceptron
        PerceptronInterface PI = gameObject.AddComponent<PerceptronInterface>();
        PI.PCT = PCT;
    }

    void Update()
    {
        if (Car == null)
            Car = gameObject.GetComponent<DemoCar>();
        else
        {
            if (ReloadANN)
            {
                if (ByAxisControl)
                    PCT.CreatePerceptron(PCT.AFS, PCT.B, PCT.AFWM, PCT.Input.Length, PCT.NIHL, 2);
                else
                    PCT.CreatePerceptron(PCT.AFS, PCT.B, PCT.AFWM, PCT.Input.Length, PCT.NIHL, 4);
                ReloadANN = false;
            }

            int i = 0;
            RaycastHit Hit;
            int s = 0;
            if (PCT.B)
                s = 1;
            float HMR = 180F / (PCT.Input.Length - 1 - s);
            while (i < PCT.Input.Length - s)
            {
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 0.1F, 0)), transform.TransformDirection(Quaternion.AngleAxis(HMR * i - 90F, Vector3.up) * Vector3.forward), out Hit, 100F, 1 << 0))
                    PCT.Input[i] = Hit.distance / 100F;
                else
                    PCT.Input[i] = 1F;
                Debug.DrawLine(transform.position + transform.TransformDirection(new Vector3(0, 0.1F, 0)), Hit.point, Color.red);
                i++;
            }
            PCT.Solution();
            if (ByAxisControl)
            {
                if (PCT.AFWM)
                {
                    Car.Turn = PCT.Output[0];
                    Car.Accelerate = PCT.Output[1];
                }
                else
                {
                    Car.Turn = PCT.Output[0] * 2F - 1F;
                    Car.Accelerate = PCT.Output[1] * 2F - 1F;
                }
            }
            else
            {
                if (PCT.AFWM)
                {
                    Car.Turn = (PCT.Output[1] + PCT.Output[0]) / 2F;
                    Car.Accelerate = (PCT.Output[3] + PCT.Output[2]) / 2F;
                }
                else
                {
                    Car.Turn = PCT.Output[1] - PCT.Output[0];
                    Car.Accelerate = PCT.Output[3] - PCT.Output[2];
                }
            }
        }
        Crash = Car.Crash;
    }
}
