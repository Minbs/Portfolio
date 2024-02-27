using CT.Common.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace CTC.GUI.Gameplay.Common.MissionBoard
{
	public class View_MissionBoardHandler : ViewBase
	{
		public MissionBoard MissionTextBoard;
		public MissionBoard TimerTextBoard;
		public MissionBoard GameStartBoard;

		private List<MissionBoard> _missionBoards = new();
		private int _index = 0;

		protected override void onBeginShow()
		{
			base.onBeginShow();
		}

		/// <summary>게임이 시작됩니다.</summary>
		/// <param name="gameMode">게임 모드입니다.</param>
		/// <param name="missionShowTime">Mission을 보여주는 시간입니다.</param>
		/// <param name="countdown">게임 시작 카운트다운입니다.</param>
		public void OnGameStartCountdown(GameModeType gameMode,
										 float missionShowTime,
										 float countdown)
		{
			MissionTextBoard.Initialize(this, missionShowTime);
			TimerTextBoard.Initialize(this, countdown);
			GameStartBoard.Initialize(this, 3);

			_missionBoards.Add(MissionTextBoard);
			_missionBoards.Add(TimerTextBoard);
			_missionBoards.Add(GameStartBoard);

			StartProceed();
		}

		public void StartProceed()
		{
			if(_index < _missionBoards.Count)
			{
				_missionBoards[_index].OnStart();
			}
			else
			{
				PopThisBoard();
			}
			_index++;
		}

		private void PopThisBoard()
		{
			ParentNavigation.PopByObject(gameObject);
		}
	}
}
