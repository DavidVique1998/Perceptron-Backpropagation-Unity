using UnityEngine;

public class Lesson3CowLearning : MonoBehaviour
{
    public TutorialCowControl TCС;
    public LessonsTutorialCowPerceptron TCP;

    private PerceptronRandomGenerationInterface PRGI;
    void Start()
    {
        PRGI = gameObject.AddComponent<PerceptronRandomGenerationInterface>();
        PRGI.PCT = TCP.PCT;
        PRGI.PLBRG.StudentData(TCP.gameObject, TCP, "PCT", TCС, "Death", "LifeTime");
    }
}
