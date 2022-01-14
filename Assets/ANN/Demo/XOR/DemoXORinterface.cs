using UnityEngine;

public class DemoXORinterface : MonoBehaviour
{
    private int[] Task;
    private int Answer;

    private float[][] LearnTask;
    private float[][] LearnAnswer;

    private Perceptron PCT;

    private PerceptronInterface PI;
    private PerceptronBackPropagationInterface PBPI;

    void Start()
    {
        int[] Layer = new int[1];                                               // create array of hiden layers
        Layer[0] = 3;

        PCT = new Perceptron();                                                 // create perceptron
        PCT.CreatePerceptron(1, false, false, 2, Layer, 1);

        Task = new int[PCT.Input.Length];                                       //  create array of task enters

        PBPI = gameObject.AddComponent<PerceptronBackPropagationInterface>();   // add learning interface
        PBPI.PCT = PCT;                                                         // perceptron to learning interface

        PI = gameObject.AddComponent<PerceptronInterface>();                    // add perceptron interface 
        PI.PCT = PCT;                                                           // perceptron to perceptron interface
        PI.Reload = true;                                                       // need to reload perceptron interface for this demo
    }

    void Update()
    {
        if (PI.Reload)
        {
            if (Task.Length != PCT.Input.Length)                        // if inputs of task != perceptron's enters lenth
            {
                if (PCT.B)
                    Task = new int[PCT.Input.Length - 1];
                else
                    Task = new int[PCT.Input.Length];
            }

            TasksAndAnswers();          // create a sample of tasks and answers
            PBPI.Task = LearnTask;       // input of tasks sample in the learning interface
            PBPI.Answer = LearnAnswer;   // input of answers sample in the learning interface
        }

        if (!PBPI.Learn)
            EnterTask();
    }

    void TasksAndAnswers()   // create a sample of tasks and answers
    {
        int t = 0;
        int a = 0;
        LearnTask = new float[(int)Mathf.Pow(2, Task.Length)][];    // create an array of the number of tasks
        LearnAnswer = new float[LearnTask.Length][];                // create an array of the number of answers (answers = tasks)
        while (a < LearnTask.Length)
        {
            if (a == 0)
            {
                t = 0;
                LearnTask[a] = new float[Task.Length];
                while (t < Task.Length)
                {
                    LearnTask[a][t] = 0;
                    t++;
                }
            }
            else
            {
                LearnTask[a] = Formulas.FromArray(LearnTask[a - 1]);
                LearnTask[a][0]++;
                t = 1;
                while (t < Task.Length)
                {
                    if (LearnTask[a][t - 1] == 2)
                    {
                        LearnTask[a][t - 1] = 0;
                        LearnTask[a][t]++;
                    }
                    t++;
                }
            }

            LearnAnswer[a] = new float[1];
            t = 0;
            while (t < Task.Length)
            {
                LearnTask[a][t] = LearnTask[a][t] % 2;
                LearnAnswer[a][0] += LearnTask[a][t];
                t++;
            }
            LearnAnswer[a][0] = LearnAnswer[a][0] % 2;
            a++;
        }
    }

    void EnterTask()    // input a task by buttons on GUI (for human player)
    {
        int i = 0;
        Answer = 0;
        while (i < Task.Length)
        {
            Task[i] = Task[i] % 2;
            Answer += Task[i];

            PCT.Input[i] = Task[i];
            i++;
        }
        Answer = Answer % 2;
        PCT.Solution();
    }

    void OnGUI()
    {
        Task = TaskLineInt(1, 1, "Enters", Task);                               // input a task by buttons on GUI (for human player)
        InterfaceGUI.Info(1, 3, "Answer", Answer);                              // answer to a task
        InterfaceGUI.Info(1, 4, "ANN answer", Mathf.Round(PCT.Output[0]));      // perceptron's answer to a task (rounded)
    }

    int[] TaskLineInt(int Column, int Line, string Name, int[] Task)
    {
        GUI.Box(new Rect(10 * Column + (Column - 1) * 180, 10 + (Line - 1) * 30, 180, 60), "");

        GUI.Label(new Rect(10 * Column + (Column - 1) * 180, 10 + (Line - 1) * 30, 180, 22), Name + " :");
        int i = 0;
        while (i < Task.Length)
        {
            if (GUI.Button(new Rect(10 * Column + (Column - 1) * 180 + i * 30, 32 + (Line - 1) * 30, 30, 30), Task[i].ToString()))
            {
                Task[i]++;
                if (Task[i] > 1)
                    Task[i] = 0;
            }
            i++;
        }
        return Task;
    }
}
