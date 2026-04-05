using System.ComponentModel;
using System.Globalization;

namespace Safi.Helpers
{
    public class DateOnlyTypeConverter : TypeConverter
    {
        private static readonly string[] _formats =
        [
            "MM/dd/yyyy",   // 07/18/2001  ← primary format
            "yyyy-MM-dd",   // 2001-07-18
            "dd/MM/yyyy",   // 18/07/2001
            "MM-dd-yyyy",   // 07-18-2001
            "yyyy/MM/dd"    // 2001/07/18
        ];

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
            => sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                if (DateOnly.TryParseExact(str.Trim(), _formats, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var date))
                    return date;

                // Fallback: try general parsing
                if (DateOnly.TryParse(str.Trim(), out var fallback))
                    return fallback;

                throw new FormatException(
                    $"Invalid date format '{str}'. Use MM/dd/yyyy (e.g. 07/18/2001).");
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
