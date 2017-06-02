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
        private DateTime minTime;
        private DateTime maxTime;
        private bool ifAlreadyInit = false;
        private string GetDateInString(DateTime curDate)
        {
            string result = "";

            result += curDate.Day.ToString() + "/" +curDate.Month.ToString() + "/" + curDate.Year.ToString();
            result += " " + curDate.Hour.ToString() + ":" + curDate.Minute.ToString();
            return result;
        }
        private void InitOutlookService()
        {
            if (!ifAlreadyInit)
            {
                oApp = new Microsoft.Office.Interop.Outlook.Application();
                mapiNamespace = oApp.GetNamespace("MAPI");
                ;
                calendarFolder =
                mapiNamespace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderCalendar);
                outlookCalendarItems = calendarFolder.Items;

                outlookCalendarItems.Sort("[Start]");
                outlookCalendarItems.IncludeRecurrences = true;

                string s1 = GetDateInString(minTime);
                string s2 = GetDateInString(maxTime);
                var filterString = "[Start] >= '" + s1 + "' AND [End] < '" + s2 + "'";
                outlookCalendarItems = outlookCalendarItems.Restrict(filterString);
                ifAlreadyInit = true;
            }
        }
        
        public void PushEvents(List<SynchronEvent> events)
        {
            InitOutlookService();
            foreach (var eventToPush in events)
            {
                var current = new Converter().ConvertSynchronToOutlook(eventToPush);
                current.Save();   
            }
            
        }

        public void DeleteEvents(List<SynchronEvent> events)
        {
            InitOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (item.Start > maxTime)
                    break;
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
            minTime = startTime;

            minTime = minTime.AddHours(-minTime.Hour);
            minTime = minTime.AddMinutes(-minTime.Minute);
            minTime = minTime.AddSeconds(-minTime.Second);
            minTime = minTime.AddMilliseconds(-minTime.Millisecond - 1);

            maxTime = finishTime;
            InitOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (item.Start > finishTime)
                    break;
                if (item.IsRecurring)
                {
                    resultList.Add(new Converter().ConvertOutlookToMyEvent(item));
                }
                else
                    resultList.Add(new Converter().ConvertOutlookToMyEvent(item));
            }
            return resultList;

        }
        public void UpdateEvents(List<SynchronEvent> needToUpdate)
        {
            InitOutlookService();

            foreach (Microsoft.Office.Interop.Outlook.AppointmentItem item in outlookCalendarItems)
            {
                if (item.Start > maxTime)
                    break;
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
