using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;

namespace DirectLineBotClient.Selectors
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate HeroCardTempalte { get; set; }

        public DataTemplate ImageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case Attachment _:
                    return ImageTemplate;
                case HeroCard _:
                    return HeroCardTempalte;
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
