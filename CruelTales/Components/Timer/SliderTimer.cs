using UnityEngine;
using UnityEngine.UI;

namespace CTC.GUI.Components.Timer
{
	public class SliderTimer : BaseTimer
	{
		[field : SerializeField]
		public Slider SliderUI { get; set; }

		protected override void Awake()
		{
			base.Awake();
			_onUpdate = updateTimer;
		}

		public override void Initialize(float startTime, float endTime = 0.001f)
		{
			base.Initialize(startTime, endTime);

			if(startTime > endTime)
			{
				SliderUI.maxValue = startTime;
				SliderUI.minValue = endTime;
			}
			else
			{
				SliderUI.maxValue = endTime;
				SliderUI.minValue = startTime;
			}
		}

		private void updateTimer()
		{
			SliderUI.value = _currentTime;
		}
	}
}
