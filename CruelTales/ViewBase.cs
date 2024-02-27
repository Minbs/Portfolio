using System;
using System.Collections.Generic;
using CT.Logger;
using CTC.DataBind;
using CTC.DataBind.Commands;
using CTC.DataBind.Contexts;
using CTC.DataBind.Setters;
using Sirenix.OdinInspector;
using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace CTC.GUI
{
	[RequireComponent(typeof(Canvas))]
	[RequireComponent(typeof(CanvasScaler))]
	public abstract class ViewBase : MonoBehaviour
	{
		[field: TitleGroup("View Base"), SerializeField]
		public Canvas Canvas { get; private set; }

		[field: TitleGroup("View Base"), SerializeField]
		public CanvasScaler CanvasScaler { get; private set; }

		[field: TitleGroup("View Base"), SerializeField]
		public RectTransform ViewTransform { get; private set; }

		[field: TitleGroup("View Base"), SerializeField]
		public float PlaneDistance { get; private set; } = 1.0f;

		public ViewNavigation ParentNavigation { get; private set; }

		[Button(ButtonHeight = 30)]
		protected virtual void resetViewBaseComponent()
		{
			Canvas = GetComponent<Canvas>();
			Canvas.planeDistance = PlaneDistance;

			CanvasScaler = GetComponent<CanvasScaler>();
			CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
			CanvasScaler.referenceResolution = new Vector2
			(
				Global.GUI.GUI_REFERENCE_RESOLUTION_X,
				Global.GUI.GUI_REFERENCE_RESOLUTION_Y
			);
			CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
			CanvasScaler.matchWidthOrHeight = 1.0F;
			CanvasScaler.referencePixelsPerUnit = Global.GUI.GUI_REFERENCE_PPU;

			ViewTransform = GetComponent<RectTransform>();
		}

#if UNITY_EDITOR
		[PropertyOrder(1)]
		[Button(ButtonHeight = 30)]
		[TitleGroup("Editor Operation")]
		public void ResetSetterComponents()
		{
			resetComponents<TextLocalizeSetter>();
			resetComponents<ButtonClickCommandBinder>();

			void resetComponents<T>() where T : IComponentResettable
			{
				var setters = GetComponentsInChildren<T>();
				foreach (var s in setters)
				{
					s.ResetComponent();
				}

				this.LogInfo($"Reset {setters.Length} {typeof(T).Name} setter components.");
			}
		}

		[SerializeField]
		[Space(10)]
		[ValueDropdown("mAllRaycastReleasableTypes")]
		[PropertyOrder(2)]
		[TitleGroup("Editor Operation")]
		private string RaycastTargetReleaseType = "TextMesh";

		private static string[] mAllRaycastReleasableTypes = new string[]
		{
			"TextMesh",
			"Image",
			"RawImage",
		};

		private static Dictionary<string, Type> mTypeTable = new()
		{
			{ "TextMesh", typeof(TMPro.TextMeshProUGUI) },
			{ "Image", typeof(Image) },
			{ "RawImage", typeof(RawImage) },
		};

		[PropertyOrder(3)]
		[Button(ButtonHeight = 30)]
		[TitleGroup("Editor Operation")]
		private void releaseRaycastComponents()
		{
			if (!mTypeTable.TryGetValue(RaycastTargetReleaseType, out var type))
			{
				this.LogError($"There is no such type as \"{type}\"");
				return;
			}

			var uiComponents = GetComponentsInChildren(type);

			foreach (var i in uiComponents)
			{
				var ui = i as Graphic;
				if (ui == null)
					continue;

				ui.raycastTarget = false;
			}
		}
#endif

		public void SetSortingOrderByTransform(int baseSortingOrder)
		{
			this.Canvas.sortingOrder = baseSortingOrder + transform.GetSiblingIndex();
		}

		public void Show(ViewNavigation parentNavigation, Action onShown = null)
		{
			ViewTransform.anchorMin = new Vector2(0, 0);
			ViewTransform.anchorMax = new Vector2(1, 1);
			ViewTransform.pivot = new Vector2(0.5f, 0.5f);
			ViewTransform.localScale = new Vector3(1, 1, 1);

			Canvas.planeDistance = PlaneDistance;

			// Set parent Navigation
			this.ParentNavigation = parentNavigation;
			// Set render camera
			this.Canvas.worldCamera = Camera.main;

			gameObject.SetActive(true);
			onBeginShow();

			// TODO : Run animation

			onShown?.Invoke();
			onAfterShow();
		}

		public void Hide(Action onHiden = null)
		{
			gameObject.SetActive(false);
			onBeginHide();

			// TODO : Run animation

			onHiden?.Invoke();
			onAfterHide();
		}

		/// <summary>네비게이션에서 창을 닫습니다.</summary>
		public void HideItself()
		{
			this.ParentNavigation?.PopByObject(this.gameObject);
		}

		protected virtual void onBeginShow() { }
		protected virtual void onAfterShow() { }

		protected virtual void onBeginHide() { }
		protected virtual void onAfterHide() { }
	}

	[RequireComponent(typeof(ContextHolder))]
	public abstract class ViewBaseWithContext : ViewBase
	{
		[field: TitleGroup("View Base"), SerializeField]
		public ContextHolder ContextHolder;
		public ContextWithView CurrentContext { get; private set; }

		protected override void resetViewBaseComponent()
		{
			base.resetViewBaseComponent();
			ContextHolder = GetComponent<ContextHolder>();
			ContextHolder.CreateContext = true;
		}

		protected virtual void Awake()
		{
			BindContext();
		}

		protected virtual void Start()
		{
			BindContext();
		}

		protected virtual void OnEnable()
		{
			BindContext();
		}

		public void BindContext()
		{
			CurrentContext = ContextHolder.Context as ContextWithView;
			CurrentContext?.BindView(this);
		}
	}
}
