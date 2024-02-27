using CTC.DataBind.Contexts;
using Slash.Unity.DataBind.Core.Data;

namespace CTC.GUI.Gameplay.Overlay.PlayerDrop
{
	public class Context_PlayerDrop : ContextWithView<View_PlayerDrop>
	{
		private readonly Property<string> playerNameProperty = new();
		public string PlayerName
		{
			get => playerNameProperty.Value;
			set => playerNameProperty.Value = value;
		}


		public void GUI_OnClick_Yes()
		{
			this.CurrentView.OnClick_Yes();
		}

		public void GUI_OnClick_No()
		{
			this.CurrentView.OnClick_No();
		}
	}
}
