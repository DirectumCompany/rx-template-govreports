using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace GD.ReportsModule
{
  partial class RegisterOfDelinquenciesByManagersServerHandlers
  {

    public override void BeforeExecute(Sungero.Reporting.Server.BeforeExecuteEventArgs e)
    {
      RegisterOfDelinquenciesByManagers.NotAllQuestionsAreAnswered = ReportsModule.Reports.Resources.RegisterOfDelinquenciesByManagers.NotAllQuestionsAreAnswered;
      RegisterOfDelinquenciesByManagers.NotAllOrdersHaveBeenCompleted = ReportsModule.Reports.Resources.RegisterOfDelinquenciesByManagers.NotAllOrdersHaveBeenCompleted;
      
      RegisterOfDelinquenciesByManagers.ActionItemExecutionTaskGuid = Constants.Module.Discriminator.ActionItemExecutionTask.ToString();
      RegisterOfDelinquenciesByManagers.ActionItemExecutionAssignmentGuid = Constants.Module.Discriminator.ActionItemExecutionAssignment.ToString();
      RegisterOfDelinquenciesByManagers.DocumentReviewTaskGuid = Constants.Module.Discriminator.DocumentReviewTask.ToString();      
      RegisterOfDelinquenciesByManagers.RequestGuid = Constants.Module.Discriminator.Request.ToString();
      
      RegisterOfDelinquenciesByManagers.ActionItemExecutionTaskDocumentsGroupGuid = Sungero.Docflow.PublicConstants.Module.TaskMainGroup.ActionItemExecutionTask.ToString();
      RegisterOfDelinquenciesByManagers.DocumentReviewTaskDocumentForReviewGroupGuid = Sungero.Docflow.PublicConstants.Module.TaskMainGroup.DocumentReviewTask.ToString();
    }

  }
}