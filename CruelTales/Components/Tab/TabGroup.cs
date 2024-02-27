using System;
using System.Collections.Generic;
using UnityEngine;

namespace CTC.GUI.Components.Tab
{
	public class TabGroup : MonoBehaviour
	{
		public Transform TabContentTransform;
		public Transform TabButtonTransform;
		public List<TabButton> _tabButtons = new();
		private TabButton selectedTab;

		public Sprite IdleSprite;
		public Sprite HoverSprite;
		public Sprite SelectedSprite;

		private void Start()
		{
			_tabButtons.Clear();
			selectedTab = null;
			var buttons = TabButtonTransform.GetComponentsInChildren<TabButton>();
			int length = buttons.Length;
			for(int i = 0; i < length; i++)
			{
				buttons[i].BindedTabGroup = this;
				_tabButtons.Add(buttons[i]);
			}
			//ResetTabs();
			//OnTabSelected(_tabButtons[0]);
			//OnTabSelected(_tabButtons[0]);
			_tabButtons[0].OnPointerClick(null);
		}

		public void ChangeTabActive(int index)
		{
			int count = TabContentTransform.childCount;
			for (int i = 0; i < count; i++)
			{
				if(i == index)
				{
					TabContentTransform.GetChild(i).gameObject.SetActive(true);
				}
				else
				{
					TabContentTransform.GetChild(i).gameObject.SetActive(false);
				}
			}
		}

		public void ResetTabs()
		{
			foreach(var button in _tabButtons)
			{
				if ((selectedTab != null && button == selectedTab)
					|| button.ButtonImage == null) continue;

				if (IdleSprite == null)
					button.ButtonImage.color = new Color(0, 0, 0, 0);
				else
				{
					button.ButtonImage.color = Color.white;
					button.ButtonImage.sprite = IdleSprite;
				}


				button.OnReset?.Invoke();
			}
		}

		public void OnTabEnter(TabButton tabButton)
		{
			ResetTabs();
			if (selectedTab == null && tabButton != selectedTab)
			{
				tabButton.ButtonImage.sprite = HoverSprite;

				if (HoverSprite == null)
					tabButton.ButtonImage.color = new Color(0, 0, 0, 0);
				else
					tabButton.ButtonImage.color = Color.white;
			}
		}

		public void OnTabExit(TabButton tabButton)
		{
			ResetTabs();
		}

		public void OnTabSelected(TabButton tabButton)
		{
			selectedTab = tabButton;
			ResetTabs();
			tabButton.ButtonImage.sprite = SelectedSprite;
			ChangeTabActive(tabButton.transform.GetSiblingIndex());

			if (SelectedSprite == null)
				tabButton.ButtonImage.color = new Color(0, 0, 0, 0);
			else
				tabButton.ButtonImage.color = Color.white;
		}
	}
}
