using System;
using System.Collections;
using CTC.Utils.Coroutines;
using TMPro;
namespace CTC.GUI.Gameplay.Common
{
	public class View_MiniGameTimer : ViewBase
	{
		private CoroutineRunner _testRunner;
		public TextMeshProUGUI RemainTimeText;

		private float _remainTime;
		private float _startRealTime;
		private bool _isExpired;
		public bool _isRunning;
		protected virtual void Awake()
		{
			_testRunner = new(this);
		}

		private void OnDisable()
		{
			StopTimer();
		}

		public virtual void Initialize(float startTime)
		{
			_isExpired = false;
			_isRunning = false;

			_remainTime = startTime;
		}

		public void StartTimer()
		{
			_isExpired = false;
			_isRunning = true;
			_startRealTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
			_testRunner.Start(TimerCoutine());
		}

		public void StopTimer()
		{
			_isRunning = false;
			_testRunner.Stop();
		}

		private IEnumerator TimerCoutine()
		{
			while(!_isExpired)
			{
				if(_remainTime <= 0)
				{
					RemainTimeText.text = "00:00";
					_isExpired = true;
				}
				else
				{
					if(_isRunning)
					{
						_remainTime -= (float)DateTime.Now.TimeOfDay.TotalSeconds - _startRealTime;
					}
					_startRealTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
					float minute = _remainTime / 60;
					float second = _remainTime % 60;
					int minuteInt = (int)Math.Truncate(minute);
					int secondInt = (int)Math.Truncate(second);
					RemainTimeText.text = $"{minuteInt.ToString("D2")}:{secondInt.ToString("D2")}";
				}
				yield return null;
			}
		}

		public void SetCurrentTime(float time)
		{
			_remainTime = time;
		}
	}
}
