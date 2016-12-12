using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public class ActionResult : List<ErrorCode>
    {
        public ActionResult()
        {
        }

        public ActionResult(ErrorCode errorCode)
        {
            Add(errorCode);
        }

        public bool IsSuccess => Count == 0;

        public static implicit operator bool(ActionResult actionResult)
        {
            return actionResult.IsSuccess;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, this.Select(x => x.ToString()).ToArray());
        }
    }

    public class ActionResultDict : Dictionary<string, ActionResult>
    {
        public bool Success
        {
            get { return Values.FirstOrDefault(aR => !aR) == null; }
        }

        public static implicit operator bool(ActionResultDict actionResultDict)
        {
            return actionResultDict.Success;
        }
    }
}