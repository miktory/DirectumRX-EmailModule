using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;

namespace DevRX.MailTemplate.Client
{
  public class ModuleFunctions
  {

    /// <summary>
    /// Отправить электронное письмо одному или нескольким получателям.
    /// </summary>
    /// 
    [Public]
    public virtual void SendMail()
    {
      MailTemplateSolution.Module.Docflow.PublicFunctions.Module.SendMail();
    }

  }
}