using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace Acklann.VCalendar
{
    public class Calendar : NameValueCollection
    {
        public Calendar()
        {
            Events = new List<Event>();
        }

        public List<Event> Events { get; set; }

        public void WriteTo(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine($"BEGIN:VCALENDAR");
                writer.WriteLine($"VERSION:2.0");
                writer.WriteLine($"CALSCALE:GREGORIAN");
                writer.WriteLine($"METHOD:PUBLISH");
                writer.WriteLine($"VERSION:2.0");

                foreach (Event item in Events)
                {
                    item.WriteTo(writer);
                }

                writer.WriteLine($"END:VCALENDAR");
            }
        }

        public void SaveTo(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));

            string folder = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                WriteTo(stream);
            }
        }

        public static Calendar ReadFile(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException($"Could not find file at '{filePath}'.");
            static void read(string x, out string name, out string value)
            {
                string[] parts = x.Split(':', System.StringSplitOptions.RemoveEmptyEntries);
                name = (parts.Length > 0 ? parts[0].ToUpperInvariant().Trim() : default);
                value = (parts.Length > 1 ? parts[1].Trim() : default);
            }

            Event newEvent = null;
            var calendar = new Calendar();
            ReaderContext context = ReaderContext.Calendar;

            using (var reader = new StreamReader(filePath))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine()?.Trim();
                    context = GetContext(line, context);
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    read(line, out string name, out string value);
                    calendar.Add(name, value);

                    switch (context)
                    {
                        case ReaderContext.BeginEvent:
                            newEvent = new Event();
                            calendar.Events.Add(newEvent);
                            context = ReaderContext.Event;
                            continue;

                        case ReaderContext.Event:
                            newEvent?.Add(name, value);
                            break;

                        case ReaderContext.EndEvent:
                            context = ReaderContext.Calendar;
                            break;

                        case ReaderContext.Calendar:
                            calendar.Add(name, value);
                            break;
                    }
                }
            }

            return calendar;
        }

        private static ReaderContext GetContext(string line, ReaderContext context)
        {
            switch (line)
            {
                case "BEGIN:VEVENT": return ReaderContext.BeginEvent;
                case "END:VEVENT": return ReaderContext.EndEvent;
                default: return context;
            }
        }
    }
}