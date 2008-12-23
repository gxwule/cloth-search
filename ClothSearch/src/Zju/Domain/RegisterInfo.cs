using System;
using Perst;
using Zju.Util;

namespace Zju.Domain
{
    public class RegisterInfo : Persistent
    {
        public String Name
        {
            get;
            set;
        }

        public int LoginTime
        {
            get;
            set;
        }

        public int SearchTime
        {
            get;
            set;
        }

        public String Md5Key
        {
            get;
            set;
        }

        public DateTime UpdateTime
        {
            get;
            set;
        }

        public RegisterInfo(string name, string md5Key)
        {
            this.Name = name;
            this.Md5Key = md5Key;
            this.LoginTime = 0;
            this.SearchTime = 0;
            this.UpdateTime = new DateTime();
        }

        public RegisterInfo() : this(null, null)
        {

        }
    }
}
