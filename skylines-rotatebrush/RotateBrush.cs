using System;
using UnityEngine;

namespace skylinesrotatebrush
{
    public class RotateBrush
    {
        private int angle = 15;
        private Texture2D currentBrush;
        private Texture2D originalBrush;
        private int dstAngle;

        public RotateBrush ()
        {
            originalBrush = new Texture2D (0, 0);
        }
        
        public void rotateLeft ()
        {
            rotate (-angle);
        }

        public void rotateRight ()
        {
            rotate (angle);
        }

        private void rotate (int angle)
        {
            try {
                currentBrush = getCurrentBrush ();
                if (currentBrush == null) {
                    return;
                }

                if (!originalBrush.name.Equals (currentBrush.name)) {
                    // brush has changed
                    dstAngle = 0;
                    originalBrush = currentBrush;
                }
                dstAngle += (angle + 360);
                dstAngle = (dstAngle % 360);

                Texture2D nextBrush = rotateTexture (originalBrush, dstAngle);
                setBrush (nextBrush);
            } catch (Exception ex) {
                DebugOutputPanel.AddMessage (ColossalFramework.Plugins.PluginManager.MessageType.Error, "RotateBrush Exception: " + ex.ToString ());
            }
        }

        private Texture2D getCurrentBrush ()
        {
            TerrainTool terrainTool = ToolsModifierControl.GetTool<TerrainTool> ();
            if (terrainTool != null) {
                return terrainTool.m_brush;
            }
            TreeTool treeTool = ToolsModifierControl.GetTool<TreeTool> ();
            if (treeTool != null) {
                return treeTool.m_brush;
            }
            ResourceTool resourceTool = ToolsModifierControl.GetTool<ResourceTool> ();
            if (resourceTool != null) {
                return resourceTool.m_brush;
            }
            PropTool propTool = ToolsModifierControl.GetTool<PropTool> ();
            if (propTool != null) {
                return propTool.m_brush;
            }
            return null;
        }

        private void setBrush (Texture2D nextBrush)
        {
            TerrainTool terrainTool = ToolsModifierControl.GetTool<TerrainTool> ();
            if (terrainTool != null) {
                terrainTool.m_brush = nextBrush;
            }
            TreeTool treeTool = ToolsModifierControl.GetTool<TreeTool> ();
            if (treeTool != null) {
                treeTool.m_brush = nextBrush;
            }
            ResourceTool resourceTool = ToolsModifierControl.GetTool<ResourceTool> ();
            if (resourceTool != null) {
                resourceTool.m_brush = nextBrush;
            }
            PropTool propTool = ToolsModifierControl.GetTool<PropTool> ();
            if (propTool != null) {
                propTool.m_brush = nextBrush;
            }
        }

        private Texture2D rotateTexture (Texture2D src, int angle)
        {
            Texture2D dst = new Texture2D (src.width, src.height);
            
            src.wrapMode = TextureWrapMode.Clamp;

            Vector3 pivot = new Vector3 (dst.width / 2 - 0.5f, dst.height / 2 - 0.5f, 0);
            Vector3 scale = Vector3.one;
            Quaternion rotation = Quaternion.Euler (0, 0, angle);
            
            Matrix4x4 translateToPivot = Matrix4x4.TRS (-1f * pivot, Quaternion.identity, scale);
            Matrix4x4 rotate = Matrix4x4.TRS (Vector3.zero, rotation, Vector3.one);
            Matrix4x4 translateBack = Matrix4x4.TRS (pivot, Quaternion.identity, scale);

            Vector3 srcCoords;
            for (int x = 0; x < dst.width; x++) {
                for (int y = 0; y < dst.height; y++) {                    
                    srcCoords = new Vector3 (x, y, 0);

                    srcCoords = translateToPivot.MultiplyPoint3x4 (srcCoords);
                    srcCoords = rotate.MultiplyPoint3x4 (srcCoords);
                    srcCoords = translateBack.MultiplyPoint3x4 (srcCoords);

                    Color dstColor = avgPixelValue (src, srcCoords);
                    dst.SetPixel (x, y, dstColor);
                }
            }
            dst.Apply ();            
            return dst;            
        }

        // http://www.leptonica.com/rotation.html
        private Color avgPixelValue (Texture2D tex, Vector3 sourceCoords)
        {
            int N = 10;
            int xPos = (int)sourceCoords.x;
            int yPos = (int)sourceCoords.y;
            
            Color f1 = tex.GetPixel (xPos, yPos);
            Color f2 = tex.GetPixel (xPos + 1, yPos);
            Color f3 = tex.GetPixel (xPos, yPos + 1);
            Color f4 = tex.GetPixel (xPos + 1, yPos + 1);
            
            int xN = Mathf.RoundToInt ((sourceCoords.x - xPos) * N);
            int yN = Mathf.RoundToInt ((sourceCoords.y - yPos) * N);

            Color avg = ((N - xN) * (N - yN) * f1 + xN * (N - yN) * f2 + yN * (N - xN) * f3 + xN * yN * f4) / (N * N);
            
            return avg;
        }
    }
}

