namespace Onestop.Layouts.Models {
    public class Length {
        public Length(float value, string unit) {
            Value = value;
            Unit = unit;
        }

        public float Value { get; private set; }
        public string Unit { get; private set; }

        public override string ToString() {
            if (Value <= 0) return "";
            return Value + Unit;
        }

        public const string Pixel = "px";
        public const string Percent = "%";
        public const string Em = "em";
        public const string Point = "pt";
    }
}