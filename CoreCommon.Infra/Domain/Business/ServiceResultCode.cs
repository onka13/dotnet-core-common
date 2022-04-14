namespace CoreCommon.Infrastructure.Domain.Business
{
    /// <summary>
    /// Base service result codes
    /// </summary>
    public class ServiceResultCode
    {
        public const int Created = 12;
        public const int Updated = 11;
        public const int Deleted = 10;
        public const int Error = 0;
        public const int ServerError = -1;
        public const int NoPermission = -2;
        public const int EmptyModel = -10;
        public const int InvalidModel = -11;
        public const int NotFound = -12;
    }
}
