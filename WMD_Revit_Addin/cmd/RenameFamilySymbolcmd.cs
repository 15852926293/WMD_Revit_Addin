using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMD_Revit_Addin.Filter;

namespace WMD_Revit_Addin.cmd
{
    public class RenameFamilySymbolCmd
    {
        ExternalCommandData commandData = null;
        UIDocument UIDoc = null;
        Document Doc = null;
        Application app = null;

        public List<string> FamilyPathList = null;
        public string PreName = "";


        public RenameFamilySymbolCmd(ExternalCommandData commandData)
        {
            this.commandData = commandData;
            UIApplication uiApp = commandData.Application;
            app = uiApp.Application;
            //UI文档s
            UIDoc = uiApp.ActiveUIDocument;
            //文档
            Doc = UIDoc.Document;

            //Family = family;
            //PreName = preName;
        }

        public void RenameFamilySymbol()
        {
            Family family = null;
            if (FamilyPathList != null)
            {
                FamilyPathList.ForEach(FamilyPath =>
                {
                    using (Transaction tr = new Transaction(Doc, "rename familySymbol"))
                    {
                        tr.Start();
                        bool b = Doc.LoadFamily(FamilyPath, out family);
                        if (family != null)
                        {
                            ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();
                            foreach (ElementId id in familySymbolIds)
                            {
                                FamilySymbol familySymbol = family.Document.GetElement(id) as FamilySymbol;
                                if (!familySymbol.Name.Contains(PreName))
                                {
                                    familySymbol.Name = PreName + familySymbol.Name;
                                }
                            }
                        }
                        tr.Commit();
                    }
                });
            }
        }
    }
}
