// Nathan Tran
// P5.cs
// 12/02/24

// Class Invariants:
// - Serves as a driver to test the functionality of all classes and interfaces.
// - Executes a series of pre-defined scenarios to demonstrate compliance with P5 requirements.
//
// Missing Implementation Invariants:
// - Does not handle runtime exceptions during testing scenarios (e.g., invalid input, null values).
// - Assumes a single-threaded execution environment for simplicity.

using System;
using System.Collections.Generic;

namespace NTp5
{
    class P5
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting P5 Driver...");

            // 1. Reused Classes: MsgStream, DurableStream, PartitionStream
            Console.WriteLine("\n--- Testing DurableStream ---");
            var durableStream = new DurableStream(5, 10, "messages.txt");
            durableStream.Append("Hello, World!");
            durableStream.Append("C# Programming is fun!");

            Console.WriteLine("DurableStream Messages:");
            foreach (var msg in durableStream.Read(0, durableStream.GetMessageCount()))
            {
                Console.WriteLine(msg);
            }

            Console.WriteLine("\n--- Testing PartitionStream ---");
            var partitionDict = new Dictionary<string, MsgStream>
            {
                { "Partition1", new MsgStream(5, 10) },
                { "Partition2", durableStream.DeepCopy() }
            };

            var partitionStream = new PartitionStream(partitionDict);
            partitionStream.AddMessage("Partition1", "Message to Partition1");
            partitionStream.AddMessage("Partition2", "Message to Partition2");

            Console.WriteLine("Partition1 Messages:");
            foreach (var msg in partitionStream.ReadMessages("Partition1", 0, 1))
            {
                Console.WriteLine(msg);
            }

            // 2. New Class: StreamWithSubscribers
            Console.WriteLine("\n--- Testing StreamWithSubscribers ---");
            var streamWithSubscribers = new StreamWithSubscribers(partitionDict);

            // Add Subscribers
            var subscriber1 = new ExampleSubscriber("Subscriber1");
            var subscriber2 = new ExampleSubscriber("Subscriber2");
            streamWithSubscribers.AddSubscriber(subscriber1);
            streamWithSubscribers.AddSubscriber(subscriber2);

            // Add Message and notify subscribers
            streamWithSubscribers.AddMessage("Partition1", "Broadcast Message 1");
            streamWithSubscribers.AddMessage("Partition2", "Broadcast Message 2");

            // 3. Heterogeneous Collection
            Console.WriteLine("\n--- Testing Heterogeneous Collections ---");
            var heterogeneousCollection = new List<object>
            {
                durableStream,
                partitionStream,
                streamWithSubscribers,
                subscriber1
            };

            foreach (var obj in heterogeneousCollection)
            {
                if (obj is DurableStream ds)
                {
                    Console.WriteLine("DurableStream Detected: Messages = " + ds.GetMessageCount());
                }
                else if (obj is PartitionStream ps)
                {
                    Console.WriteLine("PartitionStream Detected: Partition Count = " + ps.PartitionCount);
                }
                else if (obj is ISubscriber sub)
                {
                    Console.WriteLine("ISubscriber Detected: Triggering NewMessage...");
                    sub.NewMessage("Test Message for Subscriber");
                }
                else
                {
                    Console.WriteLine("Unknown Type Detected");
                }
            }

            // 4. Trigger Mode Changes
            Console.WriteLine("\n--- Triggering Mode Changes ---");
            Console.WriteLine("Resetting DurableStream...");
            durableStream.Reset();
            Console.WriteLine("DurableStream Reset Complete. Messages Count: " + durableStream.GetMessageCount());

            Console.WriteLine("Disposing PartitionStream...");
            partitionStream.Dispose();
            Console.WriteLine("PartitionStream Disposed.");

            Console.WriteLine("Removing Subscriber2...");
            streamWithSubscribers.RemoveSubscriber(subscriber2);
            streamWithSubscribers.AddMessage("Partition1", "Message after removing Subscriber2");

            Console.WriteLine("\nP5 Driver Complete.");
        }
    }
}
