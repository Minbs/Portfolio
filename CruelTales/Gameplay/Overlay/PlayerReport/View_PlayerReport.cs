using UnityEngine;
using System;

namespace CTC.GUI.Gameplay.Overlay.PlayerReport
{
	public class View_PlayerReport : ViewBaseWithContext
	{
		public Context_PlayerReport BindedContext;
		private Action _onClose;

		protected override void onBeginShow()
		{
			base.onBeginShow();
			this.BindedContext = this.CurrentContext as Context_PlayerReport;
		}

		public void BindPlayerInfo(string playerName)
		{
			BindedContext.PlayerName = playerName;
		}

		public void BindCloseAction(Action onClose)
		{
			_onClose = onClose;
		}

		public void OnClick_Yes()
		{
			Debug.Log("신고 적용");
			_onClose?.Invoke();
		}

		public void OnClick_No()
		{
			Debug.Log("신고 취소");
			_onClose?.Invoke();
		}
	}
}
