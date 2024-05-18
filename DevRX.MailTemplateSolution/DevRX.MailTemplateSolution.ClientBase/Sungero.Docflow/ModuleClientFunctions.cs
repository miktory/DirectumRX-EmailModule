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
      CreateSendMailDialog();
      //Functions.Module.Remote.Test();
      
    }
    
    [LocalizeFunction("Отправка письма", "Отправка письма")]
    public virtual void CreateSendMailDialog()
    {
      var dialog = Dialogs.CreateInputDialog("Отправить письмо");
      var email = dialog.AddString("E-Mail",true);
      var subject = dialog.AddString("Тема письма",true);
      var documents = dialog.AddSelectMany("Документ", false, Sungero.Content.ElectronicDocuments.Null);
      var dataSource = dialog.AddSelect("Источник данных", false, Sungero.Content.ElectronicDocuments.Null);
      var template = dialog.AddSelect("Шаблон письма", true, MailTemplate.Templates.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        bool isConfirmed = Dialogs.CreateConfirmDialog("Отправить письмо?").Show();
        if (isConfirmed)
        {
          foreach (var doc in documents.Value)
          {
            if (doc.LastVersion == null)
            {
              Dialogs.ShowMessage("Ошибка. Один или несколько документов не имеют созданной версии.", MessageType.Error);
              return;
            }
          }
          try
          {
            Functions.Module.Remote.SendMailByTemplate(email.Value,subject.Value,template.Value.HtmlTemplate,documents.Value.ToList(), dataSource.Value);
            Dialogs.ShowMessage("Письмо отправлено.", MessageType.Information);
          }
          catch
          {
            Dialogs.ShowMessage("Ошибка при отправке письма.", MessageType.Error);
          }
        }
      }
      
    }
  }
}