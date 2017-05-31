using System;
using System.Collections.Generic;

namespace synchronizer
{
    public class EventsSiever
    {
        public List<SynchronEvent> SieveEventsOnPeriodOfTime(DateTime startDate, DateTime finishDate,
            List<SynchronEvent> events)
        {
            var result = new List<SynchronEvent>();
            foreach (var currentEvent in events)
            {
                var currentStart = startDate;
                
                if(currentEvent.GetAllDay())
                {
                    currentStart = currentStart.AddHours(-currentStart.Hour);
                    currentStart = currentStart.AddMinutes(-currentStart.Minute);
                    currentStart = currentStart.AddSeconds(-currentStart.Second);
                    currentStart = currentStart.AddMilliseconds(-currentStart.Millisecond-1);
                }
                if(currentStart <= currentEvent.GetStart() && currentEvent.GetStart() <= finishDate)
                    result.Add(currentEvent);
            }
            return result;
        }
    }
}
