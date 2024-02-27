using CTC.Globalizations;
using CTC.SystemCore;
using LiteNetLib;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CTC.GUI.Gameplay
{
	public class View_GameplayEscapeMenu : ViewBaseWithContext
	{
		[TitleGroup("Content"), SerializeField, LabelText("Context Navigation")]

		protected override void onBeginShow()
		{
			base.onBeginShow();
		}

		protected override void onAfterHide()
		{
			base.onAfterHide();
		}

		public void OnClick_Leave()
		{
			string title = TextKey.Dialog_WantLeaveGame_Title.GetText();
			string content = TextKey.Dialog_WantLeaveGame_Content.GetText();
			GlobalService.StaticGUI
				.OpenSystemDialogPopup(isTemporary: true, title, content, responseCallback: (response) =>
				{
					if (response == DialogResult.Yes)
					{
						GlobalService.NetworkManager.Disconnect();
					}
				},
				DialogResult.Yes, DialogResult.No);
		}
	}
}
