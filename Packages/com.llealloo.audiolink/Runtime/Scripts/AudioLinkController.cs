﻿using UnityEngine;
using UnityEngine.UI;

namespace AudioLink
{
    using static Shader;
#if PVR_CCK_WORLDS
    using PVR.PSharp;
    public class AudioLinkController : PSharpBehaviour
#else
    

    public class AudioLinkController : MonoBehaviour
#endif
    {
        [Space(10)]
        public AudioLink audioLink;
        public ControllerSyncMode controllerSyncMode;
        [Space(25)]
        [Header("Internal (Do not modify)")]
        public ThemeColorController themeColorController;
        public Material audioLinkUI;
        public Slider gainSlider;
        public Slider fadeLengthSlider;
        public Slider fadeExpFalloffSlider;
        public Slider x0Slider;
        public Slider x1Slider;
        public Slider x2Slider;
        public Slider x3Slider;
        public Slider threshold0Slider;
        public Slider threshold1Slider;
        public Slider threshold2Slider;
        public Slider threshold3Slider;
        public Toggle autoGainToggle;
        public Toggle powerToggle;

        private float _initGain;
        private float _initTreble;
        private float _initBass;
        private float _initFadeLength;
        private float _initFadeExpFalloff;
        private bool _initAutoGain;
        private float _initX0;
        private float _initX1;
        private float _initX2;
        private float _initX3;
        private float _initThreshold0;
        private float _initThreshold1;
        private float _initThreshold2;
        private float _initThreshold3;

        private RectTransform _threshold0Rect;
        private RectTransform _threshold1Rect;
        private RectTransform _threshold2Rect;
        private RectTransform _threshold3Rect;

        #region PropertyIDs

        // ReSharper disable InconsistentNaming
        private int _X0;
        private int _X1;
        private int _X2;
        private int _X3;
        private int _Threshold0;
        private int _Threshold1;
        private int _Threshold2;
        private int _Threshold3;
        private int _Gain;
        private int _HitFade;
        private int _ExpFalloff;
        private int _AutoGain;
        private int _Power;
        // ReSharper restore InconsistentNaming

        private void InitIDs()
        {
            _X0 = PropertyToID("_X0");
            _X1 = PropertyToID("_X1");
            _X2 = PropertyToID("_X2");
            _X3 = PropertyToID("_X3");
            _Threshold0 = PropertyToID("_Threshold0");
            _Threshold1 = PropertyToID("_Threshold1");
            _Threshold2 = PropertyToID("_Threshold2");
            _Threshold3 = PropertyToID("_Threshold3");
            _Gain = PropertyToID("_Gain");
            _HitFade = PropertyToID("_HitFade");
            _ExpFalloff = PropertyToID("_ExpFalloff");
            _AutoGain = PropertyToID("_AutoGain");
            _Power = PropertyToID("_Power");
        }

        #endregion

        #if PVR_CCK_WORLDS
        ThemeColorController FindThemeColorController()
        {
            Transform controllerTransform = transform.Find("ThemeColorController");
            if (controllerTransform == null) return null;
            return (ThemeColorController)controllerTransform.GetComponent(typeof(ThemeColorController));
        }
        #endif
        
        private void UpdateSyncMode(Transform inputTransform, ControllerSyncMode userSyncMode, ControllerSyncMode desiredSyncMode)
        {
            #if PVR_CCK_WORLDS

                inputTransform.GetComponent<PSharpBehaviour>().enabled = (int)userSyncMode < (int)desiredSyncMode;

            #endif
        }
            public void SetControllerSyncMode(ControllerSyncMode syncMode)
            {

                UpdateSyncMode(gainSlider.transform, syncMode, ControllerSyncMode.ExcludePowerAndGain);
                UpdateSyncMode(fadeLengthSlider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(fadeExpFalloffSlider.transform, syncMode, ControllerSyncMode.None);

                UpdateSyncMode(x0Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(x1Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(x2Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(x3Slider.transform, syncMode, ControllerSyncMode.None);

                UpdateSyncMode(threshold0Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(threshold1Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(threshold2Slider.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(threshold3Slider.transform, syncMode, ControllerSyncMode.None);

                UpdateSyncMode(autoGainToggle.transform, syncMode, ControllerSyncMode.None);
                UpdateSyncMode(powerToggle.transform, syncMode, ControllerSyncMode.ExcludePower);

                themeColorController.networkSynced = (int)syncMode < (int)ControllerSyncMode.None;

            }
        public override void OnNetworkReady()
        {
            if(themeColorController == null)
            {
                Debug.LogError("[AudioLink] AudioLinkController could not find themeColorController");
            }
            else {
                themeColorController.InitializeAudioLinkThemeColors();
            }
        }
        void Start()
        {
            InitIDs();
            if (audioLink == null)
            {
                Debug.LogError("[AudioLink] Controller not connected to AudioLink");
                return;
            }
#if PVR_CCK_WORLDS
            if (themeColorController == null)
            {
                // This is here in case someone upgraded AudioLink, which updates
                // everything in the prefab, but not the outermost properties of the prefab.
                // TODO: Double check that this is how upgrading ends up working.
                Debug.Log("[AudioLink] AudioLinkController using fallback method for finding themeColorController");
                themeColorController = FindThemeColorController();
            }
#endif
            if (themeColorController == null)
            {
                // Something really weird has gone on. maybe using updated script
                // on un-updated prefab?
                Debug.LogError("[AudioLink] AudioLinkController could not find themeColorController");
            }
            else
            {
                themeColorController.audioLink = audioLink;
                themeColorController.audioLinkUI = audioLinkUI;
                //themeColorController.InitializeAudioLinkThemeColors();
            }

            SetControllerSyncMode(controllerSyncMode);

            GetSettings();

            _initGain = gainSlider.value;
            _initFadeLength = fadeLengthSlider.value;
            _initFadeExpFalloff = fadeExpFalloffSlider.value;
            _initAutoGain = autoGainToggle.isOn;
            _initX0 = x0Slider.value;
            _initX1 = x1Slider.value;
            _initX2 = x2Slider.value;
            _initX3 = x3Slider.value;
            _initThreshold0 = threshold0Slider.value;
            _initThreshold1 = threshold1Slider.value;
            _initThreshold2 = threshold2Slider.value;
            _initThreshold3 = threshold3Slider.value;
            _threshold0Rect = threshold0Slider.GetComponent<RectTransform>();
            _threshold1Rect = threshold1Slider.GetComponent<RectTransform>();
            _threshold2Rect = threshold2Slider.GetComponent<RectTransform>();
            _threshold3Rect = threshold3Slider.GetComponent<RectTransform>();

            UpdateSettings();
        }

        private void GetSettings()
        {
            // General settings
            gainSlider.SetValueWithoutNotify(audioLink.gain);
            fadeLengthSlider.SetValueWithoutNotify(audioLink.fadeLength);
            fadeExpFalloffSlider.SetValueWithoutNotify(audioLink.fadeExpFalloff);
            autoGainToggle.SetIsOnWithoutNotify(audioLink.autogain);

            // Crossover Settings
            x0Slider.SetValueWithoutNotify(audioLink.x0);
            x1Slider.SetValueWithoutNotify(audioLink.x1);
            x2Slider.SetValueWithoutNotify(audioLink.x2);
            x3Slider.SetValueWithoutNotify(audioLink.x3);
            threshold0Slider.SetValueWithoutNotify(audioLink.threshold0);
            threshold1Slider.SetValueWithoutNotify(audioLink.threshold1);
            threshold2Slider.SetValueWithoutNotify(audioLink.threshold2);
            threshold3Slider.SetValueWithoutNotify(audioLink.threshold3);

            // Send events
#if PVR_CCK_WORLDS
            //Initialize vars to store components temporarily due to PVR limitations
            var gainSliderVar = (GlobalSlider)gainSlider.GetComponent(typeof(GlobalSlider));
            var fadeLengthSliderVar = (GlobalSlider)fadeLengthSlider.GetComponent(typeof(GlobalSlider));
            var fadeExpFalloffSliderVar = (GlobalSlider)fadeExpFalloffSlider.GetComponent(typeof(GlobalSlider));
            var autoGainToggleVar = (GlobalToggle)autoGainToggle.GetComponent(typeof(GlobalToggle));
            var x0SliderVar = (GlobalSlider)x0Slider.GetComponent(typeof(GlobalSlider));
            var x1SliderVar = (GlobalSlider)x1Slider.GetComponent(typeof(GlobalSlider));
            var x2SliderVar = (GlobalSlider)x2Slider.GetComponent(typeof(GlobalSlider));
            var x3SliderVar = (GlobalSlider)x3Slider.GetComponent(typeof(GlobalSlider));
            var threshold0SliderVar = (GlobalSlider)threshold0Slider.GetComponent(typeof(GlobalSlider));
            var threshold1SliderVar = (GlobalSlider)threshold1Slider.GetComponent(typeof(GlobalSlider));
            var threshold2SliderVar = (GlobalSlider)threshold2Slider.GetComponent(typeof(GlobalSlider));
            var threshold3SliderVar = (GlobalSlider)threshold3Slider.GetComponent(typeof(GlobalSlider));

            gainSliderVar.SlideUpdate();
            fadeLengthSliderVar.SlideUpdate();
            fadeExpFalloffSliderVar.SlideUpdate();
            autoGainToggleVar.ToggleUpdate();
            x0SliderVar.SlideUpdate();
            x1SliderVar.SlideUpdate();
            x2SliderVar.SlideUpdate();
            x3SliderVar.SlideUpdate();
            threshold0SliderVar.SlideUpdate();
            threshold1SliderVar.SlideUpdate();
            threshold2SliderVar.SlideUpdate();
            threshold3SliderVar.SlideUpdate();
#endif
        }

        public void UpdateSettings()
        {
            // Update Sliders
            Vector2 anchor0 = new Vector2(x0Slider.value, 1f);
            Vector2 anchor1 = new Vector2(x1Slider.value, 1f);
            Vector2 anchor2 = new Vector2(x2Slider.value, 1f);
            Vector2 anchor3 = new Vector2(x3Slider.value, 1f);
            if (_threshold0Rect != null) _threshold0Rect.anchorMin = anchor0;
            if (_threshold0Rect != null) _threshold0Rect.anchorMax = anchor1;
            if (_threshold1Rect != null) _threshold1Rect.anchorMin = anchor1;
            if (_threshold1Rect != null) _threshold1Rect.anchorMax = anchor2;
            if (_threshold2Rect != null) _threshold2Rect.anchorMin = anchor2;
            if (_threshold2Rect != null) _threshold2Rect.anchorMax = anchor3;
            if (_threshold3Rect != null) _threshold3Rect.anchorMin = anchor3;
            // threshold3Rect.anchorMax is a constant value. Skip

            audioLinkUI.SetFloat(_X0, x0Slider.value);
            audioLinkUI.SetFloat(_X1, x1Slider.value);
            audioLinkUI.SetFloat(_X2, x2Slider.value);
            audioLinkUI.SetFloat(_X3, x3Slider.value);
            audioLinkUI.SetFloat(_Threshold0, threshold0Slider.value);
            audioLinkUI.SetFloat(_Threshold1, threshold1Slider.value);
            audioLinkUI.SetFloat(_Threshold2, threshold2Slider.value);
            audioLinkUI.SetFloat(_Threshold3, threshold3Slider.value);
            audioLinkUI.SetFloat(_Gain, gainSlider.value);
            audioLinkUI.SetFloat(_HitFade, fadeLengthSlider.value);
            audioLinkUI.SetFloat(_ExpFalloff, fadeExpFalloffSlider.value);
            audioLinkUI.SetInt(_AutoGain, autoGainToggle.isOn ? 1 : 0);
            audioLinkUI.SetInt(_Power, powerToggle.isOn ? 1 : 0);

            if (audioLink == null)
            {
                Debug.LogError("[AudioLink] Controller not connected to AudioLink");
                return;
            }
            // General settings
            audioLink.gain = gainSlider.value;
            audioLink.fadeLength = fadeLengthSlider.value;
            audioLink.fadeExpFalloff = fadeExpFalloffSlider.value;
            audioLink.autogain = autoGainToggle.isOn;

            // Crossover settings
            audioLink.x0 = x0Slider.value;
            audioLink.x1 = x1Slider.value;
            audioLink.x2 = x2Slider.value;
            audioLink.x3 = x3Slider.value;
            audioLink.threshold0 = threshold0Slider.value;
            audioLink.threshold1 = threshold1Slider.value;
            audioLink.threshold2 = threshold2Slider.value;
            audioLink.threshold3 = threshold3Slider.value;

            audioLink.UpdateSettings();

            // Toggle
            audioLink.SetAudioLinkState(powerToggle.isOn);
        }

        public void ResetSettings()
        {
            gainSlider.value = _initGain;
            fadeLengthSlider.value = _initFadeLength;
            fadeExpFalloffSlider.value = _initFadeExpFalloff;
            autoGainToggle.isOn = _initAutoGain;
            x0Slider.value = _initX0;
            x1Slider.value = _initX1;
            x2Slider.value = _initX2;
            x3Slider.value = _initX3;
            threshold0Slider.value = _initThreshold0;
            threshold1Slider.value = _initThreshold1;
            threshold2Slider.value = _initThreshold2;
            threshold3Slider.value = _initThreshold3;
            if (themeColorController != null)
            {
                themeColorController.ResetThemeColors();
            }
        }


        private float Remap(float t, float a, float b, float u, float v)
        {
            return ((t - a) / (b - a)) * (v - u) + u;
        }
        
    }

    public enum ControllerSyncMode {
        All = 0,
        ExcludePower = 1,
        ExcludePowerAndGain = 2,
        None = 3
    }
    
}
