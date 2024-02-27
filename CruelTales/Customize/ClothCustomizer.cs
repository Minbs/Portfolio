using CT.Common.Gameplay;
using CTC.GUI.Components.Tab;
using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CTC.GUI.Customize
{
	public enum SkinSetType
	{
		Dress,
		Hair,
		Shoes,
		Hammer,
		Eyebrow,
		Eye,
		Nose,
		Lips,
		Cheek,
		Headgear,
		Back,
		FaceAcc
	}
	public class ClothCustomizer : MonoBehaviour
	{

		public List<CostumeItemData> CostumeItemDatas = new();
		public SkinSetType SkinSetType;
		public List<ClothButton> clothButtons = new();
		public Transform ButtonParentTransform;
		public View_Customize View_Costume;



		private void Start()
		{
			clothButtons.AddRange(ButtonParentTransform.GetComponentsInChildren<ClothButton>());
			for (int i = 0; i < CostumeItemDatas.Count; i++)
			{
				clothButtons[i].BindedCustomizer = this;
				clothButtons[i].ButtonImage.sprite = CostumeItemDatas[i].itemSprite;
				if(CostumeItemDatas[i].itemSprite == null)
				{
					clothButtons[i].ButtonImage.color = Color.clear;
				}
				else
				{
					clothButtons[i].ButtonImage.color = Color.white;
				}
			}
		}

		public void OnButtonClicked(ClothButton clothButton)
		{
			SkinSet skinSet = new SkinSet();
			skinSet = View_Costume.CurrentSkinSet;
			CostumeItemData item = CostumeItemDatas[clothButton.transform.GetSiblingIndex()];
			switch (SkinSetType)
			{
				case SkinSetType.Dress:
					skinSet.Dress = item.itemKey;
					break;
				case SkinSetType.Hair:
					skinSet.Hair = item.itemKey;
					break;
				case SkinSetType.Shoes:
					skinSet.Shoes = item.itemKey;
					break;
				case SkinSetType.Hammer:
					skinSet.Hammer = item.itemKey;
					break;
				case SkinSetType.Eyebrow:
					skinSet.Eyebrow = item.itemKey;
					break;
				case SkinSetType.Eye:
					skinSet.Eyes = item.itemKey;
					break;
				case SkinSetType.Nose:
					skinSet.Nose = item.itemKey;
					break;
				case SkinSetType.Lips:
					skinSet.Lips = item.itemKey;
					break;
				case SkinSetType.Cheek:
					skinSet.Cheek = item.itemKey;
					break;
				case SkinSetType.Headgear:
					skinSet.Headgear = item.itemKey;
					break;
				case SkinSetType.Back:
					skinSet.Back = item.itemKey;
					break;
				case SkinSetType.FaceAcc:
					skinSet.FaceAcc = item.itemKey;
					break;
			}


			View_Costume.BindedContext.ItemName = item.itemName;
			View_Costume.BindedContext.ItemDescription = item.itemDescription;
			View_Costume.ChangeCurrentSkin(skinSet);
		}
	}
}
