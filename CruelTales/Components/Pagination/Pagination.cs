using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;

namespace CTC.GUI.Components
{
	public class Pagination : MonoBehaviour
	{
		[field: SerializeField]
		private TextMeshProUGUI _onText;

		[field: SerializeField]
		public int CurrentIndex { get; private set; }

		[field: SerializeField]
		public bool IsCircular { get; private set; }

		// Object List
		private List<object> _itemList;

		public int Count => _itemList.Count;

		public bool CanMoveNext => CurrentIndex + 1 < Count;
		public bool CanMovePrevious => CurrentIndex - 1 >= 0;

		[field: TabGroup("PageIcon"), SerializeField]
		private Transform _pageIconTransform; // 아이콘의 부모 트랜스폼
		[field: TabGroup("PageIcon"), SerializeField]
		private Sprite _currentPageIconSprite; // 현재 페이지의 아이콘 이미지
		[field: TabGroup("PageIcon"), SerializeField]
		private Sprite _defaultPageIconSprite; // 일반 아이콘 이미지
		[field: TabGroup("PageIcon"), SerializeField]
		private GameObject _pageIconPrefab;

		private Dictionary<int, (GameObject obj, Image img)> _pageIconTablebyIndex = new();

		public Action<int> OnPageChanged;

		public void Initialize(List<object> items, int initialIndex = 0)
		{
			_itemList = items;

			if (_pageIconTransform) // 페이지 아이콘이 있다면
			{
				_pageIconTablebyIndex.Clear();
				for (int i = 0; i < items.Count; i++)
				{
					var iconObject = Instantiate(_pageIconPrefab, _pageIconTransform);
					_pageIconTablebyIndex.Add(i, (iconObject, iconObject.GetComponent<Image>()));
				}
			}

			SetIndex(initialIndex);
		}

		public T GetCurrentOption<T>()
		{
			return (T)_itemList[CurrentIndex];
		}

		public void SetIndex(int index)
		{
			CurrentIndex = isValueIndex(index) ? index : 0;
			if (_pageIconTransform)
			{
				_pageIconTablebyIndex[CurrentIndex].img.sprite = _currentPageIconSprite;
			}
			OnGuiValidate();
		}

		public void NextIndex()
		{
			if (CanMoveNext)
			{
				CurrentIndex++;
			}
			else if (IsCircular)
			{
				CurrentIndex = 0;
			}

			OnGuiValidate();
		}

		public void PreviousIndex()
		{
			if (CanMovePrevious)
			{
				CurrentIndex--;
			}
			else if (IsCircular)
			{
				CurrentIndex = Count - 1;
			}

			OnGuiValidate();
		}

		private bool isValueIndex(int index)
		{
			return index >= 0 && index < Count;
		}

		public void OnGuiValidate()
		{
			if (!isValueIndex(CurrentIndex))
			{
				CurrentIndex = 0;
			}

			_onText.text = _itemList[CurrentIndex].ToString();

			if (_pageIconTransform)
			{
				int count = _pageIconTablebyIndex.Count;
				for (int i = 0; i < count; i++)
				{
					if (i == CurrentIndex)
						_pageIconTablebyIndex[i].img.sprite = _currentPageIconSprite;
					else
						_pageIconTablebyIndex[i].img.sprite = _defaultPageIconSprite;
				}
			}

			OnPageChanged?.Invoke(CurrentIndex);
		}
	}
}