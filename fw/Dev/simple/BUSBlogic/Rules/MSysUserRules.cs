using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simple.core.model;
using testfw.model.bus;

namespace BUSBlogic.Rules
{
    internal class MSysUserRules
    {
        public MSysUserRules()
        {

        }

        /// <summary>
        /// Logins the check.
        /// </summary>
        /// <param name="loginId">The login identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public void LoginCheck(RespondDto<MSysUser> reqDto, string loginId, string password)
        {
            if (string.IsNullOrEmpty(loginId))
            {
                reqDto.IsOk = false;
                reqDto.MsgCd = "E0001";
            }
            if (string.IsNullOrEmpty(password))
            {
                reqDto.IsOk = false;
                reqDto.MsgCd = "E0001";
            }
        }
    }
}
