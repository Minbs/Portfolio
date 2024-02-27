using CTC.DataBind.Formatter;
using UnityEngine;
using UnityEngine.UI;

namespace CTC.GUI.Components.Timer
{
	public class ClockTimer : BaseTimer
	{
		[field: SerializeField]
		public Image FillImage { get; set; }

		[field: SerializeField]
		public Transform ClockHandTrnasform { get; set; }
		protected override void Awake()
		{
			base.Awake();

			Initialize(5);
			_onUpdate = () =>
			{
				if (_currentTime < 0)
				{
					FillImage.fillAmount = 1;
					ClockHandTrnasform.localEulerAngles = Vector3.zero;
					return;
				}

				float timeRatio = _currentTime / _startTime;
				FillImage.fillAmount = 1 - timeRatio;
				float angle = Mathf.Lerp(0, 360, timeRatio);
				ClockHandTrnasform.localEulerAngles = new Vector3(0, 0, angle);
			};
		}
	}
}
