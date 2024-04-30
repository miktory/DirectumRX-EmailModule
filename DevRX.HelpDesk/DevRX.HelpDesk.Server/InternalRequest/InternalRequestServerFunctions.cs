using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.InternalRequest;

namespace DevRX.HelpDesk.Server
{
  partial class InternalRequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>       
    /// <summary>
    /// 
    /// </summary>
    /// <summary>
    /// Получить все обращения сотрудника, являющегося автором текущего обращения.
    /// </summary>
    /// <returns>Список обращений.</returns>
    [Remote(IsPure=true)]
    public IQueryable<IInternalRequest> GetEmployeeRequests()
    {
      // Получить список всех обращений сотрудника, указанного автором в
      // текущем обращении.
      return InternalRequests.GetAll(r => Equals(r.Author, _obj.Author));
    }

  }
}