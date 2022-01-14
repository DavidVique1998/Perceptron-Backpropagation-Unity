using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


// The Originator holds some important state that may change over time. It
// also defines a method for saving the state inside a memento and another
// method for restoring the state from it.
public class Originator 
{
    // For the sake of simplicity, the originator's state is stored inside a
    // single variable.

    public Perceptron _state { get; private set; }
    public float[] _answer { get; private set; }

    public Originator(Perceptron state)
    {
        this._state = state;
        //Debug.LogWarning("Originator: My initial state is: " + state.ToString());
    }

    public Originator(Perceptron state, float[] answer)
    {
        this._state = state;
        this._answer = _answer;
        //Debug.LogWarning("Originator: My initial state is: " + state.ToString());
    }

    // The Originator's business logic may affect its internal state.
    // Therefore, the client should backup the state before launching
    // methods of the business logic via the save() method.
    public void DoSomething(Perceptron state)
    {
        //Debug.LogWarning("Originator: I'm doing something important.");
        this._state = state;
        //Debug.LogWarning($"Originator: and my state has changed to: {_state}");
    }
    public void DoSomething(Perceptron state, float[] answer)
    {
        //Debug.LogWarning("Originator: I'm doing something important.");
        this._state = state;
        this._answer = answer;
        //Debug.LogWarning($"Originator: and my state has changed to: {_state}");
    }

    // Saves the current state inside a memento.
    public IMemento Save()
    {
        return new ConcreteMemento(this._state.DeepCopy());
    }

    public IMemento SaveAnswer()
    {
        return new ConcreteMemento(this._state.DeepCopy(),Formulas.DeepClone(this._answer));
    }

    // Restores the Originator's state from a memento object.
    public void Restore(IMemento memento)
    {
        if (!(memento is ConcreteMemento))
        {
            Debug.LogWarning("Unknown memento class " + memento.ToString());
        }
        else
        {
            this._state = memento.GetState();
        }
    }

    public void RestoreAnswer(IMemento memento)
    {
        if (!(memento is ConcreteMemento))
        {
            Debug.LogWarning("Unknown memento class " + memento.ToString());
        }
        else
        {
            this._state = memento.GetState();
            this._answer = memento.GetAnswer();
        }
    }
}

// The Memento interface provides a way to retrieve the memento's metadata,
// such as creation date or name. However, it doesn't expose the
// Originator's state.
public interface IMemento
{
    string GetName();

    Perceptron GetState();

    float[] GetAnswer();
 
    DateTime GetDate();

}


// The Concrete Memento contains the infrastructure for storing the
// Originator's state.
public class ConcreteMemento : IMemento
{
    private Perceptron _state;
    private float[] _answer;

    private DateTime _date;

    public ConcreteMemento(Perceptron state)
    {
        this._state = state;
        this._date = DateTime.Now;
    }

    public ConcreteMemento(Perceptron state, float[] answer)
    {
        this._state = state;
        this._answer = answer;
        this._date = DateTime.Now;
    }

    // The Originator uses this method when restoring its state.
    public Perceptron GetState()
    {
        return this._state;
    }

    public float[] GetAnswer() { 
        return this._answer;
    }

    // The rest of the methods are used by the Caretaker to display
    // metadata.
    public string GetName()
    {
        return $"{this._date} / ({this._state.ToString().Substring(0, 3)})...";
    }

    public DateTime GetDate()
    {
        return this._date;
    }
}


// The Caretaker doesn't depend on the Concrete Memento class. Therefore, it
// doesn't have access to the originator's state, stored inside the memento.
// It works with all mementos via the base Memento interface.
public class Caretaker
{
    private List<IMemento> _mementos = new List<IMemento>();

    public int MaxMementos { get {
            return _mementos.Count();
        } }

    private Originator _originator = null;

    public Caretaker(Originator originator)
    {
        this._originator = originator;
    }

    public void RestoreSelect(int index) {
        if (index >= 0 && index < _mementos.Count()) {
            _originator.RestoreAnswer(_mementos[index]);
        }

    }

    public string GetName(int index) {
        if(index >= 0 && index < _mementos.Count())
        return this._mementos[index].GetName();
        return "";
    }

    public void Backup()
    {
        this._mementos.Add(this._originator.Save());
    }

    public void BackupAnswer()
    {
        this._mementos.Add(this._originator.SaveAnswer());
    }

    public void Undo()
    {
        if (this._mementos.Count == 0)
        {
            return;
        }

        var memento = this._mementos.Last();
        this._mementos.Remove(memento);

        //Debug.LogWarning("Caretaker: Restoring state to: " + memento.GetName());

        try
        {
            this._originator.RestoreAnswer(memento);
        }
        catch (Exception)
        {
            this.Undo();
        }
    }

    public void ShowHistory()
    {
        //Debug.LogWarning("Caretaker: Here's the list of mementos:");

        foreach (var memento in this._mementos)
        {
            Debug.LogWarning(memento.GetName());
        }
    }
}


