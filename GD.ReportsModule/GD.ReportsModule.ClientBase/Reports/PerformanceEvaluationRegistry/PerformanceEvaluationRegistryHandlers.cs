using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class PerformanceEvaluationRegistryClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      var current = Sungero.Company.Employees.Current;
      if (current == null || current.Department == null || current.Department.BusinessUnit == null)
        e.Cancel = true;
      var businessUnit = current.Department.BusinessUnit;
      
      // Получаем номер текущего квартала для подстановки значения по умолчанию.
      var today = Calendar.Today;
      int quarterNow = (today.Month - 1) / 3 + 1;
      
      // Создаём диалог с запросом квартала и года для построение отчета.
      var dialog = Dialogs.CreateInputDialog(Resources.ReportParameters);
      var quarterNumberCtrl = dialog.AddSelect(Resources.Quarter, true, quarterNow.ToString()).From(new string[]{"1", "2", "3", "4"});
      var yearCtrl = dialog.AddDate(Resources.Year, true, Calendar.Today).AsYear();
      if (dialog.Show() != DialogButtons.Ok)
      {
        e.Cancel = true;
        return;
      }
      var year = yearCtrl.Value.Value.Year;
      int quarterNumber = 0;
      int.TryParse(quarterNumberCtrl.Value, out quarterNumber);
      // Высчитываем начальные и конечные даты выбранного и предыдущего ему кварталов.
      // Начальная дата квартала.
      var qStartDate = new DateTime(year, (quarterNumber - 1) * 3 + 1,1);
      // Конечная дата квартала.
      var qEndDate = qStartDate.AddMonths(3).AddDays(-1);
      // Начальная дата предыдущего квартала.
      var pqStartDate = qStartDate.AddMonths(-3);
      // Конечная дата предыдущего квартала.
      var pqEndDate = qEndDate.AddMonths(-3);
      
      // Передаём даты в отчёт.
      PerformanceEvaluationRegistry.LQStartDate = pqStartDate;
      PerformanceEvaluationRegistry.LQEndDate = pqEndDate;
      PerformanceEvaluationRegistry.StartDate = qStartDate;
      PerformanceEvaluationRegistry.EndDate = qEndDate;
      //PerformanceEvaluationRegistry.BusinessUnitTypeGuid = GD.PTOEDMS.PublicConstants.Module.BusinessUnitTypeGuid;
      PerformanceEvaluationRegistry.BusinessUnitId = businessUnit.Id;
      
      PerformanceEvaluationRegistry.QNumber = quarterNumber;
      PerformanceEvaluationRegistry.LQNumber = quarterNumber == 1 ? 4 : quarterNumber - 1;
      PerformanceEvaluationRegistry.NotSet = Resources.NotSet;
    }

  }
}