using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace synchronizer
{
    public class OutlookService : ICalendarService
    {
        private Microsoft.Office.Interop.Outlook.Application oApp = null;
        private Microsoft.Office.Interop.Outlook.NameSpace mapiNamespace = null;
        private Microsoft.Office.Interop.Outlook.MAPIFolder calendarFolder = null;
        private Microsoft.Office.Interop.Outlook.Items outlookCalendarItems = null;

        private void initOutlookService()
        {
            oApp = new Microsoft.Office.Interop.Outlook.Application();
            mapiNamespace = oApp.GetNamespace("MAPI");
            ;
            calendarFolder =
            mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);
            outlookCalendarItems = calendarFolder.Items;
            outlookCalendarItems.Sort("[Start]");
            outlookCalendarItems.IncludeRecurrences = true;
            //outlookCalendarItems
        }
        
        public void PushEvents(List<SynchronEvent> events)
        {
            initOutlookService();
            foreach (var eventToPush in events)
            {
                var current = new Converter().ConvertSynchronToOutlook(eventToPush);
                current.Save();   
            }
            
        }

        public void DeleteEvents(List<SynchronEvent> events)
        {
            initOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (string.IsNullOrEmpty(item.Mileage))
                    continue;
                foreach (var eventToDelete in events)
                {
                    if(item.Mileage == eventToDelete.GetId())
                        item.Delete();
                }
            }
        }

        public List<SynchronEvent> GetAllItems(DateTime startTime, DateTime finishTime)
        {
            var resultList = new List<SynchronEvent>();

            initOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (item.IsRecurring)
                {
                    resultList.Add(new Converter().ConvertOutlookToMyEvent(item));
                    /*var start = item.Start;
                    var rp = item.GetRecurrencePattern();
                    var finish = finishTime;
                    var oc = 0;
                    //item.
                    for(var cur = start; cur<=finish && ( oc < rp.Occurrences || rp.NoEndDate); cur = cur.AddDays(rp.Interval), oc++)
                    {
                        var curCurringSync = new Converter().ConvertOutlookToMyEvent(item);
                        curCurringSync.SetStart(cur);
                        curCurringSync.SetId(item.GlobalAppointmentID + oc.ToString());
                        curCurringSync.SetFinish(cur.AddMinutes(item.Duration));
                        resultList.Add(curCurringSync);
                    }*/
                }
                else
                    resultList.Add(new Converter().ConvertOutlookToMyEvent(item));
            }
            return resultList;

        }
        public void UpdateEvents(List<SynchronEvent> needToUpdate)
        {
            initOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (string.IsNullOrEmpty(item.Mileage))
                    continue;
                foreach (var eventToUpdate in needToUpdate)
                {
                    if (item.Mileage == eventToUpdate.GetId())
                    {
                        string buf = "";
                        List<string> AllParticipants = eventToUpdate.GetParticipants();

                        for (int i = 0; i < AllParticipants.Count; ++i)
                        {
                            if (i + 1 < AllParticipants.Count)
                                buf = buf + AllParticipants[i] + "; ";
                            else
                                buf = buf + AllParticipants[i];
                        }
                        item.RequiredAttendees = buf;
                        item.Subject = eventToUpdate.GetSubject();
                        item.Start = eventToUpdate.GetStart();
                        item.End = eventToUpdate.GetFinish();
                        item.Body = eventToUpdate.GetDescription();
                        item.Location = eventToUpdate.GetLocation();
                        item.Save();
                    }
                }
            }
        }
    }
}
