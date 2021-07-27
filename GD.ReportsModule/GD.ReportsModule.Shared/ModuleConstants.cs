using System;
using Sungero.Core;

namespace GD.ReportsModule.Constants
{
  public static class Module
  {
    // ГУИДы типов сущностей.
    public static class Discriminator
    {
      // "Обращения граждан".
      public static readonly Guid Request = Guid.Parse("77300c65-4db1-4baf-9321-7e8efb7805cb");
      
      // "Задача на исполнение поручений".
      public static readonly Guid ActionItemExecutionTask = Guid.Parse("c290b098-12c7-487d-bb38-73e2c98f9789");
      
      // "Задание на исполнение поручений".
      public static readonly Guid ActionItemExecutionAssignment = Guid.Parse("d238ef51-607e-46a5-b86a-ede4482f7f19");
      
      // "Задача на рассмотрение".
      public static readonly Guid DocumentReviewTask = Guid.Parse("4ef03457-8b42-4239-a3c5-d4d05e61f0b6");      
    }
  }
}