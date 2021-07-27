using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class RegisterOfDelinquenciesByManagersClientHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Client.BeforeExecuteEventArgs e)
    {
      // Получаем Организацию текущего пользователя для диалога
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
      RegisterOfDelinquenciesByManagers.StartDate = startDate.Value.Value;
      RegisterOfDelinquenciesByManagers.EndDate = endDate.Value.Value;
      RegisterOfDelinquenciesByManagers.BusinessUnitId = businessUnit.Id;
      RegisterOfDelinquenciesByManagers.BusinessUnit = businessUnit.Name;
      //RegisterOfDelinquenciesByManagers.RequestGuid = DirRX.PostMigrator.PublicConstants.Module.Discriminators.RequestsDiscriminator.ToString();
    }

  }
}