using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lithnet.activedirectory.passwordchange.web
{
    public enum PasswordTestResultCode : int
    {
        Approved = 0,
        LengthRequirementsNotMet = 1,
        ComplexityThresholdNotMet = 2,
        ComplexityPointsNotMet = 3,
        DidNotMatchApprovalRegex = 4,
        MatchedRejectRegex = 5,
        ContainsAccountName = 6,
        ContainsFullName = 7,
        Banned = 8,
        BannedNormalizedPassword = 9,
        BannedNormalizedWord = 10,
        PasswordWasBlank = 11,
        PasswordIsPwned = 50,
        PwnedGeneralError = 51,
        FilterError = 100
    }
}