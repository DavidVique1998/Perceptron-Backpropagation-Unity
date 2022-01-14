using UnityEngine;

/// <summary>
/// Interface of perceptron's random generation learning method.
/// </summary>
public class PerceptronRandomGenerationInterface : MonoBehaviour
{
    /// <summary>
    /// Perceptron.
    /// </summary>
    public Perceptron PCT;

    /// <summary>
    /// Perceptron' random generation learning method.
    /// </summary>
    public PerceptronLernByRandomGeneration PLBRG = new PerceptronLernByRandomGeneration();

    /// <summary>
    /// Is perseptron learning?
    /// </summary>
    public bool Learn = false;

    private Rect WindowRect = new Rect(Screen.width, Screen.height, 580, 245);

    private bool Settings = true;

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<PerceptronRandomGenerationInterface>());
        else
        {
            if (PCT == null)
                Debug.LogWarning("Perceptron random generation interface does not have perceptron.");
        }
    }

    private void Update()
    {
        if (PCT != null && Learn)
            PLBRG.Learn(PCT);
        PerceptronInterface PI = gameObject.GetComponent<PerceptronInterface>();
        if (PI != null)
            if (PI.Reload)
                PLBRG.Reset();
    }

    private void OnGUI()
    {
        if (PCT != null)
        {
            WindowRect = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRect, Window, "Random generation of " + transform.name);    // interface window
        }
    }

    private void Window(int ID)                                                                             // interface window
    {
        if (WindowRect.height == 65)                                                                // small window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "+"))                        // change window scale
            {
                if (Settings)
                {
                    WindowRect.width = 580;
                    WindowRect.height = 245;
                    WindowRect.x = WindowRect.x - 380;
                }
                else
                {
                    WindowRect.width = 200;
                    WindowRect.height = 245;
                }
            }

            bool Activte = false;
            Learn = InterfaceGUI.Button(1, 1, "Learn OFF", "Learn ON", Learn, ref Activte);

            if (Activte && !Learn)
                PLBRG.StopLearn(PCT);
        }
        else                                                                                        // big window
        {
            if (GUI.Button(new Rect(WindowRect.width - 20, 0, 20, 20), "-"))                        // change window scale
            {
                WindowRect.width = 200;
                WindowRect.height = 65;
                if (Settings)
                    WindowRect.x = WindowRect.x + 380;
            }

            if (GUI.Button(new Rect(WindowRect.width - 40, 0, 20, 20), "S"))                        // Settings on/off
            {
                if (Settings)
                {
                    WindowRect.width = 200;
                    WindowRect.height = 245;
                    WindowRect.x = WindowRect.x + 380;
                    Settings = false;
                }
                else
                {
                    WindowRect.width = 580;
                    WindowRect.height = 245;
                    WindowRect.x = WindowRect.x - 380;
                    Settings = true;
                }
            }

            if (Settings)
            {
                if (Learn)
                    GUI.enabled = false;
                PLBRG.AmountOfChildren = InterfaceGUI.IntArrows(2, 1, "Children amount", PLBRG.AmountOfChildren, 1);
                if (PLBRG.AmountOfChildren > 1)
                {
                    if (PLBRG.AmountOfChildren!= PLBRG.ChildrenInWave)
                        PLBRG.ChildrenByWave = InterfaceGUI.Button(3, 1, "Maximum in one time", "By waves", PLBRG.ChildrenByWave);
                    PLBRG.ChildrenInWave = InterfaceGUI.IntArrows(3, 2, "Children in wave", PLBRG.ChildrenInWave, 1, PLBRG.AmountOfChildren);
                }
                PLBRG.ChildrenDifference = InterfaceGUI.HorizontalSlider(2, 2, "Children difference", PLBRG.ChildrenDifference, 0.01F, 10F);
                PLBRG.ChildrenGradient = InterfaceGUI.Button(2, 3, "Children gradient OFF", "Children gradient ON", PLBRG.ChildrenGradient);
                
                PLBRG.GenerationEffect = InterfaceGUI.HorizontalSlider(2, 4, "Generation effect", PLBRG.GenerationEffect, 0F, 1F);
                PLBRG.GenerationSplashEffect = InterfaceGUI.HorizontalSlider(2, 5, "Splash effect coefficient", PLBRG.GenerationSplashEffect, 0F, 1F);
                PLBRG.ChanceCoefficient = InterfaceGUI.HorizontalSlider(2, 6, "Chance coefficient", PLBRG.ChanceCoefficient, 0F, 0.5F);
                PLBRG.ChangeWeightSign = InterfaceGUI.HorizontalSlider(2, 7, "Chance change sign", PLBRG.ChangeWeightSign, PLBRG.ChangeWeightSign * 100, 0F, 1F);

                PLBRG.IgnoreCollision = InterfaceGUI.Button(3, 3, "Collision ON", "Collision OFF", PLBRG.IgnoreCollision);

                GUI.enabled = true;
                PLBRG.Autosave = InterfaceGUI.Button(3, 5, "Autosave OFF", "Autosave ON", PLBRG.Autosave);
                if (PLBRG.Autosave)
                {
                    PLBRG.AutosaveStep = InterfaceGUI.IntArrows(3, 6, "Autosave step", PLBRG.AutosaveStep, 1);
                    PLBRG.AutosaveName = GUI.TextField(new Rect(390, 205, 180, 30), PLBRG.AutosaveName);
                }
            }

            bool Activte = false;
            Learn = InterfaceGUI.Button(1, 7, "Learn OFF", "Learn ON", Learn, ref Activte);

            if (Activte && !Learn)
                PLBRG.StopLearn(PCT);

            InterfaceGUI.Info(1, 1, "Best generation", PLBRG.BestGeneration);
            InterfaceGUI.Info(1, 2, "Generation", PLBRG.Generation);
            InterfaceGUI.Info(1, 3, "Children", PLBRG.ChildrenInGeneration);
            InterfaceGUI.Info(1, 4, "Best longevity", PLBRG.BestLongevity);

            //if (PLBRG.GenerationSplashEffect != 0 || PLBRG.GenerationEffect != 0 || (PLBRG.ChildrenGradient && PLBRG.AmountOfChildren != PLBRG.ChildrenInWave))
            {
                if (!Learn)
                    PLBRG.ChildrenDifferenceAfterEffects = PLBRG.ChildrenDifference;
                InterfaceGUI.InfoF2(1, 5, "Children difference", PLBRG.ChildrenDifferenceAfterEffects);
            }
            if (PLBRG.ChanceCoefficient != 0)
                InterfaceGUI.Info(1, 6, "Chance", PLBRG.Chance);
        }

        if (WindowRect.x < 0)                                           //window restriction on the screen
            WindowRect.x = 0;
        else if (WindowRect.x + WindowRect.width > Screen.width)
            WindowRect.x = Screen.width - WindowRect.width;
        if (WindowRect.y < 0)
            WindowRect.y = 0;
        else if (WindowRect.y + WindowRect.height > Screen.height)
            WindowRect.y = Screen.height - WindowRect.height;

        GUI.DragWindow(new Rect(0, 0, WindowRect.width, 20));
    }
}
