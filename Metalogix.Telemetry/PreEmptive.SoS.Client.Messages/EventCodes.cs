using System;

namespace PreEmptive.SoS.Client.Messages
{
    public sealed class EventCodes
    {
        public const string SECURITY_ACCESS_FAILED = "Security.Access.Failed";

        public const string SECURITY_ACCESS_PASSED = "Security.Access.Passed";

        public const string SECURITY_AUTH_FAILED = "Security.Auth.Failed";

        public const string SECURITY_AUTH_PASSED = "Security.Auth.Passed";

        public const string SECURITY_VALIDATION_SQL = "Security.Validation.SQL";

        public const string SECURITY_VALIDATION_COMMAND = "Security.Validation.Command";

        public const string SECURITY_VALIDATION_SCRIPTING = "Security.Validation.Scripting";

        public const string SECURITY_VALIDATION_LENGTH = "Security.Validation.Length";

        public const string SECURITY_VALIDATION_OTHER = "Security.Validation.Other";

        public const string SECURITY_VALIDATION_PASSED = "Security.Validation.Passed";

        public const string SECURITY_DOS_COUNT = "Security.DOS.Count";

        public const string SECURITY_DOS_TIME = "Security.DOS.Time";

        public const string SECURITY_DOS_SIZE = "Security.DOS.Size";

        public const string SECURITY_DOS_OTHER = "Security.DOS.Other";

        public const string SECURITY_INTEGRITY_CERT = "Security.Integrity.Cert";

        public const string SECURITY_INTEGRITY_SIGNATURE = "Security.Integrity.Signature";

        public const string SECURITY_INTEGRITY_TAMPERING = "Security.Integrity.Tampering";

        public const string SECURITY_INTEGRITY_OTHER = "Security.Integrity.Other";

        public const string SECURITY_INTEGRITY_PASSED = "Security.Integrity.Passed";

        public const string SECURITY_REPLAY_DUPLICATE = "Security.Replay.Duplicate";

        public const string SECURITY_RUNTIME_EXCEPTION = "Security.Runtime.Exception";

        public const string SECURITY_RUNTIME_VERIFICATION = "Security.Runtime.Verification";

        public const string SECURITY_RUNTIME_POLICY = "Security.Runtime.Policy";

        public const string SECURITY_RUNTIME_PASSED = "Security.Runtime.Passed";

        public const string SECURITY_VULNERABILITY_PROBE = "Security.Vulnerability.Probe";

        public const string LIFECYCLE_START = "Lifecycle.Start";

        public const string LIFECYCLE_STOP = "Lifecycle.Stop";

        public const string SYSTEM_PROFILE = "System.Profile";

        public const string FEATURE_TICK = "Feature.Tick";

        public const string FEATURE_START = "Feature.Start";

        public const string FEATURE_STOP = "Feature.Stop";

        public const string PERFORMANCE_PROBE = "Performance.Probe";

        public const string FAULT_UNCAUGHT = "Fault.Uncaught";

        public const string FAULT_CAUGHT = "Fault.Caught";

        public const string FAULT_THROWN = "Fault.Thrown";

        public const string APPLICATION_RUN = "Application.Start";

        public const string APPLICATION_STOP = "Application.Stop";

        public const string SESSION_RUN = "Session.Start";

        public const string SESSION_STOP = "Session.Stop";

        public const string APPLICATION_SIGN_OF_LIFE = "Application.SignOfLife";

        public const string APPLICATION_SHELFLIFE_WARNING = "Application.ShelfLifeWarning";

        public const string APPLICATION_SHELFLIFE_EXPIRATION = "Application.ShelfLifeExpiration";

        private EventCodes()
        {
        }
    }
}