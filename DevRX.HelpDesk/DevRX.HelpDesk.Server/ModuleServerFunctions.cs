using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DevRX.HelpDesk.Server
{
  public class ModuleFunctions
  {
    
   /// <summary>
   /// Создать внутреннее обращение.
   /// </summary>
   /// <returns>Созданное внутреннее обращение.</returns>
   [Remote]
   public static IInternalRequest CreateInternalRequest()
   {
     // Создать внутреннее обращение.
     return InternalRequests.Create();
   }


  }
}