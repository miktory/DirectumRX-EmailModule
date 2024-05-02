using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DevRX.HelpDesk.Server
{
  public class ModuleFunctions
  {

    [Remote(IsPure=true)]
    public IQueryable<IRequest> GetRequestByCompany(Sungero.Parties.ICompanyBase company)
    {
      // Получить список всех обращений сотрудника, указанного автором в
      // текущем обращении.
      return ExternalRequests.GetAll(r => Equals(r.Organization, company));
    }
    
    [Remote(IsPure=true)]
    public IQueryable<IRequest>  GetRequestById(long id)
    {
      return Requests.GetAll(r => Equals(id, r.Number));
    }  
    
   [Remote]
   public static IInternalRequest CreateInternalRequest()
   {
     // Создать внутреннее обращение.
     return InternalRequests.Create();
   }


  }
}