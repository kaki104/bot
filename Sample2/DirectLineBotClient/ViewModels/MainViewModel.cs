using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using DirectLineBotClient.Helpers;
using DirectLineBotClient.Services;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using WebSocketSharp;

namespace DirectLineBotClient.ViewModels
{
    public class MainViewModel : Observable
    {
        #region Secret
        private const string _directLineSecret = "Your bot secret";
        #endregion
        private const string _botId = "Your bot id";
        private const string _fromUser = "kaki104";
        private DirectLineClient _client;
        private Conversation _conversation;
        private string _conversationText;
        private IList<Activity> _dialogList;
        private string _inputText;
        private string _watermark;

        /// <summary>
        ///     생성자
        /// </summary>
        public MainViewModel()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                NavigationService.Navigated += NavigationService_Navigated;
                Init();
            }
            DialogList = new ObservableCollection<Activity>();
        }

        /// <summary>
        ///     입력 값
        /// </summary>
        public string InputText
        {
            get => _inputText;
            set => Set(ref _inputText, value);
        }

        /// <summary>
        ///     다이얼로그 리스트
        /// </summary>
        public IList<Activity> DialogList
        {
            get => _dialogList;
            set => Set(ref _dialogList, value);
        }

        /// <summary>
        ///     Send 커맨드
        /// </summary>
        public ICommand SendCommand { get; set; }

        /// <summary>
        ///     컨버세이션 택스트
        /// </summary>
        public string ConversationText
        {
            get => _conversationText;
            set => Set(ref _conversationText, value);
        }

        /// <summary>
        ///     초기화
        /// </summary>
        private void Init()
        {
            SendCommand = new RelayCommand(ExecuteSendCommand);
        }

        /// <summary>
        ///     대화 송신
        /// </summary>
        private async void ExecuteSendCommand()
        {
            if (string.IsNullOrEmpty(InputText)) return;

            var userMessage = new Activity
            {
                From = new ChannelAccount(_fromUser),
                Text = InputText,
                Type = ActivityTypes.Message
            };

            InputText = string.Empty;
            DialogList.Add(userMessage);

            await _client.Conversations.PostActivityAsync(_conversation.ConversationId, userMessage);
        }

        /// <summary>
        ///     페이지 네비게이션된 후~
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void NavigationService_Navigated(object sender, NavigationEventArgs e)
        {
            await StartBotConversation();
        }

        /// <summary>
        ///     대화 시작
        /// </summary>
        /// <returns></returns>
        private async Task StartBotConversation()
        {
            //다이렉트라인 클라이언트 생성
            _client = new DirectLineClient(_directLineSecret);
            _conversation = await _client.Conversations.StartConversationAsync();

            ConversationText = $"ConversationId : {_conversation.ConversationId}\n";
            ConversationText += $"StreamUrl : {_conversation.StreamUrl}\n";
            ConversationText += $"ETag : {_conversation.ETag}\n";
            ConversationText += $"ExpiresIn : {_conversation.ExpiresIn}\n";
            ConversationText += $"ReferenceGrammarId : {_conversation.ReferenceGrammarId}\n";

            var webSocketClient = new WebSocket(_conversation.StreamUrl);
            webSocketClient.OnMessage += WebSocketClient_OnMessage;
            webSocketClient.Connect();
        }

        /// <summary>
        ///     대화 수신
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void WebSocketClient_OnMessage(object sender, MessageEventArgs e)
        {
            // Occasionally, the Direct Line service sends an empty message as a liveness ping. Ignore these messages.
            if (string.IsNullOrWhiteSpace(e.Data))
                return;

            var activitySet = JsonConvert.DeserializeObject<ActivitySet>(e.Data);
            _watermark = activitySet.Watermark;

            var activities = from x in activitySet.Activities
                where x.From.Id == _botId
                select x;

            foreach (var activity in activities)
                await CoreApplication.MainView.Dispatcher
                    .RunAsync(CoreDispatcherPriority.Normal,
                    () => { DialogList.Add(activity); });

            //if (activity.Attachments != null)
            //    foreach (var attachment in activity.Attachments)
            //        switch (attachment.ContentType)
            //        {
            //            case "application/vnd.microsoft.card.hero":
            //                RenderHeroCard(attachment);
            //                break;

            //            case "image/png":
            //                Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");
            //                Process.Start(attachment.ContentUrl);
            //                break;
            //        }
        }
    }
}
