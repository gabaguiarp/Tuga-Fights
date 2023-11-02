using System;

namespace MemeFight
{
    [Serializable]
    public class StatData
    {
        public StatData(StatID id, int initialValue = 0)
        {
            ID = id;
            Value = initialValue;
        }

        public StatID ID;
        public int Value;
    }
}
