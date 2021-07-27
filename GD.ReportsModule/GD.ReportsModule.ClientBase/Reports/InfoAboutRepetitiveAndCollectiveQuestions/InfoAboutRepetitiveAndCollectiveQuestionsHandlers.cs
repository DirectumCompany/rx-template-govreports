using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class InfoAboutRepetitiveAndCollectiveQuestionsClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var current = Sungero.Company.Employees.Current;
      if (current == null || current.Department == null || current.Department.BusinessUnit == null)
      {
        e.Cancel = true;
        return;
      }
      var businessUnit = current.Department.BusinessUnit;
     
      var dialog = Dialogs.CreateInputDialog(Resources.ReportParameters);
      var startDate = dialog.AddDate(Resources.StartDate, true, Calendar.BeginningOfYear(Calendar.Today));
      var endDate = dialog.AddDate(Resources.EndDate, true, Calendar.Today);
      
      if (dialog.Show() != DialogButtons.Ok)
      {
        e.Cancel = true;
        return;
      }
      
      // Передаём параметры в отчет
      InfoAboutRepetitiveAndCollectiveQuestions.StartDate = startDate.Value.Value;
      InfoAboutRepetitiveAndCollectiveQuestions.EndDate = endDate.Value.Value;
      InfoAboutRepetitiveAndCollectiveQuestions.LYStartDate = InfoAboutRepetitiveAndCollectiveQuestions.StartDate.Value.AddYears(-1);
      InfoAboutRepetitiveAndCollectiveQuestions.LYEndDate = InfoAboutRepetitiveAndCollectiveQuestions.EndDate.Value.AddYears(-1);
      InfoAboutRepetitiveAndCollectiveQuestions.BusinessUnitId = businessUnit.Id;
      InfoAboutRepetitiveAndCollectiveQuestions.BusinessUnit = businessUnit.Name;
      InfoAboutRepetitiveAndCollectiveQuestions.RequestGuid = Constants.Module.Discriminator.Request.ToString();
    }

  }
}