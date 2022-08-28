#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1300 // Element should begin with upper-case letter

namespace CoreCommon.Data.Domain.Entitites
{
    /// <summary>
    /// Base entity interface.
    /// </summary>
    public class CosmosEntityBase
    {
        /// <summary>
        /// To explicitly disable the concurrency check, you should set the ETag property to a wildcard (*).
        /// ex: _etag = "*";
        /// </summary>
        public string _etag { get; set; }

        /*        [Timestamp]
         *        [Column("_ts")]
                public long _ts { get; set; }*/
    }
}
