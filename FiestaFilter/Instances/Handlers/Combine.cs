using System;

namespace Filter.Instances.Handlers
{
    internal class Combine<To, With>
    {
        public To PacketAttribute { get; private set; }
        public With MethodInfo { get; private set; }

        public Combine(To TransferPacketAttribute, With TransferMethodInfo)
        {
            PacketAttribute = TransferPacketAttribute;
            MethodInfo = TransferMethodInfo;
        }
    }
}
