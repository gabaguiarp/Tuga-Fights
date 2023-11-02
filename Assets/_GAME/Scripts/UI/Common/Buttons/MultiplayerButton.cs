using UnityEngine.UI;

namespace MemeFight.UI
{
    public class MultiplayerButton : Button
    {
        public override Selectable FindSelectableOnLeft()
        {
            var sel = base.FindSelectableOnLeft();
            return FilterSelectableForParentMatch(sel);
        }

        public override Selectable FindSelectableOnRight()
        {
            var sel = base.FindSelectableOnRight();
            return FilterSelectableForParentMatch(sel);
        }

        public override Selectable FindSelectableOnUp()
        {
            var sel = base.FindSelectableOnUp();
            return FilterSelectableForParentMatch(sel);
        }

        public override Selectable FindSelectableOnDown()
        {
            var sel = base.FindSelectableOnDown();
            return FilterSelectableForParentMatch(sel);
        }

        Selectable FilterSelectableForParentMatch(Selectable selectable)
        {
            if (selectable != null && selectable.transform.parent == this.transform.parent)
                return selectable;

            return null;
        }
    }
}
