using System;
using System.Collections.Generic;
using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "Credits", menuName = MenuPaths.Core + "Game Credits")]
    public class GameCreditsSO : ScriptableObject
    {
        public List<Role> roles = new List<Role>();

        [Serializable]
        public class Role
        {
            public string title = "Role Title";
            [Space(10)]
            public List<string> names = new List<string>() { "Creator Name" };
        }
    }
}
