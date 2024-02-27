using CTC.DataBind.Contexts;
using CTC.SystemCore;
using Slash.Unity.DataBind.Core.Data;

namespace CTC.GUI.Gameplay
{
	public class Context_GameplayEscapeMenu : ContextWithView<View_GameplayEscapeMenu>
	{

		public void GUI_OnClick_Setting()
		{
			this.CurrentView.OnClick_Setting();
		}

		public void GUI_OnClick_Report()
		{
			this.CurrentView.OnClick_Report();
		}

		public void GUI_OnClick_Leave()
		{
			this.CurrentView.OnClick_Leave();
		}
	}
}
