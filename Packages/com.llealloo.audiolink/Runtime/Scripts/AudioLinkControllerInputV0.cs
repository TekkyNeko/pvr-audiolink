using UnityEngine;
using PVR.PSharp;
using UnityEngine.UI;

namespace AudioLink
{
	public class AudioLinkControllerInputV0 : PSharpBehaviour
	{
		public AudioLink audioLink;
		public ThemeColorControllerV0 themeColorController;
		public AudioLinkControllerV0 audioLinkController;

		public Dropdown ThemeColorDropdown;
		public Button CustomColorButton0;
		public Button CustomColorButton1;
		public Button CustomColorButton2;
		public Button CustomColorButton3;
		public Slider Gain;
		public Slider Treble;
		public Slider Bass;
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
		public Button Reset;
		public Slider ThemeHue;
		public Slider ThemeSaturation;
		public Slider ThemeValue;

		private void Awake()
		{
			ThemeColorDropdown.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
			});

			CustomColorButton0.onClick.AddListener(() => {
				themeColorController.SelectCustomColor0();
			});

			CustomColorButton1.onClick.AddListener(() => {
				themeColorController.SelectCustomColor1();
			});

			CustomColorButton2.onClick.AddListener(() => {
				themeColorController.SelectCustomColor2();
			});

			CustomColorButton3.onClick.AddListener(() => {
				themeColorController.SelectCustomColor3();
			});

			Gain.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Gain.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Treble.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Treble.GetComponent(typeof(GlobalSlider));
				globalSlider.SlideUpdate();
				audioLinkController.UpdateSettings();
			});

			Bass.onValueChanged.AddListener((val) => {
				var globalSlider = (GlobalSlider)Bass.GetComponent(typeof(GlobalSlider));
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
			
			Reset.onClick.AddListener(() => {
				audioLinkController.ResetSettings();
			});

			ThemeHue.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
			});

			ThemeSaturation.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
			});

			ThemeValue.onValueChanged.AddListener((val) => {
				themeColorController.OnGUIchange();
			});
		}
	}
}