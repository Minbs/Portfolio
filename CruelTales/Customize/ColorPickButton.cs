using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using CTC.Assets.Scripts.GUI.Customize;

namespace CTC.GUI.Customize
{
	public class ColorPickButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public ColorPicker BindedColorPicker;
		public Color ButtonColor;
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (BindedColorPicker != null)
				BindedColorPicker.OnButtonClicked(this);
		}
	}
}
