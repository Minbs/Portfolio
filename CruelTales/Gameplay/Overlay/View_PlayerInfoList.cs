using CT.Common.DataType;
using CT.Common.DataType.Synchronizations;
using CT.Logger;
using CTC.GUI.Gameplay.Overlay.Item;
using CTC.GUI.Gameplay.Overlay.PlayerDrop;
using CTC.GUI.Gameplay.Overlay.PlayerReport;
using CTC.Networks.SyncObjects.SyncObjects;
using CTC.SystemCore;
using CTC.Utils;
using Slash.Unity.DataBind.Core.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CTC.GUI.Gameplay.Overlay
{
	public class View_PlayerInfoList : ViewBaseWithContext
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(View_PlayerInfoList));

		[field: SerializeField]
		public ViewNavigation PlayerInfoNavigation { get; private set; }

		// Network Reference
		private SyncObjectDictionary<UserId, PlayerState> _playerStateTableReference;
		private Dictionary<UserId, (GameObject GameObject, Context_PlayerInfoItem Context)> _contextItemByID = new();

		public Context_PlayerInfoList BindedContext { get; private set; }
		public GameObject PlayerListItem;
		public Transform PlayerListTransform;

		private MonoObjectPoolService _objectPool;

		protected override void Awake()
		{
			base.Awake();

			_objectPool = new MonoObjectPoolService(PlayerListTransform);
		}

		protected override void onBeginShow()
		{
			this.BindedContext = this.CurrentContext as Context_PlayerInfoList;
			PlayerInfoNavigation.Clear();
		}

		public void Initialize(SyncObjectDictionary<UserId, PlayerState> playerStateTable)
		{
			// Clear
			_contextItemByID.Clear();

			// Bind reference
			_playerStateTableReference = playerStateTable;
			_playerStateTableReference.OnAdded += onAdded;
			_playerStateTableReference.OnChanged += onChanged;
			_playerStateTableReference.OnCleared += onCleared;
			_playerStateTableReference.OnRemoved += onRemoved;

			foreach (var playerState in _playerStateTableReference.Values)
			{
				onAdded(playerState.UserId, playerState);
			}
		}

		protected override void onAfterHide()
		{
			_playerStateTableReference.OnAdded -= onAdded;
			_playerStateTableReference.OnChanged -= onChanged;
			_playerStateTableReference.OnCleared -= onCleared;
			_playerStateTableReference.OnRemoved -= onRemoved;
			_playerStateTableReference = null;

			onCleared();
		}

		private void onAdded(UserId userId, PlayerState playerState)
		{
			GameObject go = _objectPool.CreateObject(PlayerListItem);
			var contextHolder = go.GetComponent<ContextHolder>();
			contextHolder.Context = new Context_PlayerInfoItem(playerState);
			var playerInfo = contextHolder.Context as Context_PlayerInfoItem;

			playerInfo.BindPlayerInfoView(this);
			_contextItemByID.Add(playerState.UserId, (go, playerInfo));
			sortPlayerInfoTable();
		}

		private void onRemoved(UserId userId)
		{
			if (!_contextItemByID.TryGetValue(userId, out var contextTuple))
			{
				_log.Error($"There is no such UserId({userId}) in the PlayerInfoList");
				return;
			}

			_contextItemByID.Remove(userId);
			_objectPool.Release(contextTuple.GameObject);
			sortPlayerInfoTable();
		}

		private void onCleared()
		{
			Span<UserId> removeUserIDs = stackalloc UserId[_contextItemByID.Count];
			int count = 0;
			foreach (UserId id in _contextItemByID.Keys)
			{
				removeUserIDs[count] = id;
				count++;
			}

			for (int i = 0; i < count; i++)
			{
				onRemoved(removeUserIDs[i]);
			}
		}

		private void onChanged(UserId userId, PlayerState playerState)
		{
			if (!_contextItemByID.TryGetValue(userId, out var contextTuple))
			{
				_log.Error($"There is no such UserId({userId}) in the PlayerInfoList");
				return;
			}

			var playerInfo = contextTuple.Context;
			playerInfo.OnChanged(playerState);
			sortPlayerInfoTable();
		}

		private void sortPlayerInfoTable()
		{
			// Id 순으로 정렬
			var sortedTable = _contextItemByID.
							  OrderBy(item => item.Key.Id == GlobalService.BackendManager.UserId.Id)
							  .ThenBy(item => item.Key)
							  .ToDictionary(item => item.Key, item => item.Value);

			foreach (var item in sortedTable)
			{
				_contextItemByID[item.Key].Context.OnChanged(item.Value.Context); // OnChanged?
			}
		}

		public void CloseOutContent() => PlayerInfoNavigation.Clear();

		public void OnClick_Report(Context_PlayerInfoItem context)
		{
			var playerInfoListView = PlayerInfoNavigation.Switch<View_PlayerReport>();
			playerInfoListView.BindPlayerInfo(context.PlayerName);
			playerInfoListView.BindCloseAction(CloseOutContent);
		}

		public void OnClick_Drop(Context_PlayerInfoItem context)
		{
			var playerInfoListView = PlayerInfoNavigation.Switch<View_PlayerDrop>();
			playerInfoListView.BindPlayerInfo(context.PlayerName);
			playerInfoListView.BindCloseAction(CloseOutContent);
		}
	}
}
