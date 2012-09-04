using System;

namespace Duck.Tape.Test
{
    public interface IProtectedInterfaceWithEvent
    {
        event EventHandler ProtectedEvent;
    }
}