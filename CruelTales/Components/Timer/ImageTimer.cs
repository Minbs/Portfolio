using CTC.DataBind.Formatter;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace CTC.GUI.Components.Timer
{
	public class ImageTimer : BaseTimer
	{
		[field: SerializeField]
		public Image ImageUI { get; set; }

		[field: SerializeField]
		private TimerFormat _timerFormat;
		public List<Sprite> Sprites;

		protected override void Awake()
		{
			base.Awake();
			Initialize(5);
			_onUpdate = updateTimer;
		}

		private void updateTimer()
		{
			int index = 0;

			switch (_timerFormat)
			{
				case TimerFormat.Decimal:
					index = (int)Math.Round(_currentTime);
					break;
				case TimerFormat.DisplayZero:
					index = (int)Math.Truncate(_currentTime);
					break;

				case TimerFormat.NoDisplayZero:
					index = (int)Math.Ceiling(_currentTime);
					break;
			}


			if (index >= Sprites.Count)
				return;

			ImageUI.sprite = Sprites[index];
		}
	}
}
