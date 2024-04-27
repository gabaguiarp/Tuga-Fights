using UnityEngine.Localization;

namespace MemeFight.UI
{
    public class FighterDisplayData
    {
        public class FighterUnlockStateInfo
        {
            public bool IsUnlocked { get; private set; }
            public LocalizedString UnlockMessage { get; private set; }

            public FighterUnlockStateInfo(bool isUnlocked, LocalizedString unlockMessage)
            {
                IsUnlocked = isUnlocked;
                UnlockMessage = unlockMessage;
            }
        }

        public FighterProfileSO Profile { get; private set; }
        public FighterUnlockStateInfo State { get; private set; }


        public FighterDisplayData(FighterProfileSO profile, bool isUnlocked, LocalizedString unlockMessage)
        {
            Profile = profile;
            State = new FighterUnlockStateInfo(isUnlocked, unlockMessage);
        }

        public FighterDisplayData(FighterProfileSO profile)
        {
            Profile = profile;
            State = new FighterUnlockStateInfo(true, new LocalizedString());
        }
    }
}
