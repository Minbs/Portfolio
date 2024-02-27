using System;
using System.Collections;
using CTC.Utils.Coroutines;
using UnityEngine;

namespace CTC.GUI.Components.Timer
{
	public class BaseTimer : MonoBehaviour
	{
		private CoroutineRunner _testRunner;
		protected Action _onUpdate;
		public bool IsRunning { get; private set; }

		protected float _startTime;
		protected float _endTime;
		protected float _currentTime;
		private float _pastRealTime; // 실제 시간
		protected bool _isExpired;
		protected bool _isCountDown;

		protected virtual void Awake()
		{
			_testRunner = new(this);
		}

		public virtual void Initialize(float startTime, float endTime = 0.001f)
		{
			IsRunning = false;
			_isExpired = false;
			_isCountDown = startTime > endTime;
			IsRunning = false;

			_startTime = startTime;
			_endTime = endTime;
			_currentTime = _startTime;

			//_testRunner.BindOnStartCallback(() => { Debug.Log($"{nameof(_testRunner)} Start!"); });
			//_testRunner.BindOnEndCallback(() => { Debug.Log($"{nameof(_testRunner)} End!"); });
		}

		public void StartTimer()
		{
			Debug.Log($"{nameof(_testRunner)} Click!");
			_currentTime = _startTime;
			_isExpired = false;
			IsRunning = true;
			_pastRealTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
			if (_testRunner != null)
			{
				_testRunner.Start(updateCoroutine());
			}
		}

		public void SynchronizeTimer(float currentTime)
		{
			_currentTime = currentTime;
		}

		public void StopTimer()
		{
			IsRunning = false;
			_testRunner.Stop();
		}

		private void OnDisable()
		{
			StopTimer();
		}

//#if UNITY_EDITOR
//		void Update()
//		{
//			if (Input.GetKeyDown(KeyCode.A))
//			{
//				StartTimer();
//			}
//			if (Input.GetKeyDown(KeyCode.S))
//			{
//				IsRunning = !IsRunning;
//			}
//		}
//#endif

		private IEnumerator updateCoroutine()
		{
			float sign = _isCountDown ? -1 : 1;

			while (!_isExpired)
			{
				if (IsRunning)
				{
					_currentTime += ((float)DateTime.Now.TimeOfDay.TotalSeconds - _pastRealTime) * sign; //Time.deltaTime * sign;
				}

				if ((_isCountDown && _currentTime <= _endTime) ||
					(!_isCountDown && _currentTime >= _endTime))
				{
					_currentTime = _endTime;
					_isExpired = true;
				}

				_pastRealTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
				yield return null;
				_onUpdate?.Invoke();
			}
		}
	}
}
