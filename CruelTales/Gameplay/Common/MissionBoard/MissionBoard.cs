using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common.MissionBoard
{
	public class MissionBoard : MonoBehaviour
	{
		protected View_MissionBoardHandler _view_MissionBoardHandler;
		public CanvasGroup BoardCanvasGroup;

		protected float _showTime; // 전체 시간
		public virtual void Initialize(View_MissionBoardHandler view_MissionBoardHandler, float showTime)
		{
			gameObject.SetActive(true);
			_view_MissionBoardHandler = view_MissionBoardHandler;
			_showTime = showTime;
		}

		private IEnumerator Show()
		{
			BoardCanvasGroup.DOFade(1, 1);
			while (_showTime >= 0)
			{
				_showTime -= Time.deltaTime;
				yield return null;
			}

			OnEnd();
		}

		public virtual void OnStart()
		{
			StartCoroutine(Show());
		}

		public virtual void OnEnd()
		{
			DOTween.Kill(this);
			_view_MissionBoardHandler.StartProceed();
			gameObject.SetActive(false);
		}
	}
}
