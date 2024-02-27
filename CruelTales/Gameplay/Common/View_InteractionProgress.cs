using System.Collections;
using CT.Common.Gameplay;
using CTC.DataBind.Contexts;
using CTC.Globalizations;
using CTC.GUI.Components.Timer;
using CTC.Utils.Coroutines;
using Slash.Unity.DataBind.Core.Data;
using TMPro;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common
{
	public class Context_InteractionProgress : ContextWithView<View_InteractionProgress>
	{
		public Property<string> interactionInfoProperty = new();
		public string InteractionInfo
		{
			get => interactionInfoProperty.Value;
			set => interactionInfoProperty.Value = value;
		}
	}

	public class View_InteractionProgress : ViewBaseWithContext
	{
		[SerializeField]
		private GameObject _layoutObject;

		private Context_InteractionProgress BindedContext;

		public SliderTimer Slider;
		public GameObject Text;

		public CoroutineRunner _infoTextRunner;

		protected override void Awake()
		{
			base.Awake();
			_infoTextRunner = new CoroutineRunner(this);
		}

		protected override void onBeginShow()
		{
			base.onBeginShow();
			BindedContext = CurrentContext as Context_InteractionProgress;
			BindedContext.InteractionInfo = string.Empty;
		}

		/// <summary>
		/// ProgressBar 끄기
		/// </summary>
		public void ProgressBarConcealed()
		{
			Slider.StopTimer();
			Slider.gameObject.SetActive(false);
			Text.gameObject.SetActive(false);
		}

		public void OnInteractResult(InteractResultType result,
									 float ProgressTime,
									 TextKey interactionInfoText)
		{
			_infoTextRunner.Stop();
			string interactionInfo = result.GetText();

			if (result.IsSuccess())
			{
				interactionInfo = interactionInfoText.GetText();

				switch (result)
				{
					case InteractResultType.Success:
						ProgressBarConcealed();
						break;

					case InteractResultType.Success_Waitting:
						Slider.gameObject.SetActive(true);
						Text.gameObject.SetActive(true);
						Slider.Initialize(0, ProgressTime);
						break;

					case InteractResultType.Success_Start:
						Slider.gameObject.SetActive(true);
						Text.gameObject.SetActive(true);
						Slider.Initialize(0, ProgressTime);
						Slider.StartTimer();
						break;

					case InteractResultType.Success_Finished:
						Slider.gameObject.SetActive(false);
						Text.gameObject.SetActive(false);

						break;

					default:
						Debug.Assert(false);
						break;
				}
			}
			else
			{
 				Slider.gameObject.SetActive(false);
				Slider.SliderUI.value = 0;

				switch (result)
				{
					case InteractResultType.Failed:
					case InteractResultType.Failed_WrongRequest:
					case InteractResultType.Failed_Canceled:
					case InteractResultType.Failed_Disabled:
					case InteractResultType.Failed_SomeoneIsInteracting:
					case InteractResultType.Failed_YouAlreadyHaveItem:
					case InteractResultType.Failed_ItemLimit:
						_infoTextRunner.Start(showInfoText(3, 1));
						break;
					case InteractResultType.Failed_Cooltime:
					case InteractResultType.Failed_YouMoved:
					case InteractResultType.Failed_WrongPosition:
						interactionInfo = string.Empty;
						Text.gameObject.SetActive(false);
						break;
				}
			}

			BindedContext.InteractionInfo = interactionInfo;
		}

		private IEnumerator showInfoText(float showTime, float fadeDuration)
		{
			Text.gameObject.SetActive(true);
			yield return CoroutineCache.GetWaitForSecondsRealTime(showTime);
			Text.gameObject.SetActive(false);
		}
	}
}