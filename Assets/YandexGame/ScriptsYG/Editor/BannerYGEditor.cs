using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace YG
{
    [CustomEditor(typeof(BannerYG))]
    public class BannerYGEditor : Editor
    {
        BannerYG scr;
        CanvasScaler scaler;

        private void OnEnable()
        {
            scr = (BannerYG)target;
            scr.rt = scr.transform.GetChild(0).GetComponent<RectTransform>();

            scaler = scr.GetComponent<CanvasScaler>();
            if (!scaler)
                Debug.LogError("Не найден компонент CanvasScaler!");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            scr.rt.pivot = new Vector2(0, 1);

            if (scr.UIScaleMode == BannerYG.ScaleMode.ConstantPixelSize)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            }
            else if (scr.UIScaleMode == BannerYG.ScaleMode.ScaleWithScreenSize)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = scr.referenceResolution;
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

                if (scr.Match == BannerYG.MatchMode.Width)
                    scaler.matchWidthOrHeight = 0;
                else
                    scaler.matchWidthOrHeight = 1;
            }
        }
    }
}
