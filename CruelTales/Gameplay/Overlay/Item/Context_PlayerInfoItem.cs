using CTC.Networks.SyncObjects.SyncObjects;
using Slash.Unity.DataBind.Core.Data;
using UnityEngine;

namespace CTC.GUI.Gameplay.Overlay.Item
{
	public class Context_PlayerInfoItem : Context
	{
		// Reference
		private View_PlayerInfoList _playerInfoListView;

		private readonly Property<string> playerNameProperty = new();
		public string PlayerName
		{
			get => playerNameProperty.Value;
			set => playerNameProperty.Value = value;
		}

		private readonly Property<string> playerTitleProperty = new();
		public string PlayerTitle
		{
			get => playerTitleProperty.Value;
			set => playerTitleProperty.Value = value;
		}

		private readonly Property<string> playerLevelProperty = new();
		public string PlayerLevel
		{
			get => playerLevelProperty.Value;
			set => playerLevelProperty.Value = value;
		}

		private readonly Property<Sprite> playerIconProperty = new();
		public Sprite PlayerIcon
		{
			get => playerIconProperty.Value;
			set => playerIconProperty.Value = value;
		}

		private readonly Property<bool> isHostProperty = new();
		public bool IsHost
		{
			get => isHostProperty.Value;
			set => isHostProperty.Value = value;
		}

		private readonly Property<bool> isReadyProperty = new();
		public bool IsReady
		{
			get => isReadyProperty.Value;
			set => isReadyProperty.Value = value;
		}

		public Context_PlayerInfoItem(PlayerState playerState)
		{
			OnChanged(playerState);
		}

		public void BindPlayerInfoView(View_PlayerInfoList view_PlayerInfoList)
		{
			_playerInfoListView = view_PlayerInfoList;
		}

		public void OnChanged(PlayerState playerState)
		{
			PlayerName = playerState.Username;
			PlayerTitle = playerState.UserId.ToString();
			PlayerLevel = "99";
			IsHost = playerState.IsHost;
			IsReady = !IsHost & playerState.IsReady;
		}

		public void OnChanged(Context_PlayerInfoItem context)
		{
			PlayerName = context.PlayerName;
			PlayerTitle = context.PlayerTitle;
			PlayerLevel = context.PlayerLevel;
			IsHost = context.IsHost;
			IsReady = context.IsReady;
		}

		public void GUI_OnClick_Report()
		{
			if (ReferenceEquals(_playerInfoListView, null))
				return;

			_playerInfoListView.OnClick_Report(this);
		}

		public void GUI_OnClick_Drop()
		{
			if (ReferenceEquals(_playerInfoListView, null))
				return;

			_playerInfoListView.OnClick_Drop(this);
		}
	}
}
