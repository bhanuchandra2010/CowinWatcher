using SlackBotMessages;
using SlackBotMessages.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowinWatcher
{
    class SlackManager
    {
        string WebHookUrl = "https://hooks.slack.com/services/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";
        SbmClient client;
        public SlackManager()
        {
            client = new SbmClient(WebHookUrl);
        }
        public void SendMessage(string msg)
        {
            var message = new Message() { Username = "Cowin Watcher", Channel = "personal-notifications", Text = msg, IconEmoji = ":hospital:" };
            client.Send(message);
        }


    }
}
