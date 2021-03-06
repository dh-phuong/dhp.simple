﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model
{
    public sealed class MUserSp : BEntity<MUserSp>
    {
        public MUserSp()
        {
        }

        public MUserSp(DbDataReader dr)
            : base(dr)
        {
        }

        /// <summary>
        /// Gets or sets the user cd.
        /// </summary>
        /// <value>
        /// The user cd.
        /// </value>
        [PrimaryKey]
        public string UserCd { get; set; }

        public string LoginId { get; set; }

        public string Password { get; set; }

        public string UserFullName { get; set; }

        public string UserShortName { get; set; }

        public string GroupCd { get; set; }

        public string CustomerCd { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }

}
