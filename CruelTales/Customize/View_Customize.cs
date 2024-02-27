using System;
using System.Collections.Generic;
using CT.Common;
using CT.Common.Gameplay;
using CTC.SystemCore;
using Slash.Unity.DataBind.Core.Presentation;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace CTC.GUI.Customize
{
	[Serializable]
	public struct CostumeItemData
	{
		public int itemKey;
		public string skinName; // spine skin
		public Sprite itemSprite;

		public string itemName;
		public string itemDescription;
		// TODO: 정렬을 위한 데이터 : 입수일, 이름, 등급 등
	}

	public class View_Customize : ViewBaseWithContext
	{
		public SkeletonGraphic DokzaSkeletonAnimation;

		public SkinSet CurrentSkinSet { get; set; }
		// TODO : 프리셋 제작
		public DokzaSkinHandler DokzaSkinHandler;

		public Context_Customize BindedContext { get; set; }

		// Events
		private Action _onClose;

		protected override void onBeginShow()
		{
			base.onBeginShow();
			BindedContext = GetComponent<ContextHolder>().Context as Context_Customize;
			resetToPreviousSet();
			DokzaSkinHandler.PlayStillPose(false);
		}

		public void Initialize(Action onClose)
		{
			_onClose = onClose;
		}

		public void ChangeCurrentSkin(SkinSet skinSet, bool isPlayAnimation = true)
		{
			CurrentSkinSet = skinSet;
			DokzaSkinHandler.ApplySkin(skinSet);

			if (isPlayAnimation)
				DokzaSkinHandler.PlayStillPose(true);
		}

		public void ChangeSkinColor(DokzaSkinHandler.DokzaSkinSlot skinSlot, Color color)
		{
			DokzaSkinHandler.ChangeSlotColor(skinSlot, color);

			SkinSet skinSet = CurrentSkinSet;
			switch(skinSlot)
			{
				case DokzaSkinHandler.DokzaSkinSlot.Body:
					skinSet.SkinColor = new NetColor(color.r, color.g, color.b);
					break;
				case DokzaSkinHandler.DokzaSkinSlot.HairnEyebrow:
					skinSet.HairColor = new NetColor(color.r, color.g, color.b);
					break;
				case DokzaSkinHandler.DokzaSkinSlot.Eye:
					skinSet.EyesColor = new NetColor(color.r, color.g, color.b);
					break;

			}

			CurrentSkinSet = skinSet;
		}

		public void OnClick_Apply()
		{
			GlobalService.UserDataManager.UserData.SkinSet.SelectedSkinSet = CurrentSkinSet;
		}

		public void OnClick_Cancel() => resetToPreviousSet();

		public void OnClick_Close()
		{
			_onClose?.Invoke();
		}

		private void resetToPreviousSet()
		{
			ChangeCurrentSkin(GlobalService.UserDataManager.UserData.SkinSet.SelectedSkinSet, false);
		}
	}
}
