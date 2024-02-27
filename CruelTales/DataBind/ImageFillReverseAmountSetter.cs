namespace Slash.Unity.DataBind.UI.Unity.Setters
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;
	using UnityEngine.UI;

	[AddComponentMenu("Data Bind/UnityUI/Setters/[DB] Image Fill Reverse Amount Setter (Unity)")]
	public class ImageFillReverseAmountSetter : ComponentSingleSetter<Image, float>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(Image target, float value)
		{
			target.fillAmount = 1 - value;
		}
	}
}