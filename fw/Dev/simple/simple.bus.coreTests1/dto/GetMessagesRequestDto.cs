using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.model;

namespace simple.bus.coreTests1.dto
{
    public sealed class GetMessagesRequestDto : BReqDto
    {
        public short Language { get; set; }
    }
}
