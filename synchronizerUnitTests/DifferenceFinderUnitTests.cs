using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Assert = Xunit.Assert;
using synchronizer;

namespace synchronizerUnitTests
{
    public class DifferenceFinderUnitTests
    {
        [Fact]

        public void EventWhichNeedToRemove_ResultListContainsThisEvent()
        {
            var currentEvent = new SynchronEvent().SetId("123").SetSource("1");
            var needToCheck = new List<SynchronEvent> { currentEvent };
            var result = new DifferenceFinder().GetDifferenceToDelete(needToCheck, new List<SynchronEvent>());
            Assert.Equal(1, result.Count);
        }

        [Fact]

        public void EventWhichNeedToAdd_ResultListContainsThisEvent()
        {
            var currentEvent = new SynchronEvent().SetId("123").SetSource("1").SetPlacement("1");
            var needToCheck = new List<SynchronEvent> { currentEvent };
            var result = new DifferenceFinder().GetDifferenceToPush(needToCheck, new List<SynchronEvent>());
            Assert.Equal(1, result.Count);
        }

        [Fact]

        public void EventAlreadyExist_ResultListIsEmpty()
        {
            var curretEvent = new SynchronEvent().SetId("123").SetSource("1").SetPlacement("!");
            var needToCheck = new List<SynchronEvent> { curretEvent };
            var result = new DifferenceFinder().GetDifferenceToPush(needToCheck, needToCheck);
            
            Assert.Equal(0, result.Count);
        }

        [Fact]

        public void EventsIsAlreadySync_ResultListIsEmpty()
        {
            var currentEvent = new SynchronEvent().SetId("123").SetSource("1");
            var needToCheck = new List<SynchronEvent> { currentEvent };
            var result = new DifferenceFinder().GetDifferenceToDelete(needToCheck, needToCheck);
            Assert.Equal(0, result.Count);
        }
    }
}
