using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace JustEmuTarkov.Patches
{
    internal class NoCheckCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem) => true;
    }

    public static class SSL
    {
        private static ICertificatePolicy CertificatePolicyBackup;
        private static RemoteCertificateValidationCallback ServerCertificateValidationCallbackBackup;

        private static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

        [Obsolete]
        public static void Disable()
        {
            Logger.Warn("Disabling SSL verification!");
            CertificatePolicyBackup = ServicePointManager.CertificatePolicy;
            ServerCertificateValidationCallbackBackup = ServicePointManager.ServerCertificateValidationCallback;
            ServicePointManager.CertificatePolicy = new NoCheckCertificatePolicy();
            ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallback;
            Logger.Warn("Disabled SSL verification!");
        }

        [Obsolete]
        public static void Restore()
        {
            Logger.Warn("Restoring SSL verification!");
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallbackBackup;
            ServicePointManager.CertificatePolicy = CertificatePolicyBackup;
            CertificatePolicyBackup = null; ServerCertificateValidationCallbackBackup = null;
            Logger.Warn("Restored SSL verification!");
        }
    }
}