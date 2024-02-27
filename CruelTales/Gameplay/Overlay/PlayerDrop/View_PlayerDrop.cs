using System;
using UnityEngine;

namespace CTC.GUI.Gameplay.Overlay.PlayerDrop
{
	public class View_PlayerDrop : ViewBaseWithContext
	{
		public Context_PlayerDrop BindedContext;
		private Action _onClose;

		protected override void onBeginShow()
		{
			base.onBeginShow();
			this.BindedContext = this.CurrentContext as Context_PlayerDrop;
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
			Debug.Log("강퇴 적용");
			_onClose?.Invoke();
		}

		public void OnClick_No()
		{
			Debug.Log("강퇴 취소");
			_onClose?.Invoke();
		}
	}
}
