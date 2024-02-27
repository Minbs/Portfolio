#nullable enable


using System;
using CTC.GUI.Gameplay.Lobby;

namespace CTC.GUI.Gameplay
{
	[Obsolete]
	public class Navigation_SystemControl : ViewNavigation
	{
		public bool IsLobbyMain => LobbyMain != null;
		public View_LobbyMain? LobbyMain { get; private set; }

		public View_LobbyMain OpenLobbyMain()
		{
			LobbyMain = Push<View_LobbyMain>();
			LobbyMain.OpenLobbyWatting();
			return LobbyMain;
		}

		public void CloseLobbyMain()
		{
			LobbyMain = null;
			Pop<View_LobbyMain>();
		}

		public void ToggleEscapeMenu()
		{
			if (IsLobbyMain)
			{
				LobbyMain?.EscapePressed();
			}
		}
	}
}

#nullable disable