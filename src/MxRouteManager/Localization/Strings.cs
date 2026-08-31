namespace MxRouteManager.Localization;

/// <summary>Statischer String-Katalog fuer Deutsch und Englisch.</summary>
public static class Strings
{
    public static string Get(string key, string language)
    {
        var dict = language == "en" ? En : De;
        if (dict.TryGetValue(key, out var v)) return v;
        if (De.TryGetValue(key, out var d)) return d;
        return key;
    }

    public static readonly Dictionary<string, string> De = new()
    {
        // Fenster / App
        ["Window_Title"] = "MXroute Manager",
        ["App_Subtitle"] = "Manager",
        ["App_UnexpectedError"] = "Ein unerwarteter Fehler ist aufgetreten:\n\n{0}",

        // Allgemein
        ["Common_Refresh"] = "\u21BB  Aktualisieren",
        ["Common_Delete"] = "L\u00f6schen",
        ["Common_Edit"] = "Bearbeiten",
        ["Common_Save"] = "Speichern",
        ["Common_Cancel"] = "Abbrechen",
        ["Common_Ok"] = "OK",
        ["Common_Create"] = "Anlegen",
        ["Common_Apply"] = "\u00dcbernehmen",
        ["Common_Copy"] = "Kopieren",
        ["Common_Remove"] = "Entfernen",
        ["Common_ActiveDomain"] = "Aktive Domain:",
        ["Common_NoDomains"] = "Keine Domains vorhanden",
        ["Common_Type"] = "Typ",
        ["Common_Name"] = "Name",
        ["Common_Value"] = "Wert",
        ["Common_Status"] = "Status",
        ["Common_EmailAddress"] = "E-Mail-Adresse",
        ["Common_Address"] = "Adresse",
        ["Common_Target"] = "Ziel",
        ["Common_Size"] = "Gr\u00f6\u00dfe",
        ["Common_Description"] = "Beschreibung",
        ["Common_Unlimited"] = "unbegrenzt",
        ["Common_UnlimitedCap"] = "Unbegrenzt",
        ["Common_Active"] = "Aktiv",
        ["Common_Suspended"] = "Gesperrt",
        ["Common_Enabled"] = "aktiviert",
        ["Common_Disabled"] = "deaktiviert",
        ["Common_FieldSuffix"] = "{0} (Feld: {1})",

        // Sidebar / Navigation
        ["Sidebar_Disconnect"] = "Verbindung trennen",
        ["Sidebar_NotConnected"] = "Nicht verbunden",
        ["Sidebar_Language"] = "Sprache",
        ["Nav_Dashboard"] = "Dashboard",
        ["Nav_Domains"] = "Domains",
        ["Nav_Email"] = "E-Mail-Konten",
        ["Nav_Forwarders"] = "Weiterleitungen",
        ["Nav_Pointers"] = "Domain-Pointer",
        ["Nav_Spam"] = "Spam-Filter",
        ["Nav_CatchAll"] = "Catch-All",
        ["Nav_Dns"] = "DNS-Info",
        ["Nav_Verify"] = "Verifizierungs-Key",

        // Verbindung
        ["Conn_Title"] = "Mit MXroute verbinden",
        ["Conn_Desc"] = "Die Zugangsdaten findest du im mxpanel unter API-Keys (panel.mxroute.com/api-keys.php).",
        ["Conn_ServerLabel"] = "Server (X-Server)",
        ["Conn_ServerHint"] = "z. B. eagle.mxlogin.com",
        ["Conn_UserLabel"] = "Benutzername (X-Username)",
        ["Conn_KeyLabel"] = "API-Key (X-API-Key)",
        ["Conn_Remember"] = "Zugangsdaten auf diesem Rechner merken (verschl\u00fcsselt)",
        ["Conn_Connect"] = "Verbinden",
        ["Conn_Connecting"] = "Verbinde...",
        ["Conn_FillAll"] = "Bitte Server, Benutzername und API-Key ausf\u00fcllen.",
        ["Conn_Success"] = "Verbindung erfolgreich.",

        // Dashboard
        ["Dash_StorageUsage"] = "Speichernutzung",
        ["Dash_Breakdown"] = "Aufschl\u00fcsselung",
        ["Dash_Email"] = "E-Mail",
        ["Dash_Web"] = "Web",
        ["Dash_Databases"] = "Datenbanken",
        ["Dash_Backups"] = "Backups",
        ["Dash_Other"] = "Sonstiges",
        ["Dash_PerMailbox"] = "Speicher pro Postfach",
        ["Dash_UsageSummary"] = "{0} von {1}",
        ["Dash_UpdatedAt"] = "Stand: {0}",
        ["Dash_GracePeriod"] = "Kulanzfrist: noch {0} Tage (bis {1}).",

        // Domains
        ["Domains_Add"] = "+  Domain hinzuf\u00fcgen",
        ["Domains_SslActive"] = "SSL aktiv",
        ["Domains_MailHosting"] = "Mail-Hosting aktiviert",
        ["Domains_Pointers"] = "Pointer",
        ["Domains_Delete"] = "Domain l\u00f6schen",
        ["Domains_AddTitle"] = "Domain hinzuf\u00fcgen",
        ["Domains_AddPrompt"] = "Name der Domain, die dem Konto hinzugef\u00fcgt werden soll:",
        ["Domains_AddHint"] = "Hinweis: Vor dem Hinzuf\u00fcgen muss der Domain-Verifizierungs-TXT-Eintrag gesetzt sein (siehe 'Verifizierungs-Key').",
        ["Domains_Added"] = "Domain '{0}' hinzugef\u00fcgt.",
        ["Domains_DeleteTitle"] = "Domain l\u00f6schen",
        ["Domains_DeleteConfirm"] = "Domain '{0}' wirklich vom Konto entfernen? Alle zugeh\u00f6rigen E-Mail-Daten gehen verloren.",
        ["Domains_Deleted"] = "Domain '{0}' gel\u00f6scht.",
        ["Domains_MailHostingSet"] = "Mail-Hosting f\u00fcr '{0}' {1}.",
        ["Domains_LoadTitle"] = "Domains laden",

        // E-Mail-Konten
        ["Email_Create"] = "+  Konto anlegen",
        ["Email_User"] = "Benutzer",
        ["Email_Quota"] = "Quota",
        ["Email_Used"] = "Belegt",
        ["Email_SentToday"] = "Gesendet (heute)",
        ["Email_Created"] = "Konto '{0}' angelegt.",
        ["Email_Updated"] = "Konto '{0}' aktualisiert.",
        ["Email_DeleteTitle"] = "Konto l\u00f6schen",
        ["Email_DeleteConfirm"] = "E-Mail-Konto '{0}' wirklich l\u00f6schen? Alle E-Mails gehen verloren.",
        ["Email_Deleted"] = "Konto '{0}' gel\u00f6scht.",

        // Weiterleitungen
        ["Fwd_Create"] = "+  Weiterleitung anlegen",
        ["Fwd_Alias"] = "Alias",
        ["Fwd_Destinations"] = "Ziele",
        ["Fwd_Created"] = "Weiterleitung '{0}' angelegt.",
        ["Fwd_DeleteTitle"] = "Weiterleitung l\u00f6schen",
        ["Fwd_DeleteConfirm"] = "Weiterleitung '{0}' wirklich l\u00f6schen?",
        ["Fwd_Deleted"] = "Weiterleitung '{0}' gel\u00f6scht.",

        // Domain-Pointer
        ["Ptr_Create"] = "+  Pointer anlegen",
        ["Ptr_Domain"] = "Pointer-Domain",
        ["Ptr_Created"] = "Pointer '{0}' angelegt.",
        ["Ptr_DeleteTitle"] = "Pointer l\u00f6schen",
        ["Ptr_DeleteConfirm"] = "Domain-Pointer '{0}' wirklich l\u00f6schen?",
        ["Ptr_Deleted"] = "Pointer '{0}' gel\u00f6scht.",
        ["Ptr_TypeAlias"] = "Alias",
        ["Ptr_TypeRedirect"] = "Weiterleitung",

        // Spam
        ["Spam_ScoreTitle"] = "Spam-Score-Schwelle",
        ["Spam_ScoreDesc"] = "Ab diesem Score (1-50) werden E-Mails automatisch gel\u00f6scht. Niedriger = aggressiver.",
        ["Spam_Whitelist"] = "Whitelist",
        ["Spam_Blacklist"] = "Blacklist",
        ["Spam_AddEntry"] = "+ Hinzuf\u00fcgen",
        ["Spam_ScoreRange"] = "Score muss zwischen 1 und 50 liegen.",
        ["Spam_ScoreSaved"] = "Spam-Score auf {0} gesetzt.",
        ["Spam_WhitelistTitle"] = "Whitelist-Eintrag",
        ["Spam_ListPrompt"] = "E-Mail-Adresse oder Muster (Wildcards erlaubt):",
        ["Spam_WhitelistHint"] = "Beispiel: *@vertrauensw\u00fcrdig.de",
        ["Spam_WhitelistAdded"] = "Eintrag zur Whitelist hinzugef\u00fcgt.",
        ["Spam_WhitelistRemoved"] = "Whitelist-Eintrag entfernt.",
        ["Spam_BlacklistTitle"] = "Blacklist-Eintrag",
        ["Spam_BlacklistHint"] = "Beispiel: *@spam-quelle.de",
        ["Spam_BlacklistAdded"] = "Eintrag zur Blacklist hinzugef\u00fcgt.",
        ["Spam_BlacklistRemoved"] = "Blacklist-Eintrag entfernt.",

        // Catch-All
        ["CatchAll_Heading"] = "Verhalten f\u00fcr E-Mails an nicht existierende Adressen",
        ["CatchAll_Fail"] = "Ablehnen (fail)",
        ["CatchAll_FailDesc"] = "E-Mails an unbekannte Adressen werden zur\u00fcckgewiesen.",
        ["CatchAll_Blackhole"] = "Verwerfen (blackhole)",
        ["CatchAll_BlackholeDesc"] = "E-Mails werden stillschweigend gel\u00f6scht.",
        ["CatchAll_Address"] = "An Adresse weiterleiten",
        ["CatchAll_AddressDesc"] = "Alle E-Mails an ein bestimmtes Postfach weiterleiten.",
        ["CatchAll_TargetAddress"] = "Zieladresse",
        ["CatchAll_NeedAddress"] = "Bitte eine Zieladresse angeben.",
        ["CatchAll_Saved"] = "Catch-All-Einstellung gespeichert.",

        // DNS
        ["Dns_Title"] = "DNS-Konfiguration",
        ["Dns_Desc"] = "Diese Eintr\u00e4ge beim DNS-Anbieter der Domain setzen, damit E-Mail korrekt funktioniert.",
        ["Dns_Mx"] = "MX-Eintr\u00e4ge",
        ["Dns_Priority"] = "Priorit\u00e4t",
        ["Dns_Hostname"] = "Hostname",
        ["Dns_TypeName"] = "Typ / Name",
        ["Dns_Verification"] = "Domain-Verifizierung",

        // Verifizierungs-Key
        ["Verify_Desc"] = "Bevor eine neue Domain zum Konto hinzugef\u00fcgt werden kann, muss dieser TXT-Eintrag im DNS der Domain gesetzt werden. Anschlie\u00dfend 5-15 Minuten auf DNS-Propagierung warten.",
        ["Verify_TxtRecord"] = "TXT-Eintrag",
        ["Verify_Copied"] = "In die Zwischenablage kopiert.",
        ["Verify_CopyFail"] = "Konnte nicht in die Zwischenablage kopieren.",

        // Dialog: E-Mail-Konto
        ["EmailDlg_CreateTitle"] = "E-Mail-Konto anlegen",
        ["EmailDlg_EditTitle"] = "E-Mail-Konto bearbeiten",
        ["EmailDlg_Username"] = "Benutzername (vor dem @)",
        ["EmailDlg_Password"] = "Passwort",
        ["EmailDlg_NewPassword"] = "Neues Passwort (leer lassen = unver\u00e4ndert)",
        ["EmailDlg_Generate"] = "Generieren",
        ["EmailDlg_PwHint"] = "Mind. 8 Zeichen, mit Gro\u00df- und Kleinbuchstaben sowie einer Ziffer.",
        ["EmailDlg_Quota"] = "Speicher-Quota (MB, 0 = unbegrenzt)",
        ["EmailDlg_Limit"] = "Tageslimit (max. 9600)",
        ["EmailDlg_NeedUser"] = "Bitte einen Benutzernamen eingeben.",
        ["EmailDlg_BadPassword"] = "Passwort erf\u00fcllt die Anforderungen nicht (min. 8 Zeichen, Gro\u00df-/Kleinbuchstabe + Ziffer).",
        ["EmailDlg_BadQuota"] = "Quota muss eine Zahl >= 0 sein.",
        ["EmailDlg_BadLimit"] = "Tageslimit muss zwischen 0 und 9600 liegen.",

        // Dialog: Weiterleitung
        ["FwdDlg_Title"] = "Weiterleitung anlegen",
        ["FwdDlg_Alias"] = "Alias (vor dem @)",
        ["FwdDlg_Destinations"] = "Ziele (eine Adresse pro Zeile)",
        ["FwdDlg_Hint"] = "Spezielle Ziele: :blackhole: (verwerfen), :fail: (ablehnen).",
        ["FwdDlg_NeedAlias"] = "Bitte einen Alias eingeben.",
        ["FwdDlg_NeedDest"] = "Bitte mindestens ein Ziel angeben.",

        // Dialog: Pointer
        ["PtrDlg_Title"] = "Domain-Pointer anlegen",
        ["PtrDlg_Desc"] = "Eine weitere Domain, die auf die aktuelle Domain zeigt.",
        ["PtrDlg_Alias"] = "Alias",
        ["PtrDlg_Redirect"] = "Weiterleitung (Redirect)",

        // API-Fehlermeldungen
        ["Api_Timeout"] = "Zeit\u00fcberschreitung bei der Verbindung zum Server.",
        ["Api_Network"] = "Netzwerkfehler: {0}",
        ["Api_InvalidJson"] = "Ung\u00fcltige Serverantwort (kein JSON).",
        ["Api_HttpError2"] = "Fehler {0}: {1}",
        ["Api_HttpError1"] = "Fehler {0}",
    };

    public static readonly Dictionary<string, string> En = new()
    {
        // Window / App
        ["Window_Title"] = "MXroute Manager",
        ["App_Subtitle"] = "Manager",
        ["App_UnexpectedError"] = "An unexpected error occurred:\n\n{0}",

        // Common
        ["Common_Refresh"] = "\u21BB  Refresh",
        ["Common_Delete"] = "Delete",
        ["Common_Edit"] = "Edit",
        ["Common_Save"] = "Save",
        ["Common_Cancel"] = "Cancel",
        ["Common_Ok"] = "OK",
        ["Common_Create"] = "Create",
        ["Common_Apply"] = "Apply",
        ["Common_Copy"] = "Copy",
        ["Common_Remove"] = "Remove",
        ["Common_ActiveDomain"] = "Active domain:",
        ["Common_NoDomains"] = "No domains available",
        ["Common_Type"] = "Type",
        ["Common_Name"] = "Name",
        ["Common_Value"] = "Value",
        ["Common_Status"] = "Status",
        ["Common_EmailAddress"] = "Email address",
        ["Common_Address"] = "Address",
        ["Common_Target"] = "Target",
        ["Common_Size"] = "Size",
        ["Common_Description"] = "Description",
        ["Common_Unlimited"] = "unlimited",
        ["Common_UnlimitedCap"] = "Unlimited",
        ["Common_Active"] = "Active",
        ["Common_Suspended"] = "Suspended",
        ["Common_Enabled"] = "enabled",
        ["Common_Disabled"] = "disabled",
        ["Common_FieldSuffix"] = "{0} (field: {1})",

        // Sidebar / Navigation
        ["Sidebar_Disconnect"] = "Disconnect",
        ["Sidebar_NotConnected"] = "Not connected",
        ["Sidebar_Language"] = "Language",
        ["Nav_Dashboard"] = "Dashboard",
        ["Nav_Domains"] = "Domains",
        ["Nav_Email"] = "Email accounts",
        ["Nav_Forwarders"] = "Forwarders",
        ["Nav_Pointers"] = "Domain pointers",
        ["Nav_Spam"] = "Spam filter",
        ["Nav_CatchAll"] = "Catch-all",
        ["Nav_Dns"] = "DNS info",
        ["Nav_Verify"] = "Verification key",

        // Connection
        ["Conn_Title"] = "Connect to MXroute",
        ["Conn_Desc"] = "You can find your credentials in the mxpanel under API Keys (panel.mxroute.com/api-keys.php).",
        ["Conn_ServerLabel"] = "Server (X-Server)",
        ["Conn_ServerHint"] = "e.g. eagle.mxlogin.com",
        ["Conn_UserLabel"] = "Username (X-Username)",
        ["Conn_KeyLabel"] = "API key (X-API-Key)",
        ["Conn_Remember"] = "Remember credentials on this computer (encrypted)",
        ["Conn_Connect"] = "Connect",
        ["Conn_Connecting"] = "Connecting...",
        ["Conn_FillAll"] = "Please fill in server, username and API key.",
        ["Conn_Success"] = "Connected successfully.",

        // Dashboard
        ["Dash_StorageUsage"] = "Storage usage",
        ["Dash_Breakdown"] = "Breakdown",
        ["Dash_Email"] = "Email",
        ["Dash_Web"] = "Web",
        ["Dash_Databases"] = "Databases",
        ["Dash_Backups"] = "Backups",
        ["Dash_Other"] = "Other",
        ["Dash_PerMailbox"] = "Storage per mailbox",
        ["Dash_UsageSummary"] = "{0} of {1}",
        ["Dash_UpdatedAt"] = "As of: {0}",
        ["Dash_GracePeriod"] = "Grace period: {0} days remaining (until {1}).",

        // Domains
        ["Domains_Add"] = "+  Add domain",
        ["Domains_SslActive"] = "SSL active",
        ["Domains_MailHosting"] = "Mail hosting enabled",
        ["Domains_Pointers"] = "Pointers",
        ["Domains_Delete"] = "Delete domain",
        ["Domains_AddTitle"] = "Add domain",
        ["Domains_AddPrompt"] = "Name of the domain to add to the account:",
        ["Domains_AddHint"] = "Note: Before adding, the domain verification TXT record must be set (see 'Verification key').",
        ["Domains_Added"] = "Domain '{0}' added.",
        ["Domains_DeleteTitle"] = "Delete domain",
        ["Domains_DeleteConfirm"] = "Really remove domain '{0}' from the account? All associated email data will be lost.",
        ["Domains_Deleted"] = "Domain '{0}' deleted.",
        ["Domains_MailHostingSet"] = "Mail hosting for '{0}' {1}.",
        ["Domains_LoadTitle"] = "Loading domains",

        // Email accounts
        ["Email_Create"] = "+  Create account",
        ["Email_User"] = "User",
        ["Email_Quota"] = "Quota",
        ["Email_Used"] = "Used",
        ["Email_SentToday"] = "Sent (today)",
        ["Email_Created"] = "Account '{0}' created.",
        ["Email_Updated"] = "Account '{0}' updated.",
        ["Email_DeleteTitle"] = "Delete account",
        ["Email_DeleteConfirm"] = "Really delete email account '{0}'? All emails will be lost.",
        ["Email_Deleted"] = "Account '{0}' deleted.",

        // Forwarders
        ["Fwd_Create"] = "+  Create forwarder",
        ["Fwd_Alias"] = "Alias",
        ["Fwd_Destinations"] = "Destinations",
        ["Fwd_Created"] = "Forwarder '{0}' created.",
        ["Fwd_DeleteTitle"] = "Delete forwarder",
        ["Fwd_DeleteConfirm"] = "Really delete forwarder '{0}'?",
        ["Fwd_Deleted"] = "Forwarder '{0}' deleted.",

        // Domain pointers
        ["Ptr_Create"] = "+  Create pointer",
        ["Ptr_Domain"] = "Pointer domain",
        ["Ptr_Created"] = "Pointer '{0}' created.",
        ["Ptr_DeleteTitle"] = "Delete pointer",
        ["Ptr_DeleteConfirm"] = "Really delete domain pointer '{0}'?",
        ["Ptr_Deleted"] = "Pointer '{0}' deleted.",
        ["Ptr_TypeAlias"] = "Alias",
        ["Ptr_TypeRedirect"] = "Redirect",

        // Spam
        ["Spam_ScoreTitle"] = "Spam score threshold",
        ["Spam_ScoreDesc"] = "Emails scoring at or above this value (1-50) are deleted automatically. Lower = more aggressive.",
        ["Spam_Whitelist"] = "Whitelist",
        ["Spam_Blacklist"] = "Blacklist",
        ["Spam_AddEntry"] = "+ Add",
        ["Spam_ScoreRange"] = "Score must be between 1 and 50.",
        ["Spam_ScoreSaved"] = "Spam score set to {0}.",
        ["Spam_WhitelistTitle"] = "Whitelist entry",
        ["Spam_ListPrompt"] = "Email address or pattern (wildcards allowed):",
        ["Spam_WhitelistHint"] = "Example: *@trusted.com",
        ["Spam_WhitelistAdded"] = "Entry added to whitelist.",
        ["Spam_WhitelistRemoved"] = "Whitelist entry removed.",
        ["Spam_BlacklistTitle"] = "Blacklist entry",
        ["Spam_BlacklistHint"] = "Example: *@spam-source.com",
        ["Spam_BlacklistAdded"] = "Entry added to blacklist.",
        ["Spam_BlacklistRemoved"] = "Blacklist entry removed.",

        // Catch-all
        ["CatchAll_Heading"] = "Behavior for emails to non-existent addresses",
        ["CatchAll_Fail"] = "Reject (fail)",
        ["CatchAll_FailDesc"] = "Emails to unknown addresses are rejected.",
        ["CatchAll_Blackhole"] = "Discard (blackhole)",
        ["CatchAll_BlackholeDesc"] = "Emails are silently discarded.",
        ["CatchAll_Address"] = "Forward to address",
        ["CatchAll_AddressDesc"] = "Forward all emails to a specific mailbox.",
        ["CatchAll_TargetAddress"] = "Target address",
        ["CatchAll_NeedAddress"] = "Please provide a target address.",
        ["CatchAll_Saved"] = "Catch-all setting saved.",

        // DNS
        ["Dns_Title"] = "DNS configuration",
        ["Dns_Desc"] = "Set these records at the domain's DNS provider so email works correctly.",
        ["Dns_Mx"] = "MX records",
        ["Dns_Priority"] = "Priority",
        ["Dns_Hostname"] = "Hostname",
        ["Dns_TypeName"] = "Type / Name",
        ["Dns_Verification"] = "Domain verification",

        // Verification key
        ["Verify_Desc"] = "Before a new domain can be added to the account, this TXT record must be set in the domain's DNS. Then wait 5-15 minutes for DNS propagation.",
        ["Verify_TxtRecord"] = "TXT record",
        ["Verify_Copied"] = "Copied to clipboard.",
        ["Verify_CopyFail"] = "Could not copy to clipboard.",

        // Dialog: email account
        ["EmailDlg_CreateTitle"] = "Create email account",
        ["EmailDlg_EditTitle"] = "Edit email account",
        ["EmailDlg_Username"] = "Username (before the @)",
        ["EmailDlg_Password"] = "Password",
        ["EmailDlg_NewPassword"] = "New password (leave empty = unchanged)",
        ["EmailDlg_Generate"] = "Generate",
        ["EmailDlg_PwHint"] = "At least 8 characters, with upper- and lowercase letters and a digit.",
        ["EmailDlg_Quota"] = "Storage quota (MB, 0 = unlimited)",
        ["EmailDlg_Limit"] = "Daily limit (max. 9600)",
        ["EmailDlg_NeedUser"] = "Please enter a username.",
        ["EmailDlg_BadPassword"] = "Password does not meet the requirements (min. 8 chars, upper/lowercase + digit).",
        ["EmailDlg_BadQuota"] = "Quota must be a number >= 0.",
        ["EmailDlg_BadLimit"] = "Daily limit must be between 0 and 9600.",

        // Dialog: forwarder
        ["FwdDlg_Title"] = "Create forwarder",
        ["FwdDlg_Alias"] = "Alias (before the @)",
        ["FwdDlg_Destinations"] = "Destinations (one address per line)",
        ["FwdDlg_Hint"] = "Special destinations: :blackhole: (discard), :fail: (reject).",
        ["FwdDlg_NeedAlias"] = "Please enter an alias.",
        ["FwdDlg_NeedDest"] = "Please provide at least one destination.",

        // Dialog: pointer
        ["PtrDlg_Title"] = "Create domain pointer",
        ["PtrDlg_Desc"] = "An additional domain that points to the current domain.",
        ["PtrDlg_Alias"] = "Alias",
        ["PtrDlg_Redirect"] = "Redirect",

        // API error messages
        ["Api_Timeout"] = "The connection to the server timed out.",
        ["Api_Network"] = "Network error: {0}",
        ["Api_InvalidJson"] = "Invalid server response (not JSON).",
        ["Api_HttpError2"] = "Error {0}: {1}",
        ["Api_HttpError1"] = "Error {0}",
    };
}
