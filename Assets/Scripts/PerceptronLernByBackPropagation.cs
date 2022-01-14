using System.Collections;
using UnityEngine;

/// <summary>
/// Perceptron learning by back propagation method.
/// Non-MonoBehaviour script.
/// </summary>
public class PerceptronLernByBackPropagation
{
    /// <summary>
    /// The number of training steps per frame in the game mode.
    /// </summary>
    public int LearningSpeed = 1;

    /// <summary>
    /// Count of training steps.
    /// </summary>
    public int LearnIteration = 0;

    /// <summary>
    /// The strength of changing weights at training. Affects the speed and quality of learning.
    /// </summary>
    public float LearningRate = 0.5F;

    /// <summary>
    /// Indicates what should be the maximum difference between the correct answer and the response of the ANN.
    /// </summary>
    public float DesiredMaxError = 0.3F;

    /// <summary>
    /// The maximum difference between the correct answer and the response of the ANN.
    /// </summary>
    public float MaxError = 0;

    /// <summary>
    /// If the difference between the correct answer and the response of the ANN is less than DesiredMaxError, then the perceptron is counted as trained.
    /// </summary>
    public bool Learned = false;

    /// <summary>
    /// If "true", then samples of tasks and answers will be mixed during the training.
    /// </summary>
    public bool ShuffleSamples = true;

    /// <summary>
    /// If "true", then back propagation work only with output what have error more then DesiredMaxError.
    /// </summary>
    public bool WithError = false;

    /// <summary>
    /// The training of the perceptron by the back propagation method
    /// with the content of a certain number of tasks with answers.
    /// Can use LearningSpeed, DesiredMaxError and ShuffleSamples.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    /// <param name="Task">Array of tasks.</param>
    /// <param name="Answer">Array of answers.</param>
    public void Learn(Perceptron PCT, float[][] Task, float[][] Answer)
    {
        int s = 0;
        while (s < LearningSpeed)
        {
            if (ShuffleSamples)
                ShufflingSamples(Task, Answer);

            Learned = false;
            MaxError = 0;
            int a = 0;
            while (a < Answer.Length)
            {
                Formulas.FromArray(Task[a], PCT.Input);
                PCT.Solution();
                int e = 0;
                while (e < PCT.Output.Length)
                {
                    MaxError = Mathf.Max(MaxError, Mathf.Abs(Answer[a][e] - PCT.Output[e]));
                    e++;
                }
                a++;
            }

            if (MaxError <= DesiredMaxError)
                Learned = true;

            if (!Learned)
            {
                a = 0;
                while (a < Answer.Length)
                {
                    Learn(PCT, Task[a], Answer[a]);
                    a++;
                }
            }
            else
                s = LearningSpeed;
            s++;
        }
    }

    /// <summary>
    /// The training of the perceptron by the back propagation method
    /// with the contents of one task with the answer.
    /// Can not use LearningSpeed, DesiredMaxError and ShuffleSamples.
    /// It is recommended to use DesiredMaxError = 0.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    /// <param name="Task">An array of one task for each input neuron of the perceptron.</param>
    /// <param name="Answer">An array of one answer for each source neuron of the perceptron.</param>
    public void Learn(Perceptron PCT, float[] Task, float[] Answer)
    {
        Formulas.FromArray(Task, PCT.Input);
        Learn(PCT, Answer);
    }

    /// <summary>
    /// The training of the perceptron by the back propagation method
    /// with the contents of one answer. Use if the job is directly entered into the input layer of the perceptron.
    /// Can not use LearningSpeed, DesiredMaxError and ShuffleSamples.
    /// It is recommended to use DesiredMaxError = 0.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    /// <param name="Answer">An array of one answer for each source neuron of the perceptron.</param>
    public void Learn(Perceptron PCT, float[] Answer)
    {
        PCT.Solution();
        int l = 0;
        int j;
        float[][][] NeuronError = new float[PCT.NeuronWeight.Length][][];
        while (l < PCT.NeuronWeight.Length)
        {
            NeuronError[l] = new float[PCT.NeuronWeight[l].Length][];
            j = 0;
            while (j < PCT.NeuronWeight[l].Length)
            {
                NeuronError[l][j] = new float[PCT.NeuronWeight[l][j].Length];
                j++;
            }
            l++;
        }
        // calculation of the output layer error
        l = PCT.Neuron.Length - 1;
        //j = 0;

        bool WithErrorHave = false;
        int k = 0;
        while (l > 0)
        {
            j = 0;
            while (j < PCT.Neuron[l].Length)
            {
                k = 0;
                while (k < PCT.Neuron[l - 1].Length)
                {
                    if (l == PCT.Neuron.Length - 1)
                    {
                        if (WithError)
                        {
                            if (Mathf.Abs(Answer[j] - PCT.Neuron[l][j]) > DesiredMaxError)
                            {
                                //TODO PUT the description in formulas
                                NeuronError[l - 1][j][k] += (Answer[j] - PCT.Neuron[l][j]) * DX(PCT.Neuron[l][j], PCT.AFS, PCT.AFWM);
                                WithErrorHave = true;
                            }
                            else
                                NeuronError[l - 1][j][k] = 0;
                        }
                        else
                        {
                            NeuronError[l - 1][j][k] += (Answer[j] - PCT.Neuron[l][j]) * DX(PCT.Neuron[l][j], PCT.AFS, PCT.AFWM);
                            WithErrorHave = true;
                        }
                    }
                    else
                    {
                        int jj = 0;
                        while (jj < PCT.NeuronWeight[l].Length)
                        {
                            if (PCT.B)
                            {
                                if (j != PCT.Neuron[l].Length - 1)
                                    NeuronError[l - 1][j][k] += PCT.NeuronWeight[l][jj][j] * NeuronError[l][jj][j];
                            }
                            else
                                NeuronError[l - 1][j][k] += PCT.NeuronWeight[l][jj][j] * NeuronError[l][jj][j];
                            jj++;
                        }
                        if (PCT.B)
                        {
                            if (j != PCT.Neuron[l].Length - 1)
                                NeuronError[l - 1][j][k] *= DX(PCT.Neuron[l][j], PCT.AFS, PCT.AFWM);
                        }
                        else
                            NeuronError[l - 1][j][k] *= DX(PCT.Neuron[l][j], PCT.AFS, PCT.AFWM);
                    }
                    k++;
                }
                j++;
            }
            l--;
        }
        // correction of weights
        if (WithErrorHave)
        {
            l = 0;
            while (l < NeuronError.Length)
            {
                j = 0;
                while (j < NeuronError[l].Length)
                {
                    k = 0;
                    while (k < NeuronError[l][j].Length)
                    {
                        if (PCT.B && k == NeuronError[l][j].Length - 1)
                            PCT.NeuronWeight[l][j][k] += NeuronError[l][j][k] * LearningRate;
                        else
                            PCT.NeuronWeight[l][j][k] += PCT.Neuron[l][k] * NeuronError[l][j][k] * LearningRate;
                        NeuronError[l][j][k] = 0;
                        k++;
                    }
                    j++;
                }
                l++;
            }
        }

        if (WithError)
        {
            if (WithErrorHave)
                LearnIteration++;
        }
        else
            LearnIteration++;
    }

    /// <summary>
    /// Shuffling samples of tasks and answers.
    /// </summary>
    /// <param name="Task">Array of tasks.</param>
    /// <param name="Answer">Array of answers.</param>
    private void ShufflingSamples(float[][] Task, float[][] Answer)
    {
        ArrayList TempLearnTasks = new ArrayList();
        ArrayList TempLearnAnswers = new ArrayList();
        int t = 0;
        while (t < Task.Length)
        {
            TempLearnTasks.Add(Formulas.FromArray(Task[t]));
            TempLearnAnswers.Add(Formulas.FromArray(Answer[t]));
            t++;
        }
        t = 0;
        while (TempLearnTasks.Count != 0)
        {
            int ac = Random.Range(0, TempLearnTasks.Count);
            Task[t] = (float[])TempLearnTasks[ac];
            Answer[t] = (float[])TempLearnAnswers[ac];
            t++;
            TempLearnTasks.RemoveAt(ac);
            TempLearnAnswers.RemoveAt(ac);
        }
    }

    /// <summary>
    /// Modification of the weights of the perceptron before training.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    /// <param name="MSW">If "true", then weights are modified by a special algorithm. If "false", then all weights  are generated randomly in the range from -0.5 to 0.5.</param>
    public void ModificateStartWeights(Perceptron PCT, bool MSW)
    {
        int l = 0;
        int k = 0;
        int j = 0;
        while (l < PCT.NeuronWeight.Length)
        {
            k = 0;
            float b = 0;
            if (MSW)
            {
                if (PCT.B)
                    b = 0.7F * Mathf.Pow(PCT.NeuronWeight[l].Length, 1F / (PCT.NeuronWeight[l][k].Length - 1));
                else
                    b = 0.7F * Mathf.Pow(PCT.NeuronWeight[l].Length, 1F / PCT.NeuronWeight[l][k].Length);
            }
            while (k < PCT.NeuronWeight[l].Length)
            {
                if (MSW && l < PCT.NeuronWeight.Length-1)
                {
                    j = 0;
                    float v = 0;
                    while (j < PCT.NeuronWeight[l][k].Length)
                    {
                        PCT.NeuronWeight[l][k][j] = Formulas.Randomizer(0.5F);
                        v += Mathf.Pow(PCT.NeuronWeight[l][k][j], 2);
                        j++;
                    }
                    v = Mathf.Pow(v, 0.5F);
                    j = 0;
                    while (j < PCT.NeuronWeight[l][k].Length)
                    {
                        if (PCT.B && j == PCT.NeuronWeight[l][k].Length - 1)
                            PCT.NeuronWeight[l][k][j] = Formulas.Randomizer(b);
                        else
                            PCT.NeuronWeight[l][k][j] = (b * PCT.NeuronWeight[l][k][j]) / v;
                        j++;
                    }
                }
                else
                {
                    j = 0;
                    while (j < PCT.NeuronWeight[l][k].Length)
                    {
                        PCT.NeuronWeight[l][k][j] = Formulas.Randomizer(0.5F);
                        j++;
                    }
                }
                k++;
            }
            l++;
        }
    }

    /// <summary>
    /// Search for a derivative of the value of a particular neuron.
    /// </summary>
    /// <param name="X">The value of a neuron.</param>
    /// <param name="ActivationFunctionScale">Activation function scale.</param>
    /// <param name="ActivationFunctionWithMinus">Activation function with minus.</param>
    /// <returns></returns>
    private float DX(float X, float ActivationFunctionScale, bool ActivationFunctionWithMinus)
    {
        //ActivationFunctionScale = 1;
        int Minus = 0;
        if (ActivationFunctionWithMinus)
            Minus = 1;
        X = ((Minus + ActivationFunctionScale * X) * (1F - ActivationFunctionScale * X)) / (1F + Minus);
        return X;
    }
}
