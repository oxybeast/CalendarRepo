using System.Linq;
using Google.Apis.Calendar.v3.Data;
using org.pdfclown.util;
using outlook2 = Microsoft.Office.Interop.Outlook;
using System.Text;
using System;
using System.Collections.Generic;

namespace synchronizer
{
    public class Converter
    {
        private readonly string _outlook = "Outlook";
        private readonly string _google = "Google";
        public SynchronEvent ConvertOutlookToMyEvent(Microsoft.Office.Interop.Outlook.AppointmentItem outlookItem)
        {
            var result = new SynchronEvent()
                .SetStart(outlookItem.Start)
                .SetDuration(outlookItem.Duration)
                .SetLocation(outlookItem.Location)
                .SetSubject(outlookItem.Subject)
                .SetCompanions(outlookItem.RequiredAttendees)
                .SetId(outlookItem.GlobalAppointmentID)
                .SetFinish(outlookItem.End)
                .SetSource(_outlook)
                .SetDescription(outlookItem.Body)
                .SetPlacement(_outlook)
                .SetAllDay(outlookItem.AllDayEvent);
            
            if (!string.IsNullOrEmpty(outlookItem.Mileage))
            {
                result.SetId(outlookItem.Mileage);
                if (outlookItem.Mileage.IndexOf(_outlook) > -1)
                    result.SetSource(_outlook);
                else
                {
                    result.SetSource(_google);
                    outlookItem.Categories = "Orange Category";
                    outlookItem.Save();
                }
            }
            else
            {
                Guid id = Guid.NewGuid();
                outlookItem.Mileage = _outlook + id.ToString();
                outlookItem.Save();
                result.SetSource(_outlook);
                result.SetId(outlookItem.Mileage);
            }
            return result;
        }

        public Event ConvertMyEventToGoogle(SynchronEvent synchronEvent)
        {
            var eventDateTime = new EventDateTime
            {
                DateTime = synchronEvent.GetStart(),
                TimeZone = "Europe/Moscow"
            };
            var eventDateTimeEnd = new EventDateTime
            {
                DateTime = synchronEvent.GetFinish(),
                TimeZone = "Europe/Moscow"
            };
            var googleEvent = new Event
            {
                Location = synchronEvent.GetLocation(),
                Summary = synchronEvent.GetSubject(),
                Start = eventDateTime,
                End = eventDateTimeEnd,
                Description = synchronEvent.GetDescription(),
                ColorId = "11",
            };

            if(synchronEvent.GetAllDay())
            {
                googleEvent.Start.DateTime = null;
                googleEvent.Start.DateTimeRaw = null;
                googleEvent.Start.Date = synchronEvent.GetStart().Year.ToString() + "-" +"0" + synchronEvent.GetStart().Month.ToString() + "-" + synchronEvent.GetStart().Day.ToString();
                googleEvent.End.DateTime = null;
                googleEvent.End.DateTimeRaw = null;
                googleEvent.End.Date = synchronEvent.GetStart().Year.ToString() + "-" + "0" + synchronEvent.GetStart().Month.ToString() + "-" + synchronEvent.GetStart().AddDays(1).Day.ToString();

            }
            if (synchronEvent.GetSource() == _outlook)
            {
                googleEvent.ExtendedProperties = new Event.ExtendedPropertiesData
                {
                    Shared = new BiDictionary<string, string> {{_outlook, synchronEvent.GetId()}}
                };
            }
           
            EventAttendee[] attendees = new EventAttendee[synchronEvent.GetParticipants().Count];
            List<string> AllParticipants = synchronEvent.GetParticipants();
            for(int i = 0; i < AllParticipants.Count;++i)
            {
                var evAt = new EventAttendee
                {
                    Email = AllParticipants[i],
                    ResponseStatus = "needsAction"
                };
                attendees[i] = evAt;
            }
            googleEvent.Attendees = attendees;
            //googleEvent.Attendees.Add(evAt);
            return googleEvent;
        }

        public SynchronEvent ConvertGoogleEventToSynchronEvent(Event googleEvent)
        {
            var result = new SynchronEvent();
            if (googleEvent.Attendees != null)
            {
                foreach (var email in googleEvent.Attendees)
                    result.AddCompanions(email.Email);
            }
            if(googleEvent.Start.Date != null)
            {
                result.SetAllDay(true);
                string date = googleEvent.Start.Date;
                string[] q = date.Split('-');
                var year = int.Parse(q[0]);
                var month = int.Parse(q[1]);
                var day = int.Parse(q[2]);
                System.DateTime buf = new System.DateTime(year, month, day);
                result.SetStart(buf);
                result.SetFinish(buf.AddDays(1));
            }
            else
            {
                result.SetStart(googleEvent.Start.DateTime.Value)
                .SetFinish(googleEvent.End.DateTime.Value);
            }
                result
                .SetLocation(googleEvent.Location)
                .SetDescription(googleEvent.Description)
                .SetSubject(googleEvent.Summary)
                .SetSource(_google)
                .SetId(googleEvent.Id)
                .SetPlacement(_google);
            
            if (googleEvent.ExtendedProperties != null && googleEvent.ExtendedProperties.Shared != null 
                && googleEvent.ExtendedProperties.Shared.ContainsKey(_outlook))
            {
                result.SetId(googleEvent.ExtendedProperties.Shared[_outlook]);
                result.SetSource(_outlook);
            }
            else
            {
                result.SetSource(_google);
            }
            return result;
        }

        public Microsoft.Office.Interop.Outlook.AppointmentItem ConvertSynchronToOutlook(SynchronEvent synchronEvent)
        {
            var outlookApp = new outlook2.Application();

            var result =  (outlook2.AppointmentItem)outlookApp.CreateItem(outlook2.OlItemType.olAppointmentItem);

            result.Start = synchronEvent.GetStart();
            result.End = synchronEvent.GetFinish();
            result.Subject = synchronEvent.GetSubject();
            result.Location = synchronEvent.GetLocation();
            result.Body = synchronEvent.GetDescription();
            result.AllDayEvent = synchronEvent.GetAllDay();

            string buf = "";
            List<string> AllParticipants = synchronEvent.GetParticipants();

            for(int i = 0; i < AllParticipants.Count;++i)
            {
                if (i + 1 < AllParticipants.Count)
                    buf = buf + AllParticipants[i] + "; ";
                else
                    buf = buf + AllParticipants[i];
            }
            result.RequiredAttendees = buf;
            result.ResponseRequested = true;
            if (synchronEvent.GetSource() == _google)
            {
                result.Mileage = synchronEvent.GetId();
                result.Categories = "Orange Category";
            }
            return result;
        }
    }
}
