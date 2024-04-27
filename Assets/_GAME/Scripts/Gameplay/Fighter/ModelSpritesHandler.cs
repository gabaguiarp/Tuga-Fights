using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MemeFight.Characters
{
    public class ModelSpritesHandler : MonoBehaviour
    {
        enum Side { Left, Right }
        enum LayerOrder { Default, InFront, Behind, Frontmost }

        [SerializeField] Side _modelSide;

        #region Comparers
        readonly Dictionary<string, LayerOrder> LeftSideSpritesOrder = new Dictionary<string, LayerOrder>()
        {
            ["armR"] = LayerOrder.Frontmost,
            ["head"] = LayerOrder.Frontmost,
            ["torso"] = LayerOrder.Frontmost,
            ["shirt"] = LayerOrder.Frontmost,
            ["coat"] = LayerOrder.Frontmost,
            ["skirt"] = LayerOrder.Frontmost,
            ["armL"] = LayerOrder.Behind,
            ["legR"] = LayerOrder.Frontmost,
            ["legL"] = LayerOrder.Frontmost
        };

        readonly Dictionary<string, LayerOrder> RightSideSpritesOrder = new Dictionary<string, LayerOrder>()
        {
            ["armR"] = LayerOrder.InFront,  // previously infront
            ["head"] = LayerOrder.InFront,  // previously infront
            ["torso"] = LayerOrder.Default,
            ["shirt"] = LayerOrder.InFront,
            ["coat"] = LayerOrder.Default,  // previsouly infront
            ["skirt"] = LayerOrder.InFront,  // previsouly infront
            ["armL"] = LayerOrder.Behind,
            ["legR"] = LayerOrder.Default, // previously infront
            ["legL"] = LayerOrder.Behind
        };
        #endregion

        public void ApplySetup()
        {
            foreach (SpriteRenderer renderer in transform.GetComponentsInChildren<SpriteRenderer>(true))
            {
                CheckObjectNameAndApplySetup(renderer);
            }

            void CheckObjectNameAndApplySetup(SpriteRenderer renderer)
            {
                var comparer = _modelSide.Equals(Side.Left) ? LeftSideSpritesOrder : RightSideSpritesOrder;
                foreach (var kvp in comparer)
                {
                    if (renderer.gameObject.name.Contains(kvp.Key))
                    {
                        try
                        {
                            renderer.sortingLayerName = kvp.Value.ToString();
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }
            }

            Debug.Log("Layers setup applied".ToUpper());
#if UNITY_EDITOR
            EditorUtility.SetDirty(this.gameObject);
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ModelSpritesHandler))]
    public class ModelSpriteHandlerEditor : Editor
    {
        ModelSpritesHandler _handler;

        void OnEnable()
        {
            _handler = (ModelSpritesHandler)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Apply Setup"))
            {
                _handler.ApplySetup();
            }
        }
    }
#endif
}
