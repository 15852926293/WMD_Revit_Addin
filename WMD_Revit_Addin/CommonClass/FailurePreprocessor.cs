using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
namespace WMD_Revit_Addin
{
    public class FailurePreprocessor : IFailuresPreprocessor
    {
        private string _failureMessage;
        public string FailureMessage
        {
            get { return _failureMessage; }
            set { _failureMessage = value; }
        }
        private bool _error;
        public bool HasError
        {
            get { return _error; }
            set { _error = value; }
        }
        public FailureProcessingResult PreprocessFailures(FailuresAccessor fa)
        {
            IList<FailureMessageAccessor> lstFma = fa.GetFailureMessages();
            if (lstFma.Count() == 0) return FailureProcessingResult.Continue;
            foreach (FailureMessageAccessor item in lstFma)
            {
                if (item.GetSeverity() == FailureSeverity.Warning)
                {
                    _error = false;
                    fa.DeleteWarning(item);
                }
                else if (item.GetSeverity() == FailureSeverity.Error)
                {
                    if (item.HasResolutions())
                    {
                        fa.ResolveFailure(item);
                        _failureMessage = "FailurePreprocessor";
                        _error = true;
                        return FailureProcessingResult.ProceedWithRollBack;
                    }
                }
            }
            return FailureProcessingResult.Continue;
        }
    }
}
