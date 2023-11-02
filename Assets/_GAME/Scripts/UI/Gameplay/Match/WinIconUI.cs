using UnityEngine;
using UnityEngine.UI;

public class WinIconUI : MonoBehaviour
{
    [SerializeField] Image _image;
    [SerializeField] Sprite _offIcon;
    [SerializeField] Sprite _onIcon;

    public bool IsOn => _image.sprite == _onIcon;

    public void SetHighlightOn(bool turnOn)
    {
        _image.sprite = turnOn ? _onIcon : _offIcon;
    }
}
