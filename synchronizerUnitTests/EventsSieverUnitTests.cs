using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using synchronizer;
using Xunit;
using Assert = Xunit.Assert;

namespace synchronizerUnitTests
{
    public class EventsSieverUnitTests
    {
        [Fact]

        public void BeginingEvent_NonExistInResultList()
        {
            var cur = new SynchronEvent();
            cur.SetStart(DateTime.Now.AddHours(-1));
            var list = new List<SynchronEvent>{cur};
            list = new EventsSiever().SieveEventsOnPeriodOfTime(DateTime.Now, DateTime.Now.AddHours(1), list);
            Assert.Equal(0, list.Count);
        }

        [Fact]

        public void EventWithStartOneHourLater_ExistInResultList()
        {
            var cur = new SynchronEvent();
            cur.SetStart(DateTime.Now.AddHours(1));
            var list = new List<SynchronEvent> { cur };
            list = new EventsSiever().SieveEventsOnPeriodOfTime(DateTime.Now, DateTime.Now.AddHours(2), list);
            Assert.Equal(1, list.Count);
        }

        [Fact]

        public void EventWithStartNow_ExistInResult()
        {
            var cur = new SynchronEvent();
            var start = DateTime.Now;
            cur.SetStart(start);
            var list = new List<SynchronEvent> { cur };
            list = new EventsSiever().SieveEventsOnPeriodOfTime(start, start.AddMinutes(12), list);
            Assert.Equal(1, list.Count);
        }

        [Fact]

        public void OneGoogAndOneBadEvents_ResultContainsOnlyOne()
        {
            var cur1 = new SynchronEvent();
            cur1.SetStart(DateTime.Now.AddHours(-1));

            var cur2 = new SynchronEvent();
            cur2.SetStart(DateTime.Now.AddMinutes(5));

            var list = new List<SynchronEvent> { cur1, cur2 };
            list = new EventsSiever().SieveEventsOnPeriodOfTime(DateTime.Now, DateTime.Now.AddHours(1), list);
            Assert.Equal(1, list.Count);
        }
    }
}
