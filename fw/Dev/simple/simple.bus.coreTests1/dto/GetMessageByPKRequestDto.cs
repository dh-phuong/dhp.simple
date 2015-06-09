using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.model;

namespace simple.bus.coreTests1.dto
{
    public sealed class GetMessageByPKRequestDto : BReqDto
    {
        public string MessageId { get; set; }
        public short Language { get; set; }

    }
}
