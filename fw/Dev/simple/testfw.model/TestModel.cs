﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.bus.core.model;

namespace testfw.model
{
    public class TestModel : BEntity<TestModel>
    {

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}
