using UnityEngine;

public class Lesson1CowLearning : MonoBehaviour
{

	void Start ()
    {
        int i = 0;
        int c = -1;
        int p = -1;
        int a = 9; //number of angular variations
        int d = 4; //number of distance variations
        int s = 4; //number of variations of hunger
        float[][] Answers = new float[a * d * s][];
        float[][] Tasks = new float[a * d * s][];
        while (i < Answers.Length)
        {
            if (i % (a * s) == 0)
                c++;
            if (i % a == 0)
                p++;
            Tasks[i] = new float[3];
            Tasks[i][0] = ((-90F + 180F / (a - 1) * (i % a)) / (c + p + 1)) / 180F;
            Tasks[i][1] = ((41F - 41F / d * (c % d)) / (c + 1)) / 41F;
            Tasks[i][2] = (50F - 50F / s * (p % s)) / 50F;
            Answers[i] = new float[2];
            Answers[i][0] = Tasks[i][0];
            Answers[i][1] = 0;
            if (Tasks[i][1] > 3.5F / 41F && Mathf.Abs(Tasks[i][0]) < 45F / 180F)
                Answers[i][1] = 1;
            if (Tasks[i][2] < 25F / 50F && Mathf.Abs(Tasks[i][0]) < 2.5F / 180F)
                Answers[i][1] = 1;
            i++;
        }
        PerceptronBackPropagationInterface PLBBPI = gameObject.AddComponent<PerceptronBackPropagationInterface>();
        PLBBPI.Task = Tasks;
        PLBBPI.Answer = Answers;
        PLBBPI.PCT = gameObject.GetComponent<LessonsTutorialCowPerceptron>().PCT;
    }
	
	void Update () {
		
	}
}
