using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackTestCouvertureOptions
{
    public class ParameterException : Exception
    {
        DateTime m_errorTime;
        static ushort s_errorNumber;

        public ParameterException()
            : base("Invalid parameter.")
        {
            m_errorTime = DateTime.Now;
            s_errorNumber++;
        }

        public ParameterException(string message)
            : base(message)
        {
            m_errorTime = DateTime.Now;
            s_errorNumber++;
        }
    }
}
