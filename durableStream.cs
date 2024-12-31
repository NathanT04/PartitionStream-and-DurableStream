// Nathan Tran
// DurableStream.cs
// 10/27/24

// Class Invariants:
// - Maintains a file-backed persistent store for messages.
// - Guarantees that file operations are synchronized with the in-memory message list.
// - Messages are written to the file immediately upon appending.
//
// Missing Implementation Invariants:
// - Assumes that the file path provided is writable and accessible.
// - Does not handle concurrent access to the same file from different processes.
// - Does not guarantee atomicity for Reset() and Dispose() operations.

using System;
using System.IO;

namespace NTp5
{
    public class DurableStream : MsgStream, IDisposable
    {
        private readonly string filePath;
        private StreamWriter fileWriter;

        public DurableStream(int maxCapacity, int operationLimit, string filePath)
            : base(maxCapacity, operationLimit)
        {
            this.filePath = filePath ?? throw new ArgumentNullException(nameof(filePath), "filePath cannot be null.");
            InitializeFileWriter();

            if (File.Exists(filePath))
            {
                LoadMessagesFromFile();
            }
        }

        private void InitializeFileWriter()
        {
            fileWriter = new StreamWriter(filePath, true) { AutoFlush = true };
        }

        public override void Append(string message)
        {
            base.Append(message);
            fileWriter.WriteLine(message);
        }

        private void LoadMessagesFromFile()
        {
            using (StreamReader fileReader = new StreamReader(filePath))
            {
                string line;
                while ((line = fileReader.ReadLine()) != null && messageCount < maxCapacity)
                {
                    messages[messageCount++] = line;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            fileWriter.Close();
            fileWriter.Dispose();

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            InitializeFileWriter();
        }

        public void Dispose()
        {
            fileWriter?.Dispose();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        public override MsgStream DeepCopy()
        {
            DurableStream copy = new DurableStream(maxCapacity, operationLimit, filePath + "_copy");
            Array.Copy(messages, copy.messages, messageCount);
            copy.messageCount = messageCount;
            copy.currentOperationCount = currentOperationCount;
            return copy;
        }
    }
}
