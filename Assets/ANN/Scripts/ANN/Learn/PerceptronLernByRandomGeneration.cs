using UnityEngine;

/// <summary>
/// Perceptron learning by randon generation method.
/// Non-MonoBehaviour script.
/// </summary>
public class PerceptronLernByRandomGeneration
{
    /// <summary>
    /// Amount of "children" in each generation.
    /// </summary>
    public int AmountOfChildren = 20;

    /// <summary>
    /// If "true" - by waves. If "false" - use maximum in one time.
    /// </summary>
    public bool ChildrenByWave = true;
    private int ChildrenOneByOneLeft = 0;

    /// <summary>
    /// Amount of "children" in each generation's wave.
    /// </summary>
    public int ChildrenInWave = 1;
    private int ChildrenInWaveMustBe = 1;
    private int ChildrenInWavePased = 1;

    /// <summary>
    /// The best generation.
    /// </summary>
    public int BestGeneration = 0;

    /// <summary>
    /// Total number of generations.
    /// </summary>
    public int Generation;

    /// <summary>
    /// The number of "children" in the last generation.
    /// </summary>
    public int ChildrenInGeneration;

    /// <summary>
    /// The best "longevity" at the moment.
    /// </summary>
    public float BestLongevity = -Mathf.Infinity;

    /// <summary>
    /// The difference of weights between generations.
    /// </summary>
    public float ChildrenDifference = 2F;

    /// <summary>
    /// The difference of weights with the coefficient of influence between generations.
    /// </summary>
    public float ChildrenDifferenceAfterEffects = 2F;

    /// <summary>
    /// Linearly smooths the difference of weights between "children" in the generation.
    /// </summary>
    public bool ChildrenGradient = false;

    /// <summary>
    /// The coefficient of reducing of influence on the difference of weights between generations. Effects on condition that it is not equal to zero and the current generation was better than the previous one.
    /// </summary>
    public float GenerationEffect = 0F;

    /// <summary>
    /// The coefficient of increasing of influence on the difference in weight between generations. Effects on condition that it is not zero and the present generation was worse than the previous one.
    /// </summary>
    public float GenerationSplashEffect = 0F;

    /// <summary>
    /// Chance to change sign of some weights after it generation.
    /// </summary>
    public float ChangeWeightSign;

    /// <summary>
    /// Chance to choose the current worst generation. It changes with each new generation.
    /// </summary>
    public float Chance = 100;
    private float ChanceInBestGeneration = 100;

    /// <summary>
    /// The coefficient of influence on the chance of choosing the current worst generation. Effects on condition that it is not zero and the present generation was worse than the previous one.
    /// </summary>
    public float ChanceCoefficient = 0F;

    /// <summary>
    /// Auto-save on/off.
    /// </summary>
    public bool Autosave = false;

    /// <summary>
    /// Auto-save step.
    /// </summary>
    public int AutosaveStep = 10;

    /// <summary>
    /// Name of perceptron for auto-save.
    /// </summary>
    public string AutosaveName = "";

    /// <summary>
    /// Ignore children collision on/off.
    /// </summary>
    public bool IgnoreCollision = true;

    private GameObject Student;
    private Object StudentControls;
    private Object HereIsANN;
    private string PerceptronName;
    private string StudentCrash;
    private string StudentLife;
    private bool HaveStudentData = false;
    private bool HaveStudent = false;
    private GameObject[] Child;
    private Perceptron[] ChildPCT;
    private bool[] ChildCrash;
    private float[] Longevity;
    private float BestLongevityInGeneration = -Mathf.Infinity;
    private float[][][] BestChildPCTInGeneration;
    private float[][][] BestChildPCT;
    private float GenerationSplashEffectStorage = 0;
    private float GenerationEffectStorage = 0F;

    /// <summary>
    /// Data collection for training.
    /// </summary>
    /// <param name="Student">The main game object (GameObject) is to learn.</param>
    /// <param name="HereIsANN">Script containing perceptron.</param>
    /// <param name="PerceptronName">The name of the variable (Perceptron) which is called the perceptron in the script that contains the perceptron (HereIsANN).</param>
    /// <param name="StudentControls">Script for control the main game object.</param>
    /// <param name="StudentCrash">The name of the variable (bool) which is called the cause of the "crash" of the game object in the control script (StudentControls).</param>
    /// <param name="StudentLife">The name of the variable (float) is called the "longevity" of the game object in the control script (StudentControls).</param>
    public void StudentData(GameObject Student, Object HereIsANN, string PerceptronName, Object StudentControls, string StudentCrash, string StudentLife)
    {
        if (Student != null && HereIsANN != null && StudentControls != null)
        {
            this.Student = Student;
            this.HereIsANN = HereIsANN;
            this.PerceptronName = PerceptronName;
            this.StudentControls = StudentControls;
            this.StudentCrash = StudentCrash;
            this.StudentLife = StudentLife;
            HaveStudentData = true;
        }
    }

    /// <summary>
    /// The training of the perceptron by the random generation.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    public void Learn(Perceptron PCT)
    {
        if (ChanceCoefficient < 0)
            ChanceCoefficient = 0;
        else if (ChanceCoefficient > 0.5F)
            ChanceCoefficient = 0.5F;
        if (GenerationSplashEffect < 0)
            GenerationSplashEffect = 0;
        else if (GenerationSplashEffect > 1)
            GenerationSplashEffect = 1;
        if (Generation == 0)
            Generation = 1;

        if (HaveStudentData)
        {
            if (!HaveStudent)
            {
                StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), true);
                StudentControls.GetType().GetField(StudentLife).SetValue(Student.GetComponent(StudentControls.GetType()), 0);
                HaveStudent = true;
            }
            else
            {
                if (ChildrenInGeneration == 0)
                {
                    ChildrenInGeneration = AmountOfChildren;
                    ChildrenInWavePased = 0;
                    CreateChildren();
                }
                else if (ChildrenInWaveMustBe == 0 && ChildrenByWave)
                {
                    CreateChildren();
                }
                else
                {
                    int i = 0;
                    while (i < Child.Length)
                    {
                        if (!ChildrenByWave && Child[i] == null && ChildrenOneByOneLeft > 0)
                        {
                            CreateChild(i);
                        }
                        else if (Child[i] != null)
                        {
                            Object TempObject = Child[i].GetComponent(HereIsANN.GetType());
                            //Modification of new children's weight
                            if (ChildPCT[i] == null)
                            {
                                ChildPCT[i] = (Perceptron)HereIsANN.GetType().GetField(PerceptronName).GetValue(TempObject);
                                if (PCT.B)
                                    ChildPCT[i].CreatePerceptron(PCT.AFS, PCT.B, PCT.AFWM, PCT.Input.Length - 1, Formulas.FromArray(PCT.NIHL, -1), PCT.Output.Length);
                                else
                                    ChildPCT[i].CreatePerceptron(PCT.AFS, PCT.B, PCT.AFWM, PCT.Input.Length, PCT.NIHL, PCT.Output.Length);

                                int l = 0;
                                while (l < PCT.NeuronWeight.Length)
                                {
                                    int k = 0;
                                    while (k < PCT.NeuronWeight[l].Length)
                                    {
                                        int j = 0;
                                        while (j < PCT.NeuronWeight[l][k].Length)
                                        {
                                            ChildrenDifferenceAfterEffects = ChildrenDifference;
                                            if (GenerationSplashEffect != 0 && GenerationSplashEffectStorage != 0)
                                                ChildrenDifferenceAfterEffects = ChildrenDifferenceAfterEffects * (1F + GenerationSplashEffectStorage);
                                            if (GenerationEffect != 0 && GenerationEffectStorage != 0)
                                                ChildrenDifferenceAfterEffects = ChildrenDifferenceAfterEffects / (1F + GenerationEffectStorage);
                                            if (ChildrenGradient)
                                                ChildrenDifferenceAfterEffects = ChildrenDifferenceAfterEffects * ((ChildrenInWavePased + 1F) / AmountOfChildren);
                                            ChildPCT[i].NeuronWeight[l][k][j] = PCT.NeuronWeight[l][k][j] + Random.Range(-ChildrenDifferenceAfterEffects, ChildrenDifferenceAfterEffects);

                                            if (ChangeWeightSign > Random.Range(0F, 1F))
                                                ChildPCT[i].NeuronWeight[l][k][j] = -ChildPCT[i].NeuronWeight[l][k][j];
                                            j++;
                                        }
                                        k++;
                                    }
                                    l++;
                                }
                                ChildrenInWavePased++;
                            }

                            TempObject = Child[i].GetComponent(StudentControls.GetType());

                            if (!ChildCrash[i])
                                ChildCrash[i] = (bool)StudentControls.GetType().GetField(StudentCrash).GetValue(TempObject);

                            Longevity[i] = (float)StudentControls.GetType().GetField(StudentLife).GetValue(TempObject);

                            if (ChildCrash[i])
                            {
                                if (BestLongevityInGeneration < Longevity[i])
                                {
                                    BestLongevityInGeneration = Longevity[i];
                                    if (BestLongevity < BestLongevityInGeneration)
                                    {
                                        BestGeneration = Generation;
                                        BestLongevity = BestLongevityInGeneration;
                                        BestChildPCT = Formulas.FromArray(ChildPCT[i].NeuronWeight);
                                    }
                                    else
                                    {
                                        BestChildPCTInGeneration = Formulas.FromArray(ChildPCT[i].NeuronWeight);
                                    }
                                }
                                if (Child[i] != null)
                                {
                                    Object.Destroy(Child[i]);
                                    ChildrenInWaveMustBe--;
                                    if (ChildrenInWaveMustBe == 0 || !ChildrenByWave)
                                    {
                                        StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), true);
                                        StudentControls.GetType().GetField(StudentLife).SetValue(Student.GetComponent(StudentControls.GetType()), 0);
                                    }
                                    ChildrenInGeneration--;
                                }
                            }
                        }
                        i++;
                    }

                    if (ChildrenInGeneration == 0)
                    {
                        Child = new GameObject[0];
                        if (BestLongevity <= BestLongevityInGeneration)
                        {
                            if (BestGeneration == Generation)
                            {
                                BestLongevity = BestLongevityInGeneration;
                                ChanceInBestGeneration = Chance;
                                PCT.NeuronWeight = Formulas.FromArray(BestChildPCT);
                            }
                            if (GenerationEffect != 0)
                                GenerationEffectStorage += GenerationEffect;
                            if (GenerationSplashEffect != 0)
                                GenerationSplashEffectStorage = 0;
                        }
                        else
                        {
                            if (ChanceCoefficient != 0)
                            {
                                float P = 100 * Mathf.Exp((BestLongevityInGeneration - BestLongevity) / Chance);
                                if (P > Random.Range(0F, 100F))
                                {
                                    BestGeneration = Generation;
                                    BestLongevity = BestLongevityInGeneration;
                                    BestChildPCT = Formulas.FromArray(BestChildPCTInGeneration);
                                    ChanceInBestGeneration = Chance;

                                    if (GenerationEffect != 0)
                                        GenerationEffectStorage = 0;
                                    if (GenerationSplashEffect != 0)
                                        GenerationSplashEffectStorage = 0;

                                    PCT.NeuronWeight = Formulas.FromArray(BestChildPCT);
                                }
                                else
                                {
                                    if (GenerationEffect != 0)
                                        GenerationEffectStorage = 0;
                                    if (GenerationSplashEffect != 0)
                                        GenerationSplashEffectStorage += GenerationSplashEffect;
                                    if (GenerationSplashEffectStorage > ChildrenDifference * 2)
                                        GenerationSplashEffectStorage = ChildrenDifference * 2;
                                }
                            }
                            else
                            {
                                if (GenerationEffect != 0)
                                    GenerationEffectStorage = 0;
                                if (GenerationSplashEffect != 0)
                                    GenerationSplashEffectStorage += GenerationSplashEffect;
                                if (GenerationSplashEffectStorage > ChildrenDifference * 2)
                                    GenerationSplashEffectStorage = ChildrenDifference * 2;
                            }
                        }
                        if (ChanceCoefficient != 0)
                            Chance = Chance * (1 - ChanceCoefficient);
                        if (Autosave && Generation % AutosaveStep == 0)
                        {
                            if (BestGeneration == Generation)
                            {
                                PCT.Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);
                            }
                            else
                            {
                                ChildPCT[0].NeuronWeight = BestChildPCT;
                                ChildPCT[0].Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);
                            }
                        }
                        Generation++;
                        BestLongevityInGeneration = -Mathf.Infinity;
                        ChildrenOneByOneLeft = AmountOfChildren;
                    }
                }
            }
        }
    }

    private void CreateChildren()
    {
        ChildrenOneByOneLeft = AmountOfChildren;
        ChildrenInWaveMustBe = ChildrenInWave;
        if (ChildrenInWaveMustBe > ChildrenInGeneration)
            ChildrenInWaveMustBe = ChildrenInGeneration;

        ClearGeneration();
        Child = new GameObject[ChildrenInWaveMustBe];
        ChildPCT = new Perceptron[ChildrenInWaveMustBe];
        ChildCrash = new bool[ChildrenInWaveMustBe];
        Longevity = new float[ChildrenInWaveMustBe];

        int i = 0;
        while (i < ChildrenInWaveMustBe)
        {
            CreateChild(i);
            i++;
        }
    }

    private void CreateChild(int c)
    {
        ChildrenOneByOneLeft--;
        ChildPCT[c] = null;
        ChildCrash[c] = false;
        Longevity[c] = 0;

        Child[c] = Object.Instantiate(Student);
        Child[c].name = "StudentChild";

        foreach (PerceptronInterface pi in Child[c].GetComponentsInChildren<PerceptronInterface>())
            Object.Destroy(pi);
        foreach (PerceptronRandomGenerationInterface prgi in Child[c].GetComponentsInChildren<PerceptronRandomGenerationInterface>())
            Object.Destroy(prgi);

        //StudentControls.GetType().GetField(StudentCrash).SetValue(Child[c].GetComponent(StudentControls.GetType()), false);

        if (IgnoreCollision)
        {
            foreach (Collider col in Child[c].GetComponentsInChildren<Collider>())
            {
                int i = 0;
                while (i < Child.Length)
                {
                    if (Child[i] != null && i != c)
                    {
                        foreach (Collider colp in Child[i].GetComponentsInChildren<Collider>())
                            Physics.IgnoreCollision(col, colp, true);
                    }
                    i++;
                }
                foreach (Collider colp in Student.GetComponentsInChildren<Collider>())
                    Physics.IgnoreCollision(col, colp, true);
            }

            foreach (Collider2D col in Child[c].GetComponentsInChildren<Collider2D>())
            {
                int i = 0;
                while (i < Child.Length)
                {
                    if (Child[i] != null && i != c)
                    {
                        foreach (Collider2D colp in Child[i].GetComponentsInChildren<Collider2D>())
                            Physics2D.IgnoreCollision(col, colp, true);
                    }
                    i++;
                }
                foreach (Collider2D colp in Student.GetComponentsInChildren<Collider2D>())
                    Physics2D.IgnoreCollision(col, colp, true);
            }
        }
    }

    /// <summary>
    /// Immediate stop learning with the transfer of information of the best perceptron from the best generation to the perceptron what is studying.
    /// </summary>
    /// <param name="PCT">Perceptron.</param>
    public void StopLearn(Perceptron PCT)
    {
        if (PCT != null && ChildCrash != null)
        {
            int i = 0;
            while (i < ChildCrash.Length)
            {
                ChildCrash[i] = true;
                i++;
            }
            Learn(PCT);
        }
        if (PCT != null && BestChildPCT != null)
        {
            PCT.NeuronWeight = Formulas.FromArray(BestChildPCT);
            if (Autosave)
            {
                PCT.Save(AutosaveName + "_G-" + Generation + "_L-" + BestLongevityInGeneration + "_BG-" + BestGeneration + "_BL-" + BestLongevity);
            }
        }
        ClearGeneration();
        Generation = BestGeneration + 1;
        Chance = ChanceInBestGeneration;
        GenerationEffectStorage = 0;
        GenerationSplashEffectStorage = 0;
        ChildrenInWaveMustBe = 0;
        ChildrenOneByOneLeft = 0;
        ChildrenInGeneration = 0;
        HaveStudent = false;
        StudentControls.GetType().GetField(StudentCrash).SetValue(Student.GetComponent(StudentControls.GetType()), false);
    }

    private void ClearGeneration()
    {
        if (Child != null)
        {
            int i = 0;
            while (i < Child.Length)
            {
                if (Child[i] != null)
                {
                    Object.Destroy(Child[i]);
                }
                i++;
            }
            Child = new GameObject[0];
        }
    }

    /// <summary>
    /// Reset learning info.
    /// </summary>
    public void Reset()
    {
        ClearGeneration();
        Generation = 0;
        BestLongevity = -Mathf.Infinity;
        BestLongevityInGeneration = -Mathf.Infinity;
        Chance = 100;
        HaveStudent = false;
    }
}
