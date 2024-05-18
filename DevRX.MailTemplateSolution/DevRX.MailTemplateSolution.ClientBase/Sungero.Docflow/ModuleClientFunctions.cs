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
      var documents = dialog.AddSelectMany("Документ", false, Sungero.Content.ElectronicDocuments.Null);
      var template = dialog.AddSelect("Шаблон пиьсма", true, MailTemplate.Templates.Null);
      if (dialog.Show() == DialogButtons.Ok)
      {
        try
        {
        Functions.Module.Remote.SendMailByTemplate(email.Value,template.Value.HtmlTemplate,documents.Value.ToList());
        }
        catch {}
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