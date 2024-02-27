using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common.MissionBoard
{
	public class TimerMissionBoard : MissionBoard
	{
		private TextMeshProUGUI _timerText;
		public override void Initialize(View_MissionBoardHandler view_MissionBoardHandler, float showTime)
		{
			gameObject.SetActive(true);
			_view_MissionBoardHandler = view_MissionBoardHandler;
			_showTime = showTime;

			_timerText = GetComponentInChildren<TextMeshProUGUI>();
			_timerText.text =  ((int)_showTime).ToString();
		}

		private IEnumerator Show()
		{
			BoardCanvasGroup.DOFade(1, 1);
			while (_showTime >= 0)
			{
				_showTime -= Time.deltaTime;
				_timerText.text = Math.Ceiling(_showTime).ToString();
				Debug.Log(_showTime);
				yield return null;
			}

			OnEnd();
		}

		public override void OnStart()
		{
			StartCoroutine(Show());
		}

		public override void OnEnd()
		{
			DOTween.Kill(this);
			_view_MissionBoardHandler.StartProceed();
			gameObject.SetActive(false);
		}
	}
}
