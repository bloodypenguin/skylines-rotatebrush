using System;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace skylinesrotatebrush
{
    public class RotateBrushLoader : LoadingExtensionBase
    {
        SavedInputKey rotateRight;
        SavedInputKey rotateLeft;

        public override void OnLevelLoaded (LoadMode mode)
        {
            if (mode != LoadMode.LoadMap && mode != LoadMode.NewMap) {
                return;
            }

            rotateRight = new SavedInputKey (Settings.cameraRotateRight, Settings.gameSettingsFile, DefaultSettings.cameraRotateRight, false);
            rotateLeft = new SavedInputKey (Settings.cameraRotateLeft, Settings.gameSettingsFile, DefaultSettings.cameraRotateLeft, false);

            UIInput.eventProcessKeyEvent += handleRotation;
        }

        void handleRotation (UnityEngine.EventType eventType, UnityEngine.KeyCode keyCode, UnityEngine.EventModifiers modifiers)
        {
            if (eventType == EventType.KeyDown && modifiers == EventModifiers.Control) {
                if (keyCode == rotateLeft.Key) {
                    RotateBrush.rotateLeft ();
                }
                if (keyCode == rotateRight.Key) {
                    RotateBrush.rotateRight ();
                }
            }
        }

        public override void OnLevelUnloading ()
        {
            UIInput.eventProcessKeyEvent -= handleRotation;
            base.OnLevelUnloading ();
        }
    }
}

