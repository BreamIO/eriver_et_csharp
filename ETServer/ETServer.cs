﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eriver

{
    class ETServer
    {
        static void Main(string[] args)
        {
            Tracker tracker = new MockTracker(1, 44);
            tracker.register_onETEvent(delegate(ETEvent e) 
            {
                Console.WriteLine(e);
                Protocol.EndCalDelegate hej = delegate()
                {

                };
            });
            Console.WriteLine("Hej!");
            
        }
    }
}