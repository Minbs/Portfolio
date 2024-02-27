using UnityEngine;
using TMPro;
using CTC.DataBind.Formatter;
using System;

namespace CTC.GUI.Components.Timer
{
	public class TextTimer : BaseTimer
	{
		[field: SerializeField]
		public TextMeshProUGUI TextUI { get; private set; }

		[field: SerializeField]
		private TimerFormat _timerFormat;

		[field: SerializeField]
		private int _digit = 0;

		protected override void Awake()
		{
			base.Awake();
			Initialize(5);
			_onUpdate = () =>
			{
				switch (_timerFormat)
				{
					case TimerFormat.Decimal:
						TextUI.text = Math.Round(_currentTime, _digit).ToString();
						break;

					case TimerFormat.DisplayZero:
						TextUI.text = Math.Truncate(_currentTime).ToString();
						break;

					case TimerFormat.NoDisplayZero:
						TextUI.text = Math.Ceiling(_currentTime).ToString();
						break;
				}
			};
		}
	}
}
