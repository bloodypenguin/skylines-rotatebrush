using System;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace skylinesrotatebrush
{
    public class RotateBrushLoader : LoadingExtensionBase
    {
        SavedInputKey keyRotateRight;
        SavedInputKey keyRotateLeft;

        RotateBrush rotateBrush;

        public override void OnLevelLoaded (LoadMode mode)
        {
            if (mode != LoadMode.LoadMap && mode != LoadMode.NewMap) {
                return;
            }

            keyRotateRight = new SavedInputKey (Settings.cameraRotateRight, Settings.gameSettingsFile, DefaultSettings.cameraRotateRight, false);
            keyRotateLeft = new SavedInputKey (Settings.cameraRotateLeft, Settings.gameSettingsFile, DefaultSettings.cameraRotateLeft, false);

            UIInput.eventProcessKeyEvent += handleRotation;
            rotateBrush = new RotateBrush();
        }

        void handleRotation (UnityEngine.EventType eventType, UnityEngine.KeyCode keyCode, UnityEngine.EventModifiers modifiers)
        {
            if (eventType == EventType.KeyDown && modifiers == EventModifiers.Control) {
                if (keyCode == keyRotateLeft.Key) {
                    rotateBrush.rotateLeft();
                }
                if (keyCode == keyRotateRight.Key) {
                    rotateBrush.rotateRight();
                }
            }
        }

        public override void OnLevelUnloading ()
        {
            UIInput.eventProcessKeyEvent -= handleRotation;
            rotateBrush = null;
            base.OnLevelUnloading ();
        }
    }
}

