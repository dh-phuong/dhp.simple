using System;
using System.Data.Common;
using simple.core.attribute;
using simple.core.model;

namespace testfw.model.appr
{
    public sealed class MStaff : BEntity<MStaff>
    {
        public MStaff()
        {
        }

        public MStaff(DbDataReader dr)
            : base(dr)
        {
        }

        [PrimaryKey]
        public string StaffCd { get; set; }

        public string StaffName { get; set; }

        public string Image { get; set; }

        public short TypeOfStaff { get; set; }

        public decimal AnnualDays { get; set; }

        public DateTime AnnualToDate { get; set; }

        public int DepartmentId { get; set; }

        public int Position { get; set; }

        public short Sex { get; set; }

        public DateTime BirthDay { get; set; }

        public int BirthPlace { get; set; }

        public string IdNo { get; set; }

        public DateTime IssueDate { get; set; }

        public int IssuePlace { get; set; }

        public string Tel { get; set; }

        public string Tel2 { get; set; }

        public string EmailAddress { get; set; }

        public string Address1 { get; set; }

        public int Province1 { get; set; }

        public int District1 { get; set; }

        public int Ward1 { get; set; }

        public string Address2 { get; set; }

        public int Province2 { get; set; }

        public int District2 { get; set; }

        public int Ward2 { get; set; }

        public string AccountNo { get; set; }

        public string BankName { get; set; }

        public DateTime StartWorkDate { get; set; }

        public DateTime EndWorkDate { get; set; }

        public string SocialInsurNo { get; set; }

        public string TaxNo { get; set; }

        public short StatusFlag { get; set; }

        public override string FixCd(string text, char paddingChar = '0')
        {
            throw new NotImplementedException();
        }
    }
}