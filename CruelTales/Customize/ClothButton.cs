using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
namespace CTC.GUI.Customize
{
	public class ClothButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
	{
		public ClothCustomizer BindedCustomizer;
		public Image ButtonImage;
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if(BindedCustomizer != null)
			BindedCustomizer.OnButtonClicked(this);
		}
	}
}
