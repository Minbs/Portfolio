using CT.Common.Gameplay;
using CT.Logger;
using CTC.Networks.SyncObjects.SyncObjects;
using CTS.Instance.ClientShared;
using TMPro;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common
{
	public class View_TeamScore : ViewBase
	{
		// Log
		private static readonly ILog _log = LogManager.GetLogger(typeof(View_TeamScore));

		public TextMeshProUGUI RedTeamScoreText;
		public TextMeshProUGUI BlueTeamScoreText;

		public RectTransform RedLineTransform;
		public RectTransform BlueLineTransform;

		public GameObject RedTeamUserIcon;
		public GameObject BlueTeamUserIcon;

		public float LineInitialWidth = 120;
		public float LineExpandWidth = 100;

		public RectTransform RedUserIconTransform;
		public RectTransform BlueUserIconTransform;

		private int _blueTeamUserCount;
		private int _redTeamUserCount;

		public void AddPlayerIcon(Faction faction, SkinSet skinSet)
		{
			if (faction == Faction.Red)
			{
				GameObject go = Instantiate(RedTeamUserIcon, RedUserIconTransform);
				Item_PCIcon icon = go.GetComponent<Item_PCIcon>();
				icon.DokzaSkinHandler.ApplySkin(skinSet);
				_redTeamUserCount++;
			}

			if (faction == Faction.Blue)
			{
				GameObject go = Instantiate(BlueTeamUserIcon, BlueUserIconTransform);
				Item_PCIcon icon = go.GetComponent<Item_PCIcon>();
				icon.DokzaSkinHandler.ApplySkin(skinSet);
				_blueTeamUserCount++;
			}
		}

		public void ClearPlayerIcon()
		{
			foreach (Transform child in RedUserIconTransform)
			{
				Destroy(child.gameObject);
			}

			foreach (Transform child in BlueUserIconTransform)
			{
				Destroy(child.gameObject);
			};
		}

		public void Initialize(Dueoksini_MiniGameController minigameController)
		{
			_blueTeamUserCount = 0;
			_redTeamUserCount = 0;

			ClearPlayerIcon();

			SetTeamScore(Faction.Red, 0);
			SetTeamScore(Faction.Blue, 0);

			var playerTable = minigameController
				.GameplayController
				.RoomSessionManager
				.PlayerStateTable;

			foreach (var state in playerTable.Values)
			{
				AddPlayerIcon(state.Faction, state.CurrentCostume.GetSkinSet());
			}

			SetLineWidthByMemberNum(RedLineTransform, _redTeamUserCount);
			SetLineWidthByMemberNum(BlueLineTransform, _blueTeamUserCount);
		}

		/// <summary>
		/// 인원수에 따라서 이미지 크기 늘리기
		/// </summary>
		/// <param name="num"></param>
		public void SetLineWidthByMemberNum(RectTransform lineTransform, int num)
		{
			Vector2 size = lineTransform.sizeDelta;
			size.x = num * LineExpandWidth + LineInitialWidth;
			lineTransform.sizeDelta = size;
		}

		public void SetTeamScore(Faction faction, int score)
		{
			string scoreStr = score.ToString();

			if (faction == Faction.Red)
			{
				RedTeamScoreText.text = scoreStr;
			}
			else if (faction == Faction.Blue)
			{
				BlueTeamScoreText.text = scoreStr;
			}
		}
	}
}
