using System.Collections;
using CT.Common.Gameplay;
using CTC.DataBind.Contexts;
using CTC.Globalizations;
using CTC.Utils.Coroutines;
using Slash.Unity.DataBind.Core.Data;

namespace CTC.GUI.Gameplay.Common
{
	public class Context_FeverTime : ContextWithView<View_FeverTime>
	{
		public Property<string> feverTimeInfoProperty = new();
		public string FeverTimeInfo
		{
			get => feverTimeInfoProperty.Value;
			set => feverTimeInfoProperty.Value = value;
		}
	}

	public class View_FeverTime : ViewBaseWithContext
	{
		private CoroutineRunner _closeCoroutine;

		public Context_FeverTime BindedContext { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			_closeCoroutine = new CoroutineRunner(this);
		}

		protected override void onBeginShow()
		{
			BindedContext = CurrentContext as Context_FeverTime;
			BindedContext.FeverTimeInfo = string.Empty;
			base.onBeginShow();
			_closeCoroutine.Start(close());
		}

		public void Initialized(GameModeType gameMode)
		{
			TextKey infoKey = gameMode switch
			{
				GameModeType.RedHood => TextKey.MG_RedHood_Fever,
				GameModeType.Dueoksini => TextKey.MG_DueokSini_Fever,
				_ => TextKey.None
			};
			BindedContext.FeverTimeInfo = infoKey.GetText();
		}

		private IEnumerator close()
		{
			yield return CoroutineCache.GetWaitForSeconds(3.0f);
			ParentNavigation.PopByObject(gameObject);
		}
	}
}
