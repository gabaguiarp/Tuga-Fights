using UnityEngine.Events;

public class GenericEventSO<T> : BaseSO
{
    public event UnityAction<T> OnRaised;

    public void Raise(T arg)
    {
        OnRaised?.Invoke(arg);
    }
}
