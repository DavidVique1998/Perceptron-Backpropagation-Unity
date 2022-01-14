using UnityEngine;
public class LessonsTutorialCowPerceptron : MonoBehaviour
{
    private TutorialCowControl THC;             //Cow control
    public Perceptron PCT = new Perceptron();   //Perceptron
    private PerceptronInterface PI;             //Perceptron interface

    void Start()
    {
        //Find cow control
        THC = gameObject.GetComponent<TutorialCowControl>();
        //Hiden layers and neurons
        int[] Layers = new int[2];
        Layers[0] = 9;
        Layers[1] = 9;
        //Create perceptron
        PCT.CreatePerceptron(1, false, true, 3, Layers, 2);
        //Add perceptron interface to game object & add perceptron to interface
        PI = gameObject.AddComponent<PerceptronInterface>();
        PI.PCT = PCT;
    }

    void Update()
    {
        //Convert vaule
        PCT.Input[0] = THC.AngleToFood / 180F;      //Work with angles. Min vaule = -180, max vaule = 180
        PCT.Input[1] = THC.DistanceToFood / 41F;    //Work with distance. Max vaule = 41
        PCT.Input[2] = THC.Satiety / 50F;           //Work with satiety. Min vaule = 0, max vaule = 50
        PCT.Solution();                             //Perceptron solution
                        //For this tutorial not need to convert vaule
        THC.Turn = PCT.Output[0];
        THC.Move = PCT.Output[1];
    }
}