using food_service.ProductService.Application.Service;

namespace food_service.ProductService.Infrastructure.ImplementService;

public class InventoryDateProvider : IInventoryDateProvider
{
    private const string DefaultTimeZoneId = "Asia/Ho_Chi_Minh";
    private readonly TimeZoneInfo _timeZone;

    public InventoryDateProvider(IConfiguration configuration)
    {
        var configuredTimeZoneId = configuration["Inventory:TimeZoneId"] ?? DefaultTimeZoneId;
        _timeZone = ResolveTimeZone(configuredTimeZoneId);
    }

    public DateOnly Today
    {
        get
        {
            var localNow = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, _timeZone);
            return DateOnly.FromDateTime(localNow.DateTime);
        }
    }

    private static TimeZoneInfo ResolveTimeZone(string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException) when (
            TimeZoneInfo.TryConvertIanaIdToWindowsId(timeZoneId, out var windowsTimeZoneId))
        {
            return TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId);
        }
    }
}
