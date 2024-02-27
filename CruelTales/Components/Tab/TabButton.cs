using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CTC.GUI.Components.Tab
{
	public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public TabGroup BindedTabGroup;
		public Image ButtonImage { get; set; }
		public Action OnReset;

		protected virtual void Awake()
		{
			ButtonImage = GetComponent<Image>();
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			BindedTabGroup.OnTabEnter(this);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			BindedTabGroup.OnTabExit(this);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			BindedTabGroup.OnTabSelected(this);
		}

		public virtual void OnResetButton() { }
	}
}
