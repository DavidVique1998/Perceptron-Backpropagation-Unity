using UnityEngine;

/// <summary>
/// Perceptron interface.
/// </summary>
public class PerceptronInterface : MonoBehaviour
{
    /// <summary>
    /// Perceptron.
    /// </summary>
    public Perceptron PCT;

    /// <summary>
    /// How many inputs have perceptron.
    /// </summary>
    public int Inputs = 0;

    private int Layers = 0;
    private int SelectLayer = 0;
    private int[] Layer;

    /// <summary>
    /// Reload perceptron.
    /// </summary>
    public bool Reload = false;

    private Rect WindowRectInterface = new Rect(0, Screen.height, 390, 245);
    private Rect WindowRectVisualization;
    private bool Resizing = false;
    private bool ShowVisualization = false;
    private bool VisualizationWeightsFade = false;
    private bool VisualizationNeurons = true;
    private float VisualizationWeightsWidth = 1;
    private string PerceptronName = "";

    private void Start()
    {
        if (gameObject.name == "StudentChild")
            Destroy(gameObject.GetComponent<PerceptronInterface>());
        else
        {
            if (PCT != null)
            {
                Inputs = PCT.Input.Length;              //how many inputs in perceptron for interface
                if (PCT.B)
                    Inputs = PCT.Input.Length - 1;

                Layer = new int[PCT.NIHL.Length];       //array of hiden layer for interface
                int i = 0;
                while (i < Layer.Length)
                {
                    if (PCT.B)
                        Layer[i] = PCT.NIHL[i] - 1;
                    else
                        Layer[i] = PCT.NIHL[i];
                    i++;
                }
            }
            else
                Debug.LogWarning("Рerceptron interface does not have perceptron.");
        }

        WindowRectVisualization.x = Screen.width / 3;
        WindowRectVisualization.y = Screen.height / 3;
        WindowRectVisualization.width = Screen.width / 3;
        WindowRectVisualization.height = Screen.height / 3;
    }

    private void OnGUI()
    {
        if (PCT != null)
        {
            WindowRectInterface = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectInterface, WindowInterface, "Perceptron of " + transform.name);  // interface window
            if (ShowVisualization)
                WindowRectVisualization = GUI.Window(GUIUtility.GetControlID(FocusType.Passive), WindowRectVisualization, WindowVisualization, "Visualization of " + transform.name);  // visualization window
        }
    }

    private void WindowInterface(int ID)                                                             // interface window
    {
        Reload = false;
        if (WindowRectInterface.height == 65)                                                        // small window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "+"))                // change window scale
            {
                WindowRectInterface.width = 390;
                WindowRectInterface.height = 245;
                WindowRectInterface.x = WindowRectInterface.x - 190;
            }
            ShowVisualization = InterfaceGUI.Button(1, 1, "Visualization OFF", "Visualization ON", ShowVisualization);  // perceptron's visualization (ON / OFF)
        }
        else                                                                                        // big window
        {
            if (GUI.Button(new Rect(WindowRectInterface.width - 20, 0, 20, 20), "-"))               // change window scale
            {
                WindowRectInterface.width = 200;
                WindowRectInterface.height = 65;
                WindowRectInterface.x = WindowRectInterface.x + 190;
            }
            PerceptronBackPropagationInterface PBPI = gameObject.GetComponent<PerceptronBackPropagationInterface>();
            if (PBPI != null)
                if (PBPI.Learn)
                    GUI.enabled = false;                                                                // disable GUI if GO have lerning interface and perceptron is study
            PerceptronRandomGenerationInterface PRGI = gameObject.GetComponent<PerceptronRandomGenerationInterface>();
            if (PRGI != null)
                if (PRGI.Learn)
                    GUI.enabled = false;                                                                // disable GUI if GO have lerning interface and perceptron is study
            PCT.AFWM = InterfaceGUI.Button(1, 1, "Without Minus", "With Minus", PCT.AFWM, ref Reload);
            PCT.B = InterfaceGUI.Button(1, 2, "Bias OFF", "Bias ON", PCT.B, ref Reload);
            PCT.AFS = InterfaceGUI.HorizontalSlider(1, 3, "Scale", PCT.AFS, 0.1F, 5F, ref Reload);
            Inputs = InterfaceGUI.IntArrows(1, 4, "Inputs", Inputs, 1);
            Layers = InterfaceGUI.IntArrows(1, 5, "Layers", PCT.NIHL.Length, 0, ref Reload);

            if (Inputs != PCT.Input.Length && !PCT.B)
                Reload = true;
            else if (Inputs != PCT.Input.Length - 1 && PCT.B)
                Reload = true;

            if (Reload)
                LayersModification();       // create new array of hiden layer
            if (Layers != 0)
            {
                SelectLayer = InterfaceGUI.IntArrows(1, 6, "Select Layer", true, SelectLayer, 0, false, Layers - 1);
                Layer[SelectLayer] = InterfaceGUI.IntArrows(1, 7, "Neurons in layer", Layer[SelectLayer], 1, ref Reload);
            }
            GUI.enabled = true;
            InterfaceGUI.Info(2, 4, "Outputs", PCT.Output.Length);

            ShowVisualization = InterfaceGUI.Button(2, 1, "Visualization OFF", "Visualization ON", ShowVisualization);          // perceptron's visualization (ON / OFF)

            PerceptronName = GUI.TextField(new Rect(200, 145, 180, 30), PerceptronName);
            bool Save = false;
            Save = InterfaceGUI.Button(2, 6, "Save", "Save", Save);
            bool Load = false;
            Load = InterfaceGUI.Button(2, 7, "Load", "Load", Load);
            if (Save)
                PCT.Save(PerceptronName);
            else if (Load)
            {
                PCT.Load(PerceptronName);
                Layers = PCT.NIHL.Length;
                Layer = Formulas.FromArray(PCT.NIHL);
                if (PCT.B)
                {
                    Inputs = PCT.Input.Length - 1;
                    int i = 0;
                    while (i < Layers)
                    {
                        Layer[i]--;
                        i++;
                    }
                }
                else
                    Inputs = PCT.Input.Length;
            }
        }

        if (Reload)
            PCT.CreatePerceptron(PCT.AFS, PCT.B, PCT.AFWM, Inputs, Layer, PCT.Output.Length);   // create perceptron

        if (WindowRectInterface.x < 0)                                                          //window restriction on the screen
            WindowRectInterface.x = 0;
        else if (WindowRectInterface.x + WindowRectInterface.width > Screen.width)
            WindowRectInterface.x = Screen.width - WindowRectInterface.width;
        if (WindowRectInterface.y < 0)
            WindowRectInterface.y = 0;
        else if (WindowRectInterface.y + WindowRectInterface.height > Screen.height)
            WindowRectInterface.y = Screen.height - WindowRectInterface.height;

        GUI.DragWindow(new Rect(0, 0, WindowRectInterface.width, 20));
    }

    private void WindowVisualization(int ID)
    {
        if (WindowRectVisualization.width == Screen.width && WindowRectVisualization.height == Screen.height)   // change window scale
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "_"))
            {
                WindowRectVisualization.x = Screen.width / 3;
                WindowRectVisualization.y = Screen.height / 3;
                WindowRectVisualization.width = Screen.width / 3;
                WindowRectVisualization.height = Screen.height / 3;
            }
        }
        else
        {
            if (GUI.Button(new Rect(WindowRectVisualization.width - 40, 0, 20, 20), "▄"))
            {
                WindowRectVisualization.x = 0;
                WindowRectVisualization.y = 0;
                WindowRectVisualization.width = Screen.width;
                WindowRectVisualization.height = Screen.height;
            }
        }
        if (GUI.Button(new Rect(WindowRectVisualization.width - 20, 0, 20, 20), "x"))                // close window
        {
            ShowVisualization = false;
        }

        Visualization(WindowRectVisualization.width, WindowRectVisualization.height);

        if (WindowRectVisualization.x < 0)                                                          //window restriction on the screen
            WindowRectVisualization.x = 0;
        else if (WindowRectVisualization.x + WindowRectVisualization.width > Screen.width)
            WindowRectVisualization.x = Screen.width - WindowRectVisualization.width;
        if (WindowRectVisualization.y < 0)
            WindowRectVisualization.y = 0;
        else if (WindowRectVisualization.y + WindowRectVisualization.height > Screen.height)
            WindowRectVisualization.y = Screen.height - WindowRectVisualization.height;

        GUI.DragWindow(new Rect(0, 0, WindowRectVisualization.width, 20));

        Vector2 Mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
        Rect MouseZone = new Rect(WindowRectVisualization.width - 20, WindowRectVisualization.height - 20, 20, 20);
        Rect Resize = new Rect();
        if (Event.current.type == EventType.MouseDown && MouseZone.Contains(Mouse))
        {
            Resizing = true;
            Resize = new Rect(Mouse.x, Mouse.y, WindowRectVisualization.width, WindowRectVisualization.height);
        }
        else if (Event.current.type == EventType.MouseUp && Resizing)
        {
            Resizing = false;
        }
        else if (!Input.GetMouseButton(0))
        {
            Resizing = false;
        }
        else if (Resizing)
        {
            WindowRectVisualization.width = Mathf.Max(100, Resize.width + (Mouse.x - Resize.x));
            WindowRectVisualization.height = Mathf.Max(100, Resize.height + (Mouse.y - Resize.y));
            WindowRectVisualization.xMax = Mathf.Min(Screen.width, WindowRectVisualization.xMax);
            WindowRectVisualization.yMax = Mathf.Min(Screen.height, WindowRectVisualization.yMax);
        }
        if (WindowRectVisualization.width < 390)
            WindowRectVisualization.width = 390;
    }

    private void Visualization(float width, float height)
    {
        Vector2 v = new Vector2(20, 30);
        width -= 40;
        height -= 80;

        if (VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Fade"))
            VisualizationWeightsFade = false;
        if (!VisualizationWeightsFade && GUI.Button(new Rect(10, 25, 85, 30), "Triange"))
            VisualizationWeightsFade = true;

        if (VisualizationNeurons && GUI.Button(new Rect(105, 25, 85, 30), "Value"))
            VisualizationNeurons = false;
        if (!VisualizationNeurons && GUI.Button(new Rect(105, 25, 85, 30), "Dot"))
            VisualizationNeurons = true;

        VisualizationWeightsWidth = InterfaceGUI.HorizontalSlider(2, 1, "Weights width", VisualizationWeightsWidth, 0.1F, 10);
        float W = width / (PCT.NIHL.Length + 1);
        int l = 0;

        TextAnchor SaveTA = GUI.skin.GetStyle("Label").alignment;
        if (VisualizationNeurons)
            GUI.skin.GetStyle("Label").alignment = TextAnchor.MiddleCenter;

        if (Event.current.type == EventType.Repaint)
        {
            while (l < PCT.NeuronWeight.Length)
            {
                int j = 0;
                while (j < PCT.NeuronWeight[l].Length)
                {
                    float H1 = height / PCT.NeuronWeight[l].Length;
                    if (PCT.B && l != PCT.NeuronWeight.Length - 1)
                        H1 = height / (PCT.NeuronWeight[l].Length + 1);
                    int k = 0;
                    while (k < PCT.NeuronWeight[l][j].Length)
                    {
                        float H2 = height / PCT.NeuronWeight[l][j].Length;
                        Vector2 P1 = new Vector2(W * l, H2 / 2 + k * H2 + 40);
                        Vector2 P2 = new Vector2(W * (l + 1), H1 / 2 + j * H1 + 40);

                        DrawANNWeight.Line(P1 + v, P2 + v, VisualizationWeightsWidth * PCT.NeuronWeight[l][j][k], Mathf.Abs(Formulas.ActivationFunction(PCT.Neuron[l][k] * PCT.NeuronWeight[l][j][k], PCT.AFS, PCT.AFWM)), VisualizationWeightsFade);
                        if (VisualizationNeurons)
                        {
                            if (j == PCT.NeuronWeight[l].Length - 1)
                            {
                                GUI.Box(new Rect(P1.x, P1.y + 15, 40, 30), "");
                                GUI.Label(new Rect(P1.x, P1.y + 15, 40, 30), PCT.Neuron[l][k].ToString("f2"));
                            }
                            if (l == PCT.NeuronWeight.Length - 1 && k == PCT.NeuronWeight[l][j].Length - 1)
                            {
                                GUI.Box(new Rect(P2.x, P2.y + 15, 40, 30), "");
                                GUI.Label(new Rect(P2.x, P2.y + 15, 40, 30), PCT.Neuron[l + 1][j].ToString("f2"));
                            }
                        }
                        else
                        {
                            if (j == PCT.NeuronWeight[l].Length - 1)
                                GUI.Box(new Rect(P1.x - 6 + v.x, P1.y - 6 + v.y, 12, 12), "");
                            if (l == PCT.NeuronWeight.Length - 1 && k == PCT.NeuronWeight[l][j].Length - 1)
                                GUI.Box(new Rect(P2.x - 6 + v.x, P2.y - 6 + v.y, 12, 12), "");
                        }
                        k++;
                    }
                    j++;
                }
                l++;
            }
        }
        if (VisualizationNeurons)
            GUI.skin.GetStyle("Label").alignment = SaveTA;
    }

    private void LayersModification()       // create new array of hiden layer
    {
        if (Layers != 0)
        {
            int i = 0;
            int[] OldLayer = new int[Layer.Length];
            while (i < OldLayer.Length)
            {
                OldLayer[i] = Layer[i];
                i++;
            }
            Layer = new int[Layers];
            i = 0;
            while (i < Mathf.Min(OldLayer.Length, Layer.Length))
            {
                Layer[i] = OldLayer[i];
                i++;
            }
            while (i < Layer.Length)
            {
                Layer[i] = 1;
                i++;
            }
        }
        else
            Layer = new int[0];
    }
}
