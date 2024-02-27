using System.Collections.Generic;
using System.Linq;
using CTC.Globalizations;
using CTC.GUI.Components;
using CTC.GUI.Option;
using Slash.Unity.DataBind.Core.Data;
using Slash.Unity.DataBind.Core.Presentation;
using Slash.Unity.DataBind.Scripts.Customs;
using TMPro;
using UnityEngine;

namespace CTC
{
	public class VSyncOption
	{
		public TextKey OptionName;
		public int VSyncValue;

		public VSyncOption(TextKey optionName, int vSyncValue)
		{
			OptionName = optionName;
			VSyncValue = vSyncValue;
		}

		public override string ToString()
		{
			return OptionName.GetText();
		}
	}

	public class DisplayOption : MonoBehaviour
    {
		enum ScreenMode
		{
			FullScreen,
			Borderless,
			Windowed
		}





		//public TMP_Dropdown ResolutionDropdown;
		private Context_DisplayOption _context_DisplayOption;
		private Resolution[] _resolutions;
		private Resolution _selectedResolution;

		public TMP_Dropdown FullScreenDropdown;
		private FullScreenMode _fullScreenMode;

		public TMP_Dropdown QualityDropdown;
		private int _qualityIndex = 0;

		public Pagination vSync;
		private readonly List<object> _vSyncOptions = new()
		{
			new VSyncOption(TextKey.OptionParameter_VSyncOff, 0),
			new VSyncOption(TextKey.OptionParameter_VSyncOn_0, 1),
			new VSyncOption(TextKey.OptionParameter_VSyncOn_1, 2),
		};

		public Pagination FrameRate;
		private readonly List<object> _frameRateOptions = new()
		{
			30, 60, 144,
		};

		public void Init()
		{
			_resolutions = Screen.resolutions;
			var resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct();
			_resolutions = resolutions.OrderBy(resolution => resolution.width).ToArray();

			_selectedResolution = Screen.currentResolution;
	
			//	ResolutionDropdown.ClearOptions();
			List<string> options = new();

			int currentResolutionIndex = 0;
			for (int i = 0; i < _resolutions.Length; i++)
			{
				string option = _resolutions[i].width + " x " + _resolutions[i].height;
				_context_DisplayOption.ResolutionsProperty.Add(new ContextValueString(option));
				//options.Add(option);

				if (_resolutions[i].width == Screen.currentResolution.width
					&& _resolutions[i].height == Screen.currentResolution.height)
				{
					currentResolutionIndex = i;

				}
			}

			_context_DisplayOption.ResolutionsProperty.RemoveAt(0);
			_fullScreenMode = Screen.fullScreenMode;

			vSync.Initialize(_vSyncOptions);
			FrameRate.Initialize(_frameRateOptions);
		}
		public void SetResolution(int resolutionIndex)
		{
			_selectedResolution = _resolutions[resolutionIndex];
		}

		public void SetFullScreenMode(int fullScreenIndex)
		{
			if (fullScreenIndex == (int)ScreenMode.FullScreen)
			{
				if (Application.platform == RuntimePlatform.WindowsPlayer)
				{
					_fullScreenMode = FullScreenMode.ExclusiveFullScreen;
				}
				else if (Application.platform == RuntimePlatform.OSXPlayer)
				{
					_fullScreenMode = FullScreenMode.MaximizedWindow;
				}
			}
			else if(fullScreenIndex == (int)ScreenMode.Borderless)
			{
				_fullScreenMode = FullScreenMode.FullScreenWindow;
			}
			else if(fullScreenIndex == (int)ScreenMode.Windowed)
			{
				_fullScreenMode = FullScreenMode.Windowed;
			}
		}

		public void SetQuality(int qualityIndex)
		{
			_qualityIndex = qualityIndex;
		}

		public int GetFullScreenModeIndex(FullScreenMode fullScreenMode)
		{
			if (fullScreenMode == FullScreenMode.MaximizedWindow || fullScreenMode == FullScreenMode.ExclusiveFullScreen) return (int)ScreenMode.FullScreen;
			else if (fullScreenMode == FullScreenMode.FullScreenWindow) return (int)ScreenMode.Borderless;
			else if (fullScreenMode == FullScreenMode.Windowed) return (int)ScreenMode.Windowed;
			else return -1;
		}

		public void Apply()
		{
			Screen.SetResolution(_selectedResolution.width, _selectedResolution.height, Screen.fullScreen);
			Screen.fullScreenMode = _fullScreenMode;
			QualitySettings.SetQualityLevel(_qualityIndex);

			// VSync
			var curVSyncOption = vSync.GetCurrentOption<VSyncOption>();
			QualitySettings.vSyncCount = curVSyncOption.VSyncValue;

			// Frame Rate
			Application.targetFrameRate = FrameRate.GetCurrentOption<int>();
		}
	}
}