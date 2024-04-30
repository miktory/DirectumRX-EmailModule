using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using DevRX.HelpDesk.Request;

namespace DevRX.HelpDesk.Shared
{
  partial class RequestFunctions
  {

    /// <summary>
    /// 
    /// </summary>       
    public string FormatRequestSubject()
    {
      return string.Format("{0} № {1} от {2}: {3}",_obj.RequestKind, _obj.Number.ToString(), _obj.CreatedDate.ToString().Split(' ')[0], _obj.Description.Substring(0,50));
    }

  }
}