using System;
namespace ShipWithMeWeb.Responses
{
    public sealed class BadRequestResponse
    {
        public int ErrorCode { get; }

        public string EnglishMessage { get; }

        private const int PostAlreadyReportedByUserErrorCode = 1;

        private const int ChatWithPostOwnerDoesNotExistErrorCode = 2;

        private const int EmailAddressAlreadyUsedErrorCode = 3;

        private const int UserNameAlreadyUsedErrorCode = 4;

        private const int BadPasswordErrorCode = 5;

        private BadRequestResponse(int errorCode, string englishMessage)
        {
            ErrorCode = errorCode;
            EnglishMessage = englishMessage;
        }

        public static BadRequestResponse PostAlreadyReportedByUser()
        {
            return new BadRequestResponse(
                PostAlreadyReportedByUserErrorCode,
                "This post has already been reported by the user.");
        }

        public static BadRequestResponse ChatWithPostOwnerDoesNotExist()
        {
            return new BadRequestResponse(
                ChatWithPostOwnerDoesNotExistErrorCode,
                "No chat has been started with the post owner.");
        }

        public static BadRequestResponse EmailAddressAlreadyUsed()
        {
            return new BadRequestResponse(
                EmailAddressAlreadyUsedErrorCode,
                "Email is already in use.");
        }

        public static BadRequestResponse UserNameAlreadyUsed()
        {
            return new BadRequestResponse(
                UserNameAlreadyUsedErrorCode,
                "UserName is already in use.");
        }

        public static BadRequestResponse BadPassword()
        {
            return new BadRequestResponse(
                BadPasswordErrorCode,
                "Password does not meet the criterias.");
        }
    }
}
