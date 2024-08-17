using Metalogix.SharePoint.Adapters;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.SharePoint.Adapters.Authentication
{
    [MenuOrder(1)]
    [MenuText("Auto Detect")]
    public class AutoDetectInitializer : AuthenticationInitializer
    {
        public AutoDetectInitializer()
        {
        }

        private AuthenticationInitializer GetCorrectInitializer(SharePointAdapter adapter)
        {
            AuthenticationInitializer authenticationInitializer;
            List<Type> types = new List<Type>(AuthenticationInitializer.AvailableInitializerTypes);
            Type type = base.GetType();
            types.Sort((Type leftType, Type rightType) => AutoDetectPriorityAttribute.GetAutoDetectPriority(leftType)
                .CompareTo(AutoDetectPriorityAttribute.GetAutoDetectPriority(rightType)));
            foreach (Type type1 in types)
            {
                if (type1 == type)
                {
                    continue;
                }

                try
                {
                    AuthenticationInitializer authenticationInitializer1 = AuthenticationInitializer.Create(type1);
                    if (authenticationInitializer1.TestAuthenticationSetup(adapter))
                    {
                        authenticationInitializer = authenticationInitializer1;
                        return authenticationInitializer;
                    }
                }
                catch
                {
                }
            }

            if (AuthenticationUtilities.ForwardsToSharePointOnlineLoginPage(adapter))
            {
                foreach (Type type2 in types)
                {
                    if (!IsDefaultO365Authenticator.GetIsDefaulO365Authenticator(type2))
                    {
                        continue;
                    }

                    authenticationInitializer = AuthenticationInitializer.Create(type2);
                    return authenticationInitializer;
                }
            }

            return new WindowsInitializer();
        }

        protected override void SpecializedInitActions(SharePointAdapter adapter)
        {
            AuthenticationInitializer correctInitializer = this.GetCorrectInitializer(adapter);
            correctInitializer.Credentials = base.Credentials;
            correctInitializer.Certificates = base.Certificates;
            correctInitializer.InitializeAuthenticationSettings(adapter);
        }
    }
}