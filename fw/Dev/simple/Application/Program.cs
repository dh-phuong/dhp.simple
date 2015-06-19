using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hpsofts.Extension;

namespace Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var staff = "m_staff".Camelize()
                                .InstanceOf<testfw.model.appr.MStaff>();
            staff.StaffCd = "01";
            Console.WriteLine(staff.ToString());
            var cin = Console.ReadLine();
            hpsofts.security.Cryptography c = new hpsofts.security.Cryptography();
            
            Debug.WriteLine(c.Encrypt(cin, "top1", hpsofts.security.Bits.Bit128));

        }
    }
}
