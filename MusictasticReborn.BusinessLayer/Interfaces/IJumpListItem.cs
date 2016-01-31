using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace MusictasticReborn.UserControls.Extensions
{
    public interface IJumpListItem
    {
        string Text { get; }

        bool IsHeader { get; }

        SolidColorBrush Background { get; }
    }
}
