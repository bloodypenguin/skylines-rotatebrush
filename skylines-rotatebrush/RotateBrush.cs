using System;
using System.Reflection;
using UnityEngine;

namespace RotateBrushImproved
{
    public class RotateBrush
    {
        private readonly int Angle = 5;
        private Texture2D _originalBrush;
        private int _dstAngle;

        public RotateBrush()
        {
            _originalBrush = new Texture2D(0, 0);
        }

        public void RotateLeft()
        {
            Rotate(false);
        }

        public void RotateRight()
        {
            Rotate(true);
        }

        private void Rotate(bool forward)
        {
            var angle = forward ? Angle : -Angle;

            try
            {
                var currentBrush = GetCurrentBrush();
                if (currentBrush == null)
                {
                    return;
                }
                if (!_originalBrush.name.Equals(currentBrush.name))
                {
                    // brush has changed
                    _dstAngle = 0;
                    _originalBrush = currentBrush;
                }
                _dstAngle = (_dstAngle + angle + 360) % 360;

                var nextBrush = RotateTexture(_originalBrush, _dstAngle);
                SetBrush(nextBrush);
            }
            catch (Exception ex)
            {
                DebugOutputPanel.AddMessage(ColossalFramework.Plugins.PluginManager.MessageType.Error, "RotateBrush Exception: " + ex.ToString());
            }
        }

        private static Texture2D GetCurrentBrush()
        {
            var tool = ToolsModifierControl.GetCurrentTool<ToolBase>();
            var brushField = GetBrushField(tool);
            return (Texture2D) brushField?.GetValue(tool);
        }

        private static void SetBrush(Texture2D nextBrush)
        {
            var tool = ToolsModifierControl.GetCurrentTool<ToolBase>();
            var brushField = GetBrushField(tool);
            brushField?.SetValue(tool, nextBrush);
        }

        private static FieldInfo GetBrushField(ToolBase tool)
        {
            var brushField = tool?.GetType()
                .GetField("m_brush",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (brushField == null || brushField.FieldType != typeof(Texture2D))
            {
                return null;
            }
            return brushField;
        }

        private Texture2D RotateTexture(Texture2D src, int angle)
        {
            var dst = new Texture2D(src.width, src.height);

            src.wrapMode = TextureWrapMode.Clamp;

            var pivot = new Vector3(dst.width / 2 - 0.5f, dst.height / 2 - 0.5f);
            var scale = Vector3.one;
            var rotation = Quaternion.Euler(0, 0, angle);

            var translateToPivot = Matrix4x4.TRS(-1f * pivot, Quaternion.identity, scale);
            var rotate = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
            var translateBack = Matrix4x4.TRS(pivot, Quaternion.identity, scale);

            for (var x = 0; x < dst.width; x++)
            {
                for (var y = 0; y < dst.height; y++)
                {
                    var srcCoords = new Vector3(x, y, 0);

                    srcCoords = translateToPivot.MultiplyPoint3x4(srcCoords);
                    srcCoords = rotate.MultiplyPoint3x4(srcCoords);
                    srcCoords = translateBack.MultiplyPoint3x4(srcCoords);

                    var dstColor = AvgPixelValue(src, srcCoords);
                    dst.SetPixel(x, y, dstColor);
                }
            }
            dst.Apply();
            return dst;
        }

        // http://www.leptonica.com/rotation.html
        private static Color AvgPixelValue(Texture2D tex, Vector3 sourceCoords)
        {
            const int n = 10;
            var xPos = (int)sourceCoords.x;
            var yPos = (int)sourceCoords.y;

            var f1 = tex.GetPixel(xPos, yPos);
            var f2 = tex.GetPixel(xPos + 1, yPos);
            var f3 = tex.GetPixel(xPos, yPos + 1);
            var f4 = tex.GetPixel(xPos + 1, yPos + 1);

            var xN = Mathf.RoundToInt((sourceCoords.x - xPos) * n);
            var yN = Mathf.RoundToInt((sourceCoords.y - yPos) * n);

            var avg = ((n - xN) * (n - yN) * f1 + xN * (n - yN) * f2 + yN * (n - xN) * f3 + xN * yN * f4) / (n * n);

            return avg;
        }
    }
}

