using CTC.GUI.Components.Tab;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CTC.GUI.Customize
{
	public class CostumeTabButton : TabButton
	{
		public TextMeshProUGUI _text;
		public Image _icon;

		public Color textIdleColor;
		public Color textActiveColor;

		public Sprite iconIdleSprite;
		public Sprite iconActiveSprite;


		protected override void Awake()
		{
			base.Awake();
			OnReset = OnResetButton;
		}

		public override void OnPointerEnter(PointerEventData eventData)
		{
			BindedTabGroup.OnTabEnter(this);
		}

		public override void OnPointerExit(PointerEventData eventData)
		{
			BindedTabGroup.OnTabExit(this);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			BindedTabGroup.OnTabSelected(this);
			_text.color = textActiveColor;
			_icon.sprite = iconActiveSprite;
		}

		public override void OnResetButton()
		{
			_text.color = textIdleColor;
			_icon.sprite = iconIdleSprite;
		}
	}
}
