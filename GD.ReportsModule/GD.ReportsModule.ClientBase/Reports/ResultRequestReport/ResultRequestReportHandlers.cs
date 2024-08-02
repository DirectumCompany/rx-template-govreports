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
        return;
      }
      
      var dialog = Dialogs.CreateInputDialog(Resources.ReportParameters);
      var businessUnit = current.Department.BusinessUnit;
      var allBusinessUnit = dialog.AddBoolean(Resources.AllBusinessUnits, true);
      var selectedBusinessUnit = dialog.AddSelect(Reports.Resources.ResultRequestReport.SelectBusinessUnit, false, businessUnit)
        .From(Sungero.Company.BusinessUnits.GetAll());
      
      dialog.SetOnRefresh(
        args =>
        {
          selectedBusinessUnit.IsVisible = !allBusinessUnit.Value.Value;
          selectedBusinessUnit.IsRequired = !allBusinessUnit.Value.Value;
        });
      
      var startDate = dialog.AddDate(Resources.StartDate, true, Calendar.BeginningOfYear(Calendar.Today));
      var endDate = dialog.AddDate(Resources.EndDate, true, Calendar.Today);
      
      if (dialog.Show() != DialogButtons.Ok)
      {
        e.Cancel = true;
        return;
      }
      
      ResultRequestReport.BusinessUnitName = !allBusinessUnit.Value.Value ?
        selectedBusinessUnit.Value.Name : Reports.Resources.ResultRequestReport.ForAllBusinessUnit;
      ResultRequestReport.StartDate = startDate.Value.Value;
      ResultRequestReport.EndDate = endDate.Value.Value;
      if (selectedBusinessUnit.Value != null)
        businessUnit = selectedBusinessUnit.Value;
      ResultRequestReport.BusinessUnitId = businessUnit.Id;
      ResultRequestReport.AllBusinessUnit = allBusinessUnit.Value == true;
    }

  }
}