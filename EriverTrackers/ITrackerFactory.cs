using Eriver.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EriverTrackers
{
    interface ITrackerFactory
    {
        string getName();
        ITracker Create();
    }
}
