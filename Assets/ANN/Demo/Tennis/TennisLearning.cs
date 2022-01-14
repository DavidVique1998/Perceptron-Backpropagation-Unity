using UnityEngine;

public class TennisLearning : MonoBehaviour
{
    public GameObject StudentGO;
    public TennisANN Student;
    public string Crash;
    public string Life;

    private PerceptronRandomGenerationInterface PRGI;

    void Start()
    {
        PRGI = gameObject.AddComponent<PerceptronRandomGenerationInterface>();
        PRGI.PCT = Student.PCT;
        PRGI.PLBRG.AmountOfChildren = 100;
        PRGI.PLBRG.ChildrenInWave = 10;
        PRGI.PLBRG.ChildrenDifference = 1F;
        PRGI.PLBRG.ChildrenGradient = true;
        PRGI.PLBRG.ChanceCoefficient = 0.01F;
        PRGI.PLBRG.GenerationEffect = 0.1F;
        PRGI.PLBRG.GenerationSplashEffect = 0.1F;
        PRGI.PLBRG.StudentData(StudentGO, Student, "PCT", Student, Crash, Life);
    }
}
