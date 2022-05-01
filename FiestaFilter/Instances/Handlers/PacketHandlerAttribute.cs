using System;

namespace Filter.Instances.Handlers
{
    internal class PacketHandlerAttribute : Attribute
    {
        public byte Header { get; private set; }
        public ushort Type { get; private set; }

        public PacketHandlerAttribute(byte TransferHeader, ushort TransferType)
        {
            Header = TransferHeader;
            Type = TransferType;
        }
    }
}
