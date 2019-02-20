namespace Lithnet.ActiveDirectory.PasswordChange.Web
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
        GeneralError = 51,
        FilterError = 100
    }
}