using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;

namespace DirectLineBotClient.Converters
{
    public class AttachmentToHeroCardConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is IList list)) return value;
            var item = list.Cast<object>().FirstOrDefault();
            if (!(item is Attachment attachment)) return value;
            switch (attachment.ContentType)
            {
                case "application/vnd.microsoft.card.hero":
                    var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());
                    return new List<object>
                    {
                        heroCard
                    };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
