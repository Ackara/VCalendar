using System.Collections.Generic;

namespace Acklann.VCalendar
{
    public interface ICalendarComponent
    {
        List<Property> Properties { get; }
    }
}