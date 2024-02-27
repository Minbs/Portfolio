using Slash.Unity.DataBind.Core.Presentation;
using UnityEngine;

namespace CTC.DataBind.DataProviders
{
	public class InterpolatedNumberProvider : DataProvider
	{
		public DataBinding Min;

		public DataBinding Max;

		public DataBinding Proceed;

		private float _value;

		/// <inheritdoc />
		public override object Value
		{
			get
			{
				return this._value;
			}
		}

		public override void Disable()
		{
			base.Disable();
			this.Min.ValueChanged -= this.OnDataChanged;
			this.Max.ValueChanged -= this.OnDataChanged;
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();
			base.Disable();
			this.Min.ValueChanged += this.OnDataChanged;
			this.Max.ValueChanged += this.OnDataChanged;
		}

		public override void Init()
		{
			this.AddBinding(this.Min);
			this.AddBinding(this.Max);
		}

		/// <inheritdoc />
		public override void Deinit()
		{
			this.RemoveBinding(this.Min);
			this.RemoveBinding(this.Max);
		}

		private void OnDataChanged()
		{
			_value = Mathf.Lerp(Min.GetValue<float>(),Max.GetValue<float>(), _value);
			this.OnValueChanged();
		}

		protected override void UpdateValue()
		{
			this.OnValueChanged();
		}
	}
}
