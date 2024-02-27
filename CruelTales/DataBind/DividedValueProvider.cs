using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;

namespace CTC.DataBind.DataProviders
{
	[AddComponentMenu("Custom Data Bind/Provider/[CDB] Divided Value Provider")]
	public class DividedValueProvider : DataProvider
	{
		public DataBinding Numerator;

		public DataBinding Denominator;

		private float quotient; // 나누기 몫

		/// <inheritdoc />
		public override object Value
		{
			get
			{
				return this.quotient;
			}
		}

		public override void Disable()
		{
			base.Disable();
			this.Numerator.ValueChanged -= this.OnDataChanged;
			this.Denominator.ValueChanged -= this.OnDataChanged;
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();
			base.Disable();
			this.Numerator.ValueChanged += this.OnDataChanged;
			this.Denominator.ValueChanged += this.OnDataChanged;
		}

		public override void Init()
		{
			this.AddBinding(this.Numerator);
			this.AddBinding(this.Denominator);
		}

		/// <inheritdoc />
		public override void Deinit()
		{
			this.RemoveBinding(this.Numerator);
			this.RemoveBinding(this.Denominator);
		}

		private void OnDataChanged()
		{
			quotient = this.Numerator.GetValue<float>() / this.Denominator.GetValue<float>();
			this.OnValueChanged();
		}

		protected override void UpdateValue()
		{
			this.OnValueChanged();
		}
	}
}
