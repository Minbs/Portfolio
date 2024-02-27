using CTC.DataBind.Contexts;
using Slash.Unity.DataBind.Core.Data;

namespace CTC.GUI.Customize
{
	public class Context_Customize : ContextWithView<View_Customize>
	{
		private readonly Property<string> itemNameProperty = new();
		private readonly Property<string> itemDescriptionProperty = new();

		public string ItemName
		{
			get => itemNameProperty.Value;
			set => itemNameProperty.Value = value;
		}

		public string ItemDescription
		{
			get => itemDescriptionProperty.Value;
			set => itemDescriptionProperty.Value = value;
		}

		public void GUI_OnClick_Apply()
		{
			CurrentView.OnClick_Apply();
		}

		public void GUI_OnClick_Cancel()
		{
			CurrentView.OnClick_Cancel();
		}

		public void GUI_OnClick_Close()
		{
			CurrentView.OnClick_Close();
		}
	}
}
