// ISubscriber.cs
// Class Invariants:
// - The ISubscriber interface guarantees the presence of the NewMessage(string message) method in all implementing classes.
// - This interface provides no assumptions about how the message is processed or stored by the implementing classes.
//
// Missing Implementation Invariants:
// - Concrete implementations must define how NewMessage(string message) processes or stores messages.
// - No assumptions are made about the side effects or dependencies of NewMessage.

namespace NTp5
{
    public interface ISubscriber
    {
        void NewMessage(string message);
    }
}
