// Nathan Tran
// MsgStream.cs
// 10/27/24

// Class Invariants:
// - Messages cannot exceed the maxCapacity or operationLimit set during initialization.
// - No duplicate messages are allowed in the stream.
// - Messages are stored in insertion order.
//
// Missing Implementation Invariants:
// - Does not guarantee thread-safety for operations (e.g., Append, Read).
// - Assumes that all input messages are non-null and meet length requirements.

using System;

namespace NTp5
{
    public class MsgStream
    {
        protected string[] messages;
        protected int maxCapacity;
        protected int operationLimit;
        protected int currentOperationCount;
        protected int messageCount;
        private const int MaxWordLength = 20;

        public MsgStream(int maxCapacity, int operationLimit)
        {
            if (maxCapacity <= 0)
            {
                throw new ArgumentException("maxCapacity must be greater than 0.");
            }

            if (operationLimit <= 0)
            {
                throw new ArgumentException("operationLimit must be greater than 0.");
            }

            this.maxCapacity = maxCapacity;
            this.operationLimit = operationLimit;
            messages = new string[maxCapacity];
            currentOperationCount = 0;
            messageCount = 0;
        }

        public int GetMessageCount()
        {
            return messageCount;
        }

        public MsgStream(MsgStream other)
        {
            maxCapacity = other.maxCapacity;
            operationLimit = other.operationLimit;
            currentOperationCount = other.currentOperationCount;
            messageCount = other.messageCount;
            messages = (string[])other.messages.Clone();
        }

        public virtual MsgStream DeepCopy()
        {
            return new MsgStream(this);
        }

        public virtual void Append(string message)
        {
            if (message == null || message.Length > 100)
            {
                throw new ArgumentException("Message cannot be null or exceed the maximum length.");
            }

            string[] words = message.Split(' ');
            foreach (var word in words)
            {
                if (word.Length > MaxWordLength)
                {
                    throw new ArgumentException($"Each word must be shorter than {MaxWordLength} characters. Word '{word}' is too long.");
                }
            }

            foreach (var msg in messages)
            {
                if (msg == message)
                {
                    throw new InvalidOperationException("Duplicate messages are not allowed.");
                }
            }

            IsOperationAllowed();

            if (messageCount >= maxCapacity)
            {
                throw new InvalidOperationException("Cannot add more messages, capacity reached.");
            }

            messages[messageCount++] = message;
            currentOperationCount++;
        }

        public virtual string[] Read(int startIndex, int count)
        {
            IsOperationAllowed();
            IsWithinBounds(startIndex, count);

            string[] result = new string[count];
            Array.Copy(messages, startIndex, result, 0, count);

            currentOperationCount++;
            return result;
        }

        public virtual void Reset()
        {
            messages = new string[maxCapacity];
            messageCount = 0;
            currentOperationCount = 0;
        }

        protected void IsOperationAllowed()
        {
            if (currentOperationCount >= operationLimit)
            {
                throw new InvalidOperationException("Operation limit exceeded.");
            }
        }

        protected void IsWithinBounds(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0 || (startIndex + count) > messageCount)
            {
                throw new ArgumentOutOfRangeException("Requested message range is out of bounds.");
            }
        }
    }
}
