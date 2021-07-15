using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class ResultRequestReportClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var current = Sungero.Company.Employees.Current;      
      if (current == null || current.Department == null || current.Department.BusinessUnit == null)
      {
        e.Cancel = true;
      }
      
      var dialog = Dialogs.CreateInputDialog(Resources.ReportParameters);
      var currentBusinessUnit = current.Department.BusinessUnit;
      var startDate = dialog.AddDate(Resources.StartDate, true, Calendar.BeginningOfYear(Calendar.Today));
      var endDate = dialog.AddDate(Resources.EndDate, true, Calendar.Today);
      
      if (dialog.Show() != DialogButtons.Ok)
        e.Cancel = true;
      
      ResultRequestReport.StartDate = startDate.Value.Value;
      ResultRequestReport.EndDate = endDate.Value.Value;
      ResultRequestReport.BusinessUnitId = currentBusinessUnit.Id;
    }

  }
}