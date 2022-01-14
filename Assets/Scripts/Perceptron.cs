using UnityEngine;
using System.IO;

/// <summary>
/// Perceptron - artificial neural network.
/// Non-MonoBehaviour script.
/// </summary>
public class Perceptron
{
    /// <summary>
    /// Activation function scale. 1 - default scale.
    /// </summary>
    public float AFS = 1F;

    /// <summary>
    /// Bias. If "true", then bias neuron is created in each layer, except the output layer. This neuron is always equal to one. His weight bonds affect to all the neurons of the next layer, except bias neuron.
    /// </summary>
    public bool B = false;

    /// <summary>
    /// Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to input and output neurons. If "false" - from 0 to 1.
    /// </summary>            
    public bool AFWM = false;

    /// <summary>
    /// Input layer of perceptron. The size of the array indicates the number of input values, with the bias neuron (if B == true, the last neuron is always = 1).
    /// </summary>
    [SerializeField]
    public float[] Input;

    /// <summary>
    /// The number of neurons in the hiden layers, with the bias neuron. The size of the array indicates the number of hidden layers, with the bias neuron (if B == true, the last neuron of each layer is always = 1).
    /// </summary>
    [SerializeField]
    public int[] NIHL;

    /// <summary>
    /// Output layer of perceptron.
    /// </summary>
    [SerializeField]
    public float[] Output;

    /// <summary>
    /// The value of neurons. The first part of the array corresponds to the layer of the perceptron. The second part of the array is the number of the neuron in layer.
    /// </summary>
    [SerializeField]
    public float[][] Neuron;            //layer , neuron

    /// <summary>
    /// The value of the weights between neurons. The first part of the array corresponds to the layer of the ANN. The second part of the array is the number of the next layer's neuron. The third part of the array is the number of the current layer's neuron.
    /// </summary>
    [SerializeField]
    public float[][][] NeuronWeight;    //layer , neuronTo, neuronFrom

    /// <summary>
    /// Create perceptron using parameters.
    /// </summary>
    /// <param name="AFS">Activation function scale. 1 - default scale.</param>
    /// <param name="B">If "true", then bias neuron is created in each layer, except the output layer. This neuron is always equal to one. His weight bonds affect to all the neurons of the next layer, except  bias neuron.</param>
    /// <param name="AFWM">Activation function with minus. If "true", then all neurons perceive values from -1 to 1. This also applies to input and output neurons. If "wrong" - from 0 to 1.</param>
    /// <param name="NumberOfInputs">The number of input neurons, without the bias neuron.</param>
    /// <param name="NIHL">An array indicating the number of neurons (without a bias neuron) in each hidden layer of the ANN. The size of the array indicates the number of hidden layers.</param>
    /// <param name="NumbersOfOutputs">Number of output neurons.</param>
    public void CreatePerceptron(float AFS, bool B, bool AFWM, int NumberOfInputs, int[] NIHL, int NumbersOfOutputs)
    {
        this.AFS = AFS;
        this.B = B;
        this.AFWM = AFWM;
        if (B)
            Input = new float[NumberOfInputs + 1];
        else
            Input = new float[NumberOfInputs];
        if (NIHL == null)
            NIHL = new int[0];
        if (B)
        {
            int i = 0;
            this.NIHL = new int[NIHL.Length];
            while (i < NIHL.Length)
            {
                this.NIHL[i] = NIHL[i] + 1;
                i++;
            }
        }
        else
            this.NIHL = NIHL;
        Output = new float[NumbersOfOutputs];
        CreatingNeurons(null);
    }

    /// <summary>
    /// Create perceptron using default parameters. (Activation function scale = 1, Bias is false, Activation function with minus is false.)
    /// </summary>
    /// <param name="NumberOfInputs">The number of input neurons, without the bias neuron.</param>
    /// <param name="NIHL">An array indicating the number of neurons (without a bias neuron) in each hidden layer of the ANN. The size of the array indicates the number of hidden layers.</param>
    /// <param name="NumbersOfOutputs">Number of output neurons.</param>
    public void CreatePerceptron(int NumberOfInputs, int[] NIHL, int NumbersOfOutputs)
    {
        CreatePerceptron(1, false, false, NumberOfInputs, NIHL, NumbersOfOutputs);
    }

    /// <summary>
    /// Load perceptron from file.
    /// </summary>
    /// <param name="PerceptronFile">Name of file</param>
    public void Load(string PerceptronFile)
    {
        if (PerceptronFile != "")
        {
            if (Directory.Exists(Application.dataPath + "/Statics"))
            {
                if (File.Exists(Application.dataPath + "/Statics/" + PerceptronFile + ".ann"))
                {
                    StreamReader SR = File.OpenText(Application.dataPath + "/Statics/" + PerceptronFile + ".ann");
                    string Version = SR.ReadLine();
                    if (Version == "PerceptronStatic V1.0")
                    {
                        AFS = Formulas.StringToInt(SR.ReadLine());
                        B = Formulas.StringToBool(SR.ReadLine());
                        AFWM = Formulas.StringToBool(SR.ReadLine());
                        if (B)
                            Input = new float[Formulas.StringToInt(SR.ReadLine()) + 1];
                        else
                            Input = new float[Formulas.StringToInt(SR.ReadLine())];
                        NIHL = new int[Formulas.StringToInt(SR.ReadLine())];
                        int i = 0;
                        while (i < NIHL.Length)
                        {
                            if (B)
                                NIHL[i] = Formulas.StringToInt(SR.ReadLine()) + 1;
                            else
                                NIHL[i] = Formulas.StringToInt(SR.ReadLine());
                            i++;
                        }
                        Output = new float[Formulas.StringToInt(SR.ReadLine())];
                        CreatingNeurons(SR);

                        SR.Close();
                        Debug.Log("Perceptron loaded.");
                    }
                    else
                    {
                        SR.Close();
                        Debug.LogWarning("Perceptron not loaded. Unsuitable version of perceptron. Version is " + Version);
                    }
                }
                else
                    Debug.LogWarning("Perceptron not loaded. There is no such file name.");
            }
            else
                Debug.LogWarning("Perceptron not loaded. Folder for the perceptron does not exist.");
        }
        else
            Debug.LogWarning("Perceptron not loaded. File name not entered.");
    }

    /// <summary>
    /// Perceptron solution. :)
    /// </summary>
    public void Solution()
    {
        if (B)
            Input[Input.Length - 1] = 1;
        int l = 1;
        int k = 0;
        while (l < Neuron.Length)
        {
            k = 0;
            while (k < Neuron[l].Length)
            {
                if (B && l != Neuron.Length - 1)
                {
                    if (k == Neuron[l].Length - 1)
                        Neuron[l][k] = 1;
                    else
                        Neuron[l][k] = Formulas.ActivationFunction(Sumator(Neuron[l - 1], NeuronWeight[l - 1][k]), AFS, AFWM);
                }
                else
                    Neuron[l][k] = Formulas.ActivationFunction(Sumator(Neuron[l - 1], NeuronWeight[l - 1][k]), AFS, AFWM);
                k++;
            }
            l++;
        }
    }

    /// <summary>
    /// The sum of all values of neurons multiplied with weight.
    /// </summary>
    /// <param name="Neuron">Neurons of a certain layer.</param>
    /// <param name="NeuronWeight">The weights of the same layer as the neurons.</param>
    /// <returns></returns>
    private float Sumator(float[] Neuron, float[] NeuronWeight)
    {
        int k = 0;
        float Sum = 0;
        while (k < Neuron.Length)
        {
            if (B && k == Neuron.Length - 1)
                Sum += NeuronWeight[k];
            else
                Sum += Neuron[k] * NeuronWeight[k];
            k++;
        }
        return Sum;
    }

    /// <summary>
    /// The creation of neurons and their relationships.
    /// </summary>
    /// <param name="SR">Specified stream from downloaded file. Use "null" if the file is not used.</param>
    public void CreatingNeurons(StreamReader SR)
    {
        Neuron = new float[1 + NIHL.Length + 1][];
        NeuronWeight = new float[1 + NIHL.Length][][];
        Neuron[0] = new float[Input.Length];
        Input = Neuron[0];

        int l = 1;
        int j = 0;
        int k = 0;
        if (NIHL.Length != 0)
        {
            while (l < NIHL.Length + 1)
            {
                Neuron[l] = new float[NIHL[l - 1]];
                if (B)
                {
                    NeuronWeight[l - 1] = new float[Neuron[l].Length - 1][];
                }
                else
                {
                    NeuronWeight[l - 1] = new float[Neuron[l].Length][];
                }

                j = 0;
                while (j < Neuron[l].Length)
                {
                    if (B)
                    {
                        if (j != Neuron[l].Length - 1)
                            NeuronWeight[l - 1][j] = new float[Neuron[l - 1].Length];
                    }
                    else
                        NeuronWeight[l - 1][j] = new float[Neuron[l - 1].Length];
                    k = 0;
                    if (B)
                    {
                        if (j != Neuron[l].Length - 1)
                            while (k < NeuronWeight[l - 1][j].Length)
                            {
                                float r = 0;
                                if (SR != null)
                                    r = Formulas.StringToFloat(SR.ReadLine());
                                else
                                    r = Formulas.Randomizer(0.5F);
                                NeuronWeight[l - 1][j][k] = r;
                                k++;
                            }
                    }
                    else
                    {
                        while (k < NeuronWeight[l - 1][j].Length)
                        {
                            float r = 0;
                            if (SR != null)
                                r = Formulas.StringToFloat(SR.ReadLine());
                            else
                                r = Formulas.Randomizer(0.5F);
                            NeuronWeight[l - 1][j][k] = r;
                            k++;
                        }
                    }
                    j++;
                }
                l++;
            }
        }
        Neuron[l] = new float[Output.Length];
        Output = Neuron[l];
        NeuronWeight[l - 1] = new float[Neuron[l].Length][];
        j = 0;
        while (j < Neuron[l].Length)
        {
            NeuronWeight[l - 1][j] = new float[Neuron[l - 1].Length];
            k = 0;
            while (k < NeuronWeight[l - 1][j].Length)
            {
                float r = 0;
                if (SR != null)
                    r = Formulas.StringToFloat(SR.ReadLine());
                else
                    r = Formulas.Randomizer(0.5F);
                NeuronWeight[l - 1][j][k] = r;
                k++;
            }
            j++;
        }
    }

    /// <summary>
    /// Saving perceptron parameters to a file.
    /// </summary>
    /// <param name="PerceptronFile">Name of file.</param>
    public void Save(string PerceptronFile)
    {
        if (PerceptronFile != "")
        {
            if (!Directory.Exists(Application.dataPath + "/Statics"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Statics");
            }
            if (File.Exists(Application.dataPath + "/Statics/" + PerceptronFile + ".ann"))
                File.Delete(Application.dataPath + "/Statics/" + PerceptronFile + ".ann");

            if (!File.Exists(Application.dataPath + "/Statics/" + PerceptronFile + ".ann"))
            {
                StreamWriter SC = File.CreateText(Application.dataPath + "/Statics/" + PerceptronFile + ".ann");
                SC.WriteLine("PerceptronStatic V1.0");
                SC.WriteLine(AFS);
                SC.WriteLine(B);
                SC.WriteLine(AFWM);
                if (B)
                    SC.WriteLine(Input.Length - 1);
                else
                    SC.WriteLine(Input.Length);
                SC.WriteLine(NIHL.Length);
                int i = 0;
                while (i < NIHL.Length)
                {
                    if (B)
                        SC.WriteLine(NIHL[i] - 1);
                    else
                        SC.WriteLine(NIHL[i]);
                    i++;
                }
                SC.WriteLine(Output.Length);
                int l = 0;
                int j;
                int k;
                while (l < NeuronWeight.Length)
                {
                    j = 0;
                    while (j < NeuronWeight[l].Length)
                    {
                        k = 0;
                        while (k < NeuronWeight[l][j].Length)
                        {
                            SC.WriteLine(NeuronWeight[l][j][k]);
                            k++;
                        }
                        j++;
                    }
                    l++;
                }
                SC.Close();
            }
            Debug.Log("Perceptron saved.");
        }
        else
            Debug.LogWarning("Perceptron not saved. File name not entered.");
    }

    public Perceptron ShallowCopy()
    {
        return (Perceptron)this.MemberwiseClone();
    }

    public Perceptron DeepCopy()
    {
        Perceptron clone = ShallowCopy();
        clone.Input = Formulas.DeepClone(Input);
        clone.Output = Formulas.DeepClone(Output);
        clone.NIHL = Formulas.DeepClone(NIHL);
        clone.NeuronWeight = Formulas.DeepClone(NeuronWeight);
        clone.Neuron = Formulas.DeepClone(Neuron);
        return clone;
    }
}

