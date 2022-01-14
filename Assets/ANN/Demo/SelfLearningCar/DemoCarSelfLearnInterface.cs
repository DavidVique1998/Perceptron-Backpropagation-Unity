using UnityEngine;

public class DemoCarSelfLearnInterface : MonoBehaviour
{
    public GameObject StudentGO;
    public DemoCarANN Student;
    public string Crash;
    public string Life;

    private PerceptronRandomGenerationInterface PRGI;

    private float CarsSpeed = 10;
    

    private bool ShowMenu = true;

    void Start()
    {
        PRGI = StudentGO.gameObject.AddComponent<PerceptronRandomGenerationInterface>();
        PRGI.PCT = Student.PCT;

        PRGI.PLBRG.AmountOfChildren = 100;
        PRGI.PLBRG.ChildrenInWave = 100;
        PRGI.PLBRG.ChildrenByWave = false;
        PRGI.PLBRG.ChildrenGradient = true;
        PRGI.PLBRG.StudentData(StudentGO, Student, "PCT", Student.Car, Crash, Life);
    }

    void Update()
    {
        if (Student != null)
        {
            Student.Car.SpeedScale = CarsSpeed;
            if (Student.ReloadedANN)
            {
                PRGI.PLBRG.Reset();
                PRGI.Learn = false;
                Student.ReloadedANN = false;
            }
        }
    }

    void OnGUI()
    {
        ShowMenu = InterfaceGUI.Button(1, 1, "Show menu", "Hide menu", ShowMenu);
        if (ShowMenu)
        {
            CarsSpeed = InterfaceGUI.HorizontalSlider(1, 2, "Cars speed", CarsSpeed, 0.1F, 20F);
            Student.ByAxisControl = InterfaceGUI.Button(1, 3, "Get button", "Get axis", Student.ByAxisControl, ref Student.ReloadANN);
        }
    }
}

