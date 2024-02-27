using System;
using System.Collections.Generic;
using System.Linq;
using CT.Logger;
using CTC.SystemCore;
using UnityEngine;

namespace CTC.GUI
{
	public struct ViewObject
	{
		public ViewBase View;
		public GameObject Instance;

		public ViewObject(ViewBase view, GameObject instance)
		{
			View = view;
			Instance = instance;
		}
	}

	/// <summary>View를 추가하거나 제거하거나 전환할 수 있는 클래스입니다.</summary>
	public class ViewNavigation : MonoBehaviour
	{
		private List<(Type ViewType, GameObject Instance)> _hideViewStack = new();
		private List<ViewObject> _viewList = new();

		[SerializeField]
		private int _baseSortingOrder = 100;

		public int Count => _viewList.Count;
		public bool HasAnyView => Count != 0;

		public ViewBase CurrentView
		{
			get
			{
				if (_viewList == null || _viewList.Count == 0)
				{
					return null;
				}

				return _viewList[_viewList.Count - 1].View;
			}
		}

		private bool tryGetGuiInstance<T>(out T gui, out GameObject go) where T : ViewBase
		{
			int findIndex = _hideViewStack.FindIndex((e) => e.ViewType == typeof(T));

			if (findIndex >= 0)
			{
				go = _hideViewStack[findIndex].Instance;
				gui = go.GetComponent<T>();

				_hideViewStack.RemoveAt(findIndex);
				go.transform.SetAsLastSibling();
				_viewList.Add(new(gui, go));
				return true;
			}

			if (GlobalService.ResourcesManager.TryGetViewInstance<T>(transform, out go))
			{
				gui = go.GetComponent<T>();
				_viewList.Add(new(gui, go));
				return true;
			}

			go = null;
			gui = null;
			return false;
		}

		private void releaseView(GameObject instance)
		{
			int findIndex = _viewList.FindIndex((e) => e.Instance == instance);

			if (findIndex >= 0)
			{
				var returnView = _viewList[findIndex];
				_hideViewStack.Add((returnView.View.GetType(), returnView.Instance));
				_viewList.RemoveAt(findIndex);
				return;
			}

			this.LogError($"정상적이지 않은 UI 객체 반환입니다. 객체 이름 : {instance.name}");
		}

		/// <summary>네비게이션 내부의 모든 View를 제거합니다.</summary>
		public void Clear()
		{
			var list = _viewList.ToList();

			foreach (var view in list)
			{
				view.View.Hide();
				releaseView(view.Instance);
			}
		}

		public bool TryGetTopBy<T>(out T view) where T : ViewBase
		{
			if (CurrentView == null)
			{
				view = null;
				return false;
			}

			if (CurrentView.GetType() == typeof(T))
			{
				view = (T)CurrentView;
				return true;
			}

			view = null;
			return false;
		}

		/// <summary>해당 View로 전환합니다. 이전 View는 모두 제거됩니다.</summary>
		/// <typeparam name="T">전환할 View의 타입입니다.</typeparam>
		/// <returns>전환된 View입니다.</returns>
		public T Switch<T>(Action callback = null) where T : ViewBase
		{
			Clear();
			return Push<T>(callback);
		}

		/// <summary>해당 View를 추가합니다.</summary>
		/// <typeparam name="T">추가할 View의 타입입니다.</typeparam>
		/// <returns>추가된 View입니다.</returns>
		public T Push<T>(Action callback = null, int sortingOrder = 0) where T : ViewBase
		{
			if (tryGetGuiInstance<T>(out var gui, out var go))
			{
				gui.SetSortingOrderByTransform(_baseSortingOrder + sortingOrder);
				gui.Show(this, callback);
				return gui;
			}

			return null;
		}

		/// <summary>최상단 View를 제거합니다.</summary>
		public void Pop(Action callback = null)
		{
			if (_viewList.Count == 0)
			{
				return;
			}

			CurrentView.Hide(callback);
			releaseView(_viewList[_viewList.Count - 1].Instance);
		}

		/// <summary>해당되는 GUI 객체를 제거합니다.</summary>
		/// <param name="guiObject">GUI 객체</param>
		public void PopByObject(GameObject guiObject)
		{
			int index = _viewList.FindIndex((e) => e.Instance == guiObject);

			if (index >= 0)
			{
				_viewList[index].View.Hide();
				releaseView(_viewList[index].Instance);
			}
		}

		/// <summary>조건에 일치하는 GUI 객체를 제거합니다.</summary>
		/// <param name="predicate"></param>
		public void PopMatch(Predicate<ViewObject> predicate, Action callback = null)
		{
			int index = _viewList.FindIndex(predicate);

			if (index >= 0)
			{
				_viewList[index].View.Hide(callback);
				releaseView(_viewList[index].Instance);
			}
		}

		/// <summary>매칭되는 타입의 GUI 객체를 제거합니다.</summary>
		/// <typeparam name="T">GUI 객체 타입</typeparam>
		public void Pop<T>(Action callback = null) where T : ViewBase
		{
			PopMatch((e) => e.View.GetType() == typeof(T), callback);
		}

		public bool HasView<T>() where T : ViewBase
		{
			return TryFind<T>(out _);
		}

		/// <summary>매칭되는 타입의 GUI 객체가 있다면 반환합니다.</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryFind<T>(out T view) where T : ViewBase
		{
			int index = _viewList.FindIndex((i) => i.View.GetType() == typeof(T));

			if (index >= 0)
			{
				view = _viewList[index].View as T;
				return true;
			}
			else
			{
				view = null;
				return false;
			}
		}

		/// <summary>매칭되는 GUI 객체가 있다면 반환합니다.</summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryFind(Predicate<ViewObject> predicate, out ViewBase view)
		{
			int index = _viewList.FindIndex(predicate);

			if (index >= 0)
			{
				view = _viewList[index].View;
				return true;
			}
			else
			{
				view = null;
				return false;
			}
		}
	}
}
