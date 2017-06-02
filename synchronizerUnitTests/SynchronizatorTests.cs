using synchronizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;

namespace synchronizerUnitTests
{
    public class SynchronizatorTests
    {
        [Fact]
        public void CheckOnNonModyfing_NonOfThen()
        {
            var synchronizer = new Syncronizator();
            var calendarA = new CalendarServiceStub();
            var calendarB = new CalendarServiceStub();
            DateTime startData = DateTime.Now;
            DateTime finishDate = startData.AddMonths(1);
            synchronizer.ApplyAllUpdates(startData, finishDate, new List<ICalendarService> { calendarA, calendarB });

            Assert.True(calendarA.GetAllItems(startData, finishDate).Count == calendarB.GetAllItems(startData, finishDate).Count
                && calendarB.GetAllItems(startData, finishDate).Count == 0);
        }
        [Fact]
        public void badTest()
        {
            Assert.True(false);
        }
        [Fact]
        public void TwoCalendarsOneNonEmpty_AddedToNext1()
        {
            var synchronizer = new Syncronizator();
            var calendarA = new CalendarServiceStub();
            var calendarB = new CalendarServiceStub();
            DateTime startData = DateTime.Now;
            calendarA.AddEvent(new SynchronEvent().SetId("1234").SetStart(DateTime.Now).SetFinish(DateTime.Now.AddDays(1))
                .SetPlacement("1").SetSource("1"));
            DateTime finishDate = startData.AddMonths(1);
            synchronizer.ApplyAllUpdates(startData, finishDate, new List<ICalendarService> { calendarA, calendarB });

            Assert.True(calendarA.GetAllItems(startData, finishDate).Count == calendarB.GetAllItems(startData, finishDate).Count
                && calendarB.GetAllItems(startData, finishDate).Count == 1);
        }

        [Fact]
        public void NeedToDelete_Deleted()
        {
            var synchronizer = new Syncronizator();
            var calendarA = new CalendarServiceStub();
            var calendarB = new CalendarServiceStub();
            DateTime startData = DateTime.Now;
            calendarA.AddEvent(new SynchronEvent().SetId("1234").SetStart(DateTime.Now).SetFinish(DateTime.Now.AddDays(1))
                .SetPlacement("1").SetSource("2"));
            DateTime finishDate = startData.AddMonths(1);
            synchronizer.ApplyAllUpdates(startData, finishDate, new List<ICalendarService> { calendarA, calendarB });

            Assert.True(calendarA.GetAllItems(startData, finishDate).Count == calendarB.GetAllItems(startData, finishDate).Count
                && calendarB.GetAllItems(startData, finishDate).Count == 0);
        }

        [Fact]
        public void NeedToUpdate_Updated()
        {
            var synchronizer = new Syncronizator();
            var calendarA = new CalendarServiceStub();
            var calendarB = new CalendarServiceStub();
            DateTime startData = DateTime.Now;
            DateTime finishDate = startData.AddMonths(1);
            var curEvent = new SynchronEvent().SetId("1234").SetStart(startData.AddMinutes(15)).SetFinish(finishDate)
                .SetPlacement("1").SetSource("2");

            calendarA.AddEvent(curEvent);
            calendarB.AddEvent(curEvent.SetPlacement("2").SetSubject("check"));

            
            synchronizer.ApplyAllUpdates(startData, finishDate, new List<ICalendarService> { calendarA, calendarB });

            Assert.True(calendarA.GetAllItems(startData, finishDate)[0].GetSubject() == "check");
        }
    }

    public class CalendarServiceStub : ICalendarService
    {
        public List<SynchronEvent> Events { get; private set; }
        private string id;
        private bool SameId(SynchronEvent cur)
        {
            return cur.GetId() == id;
        }
        public void AddEvent(SynchronEvent toAdd)
        {
            Events.Add(toAdd);
        }
        public CalendarServiceStub()
        {
            Events = new List<SynchronEvent>();
        }
        public void DeleteEvents(List<SynchronEvent> events)
        {
            foreach(var curEvent in events)
            {
                id = curEvent.GetId();
                Events.RemoveAll(SameId);
            }  
        }

        public List<SynchronEvent> GetAllItems(DateTime startTime, DateTime finishTime)
        {
            return Events;
        }

        public void PushEvents(List<SynchronEvent> events)
        {
            foreach (var curEvent in events)
                Events.Add(curEvent);
        }

        public void UpdateEvents(List<SynchronEvent> needToUpdate)
        {
            foreach (var curEvent in needToUpdate)
            {
                id = curEvent.GetId();
                for (int i = 0; i < Events.Count; ++i)
                    if (SameId(Events[i]))
                        Events[i] = curEvent;
            }
        }
    }
}
