using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitAPIViwsSchedules
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        public List<ViewPlan> Views { get; }
        public List<Category> Categories { get; }
        public DelegateCommand HideCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;
            Views = GetFloorPlanViews(_doc);
            Categories = GetCategories(_doc);
            HideCommand = new DelegateCommand(OnHideCommand);
        }

        private void OnHideCommand()
        {
            if (SelectedViewPlan == null || SelectedCategory == null)
                return;

            using (var ts = new Transaction(_doc, "Save chenhes"))
            {
                ts.Start();
                SelectedViewPlan.SetCategoryHidden(SelectedCategory.Id, hide: true);
                ts.Commit();
            }
        }

        public Category SelectedCategory { get; set; }

        public ViewPlan SelectedViewPlan { get; set; }

        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);//закрытие окна
        }


        public static List<ViewPlan> GetFloorPlanViews(Document doc)
        {
            var views
               = new FilteredElementCollector(doc)
                   .OfClass(typeof(ViewPlan))
                   .Cast<ViewPlan>()
                   .Where(p => p.ViewType == ViewType.FloorPlan)
                   .ToList();
            return views;
        }

        public static List<Category> GetCategories(Document doc)
        {

            var categoryList = new List<Category>();
            Categories categories = doc.Settings.Categories;
            foreach (Category category in categories)
            {
                categoryList.Add(category);
            }
            return categoryList;
        }

    }
}
