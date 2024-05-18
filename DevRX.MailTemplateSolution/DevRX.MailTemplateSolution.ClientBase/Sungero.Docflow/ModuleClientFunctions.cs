using System;
using System.Collections.Generic;
using System.Linq;
using Sungero.Core;
using Sungero.CoreEntities;
using System.IO;

namespace DevRX.MailTemplateSolution.Module.Docflow.Client
{
  partial class ModuleFunctions
  {

    /// <summary>
    /// 
    /// </summary>
    [Public]
    public void Test()
    {
      SendMail();
      //Functions.Module.Remote.Test();
      
    }
    
    /// <summary>
    /// Отправить электронное письмо одному или нескольким получателям.
    /// </summary>
    /// <returns>True - если письмо отправлено. False - если письмо не отправлено.</returns>
    [Public, LocalizeFunction("Отправка письма", "Отправка письма из шаблона с возможностью импорта данных документа и добавления вложений")]
    public virtual bool SendMail()
    {
      var dialog = Dialogs.CreateInputDialog("Отправить письмо");
      var email = dialog.AddString("E-Mail",true);
      var subject = dialog.AddString("Тема письма",true);
      var documents = dialog.AddSelectMany("Документ", false, Sungero.Content.ElectronicDocuments.Null);
      var copyRecipients = dialog.AddSelectMany("Получатели копии", false, Sungero.Parties.People.Null);
      var dataSource = dialog.AddSelect("Источник данных", false, Sungero.Content.ElectronicDocuments.Null);
      var template = dialog.AddSelect("Шаблон письма", true, MailTemplate.Templates.Null);
      var result = false;
      if (dialog.Show() == DialogButtons.Ok)
      {
        bool isConfirmed = Dialogs.CreateConfirmDialog("Отправить письмо?").Show();
        if (isConfirmed)
        {
          foreach (var person in copyRecipients.Value)
          {
            if (String.IsNullOrEmpty(person.Email))
            {
              Dialogs.ShowMessage("У одного или нескольких получателей не указан E-Mail.", MessageType.Error);
              return false;
            }
          }
          
          foreach (var doc in documents.Value)
          {
            if (doc.LastVersion == null)
            {
              Dialogs.ShowMessage("Один или несколько документов не имеют созданной версии.", MessageType.Error);
              return false;
            }
          }
          var copyRecipientsEmails = copyRecipients.Value.Select(x => x.Email).ToList();
          try
          {
            result = Functions.Module.Remote.SendMailByTemplate(email.Value, copyRecipientsEmails, subject.Value, 
                                                       template.Value.HtmlTemplate, documents.Value.ToList(), dataSource.Value);
          }
          catch (Exception ex)
          {
            result = false;
          }
          if (result)
            Dialogs.ShowMessage("Письмо отправлено.", MessageType.Information);
          else
            Dialogs.ShowMessage("Ошибка при отправке письма.", MessageType.Error);
        }
      }
      return result;
    }

  }
}