using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace MusictasticReborn.UserControls.Extensions
{
    public class StandardJumpListHeader : IJumpListItem
    {
        public string Text { get; set; }

        public bool IsHeader => true;

        public SolidColorBrush Background { get; private set; }

        public StandardJumpListHeader(string text, SolidColorBrush background)
        {
            Text = text;
            Background = background;
        }
    }
}
