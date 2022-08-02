using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using Sungero.Domain.Initialization;

namespace GD.ReportsModule.Server
{
  public partial class ModuleInitializer
  {

    public override bool IsModuleVisible()
    {
      return true;
    }

    public override void Initializing(Sungero.Domain.ModuleInitializingEventArgs e)
    {
      // Создать таблицу отчета.
      var draftResolutionReportTableName = GD.ReportsModule.Constants.DraftResolutionReport.SourceTableName;
      Sungero.Docflow.PublicFunctions.Module.DropReportTempTables(new[] { draftResolutionReportTableName });
      Sungero.Docflow.PublicFunctions.Module.ExecuteSQLCommandFormat(Queries.DraftResolutionReport.CreateDraftResolutionReportTable, new[] { draftResolutionReportTableName });
      
      // Выдать права на отчёт.
      var allUsers = Roles.AllUsers;
      if (allUsers != null)
        Reports.AccessRights.Grant(Reports.GetDraftResolutionReport().Info, allUsers, DefaultReportAccessRightsTypes.Execute);
    }
  }
}
