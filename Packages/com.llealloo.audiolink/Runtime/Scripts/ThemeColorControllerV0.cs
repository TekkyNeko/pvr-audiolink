﻿using UnityEngine;
using UnityEngine.UI;
using System;

namespace AudioLink
{
#if PVR_CCK_WORLDS
    using PVR.PSharp;

    public class ThemeColorControllerV0 : PSharpBehaviour
#else
    public class ThemeColorControllerV0 : MonoBehaviour
#endif
    {
        [PSharpSynced(SyncType.Manual)] private int _themeColorMode;

        [Obsolete("This array will return a copy of the data, causing it to not write any data when trying to write directly to a index. Use " + nameof(GetCustomThemeColors) + " and " + nameof(SetCustomThemeColors) + " instead.", false)]
        public Color[] customThemeColors
        {
            get
            {
                return GetCustomThemeColors();
            }
            set
            {
                SetCustomThemeColors(value);
            }
        }

        [PSharpSynced(SyncType.Manual)]
        public Color themeColor1 = Color.yellow;
        [PSharpSynced(SyncType.Manual)]
        public Color themeColor2 = Color.blue;
        [PSharpSynced(SyncType.Manual)]
        public Color themeColor3 = Color.red;
        [PSharpSynced(SyncType.Manual)]
        public Color themeColor4 = Color.green;

        public AudioLink audioLink; // Initialized by AudioLinkController.

        public Dropdown themeColorDropdown; // Initialized from prefab.

        private Color[] _initCustomThemeColors;
        private int _initThemeColorMode; // Initialized from themeColorDropdown.

        private bool _processGUIEvents = true;

#if PVR_CCK_WORLDS
        private PSharpPlayer localPlayer;
#endif

        // A view-controller for customThemeColors
        public Transform extensionCanvas;
        public Slider sliderHue;
        public Slider sliderSaturation;
        public Slider sliderValue;
        public Transform[] customColorLassos;
        public int customColorIndex = 0;

        private void Start()
        {
#if PVR_CCK_WORLDS
            localPlayer = PSharpPlayer.LocalPlayer;
#endif

            _initCustomThemeColors = GetCustomThemeColors();
        }

#if PVR_CCK_WORLDS
        public override void OnDeserialization()
        {
            UpdateGUI();
            UpdateAudioLinkThemeColors();
        }
#endif
        
        /// <summary>
        /// Get a copy of the custom theme colors.
        /// </summary>
        /// <returns> An array of 4 colors. </returns>
        public Color[] GetCustomThemeColors()
        {
            return new[] {
                        themeColor1,
                        themeColor2,
                        themeColor3,
                        themeColor4
                    };
        }

        /// <summary>
        /// Set the custom theme colors. Passing an array with less than 4 colors will only set the provided slots, the rest will remain unchanged. Sending an array with more than 4 colors will only set the first 4 colors.
        /// </summary>
        /// <param name="colors"> An array of colors. </param>
        public void SetCustomThemeColors(params Color[] colors)
        {
            Color[] safeColorArray = GetCustomThemeColors();

            for (int i = 0; i < colors.Length; ++i)
            {
                if (i < safeColorArray.Length)
                {
                    safeColorArray[i] = colors[i];
                }
            }

            themeColor1 = safeColorArray[0];
            themeColor2 = safeColorArray[1];
            themeColor3 = safeColorArray[2];
            themeColor4 = safeColorArray[3];
        }
        
        public void SelectCustomColor0() { SelectCustomColorN(0); }
        public void SelectCustomColor1() { SelectCustomColorN(1); }
        public void SelectCustomColor2() { SelectCustomColorN(2); }
        public void SelectCustomColor3() { SelectCustomColorN(3); }
        public void SelectCustomColorN(int n)
        {
            customColorIndex = n;
            UpdateGUI();
        }

        public void OnGUIchange()
        {
            if (!_processGUIEvents)
            {
                return;
            }
#if PVR_CCK_WORLDS
            if (!IsOwner)
                PSharpNetworking.SetOwner(localPlayer, gameObject);
#endif
            bool modeChanged = _themeColorMode != themeColorDropdown.value;
            _themeColorMode = themeColorDropdown.value;
            Color[] themeColors = GetCustomThemeColors();
            themeColors[customColorIndex] = Color.HSVToRGB(
                sliderHue.value,
                sliderSaturation.value,
                sliderValue.value
            );
            SetCustomThemeColors(themeColors);

            if (modeChanged) UpdateGUI();
            UpdateAudioLinkThemeColors();
#if PVR_CCK_WORLDS
            Sync("_themeColorMode");
            Sync("themeColor1");
            Sync("themeColor2");
            Sync("themeColor3");
            Sync("themeColor4");
#endif
        }

        public void ResetThemeColors()
        {
            _themeColorMode = _initThemeColorMode;
            SetCustomThemeColors(_initCustomThemeColors);
            UpdateGUI();
            UpdateAudioLinkThemeColors();
#if PVR_CCK_WORLDS
            Sync("_themeColorMode");
            Sync("themeColor1");
            Sync("themeColor2");
            Sync("themeColor3");
            Sync("themeColor4");
#endif
        }

        public void UpdateGUI()
        {
            _processGUIEvents = false;
            themeColorDropdown.value = _themeColorMode;

            bool isCustom = _themeColorMode == 1;
            extensionCanvas.gameObject.SetActive(isCustom);
            for (int i = 0; i < 4; ++i)
            {
                customColorLassos[i].gameObject.SetActive(
                    i == customColorIndex
                );
            }

            // update HSV sliders
            float h, s, v;
            Color[] customThemeColors = GetCustomThemeColors();
            Color.RGBToHSV(customThemeColors[customColorIndex], out h, out s, out v);
            sliderHue.value = h;
            sliderSaturation.value = s;
            sliderValue.value = v;

            _processGUIEvents = true;
        }

        public void InitializeAudioLinkThemeColors()
        {
            if (audioLink == null) return;
            Color[] customThemeColors = GetCustomThemeColors();
            customThemeColors[0] = audioLink.customThemeColor0;
            customThemeColors[1] = audioLink.customThemeColor1;
            customThemeColors[2] = audioLink.customThemeColor2;
            customThemeColors[3] = audioLink.customThemeColor3;

            //shallow copy of the array
            _initCustomThemeColors = GetCustomThemeColors();

            _initThemeColorMode = audioLink.themeColorMode;
            _themeColorMode = _initThemeColorMode;

            UpdateGUI();
            UpdateAudioLinkThemeColors();
#if PVR_CCK_WORLDS
            if (IsOwner)
                Sync("_themeColorMode");
                Sync("themeColor1");
                Sync("themeColor2");
                Sync("themeColor3");
                Sync("themeColor4");
#endif
        }

        public void UpdateAudioLinkThemeColors()
        {
            if (audioLink == null) return;
            audioLink.themeColorMode = _themeColorMode;
            Color[] customThemeColors = GetCustomThemeColors();
            audioLink.customThemeColor0 = customThemeColors[0];
            audioLink.customThemeColor1 = customThemeColors[1];
            audioLink.customThemeColor2 = customThemeColors[2];
            audioLink.customThemeColor3 = customThemeColors[3];
            audioLink.UpdateThemeColors();
        }
    }
}
