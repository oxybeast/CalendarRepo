using System;
using System.Collections.Generic;

namespace synchronizer
{
    public class SynchronEvent
    {
        private string subject;
        private DateTime startTime;
        private DateTime finishTime;
        private int duration;
        private string location;
        private List<string> companions;
        private string description;
        private string source;
        private string placement;
        private string id;
        private bool allDayEvent;

        public SynchronEvent()
        {
            startTime = new DateTime();
            duration = 0;
            location = "";
            placement = "";
            id = "";
            allDayEvent = false;
            source = "";
            description = " ";
            subject = "";
            companions = new List<string>();
        }

        public SynchronEvent SetStart(DateTime Date)
        {
            startTime = Date;
            return this;
        }
        public SynchronEvent SetFinish(DateTime Date)
        {
            finishTime = Date;
            return this;
        }
        public SynchronEvent SetPlacement(string placement)
        {
            this.placement = placement;
            return this;
        }

        public SynchronEvent SetAllDay(bool isAllDay)
        {
            allDayEvent = isAllDay;
            return this;
        }
        public SynchronEvent SetId(string id)
        {
            this.id = id;
            return this;
        }

        public SynchronEvent SetDescription(string Description)
        {
            description = Description;
            return this;
        }
        public SynchronEvent SetDuration(int Duration)
        {
            duration = Duration;
            return this;
        }

        public SynchronEvent SetLocation(string place)
        {
            location = place;
            return this;
        }

        public SynchronEvent SetSubject(string Subject)
        {
            subject = Subject;
            return this;
        }

        public SynchronEvent SetSource(string source)
        {
            this.source = source;
            return this;
        }

        public string GetSource()
        {
            return source;
        }
        private bool IsNotEmail(string s)
        {
            return s.IndexOf("@") < 0;
        }
        private List<string> ParseParticipantsString(string stringOfParticipants)
        {
            if (stringOfParticipants == null)
                return new List<string>();

            var result = new List<string>(stringOfParticipants.Split(';'));

            result.RemoveAll(IsNotEmail);

            for(int i = 0; i < result.Count;++i)
            {
               result[i] = result[i].TrimStart(new char[1] { ' ' });
               result[i] = result[i].TrimEnd(new char[1] { ' ' });
            }
            return result;
        }
        public SynchronEvent AddCompanions(string participant)
        {
            companions.Add(participant);
            return this;
        }
        public SynchronEvent SetCompanions(string allParticipants)
        {
            companions = ParseParticipantsString(allParticipants);
            return this;
        }
        
        public DateTime GetStart()
        {
            return startTime;
        }
        public List<string> GetParticipants()
        {
            companions.Sort();
            return companions;
        }
        public DateTime GetFinish()
        {
            return finishTime;
        }
        public bool GetAllDay()
        {
            return allDayEvent;
        }
        public string GetPlacement()
        {
            return placement;
        }
        public string GetSubject()
        {
            return subject;
        }

        public string GetLocation()
        {
            return location;
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetId()
        {
            return id;
        }

        public bool CompareOnEqual(SynchronEvent compareEvent)
        {
            bool result = true;
            result = this.GetId() == compareEvent.GetId() && this.GetLocation() == compareEvent.GetLocation() && this.GetSubject() == compareEvent.GetSubject() &&
                this.GetStart() == compareEvent.GetStart() && this.GetFinish() == compareEvent.GetFinish() && this.GetDescription() == compareEvent.GetDescription();
            result &= this.GetParticipants().Count == compareEvent.GetParticipants().Count;
            for (int i = 0; i < companions.Count && result; ++i)
                result &= companions[i] == compareEvent.companions[i];
            return result;
        }
    }
}
