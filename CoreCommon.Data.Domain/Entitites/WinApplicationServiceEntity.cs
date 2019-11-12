using System;
using System.Threading.Tasks;

namespace CoreCommon.Data.Domain.Entitites
{
    public class WinApplicationServiceEntity
    {
        public Task Task { get; set; }
        public int TotalExceptionCount { get; set; }
        public DateTime FirstExceptionDate { get; set; }
    }
}
