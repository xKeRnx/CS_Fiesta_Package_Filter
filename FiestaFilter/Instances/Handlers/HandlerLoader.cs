using System;
using System.Reflection;
using System.Collections.Generic;

using Filter.Instances.Handlers;

namespace Filter.Instances.Handlers
{
    internal class HandlerLoader
    {
        public Dictionary<byte, Dictionary<ushort, MethodInfo>> Handlers;

        public HandlerLoader() { Handlers = new Dictionary<byte, Dictionary<ushort, MethodInfo>>(); }

        public bool HasHandler(byte Header, ushort Type)
        {
            if (Handlers.ContainsKey(Header))
            {
                if (Handlers[Header].ContainsKey(Type)) { return true; }
            }

            return false;
        }

        public MethodInfo GetHandler(byte Header, ushort Type)
        {
            Dictionary<ushort, MethodInfo> HeaderDictionary;

            MethodInfo TypeMethod;

            if (Handlers.TryGetValue(Header, out HeaderDictionary))
            {
                if (HeaderDictionary.TryGetValue(Type, out TypeMethod)) { return TypeMethod; }
            }

            return null;
        }

        public Action GetAction(MethodInfo TypeMethod, params object[] Args) { return () => TypeMethod.Invoke(null, Args); }
    }
}
