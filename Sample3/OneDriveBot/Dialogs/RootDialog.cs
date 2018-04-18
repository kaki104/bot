using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotAuth;
using BotAuth.AADv2;
using BotAuth.Dialogs;
using BotAuth.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using OneDriveBot.Models;

namespace OneDriveBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly AuthenticationOptions _authenticationOptions = new AuthenticationOptions
        {
            Authority = ConfigurationManager.AppSettings["aad:Authority"],
            ClientId = ConfigurationManager.AppSettings["aad:ClientId"],
            ClientSecret = ConfigurationManager.AppSettings["aad:ClientSecret"],
            Scopes = new[] {"Files.Read"},
            RedirectUrl = ConfigurationManager.AppSettings["aad:Callback"],
            MagicNumberView = "/magic.html#{0}"
        };

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            switch (activity.Text.ToLower())
            {
                case "get music files":
                    // Using the OneDrive search API through the Microsoft Graph
                    //var query = "https://graph.microsoft.com/v1.0/me/drive/search(q='{0}')?$select=id,name,size,webUrl&$top=5&$expand=thumbnails";
                    var query = "https://graph.microsoft.com/v1.0/me/drive/special/music/children?$top=2";
                    // Save the query so we can run it after authenticating
                    context.ConversationData.SetValue("GraphQuery", query);
                    // Forward the dialog to the AuthDialog to sign the user in and get an access token for calling the Microsoft Graph and then execute the specific action
                    await context.Forward(new AuthDialog(new MSALAuthProvider(), _authenticationOptions),
                        GetOneDriveMusicFiles, activity, CancellationToken.None);
                    return;
                case "get photo files":
                    var q2 = "https://graph.microsoft.com/v1.0/me/drive/special/cameraroll/children?$orderby=createdDateTime desc&$filter=file/mimeType eq 'image/jpeg'&$top=5";
                    context.ConversationData.SetValue("GraphQuery", q2);
                    await context.Forward(new AuthDialog(new MSALAuthProvider(), _authenticationOptions),
                        GetOneDrivePhotoFiles, activity, CancellationToken.None);
                    return;
            }

            // calculate something for us to return
            var length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }

        private async Task GetOneDrivePhotoFiles(IDialogContext context, IAwaitable<AuthResult> result)
        {
            // Getting the token from the Microsoft Graph
            var tokenInfo = await result;

            // Get the Documents from the OneDrive of the Signed-In User
            var json = await new HttpClient().GetWithAuthAsync(tokenInfo.AccessToken,
                context.ConversationData.GetValue<string>("GraphQuery"));

            var root = JsonConvert.DeserializeObject<Rootobject>(json.ToString());
            var reply = ((Activity) context.Activity).CreateReply();
            foreach (var photo in root.value)
            {
                var card = new HeroCard
                {
                    Title = photo.name,
                    Subtitle = $"Taken: {photo.photo?.takenDateTime}",
                    Images = new List<CardImage>
                    {
                        new CardImage(photo.microsoftgraphdownloadUrl)
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.DownloadFile, "Download File", value: photo.microsoftgraphdownloadUrl)
                    }
                };
                reply.Attachments.Add(card.ToAttachment());
            }
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var client = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            await client.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task GetOneDriveMusicFiles(IDialogContext context, IAwaitable<AuthResult> result)
        {
            // Getting the token from the Microsoft Graph
            var tokenInfo = await result;

            // Get the Documents from the OneDrive of the Signed-In User
            var json = await new HttpClient().GetWithAuthAsync(tokenInfo.AccessToken,
                context.ConversationData.GetValue<string>("GraphQuery"));

            var root = JsonConvert.DeserializeObject<Rootobject>(json.ToString());
            var reply = ((Activity) context.Activity).CreateReply();

            foreach (var music in root.value)
            {
                var audioCard = new AudioCard
                {
                    Title = music.name,
                    Subtitle = $"Artist: {music.audio.artist}, Genre: {music.audio.genre}",
                    Media = new List<MediaUrl>
                    {
                        new MediaUrl(music.microsoftgraphdownloadUrl)
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.OpenUrl, "Open File", value: music.webUrl)
                    }
                };
                if (reply.Attachments.Any() == false) audioCard.Autostart = true;
                reply.Attachments.Add(audioCard.ToAttachment());
            }
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var client = new ConnectorClient(new Uri(context.Activity.ServiceUrl));
            await client.Conversations.ReplyToActivityAsync(reply);
        }
    }
}