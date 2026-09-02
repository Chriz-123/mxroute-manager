namespace MxRouteManager.Models;

/// <summary>
/// Ein MXroute-Mailserver, wie er auf der Statusseite und im X-Server-Header vorkommt.
/// </summary>
public sealed class MailServer
{
    public string Hostname { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string PanelType { get; init; } = "";

    /// <summary>Wird vom editierbaren ComboBox-Textfeld verwendet.</summary>
    public override string ToString() => Hostname;
}
