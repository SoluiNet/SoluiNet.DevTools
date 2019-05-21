using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Core.UI
{
    public class RemovableUserControl : UserControl
    {
        public delegate void ElementRemoval();

        public ElementRemoval RemoveElement { get; set; }
    }
}
