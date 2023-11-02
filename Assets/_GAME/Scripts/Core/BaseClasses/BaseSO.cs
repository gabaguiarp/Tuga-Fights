using UnityEngine;

public class BaseSO : ScriptableObject
{
    [TextArea(3, 10)]
    [SerializeField] protected string _description;
}
