using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MemeFight.UI
{
    public class BumperSequenceController : ManagedBehaviour
    {
        [Serializable]
        struct Bumper
        {
            public Bumper(float displayTime = 4.0f, bool fadeIn = true, bool fadeOut = true)
            {
                name = "Bumper";
                content = null;
                this.displayTime = displayTime;
                this.fadeIn = fadeIn;
                this.fadeOut = fadeOut;
            }

            public string name;
            public GameObject content;
            [Range(0.5f, 8.0f)]
            public float displayTime;
            public bool fadeIn;
            public bool fadeOut;
        }

        [SerializeField] BumperSequenceUI _sequenceUI;
        [SerializeField] List<Bumper> _bumpers = new List<Bumper>();

        [Space(10)]
        [Tooltip("The scene to load after the bumpers sequence finishes.")]
        [SerializeField] SceneReferenceSO _nextScene;

        [Header("Info")]
        [SerializeField, ReadOnly] int _currentBumperIndex = -1;

        SceneLoader _sceneLoader;

        protected override void Awake()
        {
            base.Awake();

            // Disable all bumpers
            _bumpers.ForEach(b => b.content.SetActive(false));
        }

        protected override void OnSceneReady()
        {
            _sceneLoader = SceneLoader.Instance;

            StartCoroutine(BumperSequenceLoop());
        }

        IEnumerator BumperSequenceLoop()
        {
            yield return CoroutineUtils.WaitOneFrame;

            while (_currentBumperIndex < _bumpers.LastIndex())
            {
                _currentBumperIndex++;

                Bumper bumper = _bumpers[_currentBumperIndex];
                bumper.content.SetActive(true);

                _sequenceUI.Fade(true, !bumper.fadeIn);
                if (bumper.fadeIn)
                {
                    yield return _sequenceUI.WaitForFade;
                }
                
                yield return CoroutineUtils.GetWaitRealtime(bumper.displayTime);

                _sequenceUI.Fade(false, !bumper.fadeOut);
                if (bumper.fadeOut)
                {
                    yield return _sequenceUI.WaitForFade;
                }

                bumper.content.SetActive(false);
            }

            yield return CoroutineUtils.WaitOneFrame;

            OnSequenceComplete();
        }

        void OnSequenceComplete()
        {
            _sceneLoader.LoadScene(_nextScene, true);
        }
    }
}
