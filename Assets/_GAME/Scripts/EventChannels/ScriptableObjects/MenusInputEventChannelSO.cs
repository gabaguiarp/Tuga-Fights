using UnityEngine;
using UnityEngine.Events;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "MenusInputChannel", menuName = MenuPaths.InputEventChannels + "Menus Input Event Channel")]
    public class MenusInputEventChannelSO : BaseSO
    {
        public event UnityAction OnSwitchTeam;
        public event UnityAction OnPause;
        public event UnityAction OnBack;

        public void RaiseSwitchTeamEvent() => OnSwitchTeam?.Invoke();
        public void RaisePauseEvent() => OnPause?.Invoke();
        public void RaiseBackEvent() => OnBack?.Invoke();
    }
}
