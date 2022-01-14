using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Car;

public class PBCar : MonoBehaviour
{
    /// <summary>
    /// Object to control and learn.
    /// </summary>
    private CarController CC;
    /// <summary>
    /// Perceptron to control in learn.
    /// </summary>
    private Perceptron PCT = new Perceptron();
    /// <summary>
    /// Association type of learn with perceptron
    /// </summary>
    private PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();

    private Originator originator;

    private Caretaker caretaker;

    PercepInterfaz PI;
    PBInterface PBI;
    // Start is called before the first frame update
    void Start()
    {
        CC = gameObject.GetComponent<CarController>();
        PCT.CreatePerceptron(1, false, true, 6, null, 2);

        PLBBP.LearningRate = 0.5F;
        PLBBP.DesiredMaxError = 0.5F;
        PLBBP.WithError = true;

        PI = gameObject.AddComponent<PercepInterfaz>();
        PI.PCT = PCT;
        PI.Answer = new float[2];

        originator = new Originator(PCT, PI.Answer);
        caretaker = new Caretaker(originator);
        PI.caretaker = caretaker;

        PBI = gameObject.AddComponent<PBInterface>();
        PBI.PCT = PCT;
        PBI.PLBBP = PLBBP;
    }

    // Update is called once per frame
    void Update()
    {
        if (PBI.Learn)
        {
            RaycastHit Hit;
            int i = 0;
            while (i < 5)
            {
                Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 1, 0.5F)), transform.TransformDirection(Quaternion.AngleAxis(i * 40 - 80, Vector3.up) * Vector3.forward) * 150, Color.black);
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 1, 0.5F)), transform.TransformDirection(Quaternion.AngleAxis(i * 40 - 80, Vector3.up) * Vector3.forward), out Hit, 150))
                {
                    PCT.Input[i] = Hit.distance / 150F;

                }
                else
                {
                    PCT.Input[i] = 1;
                }
                i++;
            }

            //Debug.Log("Tamaño de la entrada: "+ PCT.Input.Length.ToString());

            PCT.Input[i] = (CC.CurrentSpeed * Mathf.Sign(Vector3.Cross(-transform.right, gameObject.GetComponent<Rigidbody>().velocity).y)) / 150F;
            Array.Copy(PCT.Input, 0, PI.Inps, 0, PCT.Input.Length);
            PCT.Solution();
            if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0)
            {
                if (CrossPlatformInputManager.GetAxis("Horizontal") != 0)
                {
                    PI.Answer[0] = CrossPlatformInputManager.GetAxis("Horizontal");
                }
                else
                {
                    PI.Answer[0] = PCT.Output[0];
                }
                if (CrossPlatformInputManager.GetAxis("Vertical") != 0)
                {
                    PI.Answer[1] = CrossPlatformInputManager.GetAxis("Vertical");
                }
                else
                {
                    PI.Answer[1] = PCT.Output[1];
                }
                PLBBP.Learn(PCT, PI.Answer);
                originator.DoSomething(PCT, PI.Answer);
                caretaker.BackupAnswer();

            }
            Array.Copy(PCT.Output, 0, PI.Output, 0, PCT.Output.Length);
            CC.Move(PCT.Output[0], PCT.Output[1], PCT.Output[1], -1);
            //Debug.Log("\t\tIn[0]: " + Math.Round(PCT.Input[0] * 100, 2) + "\t\tIn[1]: " + Math.Round(PCT.Input[1] * 100, 2) + "\t\tIn[2]: " + Math.Round(PCT.Input[2] * 100, 2) + "\t\tIn[3]: " + Math.Round(PCT.Input[3] * 100, 2) + "\t\tIn[4]: " + Math.Round(PCT.Input[4] * 100, 2) + "\t\tIn[5]: " + Math.Round(PCT.Input[5] * 100, 2) + "\t\tDirección: " + PCT.Output[0] + "\t\tAceleración: " + PCT.Output[1] + "\n");

        }
        else {
            if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0)
            {
                float[] Answer = new float[PCT.Output.Length];
                if (CrossPlatformInputManager.GetAxis("Horizontal") != 0)
                {
                    Answer[0] = CrossPlatformInputManager.GetAxis("Horizontal");
                }
                else
                {
                    Answer[0] = PCT.Output[0];
                }
                if (CrossPlatformInputManager.GetAxis("Vertical") != 0)
                {
                    Answer[1] = CrossPlatformInputManager.GetAxis("Vertical");
                }
                else
                {
                    Answer[1] = PCT.Output[1];
                }
                CC.Move(Answer[0], Answer[1], Answer[1], -1);
            }
            CompareMemento();
        }
    }

    private void CompareMemento()
    {
        if (!originator._state.Equals(PCT))
        {
            PCT.NIHL = Formulas.DeepClone(originator._state.NIHL);
            PCT.Input = Formulas.DeepClone(originator._state.Input);
            PCT.NeuronWeight = Formulas.DeepClone(originator._state.NeuronWeight);
            PCT.Neuron = Formulas.DeepClone(originator._state.Neuron);
            PCT.Output = Formulas.DeepClone(originator._state.Output);
            Array.Copy(PCT.Input, 0, PI.Inps, 0, PCT.Input.Length);
            Array.Copy(PCT.Output, 0, PI.Output, 0, PCT.Output.Length);
            Array.Copy(originator._answer, 0, PI.Answer, 0, originator._answer.Length);
            originator.DoSomething(PCT, PI.Answer);
        }
    }



    private void OnGUI() {
        if (CrossPlatformInputManager.GetAxis("Horizontal") < 0)
        {
            GUI.Box(new Rect(Screen.width / 2F - 75, Screen.height - 50, 50, 50), "<");
        }
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0)
        {
            GUI.Box(new Rect(Screen.width / 2F + 25, Screen.height - 50, 50, 50), ">");
        }
        if (CrossPlatformInputManager.GetAxis("Vertical") > 0)
        {
            GUI.Box(new Rect(Screen.width / 2F - 25, Screen.height - 100, 50, 50), "^");
        }
        if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
        {
            GUI.Box(new Rect(Screen.width / 2F - 25, Screen.height - 50, 50, 50), "v");
        }
    }
}
