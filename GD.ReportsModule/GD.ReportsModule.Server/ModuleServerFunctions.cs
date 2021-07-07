using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule.Server
{
  public class ModuleFunctions
  {

    #region Отчеты
    
    #region Проект резолюции
    
    
    /// <summary>
    /// Получить данные для отчета.
    /// </summary>
    /// <param name="reportSessionId">ID сессии вызова отчёта.</param>
    /// <param name="actionItems">Список задач на исполнение поручения.</param>
    /// <returns>Данные для формирования отчета.</returns>
    [Public]
    public static List<Structures.DraftResolutionReport.DraftResolutionReportParameters> GetDraftResolutionReportData(string reportSessionId,
                                                                                                                      List<Sungero.RecordManagement.IActionItemExecutionTask> actionItems)
    {
      var reportData = new List<Structures.DraftResolutionReport.DraftResolutionReportParameters>();
      
      if (actionItems.Any())
      {
        foreach (var actionItemTask in actionItems)
        {
          // Равноправное поручение.
          if (actionItemTask.IsCompoundActionItem == true)
          {
            foreach (var part in actionItemTask.ActionItemParts)
            {
              var deadline = part.Deadline ?? actionItemTask.FinalDeadline ?? null;
              var data = GetActionItemDraftResolutionReportData(reportSessionId,
                                                                part.Assignee,
                                                                deadline,
                                                                part.ActionItemPart);
              reportData.Add(data);
            }
          }
          else
          {
            // Поручение с соисполнителями.
            var deadline = actionItemTask.Deadline;
            var subAssignees = actionItemTask.CoAssignees.Select(a => a.Assignee).ToList();
            var data = GetActionItemDraftResolutionReportData(reportSessionId,
                                                              actionItemTask.Assignee,
                                                              deadline,
                                                              string.Empty,
                                                              subAssignees.Any());
            reportData.Add(data);
            
            foreach (var subAssignee in subAssignees)
            {
              var subAssigneeData = GetActionItemDraftResolutionReportData(reportSessionId,
                                                                           subAssignee,
                                                                           deadline,
                                                                           string.Empty);
              reportData.Add(subAssigneeData);
            }
          }
        }
      }
      
      return reportData;
    }
    
    /// <summary>
    /// Сформировать данные о исполнителе.
    /// </summary>
    /// <param name="reportSessionId">ID сессии вызова отчёта.</param>
    /// <param name="assignee">Исполнитель.</param>
    /// <param name="deadline">Срок исполнения.</param>
    /// <param name="resolutionText">Текст резолюции.</param>
    /// <param name="isUnifier">Исполнитель является ответственным.</param>
    /// <returns>Данные исполнителя.</returns>
    public static Structures.DraftResolutionReport.DraftResolutionReportParameters GetActionItemDraftResolutionReportData(string reportSessionId,
                                                                                                                          Sungero.Company.IEmployee assignee,
                                                                                                                          Nullable<DateTime> deadline,
                                                                                                                          string resolutionText,
                                                                                                                          bool isUnifier = false)
    {
      var data = new Structures.DraftResolutionReport.DraftResolutionReportParameters();
      data.ReportSessionId = reportSessionId;
      
      var assigneeShortName = Sungero.Company.PublicFunctions.Employee.GetReverseShortName(assignee);
      data.PerformersLabel = string.Format("<b>{0}</b>", assigneeShortName);
      if (isUnifier)
      {
        data.PerformersLabel = string.Format("<b>{0}</b> {1}",
                                             Reports.Resources.DraftResolutionReport.UnifyingExecutorPrefix,
                                             data.PerformersLabel);
      }
      
      if (deadline.HasValue)
      {
        data.Deadline = deadline.Value.HasTime() ? deadline.Value.ToUserTime().ToString("g") : deadline.Value.ToString("d");
        data.PerformersLabel += string.Format(" {0} {1}",
                                              Resources.Deadline.ToString().ToLower(),
                                              data.Deadline);
      }
      
      data.ResolutionLabel = resolutionText;
      
      return data;
    }
    
    /// <summary>
    /// Сформировать строку с информацией о документе.
    /// </summary>
    /// <param name="document">Документ.</param>
    /// <returns>Данные о документе.</returns>
    [Public]
    public static string GetDraftResolutionReportDocumentInfo(Sungero.Docflow.IOfficialDocument document)
    {
      var documentInfo = string.Empty;
      
      if (!string.IsNullOrWhiteSpace(document.RegistrationNumber))
      {
        documentInfo += string.Format("{0} {1}",
                                      Reports.Resources.DraftResolutionReport.DocumentInfoLeadPart,
                                      document.RegistrationNumber);
      }
      
      if (document.RegistrationDate.HasValue)
      {
        if (!string.IsNullOrWhiteSpace(document.RegistrationNumber))
        {
          documentInfo += Environment.NewLine;
        }
        
        documentInfo += string.Format("{0} {1}",
                                      Reports.Resources.DraftResolutionReport.DocumentInfoDateFrom,
                                      document.RegistrationDate.Value.ToString("d"));
      }
      return documentInfo;
    }
    
    /// <summary>
    /// Сформировать строку с информацией о сотруднике.
    /// </summary>
    /// <param name="user">Пользователь.</param>
    /// <returns>Данные сотрудника.</returns>
    [Public]
    public static string GetDraftResolutionReportPreparedUserInfo(Sungero.CoreEntities.IUser user)
    {
      var preparedUserInfo = string.Empty;
      var preparedEmployee = Sungero.Company.Employees.As(user);
      
      if (preparedEmployee != null)
      {
        preparedUserInfo = string.Format("{0} {1}",
                                         Reports.Resources.DraftResolutionReport.PreparedByPrefix,
                                         Sungero.Company.PublicFunctions.Employee.GetShortName(preparedEmployee, false));
        
        if (!string.IsNullOrEmpty(preparedEmployee.Phone))
        {
          var phonePart = string.Format("{0} {1}",
                                        Reports.Resources.DraftResolutionReport.PreparedByPhonePrefix,
                                        preparedEmployee.Phone);
          
          preparedUserInfo += Environment.NewLine + phonePart;
        }
      }
      else
      {
        preparedUserInfo = string.Format("{0} {1}",
                                         Reports.Resources.DraftResolutionReport.PreparedByPrefix,
                                         user.Name);
      }
      return preparedUserInfo;
    }
    
    /// <summary>
    /// Вызов отчета DraftResolutionReport.
    /// </summary>
    [Public]
    public virtual GD.ReportsModule.IDraftResolutionReport GetDraftResolutionReport()
    {
      return GD.ReportsModule.Reports.GetDraftResolutionReport();
    }
    
    #endregion
    
    #endregion

  }
}