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
      SendMailDialog();
      //Functions.Module.Remote.Test();
      
    }
    
    [Public]
    public void GetAttachment()
    {
    //  var document = Sungero.Content.ElectronicDocuments.CreateDocumentWithCreationDialog();
     // message.AddAttachment(document.LastVersion);
    // SelectDocumentDialog();
      var fileStream = File.OpenRead(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
      var attachmentName = Path.GetFileName(@"C:\Users\DirectumRobot\Downloads\windowsdesktop-runtime-6.0.29-win-x64.exe");
   //   return fileStream;
    }
    
        [LocalizeFunction("Выбрать документ", "Выбрать документ")]
    public virtual void SendMailDialog()
    {
      var dialog = Dialogs.CreateInputDialog("Отправить письмо");
      var email = dialog.AddString("E-Mail",true);
      var subject = dialog.AddString("Тема письма",true);
      var documents = dialog.AddSelectMany("Документ", false, Sungero.Content.ElectronicDocuments.Null);
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
          Functions.Module.Remote.SendMailByTemplate(email.Value,subject.Value,template.Value.HtmlTemplate,documents.Value.ToList());
          Dialogs.ShowMessage("Письмо отправлено.", MessageType.Information);
        }
        catch 
        {
          Dialogs.ShowMessage("Ошибка при отправке письма.", MessageType.Error);
        }
        }
      }
      
    }
    
        [LocalizeFunction("Найти все обращения компании", "Поиск обращений компании")]
    public virtual void FindRequestByCompany123()
    {
      var dialog = Dialogs.CreateInputDialog("Поиск обращений компании");
      var id = dialog.AddSelect("Организация", true, Sungero.Parties.CompanyBases.Null);
    }
      

  }
}