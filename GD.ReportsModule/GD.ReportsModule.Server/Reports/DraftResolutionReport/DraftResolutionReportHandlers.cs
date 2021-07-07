using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class DraftResolutionReportServerHandlers
  {

    public override void AfterExecute(Sungero.Reporting.Server.AfterExecuteEventArgs e)
    {
      Sungero.Docflow.PublicFunctions.Module.DeleteReportData(Constants.DraftResolutionReport.SourceTableName, DraftResolutionReport.ReportSessionId);
    }

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      // ИД отчета.
      var reportSessionId = Guid.NewGuid().ToString();
      DraftResolutionReport.ReportSessionId = reportSessionId;
      
      // Автор.
      var author = DraftResolutionReport.Author;
      if (author != null)
      {
        if (author.JobTitle != null)
          DraftResolutionReport.AuthorJobTitle = author.JobTitle.Name;
        
        DraftResolutionReport.AuthorShortName = Sungero.Company.PublicFunctions.Employee.GetReverseShortName(author);
      }
      
      // Подготовивший сотрудник.
      var preparedUser = DraftResolutionReport.PreparedUser ?? Users.Current;
      if (preparedUser != null)
      {
        DraftResolutionReport.PreparedBy = PublicFunctions.Module.GetDraftResolutionReportPreparedUserInfo(preparedUser);
      }
      
      // Номер и дата документа.
      var document = DraftResolutionReport.Document;
      DraftResolutionReport.DocumentShortName = string.Empty;
      if (document != null)
      {
        DraftResolutionReport.DocumentShortName = PublicFunctions.Module.GetDraftResolutionReportDocumentInfo(document);
      }
      
      // НОР.
      DraftResolutionReport.BusinessUnit = string.Empty;
      if (document != null && document.BusinessUnit != null)
      {
        DraftResolutionReport.BusinessUnit = document.BusinessUnit.Name;
      }
      else if (author != null && author.Department != null && author.Department.BusinessUnit != null)
      {
        DraftResolutionReport.BusinessUnit = author.Department.BusinessUnit.Name;
      }
      
      // Получить данные для отчета.
      var actionItems = DraftResolutionReport.Resolution.ToList();
      var reportData = PublicFunctions.Module.GetDraftResolutionReportData(reportSessionId, actionItems);
      
      for (var index = 0; index < reportData.Count; index++)
      {
        var item = reportData[index];
        
        if (!string.IsNullOrEmpty(item.ResolutionLabel))
        {
          item.ResolutionLabel += Environment.NewLine + Environment.NewLine;
        }
        else if (index == reportData.Count - 1)
        {
          item.ResolutionLabel += Environment.NewLine;
        }
      }
      
      // Записать данные в таблицу.
      Sungero.Docflow.PublicFunctions.Module.WriteStructuresToTable(Constants.DraftResolutionReport.SourceTableName, reportData);
    }

  }
}