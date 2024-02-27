namespace CTC.GUI.Gameplay
{
	public class View_GameplayHUD : ViewBaseWithContext
	{
		public Context_GameplayHUD BindedContext { get; private set; }
		public Navigation_SystemControl Navigation { get; private set; }

		protected override void onBeginShow()
		{
			base.onBeginShow();
			this.BindedContext = this.CurrentContext as Context_GameplayHUD;
			Navigation = (Navigation_SystemControl)ParentNavigation;
		}
	}
}
