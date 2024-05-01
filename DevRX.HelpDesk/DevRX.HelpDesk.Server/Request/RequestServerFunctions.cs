using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;

namespace DevRX.HelpDesk.Server
{
  partial class RequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>       
    public void Function()
    {
      
    }

    /// <summary>
    /// Создать связанный с обращением документ.
    /// </summary>
    /// <returns>Документ.</returns>
    [Remote]
    public IAddendumRequest CreateAddendumRequest()
    {
      var document = AddendumRequests.Create();
      document.Request = _obj;
      document.Name = string.Format("Приложение к обращению № {0}", _obj.Number);
      return document;
    }
    
        [Remote(IsPure=true)]
    public IQueryable<IAddendumRequest>  GetAddendumRequests()
    {
      return AddendumRequests.GetAll(r => Equals(r.Request.Responsible, _obj.Responsible));
    }
    
          


  }
}