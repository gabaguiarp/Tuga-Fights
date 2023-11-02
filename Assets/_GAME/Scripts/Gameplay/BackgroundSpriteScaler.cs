using UnityEngine;

namespace MemeFight
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundSpriteScaler : MonoBehaviour
    {
        enum AdjustmentSettings
        {
            NoAdjustment,
            StretchBeyondDefaultAspectRatio,
            StretchAlways
        }

        [Tooltip("Determines how the background sprite should be stretched in relation to the screen aspect ratio.\n\n" +
            "No Adjustment: Will not stretch the sprite.\n" +
            "Stretch Beyond Default Aspect Ratio: Will only stretch the sprite if the current screen aspect ratio is above " +
            "what is considered the default one, which is 16:9.\n" +
            "Stretch Always: Will stretch the sprite, in any circumstance, to fit the entire screen.")]
        [SerializeField] AdjustmentSettings _adjustmentSettings = AdjustmentSettings.StretchBeyondDefaultAspectRatio;
        [Tooltip("A reference to the camera that renders the background sprite . This will be used to determine the area the sprite " +
            "should fit. If no camera is assigned, this component will use the main camera as reference.")]
        [SerializeField] Camera _targetCamera;

        SpriteRenderer _renderer;

        void Awake()
        {
            if (_targetCamera == null)
                _targetCamera = Camera.main;

            ApplyAdjustmentSettings();
        }

        void ApplyAdjustmentSettings()
        {
            if (_adjustmentSettings == AdjustmentSettings.NoAdjustment)
            {
                return;
            }


            if (_adjustmentSettings == AdjustmentSettings.StretchAlways || !IsDefaultAspectRatio())
            {
                StretchToFitAspectRatio();
            }
        }

        void StretchToFitAspectRatio()
        {
            if (_targetCamera == null)
            {
                Debug.LogError("Cannot stretch sprite because no target camera was assigned!");
                return;
            }

            Vector3 topRightCorner = _targetCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height,
                                                                                  _targetCamera.transform.position.z));
            float worldSpaceWidth = topRightCorner.x * 2;
            float worldSpaceHeight = topRightCorner.y * 2;

            Vector3 spriteSize = GetRenderer().bounds.size;

            float scaleFactorX = worldSpaceWidth / spriteSize.x;
            float scaleFactorY = worldSpaceHeight / spriteSize.y;

            if (scaleFactorX > scaleFactorY)
            {
                scaleFactorY = scaleFactorX;
            }
            else
            {
                scaleFactorX = scaleFactorY;
            }

            transform.localScale = new Vector3(scaleFactorX, scaleFactorY, 1);
        }

        bool IsDefaultAspectRatio()
        {
            return Screen.width / Screen.height == 16 / 9;
        }

        SpriteRenderer GetRenderer()
        {
            if (_renderer == null)
                _renderer = GetComponent<SpriteRenderer>();

            return _renderer;
        }
    }
}
