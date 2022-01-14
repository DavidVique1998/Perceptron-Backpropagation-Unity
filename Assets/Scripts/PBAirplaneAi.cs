using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
    [RequireComponent(typeof(AeroplaneController))]
    public class PBAirplaneAi : MonoBehaviour
    {
        // This script represents an AI 'pilot' capable of flying the plane towards a designated target.
        // It sends the equivalent of the inputs that a user would send to the Aeroplane controller.
        [SerializeField] private float m_RollSensitivity = .2f;         // How sensitively the AI applies the roll controls
        [SerializeField] private float m_PitchSensitivity = .5f;        // How sensitively the AI applies the pitch controls
        [SerializeField] private float m_LateralWanderDistance = 5;     // The amount that the plane can wander by when heading for a target
        [SerializeField] private float m_LateralWanderSpeed = 0.11f;    // The speed at which the plane will wander laterally
        [SerializeField] private float m_MaxClimbAngle = 45;            // The maximum angle that the AI will attempt to make plane can climb at
        [SerializeField] private float m_MaxRollAngle = 45;             // The maximum angle that the AI will attempt to u
        [SerializeField] private float m_SpeedEffect = 0.01f;           // This increases the effect of the controls based on the plane's speed.
        [SerializeField] private float m_TakeoffHeight = 20;            // the AI will fly straight and only pitch upwards until reaching this height
        [SerializeField] private Transform m_Target;                    // the target to fly towards

        /// <summary>
        /// Object to control and learn.
        /// </summary>
        private AeroplaneController m_AeroplaneController;  // The aeroplane controller that is used to move the plane
        private float m_RandomPerlin;                       // Used for generating random point on perlin noise so that the plane will wander off path slightly
        private bool m_TakenOff;                            // Has the plane taken off yet

        private int maxView = 500;
        // these max angles are only used on mobile, due to the way pitch and roll input are handled
        public float maxRollAngle = 80;
        public float maxPitchAngle = 80;

        // setup script properties

        /// <summary>
        /// Perceptron to control in learn.
        /// </summary>
        private Perceptron PCT = new Perceptron();
        /// <summary>
        /// Association type of learn with perceptron
        /// </summary>
        private PerceptronLernByBackPropagation PLBBP = new PerceptronLernByBackPropagation();
        // Start is called before the first frame update
        private void Awake()
        {
            // get the reference to the aeroplane controller, so we can send move input to it and read its current state.
            m_AeroplaneController = GetComponent<AeroplaneController>();

            // pick a random perlin starting point for lateral wandering
            m_RandomPerlin = Random.Range(0f, 100f);
        }

        void Start()
        {

            PCT.CreatePerceptron(1, false, true, 15, null, 4);

            PLBBP.LearningRate = 0.5F;
            PLBBP.DesiredMaxError = 0.5F;
            PLBBP.WithError = true;

            PInterface PI = gameObject.AddComponent<PInterface>();
            PI.PCT = PCT;

            PBInterface PBI = gameObject.AddComponent<PBInterface>();
            PBI.PCT = PCT;
            PBI.PLBBP = PLBBP;
        }

        // reset the object to sensible values
        public void Reset()
        {
            m_TakenOff = false;
        }

        // fixed update is called in time with the physics system update
        private void FixedUpdate()
        {
            //Vector3 forward = transform.TransformDirection(Vector3.forward) * 500;
            //Debug.DrawRay(transform.position, forward, Color.red);
            //forward = transform.TransformDirection(Vector3.left) * 500;
            //Debug.DrawRay(transform.position, forward, Color.blue);
            //<-------------------------- Training perceptron by Raycast of colliders  --------------------------->
            RaycastHit Hit;
            int i = 0;
            while (i < PCT.Input.Length - 1)
            {
                if (i < PCT.Input.Length - 9) //-1
                {
                    Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis(i * 60 - 360, Vector3.up) * Vector3.forward) *500, Color.black);
                    if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis(i * 60 - 360, Vector3.up) * Vector3.forward), out Hit, maxView))
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
                    Debug.DrawRay(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis((i - 6) * 60 - 360+30, Vector3.right) * Vector3.forward)*500, Color.red);
                    if (Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0, 1, 20)), transform.TransformDirection(Quaternion.AngleAxis((i - 6) * 60 - 180+30, Vector3.right) * Vector3.forward), out Hit, maxView))
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
            PCT.Input[i] = (m_AeroplaneController.ForwardSpeed * Mathf.Sign(Vector3.Cross(-transform.right, gameObject.GetComponent<Rigidbody>().velocity).y)) / (maxView);
            PCT.Solution();
            float[] Answer = new float[4];
            //<-------------------------- Follow the training line  --------------------------->
            if (m_Target != null)
            {
                // make the plane wander from the path, useful for making the AI seem more human, less robotic.
                Vector3 targetPos = m_Target.position +
                                    transform.right *
                                    (Mathf.PerlinNoise(Time.time * m_LateralWanderSpeed, m_RandomPerlin) * 2 - 1) *
                                    m_LateralWanderDistance;

                // adjust the yaw and pitch towards the target
                Vector3 localTarget = transform.InverseTransformPoint(targetPos);
                float targetAngleYaw = Mathf.Atan2(localTarget.x, localTarget.z);

                float targetAnglePitch = -Mathf.Atan2(localTarget.y, localTarget.z);


                // Set the target for the planes pitch, we check later that this has not passed the maximum threshold
                targetAnglePitch = Mathf.Clamp(targetAnglePitch, -m_MaxClimbAngle * Mathf.Deg2Rad,
                                               m_MaxClimbAngle * Mathf.Deg2Rad);

                // calculate the difference between current pitch and desired pitch
                float changePitch = targetAnglePitch - m_AeroplaneController.PitchAngle;

                // AI always applies gentle forward throttle
                const float throttleInput = 0.5f;

                // AI applies elevator control (pitch, rotation around x) to reach the target angle
                float pitchInput = changePitch * m_PitchSensitivity;
                // clamp the planes roll
                float desiredRoll = Mathf.Clamp(targetAngleYaw, -m_MaxRollAngle * Mathf.Deg2Rad, m_MaxRollAngle * Mathf.Deg2Rad);
                float yawInput = 0;
                float rollInput = 0;
                if (!m_TakenOff)
                {
                    // If the planes altitude is above m_TakeoffHeight we class this as taken off
                    if (m_AeroplaneController.Altitude > m_TakeoffHeight)
                    {
                        m_TakenOff = true;
                    }
                }
                else
                {
                    // now we have taken off to a safe height, we can use the rudder and ailerons to yaw and roll
                    yawInput = targetAngleYaw;
                    rollInput = -(m_AeroplaneController.RollAngle - desiredRoll) * m_RollSensitivity;
                }

                // adjust how fast the AI is changing the controls based on the speed. Faster speed = faster on the controls.
                float currentSpeedEffect = 1 + (m_AeroplaneController.ForwardSpeed * m_SpeedEffect);
                rollInput *= currentSpeedEffect;
                pitchInput *= currentSpeedEffect;
                yawInput *= currentSpeedEffect;
                Answer[0] = rollInput;
                Answer[1] = pitchInput;
                Answer[2] = yawInput;
                Answer[3] = throttleInput;
                PLBBP.Learn(PCT, Answer);
                // pass the current input to the plane (false = because AI never uses air brakes!)
                m_AeroplaneController.Move(rollInput, pitchInput, yawInput, throttleInput, false);
                //m_AeroplaneController.Move(Answer[0], Answer[1], Answer[2], Answer[3], false);
            }
            else
            {
                Answer[0] = 0;
                Answer[1] = 0;
                Answer[2] = 0;
                Answer[3] = 0;
                PLBBP.Learn(PCT, Answer);
                //PLBBP.Learn(PCT, Answer);
                // no target set, send zeroed input to the planeW
                m_AeroplaneController.Move(0, 0, 0, 0, false);
            }
            //PLBBP.Learn(PCT, Answer);

            //m_AeroplaneController.Move(PCT.Output[0], PCT.Output[1], PCT.Output[2], PCT.Output[3], false);
            //Debug.Log("\tIn[0]: " + Math.Round(PCT.Input[0] * 100, 2) + "\tIn[1]: " + Math.Round(PCT.Input[1] * 100, 2) + "\tIn[2]: " + Math.Round(PCT.Input[2] * 100, 2) + "\tIn[3]: " + Math.Round(PCT.Input[3] * 100, 2) + "\tIn[4]: " + Math.Round(PCT.Input[4] * 100, 2) + "\tIn[5]: " + Math.Round(PCT.Input[5] * 100, 2) + "\n" +
            //    "\tIn[6]: " + Math.Round(PCT.Input[6] * 100, 2) + "\tIn[7]: " + Math.Round(PCT.Input[7] * 100, 2) + "\tIn[8]: " + Math.Round(PCT.Input[8] * 100, 2) + "\tIn[9]: " + Math.Round(PCT.Input[9] * 100, 2) + "\tIn[10]: " + Math.Round(PCT.Input[10] * 100, 2) + "\tIn[11]: " + Math.Round(PCT.Input[11] * 100, 2) +
            //    "\tDirección X: " + PCT.Output[0] + "\tDirección Y: " + PCT.Output[1] + "\tAceleración: " + PCT.Output[2] + "\tGiro: " + PCT.Output[3] + "\n");
        }


        // allows other scripts to set the plane's target
        public void SetTarget(Transform target)
        {
            m_Target = target;
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
            roll = Mathf.Clamp((intendedRollAngle - m_AeroplaneController.RollAngle), -1, 1);
            pitch = Mathf.Clamp((intendedPitchAngle - m_AeroplaneController.PitchAngle), -1, 1);
        }
    }

}
