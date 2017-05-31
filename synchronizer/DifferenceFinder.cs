using System.Collections.Generic;

namespace synchronizer
{
    public class DifferenceFinder
    {
        private bool IfNonExist(SynchronEvent needToCheck, List<SynchronEvent> events)
        {
            var notExist = true;
            foreach (var curevent in events)
            {
                if (curevent.GetId() == needToCheck.GetId())
                    notExist = false;
            }
            return notExist;
        }
        private bool IfExist(SynchronEvent needToCheck, List<SynchronEvent> events)
        {
            var exist = false;
            foreach (var curevent in events)
            {
                if (curevent.GetId() == needToCheck.GetId())
                    exist = true;
            }
            return exist;
        }
        private bool NeedToUpdate(SynchronEvent standartEvent, SynchronEvent compareEvent)
        {
            return !standartEvent.CompareOnEqual(compareEvent);
        }
        public List<SynchronEvent> GetDifferenceToPush(List<SynchronEvent> sourceList, List<SynchronEvent> targetList)
        {
            var difference = new List<SynchronEvent>();
            foreach (var eventToCheck in sourceList)
            {
                if(IfNonExist(eventToCheck, targetList) && eventToCheck.GetSource() == eventToCheck.GetPlacement())
                    difference.Add(eventToCheck);
            }
            return difference;
        }

        public List<SynchronEvent> GetDifferenceToDelete(List<SynchronEvent> needToCheck, List<SynchronEvent> standard)
        {
            var difference = new List<SynchronEvent>();
            foreach (var eventToCheck in needToCheck)
            {
                if (eventToCheck.GetSource() == eventToCheck.GetPlacement()) continue;
                if(IfNonExist(eventToCheck, standard))
                    difference.Add(eventToCheck);
            }
            return difference;
        }

        public List<SynchronEvent> GetDifferenceToUpdate(List<SynchronEvent> needToCheck, List<SynchronEvent> standard)
        {
            var difference = new List<SynchronEvent>();
            foreach(var eventToCheckInList1 in needToCheck)
            {
                foreach(var eventToCheckInList2 in standard)
                {
                    if (eventToCheckInList2.GetSource() != eventToCheckInList2.GetPlacement())
                        continue;
                    if (eventToCheckInList2.GetId() == eventToCheckInList1.GetId() && NeedToUpdate(eventToCheckInList2, eventToCheckInList1))
                    {
                        difference.Add(eventToCheckInList2);
                        break;
                    }
                }
            }
            return difference;
        }
    }
}
