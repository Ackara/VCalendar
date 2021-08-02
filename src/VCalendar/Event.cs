using System;
using System.Collections.Specialized;

namespace Acklann.VCalendar
{
    public class Event : NameValueCollection
    {
        public string UID
        {
            get => base.Get("UID");
            set => base.Set("UID", value);
        }

        public string Summary
        {
            get => base.Get("SUMMARY");
            set => base.Set("SUMMARY", value);
        }

        public string Description
        {
            get => base.Get("DESCRIPTION");
            set => base.Set("DESCRIPTION", value);
        }

        public bool Transparent
        {
            get => !string.IsNullOrEmpty(base.Get("TRANSP"));
            set => base.Set("TRANSP", (value ? "TRANSPARENT" : null));
        }

        public DateTime DTStart
        {
            get => DateTime.TryParseExact(Get("DTSTART"), "yyyyMMdd", default, System.Globalization.DateTimeStyles.None, out DateTime date) ? date : default;
            set => Set("DTSTART", value.ToString("yyyyMMdd"));
        }

        public DateTime DTEnd
        {
            get => DateTime.TryParseExact(Get("DTEND"), "yyyyMMdd", default, System.Globalization.DateTimeStyles.None, out DateTime date) ? date : default;
            set => Set("DTEND", value.ToString("yyyyMMdd"));
        }

        internal void WriteTo(System.IO.StreamWriter writer)
        {
            writer.WriteLine("BEGIN:VEVENT");
            writer.WriteLine($"UID:{UID}");
            writer.WriteLine($"SUMMARY:{Summary}");
            writer.WriteLine($"DESCRIPTION:{Description}");
            writer.WriteLine($"DTSTART:{Get("DTSTART")}");
            writer.WriteLine($"DTEND:{Get("DTEND")}");
            if (Transparent) writer.WriteLine($"TRANSP:TRANSPARENT");
            writer.WriteLine("END:VEVENT");
        }
    }
}