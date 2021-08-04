
using Microsoft.AspNetCore.Mvc;

namespace DevMeeting.Models
{
    public static class Responses
    {
        public static string Ok => "Operation was a success";

        #region Account

        #region Account

        public static string AccountLockedOut => "Your account is currently locked-out, please try again later" +
                                                 " and if you could not login again, please contact support.";

        public static string UserWithEmailExists =>
            "The entered email already exists, if you forgot your password you can reset it.";

        public static string AccountCreatedAndEmailConfirmation => 
            "The account has created. A confirmation email has been sent to your inbox " +
            "(You may check your spam inbox in case your email provider is an outdated one !).";

        public static string AccountCreationWasAFailure => "Account creation operation was a failure.";
        
        #endregion
        
        #region SignIn
        public static string SignInFailed => "The account either does not exist or the credential was not valid.";
        public static string SignInSucceeded => "Sign in was a success.";
        public static string TwoFactorRequired => "Two-Factor authentication is required, please continue signing " +
                                                  "in with two-factor authentication.";
        #endregion

        #endregion
        
    }
}