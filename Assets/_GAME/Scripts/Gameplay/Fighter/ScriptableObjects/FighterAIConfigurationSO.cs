using UnityEngine;

namespace MemeFight
{
    [CreateAssetMenu(menuName = "Gameplay/Fighter/AI Configuration", order = 1)]
    public class FighterAIConfigurationSO : ScriptableObject
    {
        public float PrewarmTime = 0.5f;
        public float AlertStateDecisionFrequency = 0.8f;
        public float MaxIdleTime = 0.5f;
        public int MoveProbability = 2;
        public float MinChainAttackDelay = 0.05f;
        public float MaxChainAttackDelay = 0.3f;
        public int MinConsecutiveAttacks = 1;
        public int MaxConsecutiveAttacks = 3;
        public float MinAttackInterval = 0.3f;
        public float MaxAttackInterval = 0.5f;
        public float AttackChainCooldownTime = 2.0f;
        [Tooltip("The probability of stepping back after finishing an attack chain.")]
        public int StepBackProbability = 1;
        public int LowerAttackProbability = 3;
        public int DefenseProbability = 2;
        public int BlockAttackProbability = 1;
        [Tooltip("How many consecutive attacks need to be registered so the AI becomes hostile.")]
        public int HostileThreshold = 2;
    }
}
