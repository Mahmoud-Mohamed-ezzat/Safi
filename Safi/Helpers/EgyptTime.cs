namespace Safi.Helpers
{

    public static class EgyptTime
    {
        private static readonly TimeZoneInfo EgyptTz =
            TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTz);
        public static DateOnly Today => DateOnly.FromDateTime(Now);
        public static DateTime FromUtc(DateTime utcTime) =>
            TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.SpecifyKind(utcTime, DateTimeKind.Utc), EgyptTz);
        public static DateTime? FromUtc(DateTime? utcTime) =>
            utcTime.HasValue ? FromUtc(utcTime.Value) : null;
    }
}
