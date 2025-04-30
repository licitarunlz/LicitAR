using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Utils
{
    public static class MessageManagerStatus
    {
        public const string navigate = "navigate";
        public const string navigateToNewTab = "navigateToNewTab";
        public const string ok = "ok";
        public const string info = "info";
        public const string error = "error";
        public const string validation = "validation";
        public const string partial = "partial";
        public const string question = "question";
        public const string exception = "exception";
    }

    public interface IMessageManager
    {
        string status { get; }
        bool HasErrors { get; }
        List<KeyValuePair<string, string>> messages { get; set; }
        object addedData { get; set; }

        void ClearMessages();
        void MessagesFromValidationErrors(IList<KeyValuePair<string, string>> ValidationsErrors, bool AppendMessages = false);
        void MergeMessageManagers(IList<IMessageManager> Messages, bool AppendMessages = false);
        void NavigateMessage(string Url, bool ToNewTab = false);
        void ErrorMessage(string ErrorMessage);
        void OkMessage(string OkMessage);
        void ValidationMessage(string ValidationMessage, string Key = null);
        void InfoMessage(string InfoMessage);
        void MergeMessageManager(IMessageManager MessageManager, bool AppendMessages = false);
        void QuestionMessage(string QuestionMessage);
        void ExceptionMessage(string ExceptionMessage, Exception ex);
    }

    public class MessageManager : IMessageManager
    {
        public string status => determineStatus();

        public bool HasErrors => messages.Any(x => x.Key != MessageManagerStatus.ok && x.Key != MessageManagerStatus.navigate && x.Key != MessageManagerStatus.info);

        public object addedData { get; set; }

        public Exception exception { get; set; }

        public List<KeyValuePair<string, string>> messages { get; set; }

       

        public MessageManager()
        {
            messages = new List<KeyValuePair<string, string>>();
        }

        public void ClearMessages()
        {
            messages.Clear();
        }

        private void AddMessage(string key, string value)
        {
            messages.Add(new KeyValuePair<string, string>(key, value));
        }

        public void MessagesFromValidationErrors(IList<KeyValuePair<string, string>> ValidationsErrors, bool AppendMessages = false)
        {
            if (AppendMessages == false)
                ClearMessages();

            if (ValidationsErrors.Count > 0)
            {
                foreach (var v in ValidationsErrors)
                {
                    AddMessage(v.Key, v.Value);
                }
            }
        }

        public void MergeMessageManager(IMessageManager MessageManager, bool AppendMessage = false)
        {
            foreach (var m in MessageManager.messages)
            {
                AddMessage(m.Key, m.Value);
            }
        }

        public void MergeMessageManagers(IList<IMessageManager> Messages, bool AppendMessages = false)
        {
            if (AppendMessages == false)
                ClearMessages();

            foreach (var cm in Messages)
            {
                foreach (var m in cm.messages)
                {
                    AddMessage(m.Key, m.Value);
                }
            }
        }

        public void OkMessage(string Message)
        {
            AddMessage(MessageManagerStatus.ok, Message);
        }

        public void NavigateMessage(string Url, bool ToNewTab = false)
        {
            if (ToNewTab)
                AddMessage(MessageManagerStatus.navigateToNewTab, Url);
            else
                AddMessage(MessageManagerStatus.navigate, Url);
        }

        public void InfoMessage(string Message)
        {
            AddMessage(MessageManagerStatus.info, Message);
        }

        public void ErrorMessage(string Message)
        {
            AddMessage(MessageManagerStatus.error, Message);
        }

        public void QuestionMessage(string Message)
        {
            AddMessage(MessageManagerStatus.question, Message);
        }

        public void ValidationMessage(string Message, string Key = null)
        {
            if (Key == null)
                AddMessage(MessageManagerStatus.validation, Message);
            else
                AddMessage(Key, Message);
        }

        public void ExceptionMessage(string ExceptionMessage, Exception ex)
        {
            AddMessage(MessageManagerStatus.exception, ExceptionMessage);
            exception = ex;

        }

        private string determineStatus()
        {
            string _status = "";

            //si no hay mensajes no pongo status
            if (messages.Count == 0)
                _status = "";

            //si hay un mensaje, en base a la key del mensaje defino el status
            else if (messages.Count == 1)
                switch (messages[0].Key)
                {
                    case MessageManagerStatus.navigate:
                        _status = MessageManagerStatus.navigate;
                        break;
                    case MessageManagerStatus.navigateToNewTab:
                        _status = MessageManagerStatus.navigateToNewTab;
                        break;
                    case MessageManagerStatus.ok:
                        _status = MessageManagerStatus.ok;
                        break;
                    case MessageManagerStatus.info:
                        _status = MessageManagerStatus.info;
                        break;
                    case MessageManagerStatus.error:
                        _status = MessageManagerStatus.error;
                        break;
                    case MessageManagerStatus.validation:
                        _status = MessageManagerStatus.validation;
                        break;
                    case MessageManagerStatus.question:
                        _status = MessageManagerStatus.question;
                        break;
                    case MessageManagerStatus.exception:
                        _status = MessageManagerStatus.exception;
                        break;
                    default: //si no es ninguno asumo que la key es el nombre de una propiedad y por ende, una validación
                        _status = MessageManagerStatus.validation;
                        break;
                }

            //Si hay más de un mensaje evaluo
            else if (messages.Count > 1)
            {
                bool hayOk = messages.Any(x => x.Key == MessageManagerStatus.ok);
                bool hayInfo = messages.Any(x => x.Key == MessageManagerStatus.info);
                bool hayQuestion = messages.Any(x => x.Key == MessageManagerStatus.question);
                bool hayException = messages.Any(x => x.Key == MessageManagerStatus.exception);
                if (HasErrors)
                {
                    //Si hay errores y hay ok e info es parcial
                    if (hayOk || hayInfo || hayQuestion)
                        _status = MessageManagerStatus.partial;
                    //Si hay errores y no hay ok e info digo que es validation (aunque haya errors, los trato igual)
                    else if (hayException)
                        _status = MessageManagerStatus.exception;
                    else
                        _status = MessageManagerStatus.validation;
                }
                else
                {
                    //Si no hay errores y hay ok y no info es ok
                    if (hayOk && !hayInfo)
                        _status = MessageManagerStatus.ok;
                    //Si no hay errores y hay info y no ok es info
                    else if (hayInfo && !hayOk)
                        _status = MessageManagerStatus.info;
                    //Si no hay errores y hay info y ok es partial
                    else if (hayInfo && hayOk)
                        _status = MessageManagerStatus.partial;
                    //Si no hay errores y no hay info ni ok es navigate (no se deberían mandar varios navigates a menos que sea deliberado por una situación particular)
                    else if (!hayInfo && !hayOk)
                        _status = MessageManagerStatus.navigate;
                    else if (hayQuestion && (!hayOk || !hayInfo)) //este no se tendria que combinar con ningun otro. Si esta solo, es un question. 
                        _status = MessageManagerStatus.question;
                }

            }

            return _status;
        }


    }
}
