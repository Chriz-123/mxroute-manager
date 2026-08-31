using System.Windows.Data;
using System.Windows.Markup;

namespace MxRouteManager.Localization;

/// <summary>
/// XAML-Markup-Erweiterung: <c>{loc:Tr SchluesselName}</c> bindet an den Loc-Indexer,
/// sodass Texte bei einem Sprachwechsel automatisch aktualisiert werden.
/// </summary>
[MarkupExtensionReturnType(typeof(object))]
public sealed class TrExtension : MarkupExtension
{
    public string Key { get; set; } = "";

    public TrExtension() { }
    public TrExtension(string key) => Key = key;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var binding = new Binding($"[{Key}]")
        {
            Source = Loc.Instance,
            Mode = BindingMode.OneWay
        };
        return binding.ProvideValue(serviceProvider);
    }
}
