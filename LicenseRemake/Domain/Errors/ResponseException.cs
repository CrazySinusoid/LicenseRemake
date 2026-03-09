using System.Text.RegularExpressions;

namespace LicenseRemake.Domain.Errors
{
    public class ResponseException : Exception
    {
        public ResponseErrorCode ErrorCode { get; }

        public string? Param { get; }

        public int? StatusCode { get; }

        public ResponseException(
            ResponseErrorCode errorCode,
            string? param = null,
            int? statusCode = null,
            Exception? inner = null) : base(errorCode.ToString(), inner)
        {
            ErrorCode = errorCode;
            Param = param;
            StatusCode = statusCode;
        }

        public int Code => (int)ErrorCode;

        public string CodeString =>
            Regex.Replace(ErrorCode.ToString(), "([a-z])([A-Z])", "$1_$2").ToLower();
    }
}
