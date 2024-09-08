#if PVR_CCK_WORLDS
using UnityEngine;
using PVR.PSharp;
using UnityEngine.UI;

namespace AudioLink
{
	public class AudioLinkControllerInput : PSharpBehaviour
	{
		public AudioLink audioLink;
		public ThemeColorController themeColorController;
		public AudioLinkController audioLinkController;
		public Button CustomColorButton3;
		public Button CustomColorButton2;
		public Button CustomColorButton1;
		public Button CustomColorButton0;
		public Toggle ThemeColorToggle;
		public Slider Gain;
		public Slider FadeLength;
		public Slider FadeExpFalloff;
		public Slider SliderX0;
		public Slider SliderX1;
		public Slider SliderX2;
		public Slider SliderX3;
		public Slider Threshold0;
		public Slider Threshold1;
		public Slider Threshold2;
		public Slider Threshold3;
		public Slider Hue;
		public Slider Saturation;
		public Slider Value;
		public Toggle AutoGainToggle;
		public Button ResetButton;
		public Toggle PowerButton;

		private void Awake()
		{
			
			CustomColorButton3.onClick.AddListener(() => {
				themeColorController.SelectCustomColor3();
				themeColorController.ForceThemeColorMode();
			});

			CustomColorButton2.onClick.AddListener(() => {
				themeColorController.SelectCustomColor2();
				themeColorController.ForceThemeColorMode();
			});

			CustomColorButton2.onClick.AddListener(() => {
				themeColorController.SelectCustomColor2();
				themeColorController.ForceThemeColorMode();
			});
			
			CustomColorButton1.onClick.AddListener(() => {
				themeColorController.SelectCustomColor1();
				themeColorController.ForceThemeColorMode();
			});

			CustomColorButton0.onClick.AddListener(() => {
				themeColorController.SelectCustomColor0();
				themeColorController.ForceThemeColorMode();
			});

			ThemeColorToggle.onValueChanged.AddListener((val) => {
				themeColorController.ToggleThemeColorMode();
			});

			Gain.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Gain.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			FadeLength.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)FadeLength.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			FadeExpFalloff.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)FadeExpFalloff.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			SliderX0.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)SliderX0.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			SliderX1.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)SliderX1.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			SliderX2.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)SliderX2.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			SliderX3.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)SliderX3.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Threshold0.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Threshold0.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Threshold1.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Threshold1.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Threshold2.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Threshold2.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Threshold3.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Threshold3.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Hue.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
				themeColorController.ForceThemeColorMode();
			});

			Saturation.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
				themeColorController.ForceThemeColorMode();
			});

			Value.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
				themeColorController.ForceThemeColorMode();
			});

			AutoGainToggle.onValueChanged.AddListener((val) => {
				audioLinkController.UpdateSettings();
				var globalToggle = (GlobalToggle)AutoGainToggle.GetComponent(typeof(GlobalToggle));
				globalToggle.ToggleUpdate();
			});

			ResetButton.onClick.AddListener(() => {
				audioLinkController.ResetSettings();
			});

			PowerButton.onValueChanged.AddListener((val) => {
				audioLinkController.UpdateSettings();
				var globalToggle = (GlobalToggle)PowerButton.GetComponent(typeof(GlobalToggle));
				globalToggle.ToggleUpdate();
			});
		}
	}
}
#endif