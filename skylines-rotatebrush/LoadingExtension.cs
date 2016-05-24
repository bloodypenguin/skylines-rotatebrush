using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

namespace RotateBrushImproved
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private static SavedInputKey _keyRotateRight;
        private static SavedInputKey _keyRotateLeft;
        private static RotateBrush _rotateBrush;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            _keyRotateRight = new SavedInputKey(Settings.mapEditorIncreaseBrushSize, Settings.inputSettingsFile, DefaultSettings.mapEditorIncreaseBrushSize, true);
            _keyRotateLeft = new SavedInputKey(Settings.mapEditorDecreaseBrushSize, Settings.inputSettingsFile, DefaultSettings.mapEditorDecreaseBrushSize, true);
            UIInput.eventProcessKeyEvent += HandleRotation;
            _rotateBrush = new RotateBrush();
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();
            UIInput.eventProcessKeyEvent -= HandleRotation;
            _rotateBrush = null;
            base.OnLevelUnloading();
        }

        private static void HandleRotation(EventType eventType, KeyCode keyCode, EventModifiers modifiers)
        {
            if (eventType != EventType.KeyDown || modifiers != EventModifiers.Control)
            {
                return;
            }
            if (keyCode == _keyRotateLeft.Key)
            {
                _rotateBrush.RotateLeft();
            }
            if (keyCode == _keyRotateRight.Key)
            {
                _rotateBrush.RotateRight();
            }
        }


    }
}

