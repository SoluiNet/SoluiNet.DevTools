using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SoluiNet.DevTools.Core.UI
{
    public class ExtendableGrid<T> : Grid where T : UserControl
    {
        public ExtendableGrid() : base()
        {
            var gridLengthConverter = new GridLengthConverter();

            this.RowDefinitions.Add(new RowDefinition() { Height = (GridLength)gridLengthConverter.ConvertFrom(35) });

            var button = new Button();

            button.Content = "+";

            button.HorizontalAlignment = HorizontalAlignment.Right;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Width = 25;
            button.Height = 25;
            button.Margin = new Thickness(5);

            button.SetValue(Grid.RowProperty, 0);

            button.Click += (object sender, RoutedEventArgs e) =>
            {
                AddElement(CreateNewElement());
            };

            this.Children.Add(button);
        }

        public delegate T NewElementCreation();

        public NewElementCreation CreateNewElement { get; set; }

        public void AddElement(T newElement)
        {
            this.RowDefinitions.Add(new RowDefinition());

            newElement.SetValue(Grid.RowProperty, this.RowDefinitions.Count - 1);

            if (newElement is RemovableUserControl)
            {
                (newElement as RemovableUserControl).RemoveElement = () =>
                {
                    this.Children.Remove(newElement);
                };
            }

            this.Children.Add(newElement);
        }
    }
}
