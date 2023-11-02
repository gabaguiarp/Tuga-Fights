using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "VoidEvent", menuName = MenuPaths.Events + "Void Event")]
public class VoidEventSO : BaseSO
{
    public event UnityAction OnRaised;

    public void Raise()
    {
        OnRaised?.Invoke();
    }
}
