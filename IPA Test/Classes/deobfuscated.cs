﻿namespace IPA_Test.Classes
{
    internal sealed class Deobfuscated : Comfort.Common.Operation, BattlEye.IBattlEyeClientRequestHandler
    {
        public BattlEye.BEClient.ERestartReason? RestartReason { get; private set; }

        void BattlEye.IBattlEyeClientRequestHandler.OnRequestRestart(BattlEye.BEClient.ERestartReason reason)
        {
            this.RestartReason = new BattlEye.BEClient.ERestartReason?(reason);
        }

        void BattlEye.IBattlEyeClientRequestHandler.OnSendPacket(byte[] bePacket)
        {
        }

        public static Comfort.Common.Operation smethod_0(Comfort.Common.StartCoroutineDelegate startCoroutineDelegate_0, BattlEye.BEClient.LogDelegate logDelegate_0)
        {
            Deobfuscated @class = new Deobfuscated();
            startCoroutineDelegate_0(@class.method_0(logDelegate_0));
            return @class;
        }

        private System.Collections.IEnumerator method_0(BattlEye.BEClient.LogDelegate logDelegate_0)
        {
            this.Succeed = true;
            return System.Linq.Enumerable.Empty<object>().GetEnumerator();
        }

        private const int int_0 = 10;

        [System.Runtime.CompilerServices.CompilerGenerated]
        private BattlEye.BEClient.ERestartReason? nullable_0;
    }
}