namespace MxRouteManager.Services;

/// <summary>Fehler aus einem API-Aufruf, aufbereitet fuer die Anzeige.</summary>
public sealed class ApiException : Exception
{
    public int StatusCode { get; }
    public string? Code { get; }
    public string? Field { get; }

    public ApiException(string message, int statusCode = 0, string? code = null, string? field = null)
        : base(message)
    {
        StatusCode = statusCode;
        Code = code;
        Field = field;
    }
}
