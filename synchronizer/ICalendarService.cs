using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synchronizer
{
    public interface ICalendarService
    {
        void PushEvents(List<SynchronEvent> events);
        
        void DeleteEvents(List<SynchronEvent> events);

        List<SynchronEvent> GetAllItems(DateTime startTime, DateTime finishTime);

        void UpdateEvents(List<SynchronEvent> needToUpdate);
    }
}
