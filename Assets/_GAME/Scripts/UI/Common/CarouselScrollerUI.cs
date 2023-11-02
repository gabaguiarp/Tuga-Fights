using UnityEngine;
using UnityEngine.UI;

namespace MemeFight.UI
{
    public class CarouselScrollerUI : ManagedBehaviour
    {
        enum State { Uninitialized, Static, Scrolling }

        [Header("Settings")]
        [SerializeField] bool _scrollOnStart = true;
        [SerializeField] float _scrollDuration = 2.0f;
        [SerializeField] float _scrollInterval = 4.0f;
        [SerializeField] Sprite[] _slides;

        [Header("References")]
        [SerializeField] Image _frame01;
        [SerializeField] Image _frame02;

        [Header("Info")]
        [SerializeField, ReadOnly] int _currentSlideIndex;
        [SerializeField, ReadOnly] State _currentState = State.Uninitialized;

        bool _isFrameOneHidden;
        float _currentStateElapsedTime;
        float _scrollRatio;

        Vector2 _slide01StartPos;
        Vector2 _slide02StartPos;

        Vector2 FrameSize => _frame01.rectTransform.sizeDelta;

        void OnValidate()
        {
            if (_slides == null || _slides.Length == 0)
                return;

            if (_slides[0] != null && _frame01 != null)
            {
                _frame01.sprite = _slides[0];
            }
        }

        protected override void Awake()
        {
            base.Awake();
            InitialSetup();
        }

        protected override void OnSceneReady()
        {
            if (_scrollOnStart)
                StartScrolling();
        }

        void InitialSetup()
        {
            _currentSlideIndex = 0;
            _frame01.sprite = _slides[_currentSlideIndex];
            _frame02.sprite = GetNextSlide();
        }

        void Update()
        {
            if (_currentState == State.Uninitialized)
                return;

            _currentStateElapsedTime += Time.deltaTime;

            if (_currentState == State.Static)
            {
                if (_currentStateElapsedTime >= _scrollInterval)
                {
                    SwitchState(State.Scrolling);
                }
            }
            else if (_currentState == State.Scrolling)
            {
                if (_scrollRatio < 1)
                {
                    _scrollRatio = _currentStateElapsedTime / _scrollDuration;
                    _frame01.rectTransform.localPosition = Vector2.Lerp(_slide01StartPos, GetTargetPosition(_slide01StartPos),
                                                                        _scrollRatio);
                    _frame02.rectTransform.localPosition = Vector2.Lerp(_slide02StartPos, GetTargetPosition(_slide02StartPos),
                                                                        _scrollRatio);
                }
                else
                {
                    OnScrollFinished();
                    SwitchState(State.Static);
                }
            }
        }

        void OnScrollFinished()
        {
            _frame01.rectTransform.anchoredPosition = GetTargetPosition(_slide01StartPos);
            _frame02.rectTransform.anchoredPosition = GetTargetPosition(_slide02StartPos);
            _currentSlideIndex = GetNextSlideIndex();
            _isFrameOneHidden = !_isFrameOneHidden;

            // Rearrange frames
            Image hiddenFrame = _isFrameOneHidden ? _frame01 : _frame02;
            hiddenFrame.rectTransform.anchoredPosition = GetOriginPosition(hiddenFrame.rectTransform.anchoredPosition);
            hiddenFrame.sprite = GetNextSlide();
        }

        void SwitchState(State state)
        {
            if (_currentState == state)
                return;

            _currentStateElapsedTime = 0.0f;

            if (state == State.Scrolling)
            {
                _slide01StartPos = _frame01.rectTransform.anchoredPosition;
                _slide02StartPos = _frame02.rectTransform.anchoredPosition;
                _scrollRatio = 0.0f;
            }

            _currentState = state;
        }

        Vector2 GetTargetPosition(Vector2 startPos) => startPos + Vector2.left * FrameSize.x;
        Vector2 GetOriginPosition(Vector2 currentAnchoredPos) => currentAnchoredPos + Vector2.right * FrameSize.x * 2;
        int GetNextSlideIndex() => (_currentSlideIndex + 1) % _slides.Length;
        Sprite GetNextSlide() => _slides[GetNextSlideIndex()];

        public void StartScrolling()
        {
            if (_currentState != State.Uninitialized)
                return;

            SwitchState(State.Static);
        }

        public void StopScrolling() => SwitchState(State.Uninitialized);
    }
}
