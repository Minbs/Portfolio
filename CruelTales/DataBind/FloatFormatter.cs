using Slash.Unity.DataBind.Core.Presentation;
using System;
using UnityEngine;

namespace CTC.DataBind.Formatter
{
	[AddComponentMenu("Custom Data Bind/Formatter/[CDB] Decimal Formatter")]
	public class FloatFormatter : DataProvider
	{
		public DataBinding Data;

		/// <summary>값</summary>
		private float _value;

		/// <summary>반올림 타입</summary>
		public TimerFormat RoundType;

		/// <summary>소수점 자리수</summary>
		public int Digits;

		public override object Value => _value;

		public override void Disable()
		{
			base.Disable();
			this.Data.ValueChanged -= this.OnDataChanged;
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();
			base.Disable();
			this.Data.ValueChanged += this.OnDataChanged;
		}

		public override void Init()
		{
			this.AddBinding(this.Data);
		}

		/// <inheritdoc />
		public override void Deinit()
		{
			this.RemoveBinding(this.Data);
		}

		private void OnDataChanged()
		{
			switch (RoundType)
			{
				case TimerFormat.Decimal:
					_value = MathF.Round(Data.GetValue<float>(), Digits);
					break;

				case TimerFormat.NoDisplayZero:
					_value = MathF.Ceiling(Data.GetValue<float>());
					break;

				case TimerFormat.DisplayZero:
					_value = MathF.Truncate(Data.GetValue<float>());
					break;
			}

			this.OnValueChanged();
		}

		protected override void UpdateValue()
		{
			this.OnValueChanged();
		}
	}
}
