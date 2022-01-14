using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Vehicles.Aeroplane;

[RequireComponent(typeof(AeroplaneController))]
public class PBAirplane : MonoBehaviour
{

    // these max angles are only used on mobile, due to the way pitch and roll input are handled
    public float maxRollAngle = 80;
    public float maxPitchAngle = 80;
    /// <summary>
    /// Object to control and learn.
    /// </summary>
    private AeroplaneController AC;

    /// <summary>
    /// Perceptron to control in learn.
    /// </summary>
    private Perceptron PCT = new Perceptron();
    /// <summary>
    /// Association type of learn with perceptron
    /// </summary>
    private PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();
    // Start is called before the first frame update

    private int maxView = 600;
    void Start()
    {

        PCT.CreatePerceptron(1, false, true, 14, null, 4);

        PLBBP.LearningRate = 0.5F;
        PLBBP.DesiredMaxError = 0.5F;
        PLBBP.WithError = true;

        PInterface PI = gameObject.AddComponent<PInterface>();
        PI.PCT = PCT;

        PBInterface PBI = gameObject.AddComponent<PBInterface>();
        PBI.PCT = PCT;
        PBI.PLBBP = PLBBP;
    }

    private void Awake()
    {
        // Set up the reference to the aeroplane controller.
        AC = gameObject.GetComponent<AeroplaneController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //<-------------------------- Training perceptron by Raycast of colliders  --------------------------->
        RaycastHit Hit;
        int i = 0;
        while (i < PCT.Input.Length - 1)
        {
            if (i < PCT.Input.Length - 9) //-1
            {
                Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis(i * 30 - 180, Vector3.up) * Vector3.forward) * 500, Color.black);
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis(i * 30 - 180, Vector3.up) * Vector3.forward), out Hit, maxView))
                {
                    PCT.Input[i] = Hit.distance / maxView;
                }
                else
                {
                    PCT.Input[i] = 1;
                }
            }
            else
            {
                Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis((i - 6) * 30 - 180 + 15, Vector3.right) * Vector3.forward) * 500, Color.red);
                if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis((i - 6) * 30 - 180 + 15, Vector3.right) * Vector3.forward), out Hit, maxView))
                {
                    PCT.Input[i] = Hit.distance / maxView;
                }
                else
                {
                    PCT.Input[i] = 1;
                }

            }

            i++;
        }
        PCT.Input[i] = (AC.ForwardSpeed * Mathf.Sign(Vector3.Cross(-transform.right, gameObject.GetComponent<Rigidbody>().velocity).y)) / (maxView);
        PCT.Solution();

        //if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0 || CrossPlatformInputManager.GetAxis("Mouse X") != 0 || CrossPlatformInputManager.GetAxis("Mouse Y") != 0)
        if (CrossPlatformInputManager.GetAxis("Horizontal") != 0 || CrossPlatformInputManager.GetAxis("Vertical") != 0 )
            {
            float[] Answer = new float[4];
            if (CrossPlatformInputManager.GetAxis("Mouse X") != 0)
            {
                Answer[0] = CrossPlatformInputManager.GetAxis("Mouse X");
            }
            else
            {
                Answer[0] = PCT.Output[0];
            }
            if (CrossPlatformInputManager.GetAxis("Mouse Y") != 0)
            {
                Answer[1] = CrossPlatformInputManager.GetAxis("Mouse Y");
            }
            else
            {
                Answer[1] = PCT.Output[1];
            }
            if (CrossPlatformInputManager.GetAxis("Horizontal") != 0)
            {
                Answer[2] = CrossPlatformInputManager.GetAxis("Horizontal");
            }
            else
            {
                Answer[2] = PCT.Output[2];
            }
            if (CrossPlatformInputManager.GetAxis("Vertical") != 0)
            {
                Answer[3] = CrossPlatformInputManager.GetAxis("Vertical");
            }
            else
            {
                Answer[3] = PCT.Output[3];
            }
            PLBBP.Learn(PCT, Answer);
        }
#if MOBILE_INPUT
            AdjustInputForMobileControls(ref roll, ref pitch, ref throttle);
#endif

        AC.Move(PCT.Output[0], PCT.Output[1], PCT.Output[2], PCT.Output[3], false);
        Debug.Log("\tIn[0]: " + Math.Round(PCT.Input[0] * 100, 2) + "\tIn[1]: " + Math.Round(PCT.Input[1] * 100, 2) + "\tIn[2]: " + Math.Round(PCT.Input[2] * 100, 2) + "\tIn[3]: " + Math.Round(PCT.Input[3] * 100, 2) + "\tIn[4]: " + Math.Round(PCT.Input[4] * 100, 2) + "\tIn[5]: " + Math.Round(PCT.Input[5] * 100, 2) + "\n"+
            "\tIn[6]: " + Math.Round(PCT.Input[6] * 100, 2) + "\tIn[7]: " + Math.Round(PCT.Input[7] * 100, 2) + "\tIn[8]: " + Math.Round(PCT.Input[8] * 100, 2) + "\tIn[9]: " + Math.Round(PCT.Input[9] * 100, 2) + "\tIn[10]: " + Math.Round(PCT.Input[10] * 100, 2) + "\tIn[11]: " + Math.Round(PCT.Input[11] * 100, 2) + 
            "\tDirección X: " + PCT.Output[0] + "\tDirección Y: " + PCT.Output[1] + "\tAceleración: " + PCT.Output[2] + "\tGiro: " + PCT.Output[3] + "\n");
    }

    private void OnGUI()
    {
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

    private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
    {
        // because mobile tilt is used for roll and pitch, we help out by
        // assuming that a centered level device means the user
        // wants to fly straight and level!

        // this means on mobile, the input represents the *desired* roll angle of the aeroplane,
        // and the roll input is calculated to achieve that.
        // whereas on non-mobile, the input directly controls the roll of the aeroplane.

        float intendedRollAngle = roll * maxRollAngle * Mathf.Deg2Rad;
        float intendedPitchAngle = pitch * maxPitchAngle * Mathf.Deg2Rad;
        roll = Mathf.Clamp((intendedRollAngle - AC.RollAngle), -1, 1);
        pitch = Mathf.Clamp((intendedPitchAngle - AC.PitchAngle), -1, 1);
    }
}
