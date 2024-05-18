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
    [Public, LocalizeFunction("Отправка письма", "Отправка письма из шаблона с возможностью импорта данных документа и добавления вложений")]
    public virtual void SendMail()
    {
      MailTemplateSolution.Module.Docflow.PublicFunctions.Module.SendMail();
    }

  }
}