using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HelloWorldBot.Dialogs
{
    [Serializable]
    public class DirectLineBotDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();

            switch (activity?.Text.ToLower())
            {
                case "show me a hero card":
                    reply.Text = "Sample message with a HeroCard attachment";
                    var heroCardAttachment = new HeroCard
                    {
                        Title = "Justice League Hero Card",
                        Text = "Displayed in the DirectLine client",
                        Images = new List<CardImage>
                        {
                            new CardImage("http://t1.daumcdn.net/movie/1a169e0fe5a9e958beaa15996ea9bc515ad6e5ad"),
                            new CardImage("http://t1.daumcdn.net/movie/a4888be3ddeb4c905a7e06a8d08ad73630d1e15c"),
                            new CardImage("http://t1.daumcdn.net/movie/1baa976213c745a66e668517ff4c87f7196d0fb2")
                        }
                    }.ToAttachment();
                    reply.Attachments.Add(heroCardAttachment);
                    break;
                case "send me a botframework image":
                    reply.Text = "Sample message with an Image attachment";
                    var imageAttachment = new Attachment()
                    {
                        ContentType = "image/png",
                        ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
                    };
                    reply.Attachments.Add(imageAttachment);
                    break;
                default:
                    reply.Text = $"You said '{activity.Text}'";
                    break;
            }
            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }
    }
}