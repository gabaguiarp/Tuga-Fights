namespace MemeFight.DebugSystem
{
    public class MatchDebugMenuUI : DebugMenuUIBase
    {
        public void AdvanceCampaignClick()
        {
            if (ResourcesManager.PersistentData.CurrentCampaignMatchIndex < ResourcesManager.CampaignStream.LastMatchIndex)
            {
                ResourcesManager.PersistentData.ProceedCampaign();
                SceneLoader.Instance.ReloadCurrentScene();
            }
            else
            {
                SceneLoader.Instance.LoadMainMenu();
            }
        }

        protected override bool ShouldEnable()
        {
            return base.ShouldEnable() && ResourcesManager.PersistentData.IsCampaignMode;
        }
    }
}
