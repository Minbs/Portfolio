using CT.Common.Gameplay;
using CTC.GUI.Customize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CTC.Assets.Scripts.GUI.Customize
{
	public class ColorPicker : MonoBehaviour
	{
		public DokzaSkinHandler.DokzaSkinSlot SkinSlotType;
		public List<ColorPickButton> ColorButtons = new();
		public Transform ButtonParentTransform;
		public View_Customize View_Customize;

		private void Start()
		{
			ColorButtons.AddRange(ButtonParentTransform.GetComponentsInChildren<ColorPickButton>());
			for (int i = 0; i < ColorButtons.Count; i++)
			{
				ColorButtons[i].BindedColorPicker = this;
				ColorButtons[i].ButtonColor = ColorButtons[i].GetComponent<Image>().color;
			}
		}

		public void OnButtonClicked(ColorPickButton colorPickButton)
		{
			View_Customize.ChangeSkinColor(SkinSlotType, colorPickButton.ButtonColor);
		}
	}
}
