using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace synchronizer
{
    public class Syncronizator
    {
        public void ApplyAllUpdates(DateTime startDate, DateTime finishDate, List<ICalendarService> calendars)
        {
            List<List<SynchronEvent>> MeetingsInTheCalendars = new List<List<SynchronEvent>>();
            
            foreach(var currentCalendar in calendars)
                MeetingsInTheCalendars.Add(new EventsSiever().SieveEventsOnPeriodOfTime(startDate, finishDate, currentCalendar.GetAllItems(startDate, finishDate)));
            
            for(int i = 0; i < calendars.Count;++i)
            {
                for (int j = 0; j < calendars.Count; ++j)
                {
                    if (i == j)
                        continue;
                    OneWaySync(calendars[i], MeetingsInTheCalendars[j], MeetingsInTheCalendars[i]);
                }
            }
        }

        private void OneWaySync(ICalendarService targetCalendarService, List<SynchronEvent> sourceMeetings, List<SynchronEvent> targetMeetings)
        {
            var nonExistInTarget = new DifferenceFinder().GetDifferenceToPush(sourceMeetings, targetMeetings);
            var needToDeleteInTarget =
                new DifferenceFinder().GetDifferenceToDelete(targetMeetings, sourceMeetings);
            var needToUpdateInTarget = new DifferenceFinder().GetDifferenceToUpdate(targetMeetings, sourceMeetings);

            targetCalendarService.PushEvents(nonExistInTarget);
            targetCalendarService.DeleteEvents(needToDeleteInTarget);
            targetCalendarService.UpdateEvents(needToUpdateInTarget);
        }
    }
}
