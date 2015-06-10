using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.model;

namespace simple.bus.coreTests1.dto
{
    public sealed class GetByLoginIDRequestDto : BReqDto
    {
        public string LoginId { get; set; }
    }

    public sealed class GetByManyLoginIDRequestDto : BReqDto
    {
        public string[] LoginId { get; set; }
    }
}
