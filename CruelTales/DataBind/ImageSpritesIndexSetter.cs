using Slash.Unity.DataBind.Foundation.Setters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CTC.DataBind.Setters
{
	/// <summary>
	/// 이미지의 스프라이트를 Sprites 배열안에 있는 스프라이트로 변경
	/// </summary>
	[AddComponentMenu("Custom Data Bind/Setters/[CDB] Image Sprites Index Setter")]
	public class ImageSpritesIndexSetter : ComponentSingleSetter<Image, int>
	{
		public List<Sprite> Sprites;

		protected override void UpdateTargetValue(Image target, int index)
		{
			if(index < Sprites.Count && index >= 0)
			{
				target.sprite = Sprites[index];
			}
		}
	}
}
