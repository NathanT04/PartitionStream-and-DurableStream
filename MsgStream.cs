// Nathan Tran
// MsgStream.cs
// 10/27/24

using System;

namespace p3
{
    public class MsgStream
    {
        // Invariants:
        // - maxCapacity > 0
        // - operationLimit > 0
        // - messageCount <= maxCapacity
        // - currentOperationCount <= operationLimit
        protected string[] messages;
        protected int maxCapacity;
        protected int operationLimit;
        protected int currentOperationCount;
        protected int messageCount;
        private const int MaxWordLength = 20;

        // Constructor
        // Precondition: maxCapacity > 0, operationLimit > 0
        // Postcondition: Initializes a new MsgStream instance with the specified maxCapacity and operationLimit.
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

        // Returns the current number of messages stored.
        // Precondition: None
        // Postcondition: Returns the value of messageCount.
        public int GetMessageCount()
        {
            return messageCount;
        }

        // Copy Constructor
        // Precondition: other is not null
        // Postcondition: Creates a deep copy of the specified MsgStream object.
        public MsgStream(MsgStream other)
        {
            maxCapacity = other.maxCapacity;
            operationLimit = other.operationLimit;
            currentOperationCount = other.currentOperationCount;
            messageCount = other.messageCount;
            messages = (string[])other.messages.Clone();
        }

        // Creates a deep copy of the current MsgStream.
        // Precondition: None
        // Postcondition: Returns a new MsgStream instance that is a deep copy of the current instance.
        public virtual MsgStream DeepCopy()
        {
            return new MsgStream(this);
        }

        // Adds a message to the MsgStream.
        // Precondition: message is not null, message.Length <= 100, no word in message exceeds MaxWordLength, message is unique
        // Postcondition: Adds the message to messages array if within capacity and increments messageCount and currentOperationCount.
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

        // Reads a specified range of messages from the MsgStream.
        // Precondition: 0 <= startIndex < messageCount, count >= 0, startIndex + count <= messageCount
        // Postcondition: Returns an array of messages from startIndex to startIndex + count.
        public virtual string[] Read(int startIndex, int count)
        {
            IsOperationAllowed();
            IsWithinBounds(startIndex, count);

            string[] result = new string[count];
            Array.Copy(messages, startIndex, result, 0, count);
            currentOperationCount++;
            return result;
        }

        // Resets the MsgStream, clearing all messages and resetting counts.
        // Precondition: None
        // Postcondition: Clears messages array, sets messageCount and currentOperationCount to 0.
        public virtual void Reset()
        {
            messages = new string[maxCapacity];
            messageCount = 0;
            currentOperationCount = 0;
        }

        // Checks if the operation limit has been reached.
        // Precondition: None
        // Postcondition: Throws InvalidOperationException if currentOperationCount >= operationLimit.
        protected void IsOperationAllowed()
        {
            if (currentOperationCount >= operationLimit)
            {
                throw new InvalidOperationException("Operation limit exceeded.");
            }
        }

        // Checks if the specified range is within bounds.
        // Precondition: 0 <= startIndex < messageCount, count >= 0, startIndex + count <= messageCount
        // Postcondition: Throws ArgumentOutOfRangeException if range is out of bounds.
        protected void IsWithinBounds(int startIndex, int count)
        {
            if (startIndex < 0 || count < 0 || (startIndex + count) > messageCount)
            {
                throw new ArgumentOutOfRangeException("Requested message range is out of bounds.");
            }
        }
    }
}
