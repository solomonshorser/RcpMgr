using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpMgr3
{
    public static class StepIdSequencer
    {
        private static long _seqNum = 0;
        private static String formatString = "{0:D10}";

        public static String getNext()
        {
            String seq = "STP" + String.Format(formatString, _seqNum);
            _seqNum += 1;
            return seq;
        }

    }

    public static class IngredientIdSequencer
    {
        private static long _seqNum = 0;
        private static String formatString = "{0:D10}";

        public static String getNext()
        {
            String seq = "ING" + String.Format(formatString, _seqNum);
            _seqNum += 1;
            return seq;
        }
    }

}
