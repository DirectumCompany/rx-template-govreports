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
      var businessUnit = Sungero.Company.Employees.Current?.Department.BusinessUnit;
      
      var dialog = Dialogs.CreateInputDialog(Resources.ReportParameters);
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
      ResultRequestReport.BusinessUnitId = selectedBusinessUnit.Value == null ? -1 : selectedBusinessUnit.Value.Id;
      ResultRequestReport.AllBusinessUnit =  allBusinessUnit.Value == true;
    }

  }
}