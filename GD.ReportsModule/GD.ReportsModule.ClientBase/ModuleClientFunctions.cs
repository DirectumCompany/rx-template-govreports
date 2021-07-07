using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Сформировать отчет "Проект резолюции".
    /// </summary>
    /// <param name="tasks">Список задач на исполнение поручения.</param>
    /// <param name="textResolution">Текст резолюции.</param>
    /// <param name="document">Документ.</param>
    /// <param name="author">Сотрудник, выдавший резолюцию.</param>
    /// <param name="parameters">Дополнительные параметры.</param>
    [Public]
    public virtual void OpenDraftResolution(List<Sungero.RecordManagement.IActionItemExecutionTask> tasks, string textResolution, Sungero.Docflow.IOfficialDocument document, Sungero.Company.IEmployee author, object[] parameters)
    {
      var report = GD.ReportsModule.PublicFunctions.Module.GetDraftResolutionReport();
      report.Resolution.AddRange(tasks);
      report.TextResolution = textResolution;
      report.Document = document;
      report.Author = author;
      report.Open();
    }

  }
}