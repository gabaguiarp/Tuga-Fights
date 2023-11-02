using UnityEngine;

public class VariableBaseSO<T> : ScriptableObject
{
    [SerializeField] T _value;

    public T Value => _value;

    protected const string kVariablesPath = "Variable/";
}
