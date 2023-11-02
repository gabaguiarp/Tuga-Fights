using UnityEngine;

[CreateAssetMenu(fileName = "SceneReference", menuName = MenuPaths.Core + "Scene Reference")]
public class SceneReferenceSO : BaseSO
{
    [Space(10)]
    [SerializeField] string _sceneName;

    public string Name => _sceneName;
}
