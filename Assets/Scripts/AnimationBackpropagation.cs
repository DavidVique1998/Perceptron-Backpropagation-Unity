using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class AnimationBackpropagation : MonoBehaviour
{
    /// <summary>
    /// Perceptron to control in learn.
    /// </summary>
    private Perceptron PCT = new Perceptron();
    /// <summary>
    /// Association type of learn with perceptron
    /// </summary>
    private PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();

    /// <summary>
    /// Desiere Response
    /// </summary>

    private PercepInterfaz PI;

    private PBInterface PBI;

    private Originator originator;

    private Caretaker caretaker;


    // Start is called before the first frame update
    void Start()
    {
        PCT.CreatePerceptron(1, false, true, 2, null, 2);

        PLBBP.LearningRate = 0.5F;
        PLBBP.DesiredMaxError = 0.5F;
        PLBBP.WithError = true;

        PI = gameObject.AddComponent<PercepInterfaz>();
        PI.PCT = PCT;
        PI.Answer = new float[2];

        PBI = gameObject.AddComponent<PBInterface>();
        PBI.PCT = PCT;
        PBI.PLBBP = PLBBP;

        AddData();
    }

    // Update is called once per frame
    void Update()
    {
        if (PBI.Learn)
        {
            StartCoroutine(OnStartLearn());

        }
        else {
            CompareMemento();
        }
    }


    void AddData()
    {
        PCT.Input[0] = 0.5F;
        PCT.Input[1] = 0.8F;
        PCT.Output[1] = 1;
        PCT.Output[0] = 0;

        for (int i = 0; i < PCT.Output.Length; i++)
        {
            PI.Answer[i] = PCT.Output[i];
        }

        originator = new Originator(PCT);
        caretaker = new Caretaker(originator);
        PI.caretaker = caretaker;
    }

     IEnumerator OnStartLearn() {
        yield return new WaitForSecondsRealtime(1);
        PLBBP.Learn(PCT, PI.Answer);
        originator.DoSomething(PCT);
        caretaker.Backup();
    }


    private void CompareMemento()
    {
        if (!originator._state.Equals(PCT))
        {
            PCT.NIHL= Formulas.DeepClone(originator._state.NIHL);
            PCT.Input = Formulas.DeepClone(originator._state.Input);
            PCT.NeuronWeight = Formulas.DeepClone(originator._state.NeuronWeight);
            PCT.Neuron = Formulas.DeepClone(originator._state.Neuron);
            PCT.Output = Formulas.DeepClone(originator._state.Output);
            Array.Copy(PCT.Input, 0, PI.Inps, 0, PCT.Input.Length);
            Array.Copy(PCT.Output, 0, PI.Output, 0, PCT.Output.Length);
            originator.DoSomething(PCT);
        }
    }
}
