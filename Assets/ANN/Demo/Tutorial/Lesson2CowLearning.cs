using UnityEngine;

public class Lesson2CowLearning : MonoBehaviour
{
    private TutorialCowControl TCC;
    public float[][] Answer = new float[1][];
    private PerceptronBackPropagationInterface PLBBPI;

    void Start()
    {
        PLBBPI = gameObject.AddComponent<PerceptronBackPropagationInterface>();
        TCC = gameObject.GetComponent<TutorialCowControl>();
        Answer[0] = new float[2];
        PLBBPI.PCT = gameObject.GetComponent<LessonsTutorialCowPerceptron>().PCT;
    }
    void Update()
    {
        if (PLBBPI.Learn)
        {
            Answer[0][0] = TCC.AngleToFood / 180F;
            if (TCC.DistanceToFood > 3.5F && Mathf.Abs(TCC.AngleToFood) < 90)
                Answer[0][1] = 1;
            else if (TCC.Satiety < 35 && TCC.DistanceToFood > 0 && Mathf.Abs(TCC.AngleToFood) < 5)
                Answer[0][1] = 1;
            else if (TCC.Move > 0)
                Answer[0][1] = 0;
            PLBBPI.Answer = Answer;
        }
    }
}
