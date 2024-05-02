using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DevRX.HelpDesk.Server
{
  public class ModuleJobs
  {

    /// <summary>
    /// 
    /// </summary>
    public virtual void SendNotificationAboutRequests()
    {
      var newRequests = Requests.GetAll(r => r.CreatedDate.Value > Calendar.Today.BeginningOfWeek());
      // Получить исполнителя по задаче,
      // в рамках данной практики - произвольный сотрудник.
      var companyName = "Служба поддержки";
      var company = Sungero.Company.Departments.GetAll().Where(x => x.Name == companyName).First();
      var manager = company.Manager;
      // Создать и стартовать задачу.
      var task= Sungero.Workflow.SimpleTasks.CreateWithNotices("Статистика по обращениям", manager);
      task.ActiveText = "Зарегистрировано обращений: " + newRequests.Count();
      task.Start();
    }

  }
}