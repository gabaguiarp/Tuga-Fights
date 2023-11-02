using UnityEngine;
using UnityEditor;

namespace MemeFight
{
    [CreateAssetMenu(fileName = "AudioCue", menuName = MenuPaths.Audio + "Audio Cue")]
    public class AudioCueSO : BaseSO
    {
        [Space(10)]
        [SerializeField] AudioClip _clip = default;

        [Header("General")]
        [SerializeField] bool _loop = false;
        [Range(0, 1)]
        [SerializeField] float _volume = 1.0f;

        [Header("Pitch")]
        [SerializeField] bool _randomizePitch = false;
        [Range(-3, 3)]
        [SerializeField] float _pitch = 1.0f;
        [Range(-3, 3)]
        [SerializeField] float _pitchMin = 0.1f;
        [Range(-3, 3)]
        [SerializeField] float _pitchMax = 1.0f;

        public AudioClip Clip => _clip;
        public bool Loop => _loop;
        public float Volume => _volume;
        public float Pitch
        {
            get
            {
                if (_randomizePitch)
                    return Randomizer.GetRandom(_pitchMin, _pitchMax);

                return _pitch;
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AudioCueSO))]
        public class AudioCueSOEditor : Editor
        {
            SerializedProperty _randomPitchProperty;

            const string ScriptPropName = "m_Script";
            const string RandomizePitchPropName = "_randomizePitch";
            const string PitchPropName = "_pitch";
            const string PitchMinPropName = "_pitchMin";
            const string PitchMaxPropName = "_pitchMax";

            void OnEnable()
            {
                _randomPitchProperty = serializedObject.FindProperty(RandomizePitchPropName);
            }

            public override void OnInspectorGUI()
            {
                if (_randomPitchProperty.boolValue)
                {
                    DrawPropertiesExcluding(serializedObject, PitchPropName, ScriptPropName);
                }
                else
                {
                    DrawPropertiesExcluding(serializedObject, PitchMinPropName, PitchMaxPropName, ScriptPropName);
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
