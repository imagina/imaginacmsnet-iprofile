using System;
using System.Collections.Generic;

namespace IProfile.Data.Entities
{
    public partial class ApiResourceClaim
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        public string Type { get; set; } = null!;

        public virtual ApiResource ApiResource { get; set; } = null!;
    }
}
