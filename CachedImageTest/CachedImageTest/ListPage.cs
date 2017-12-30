using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms;

namespace CachedImageTest
{
    public class ListPage : ContentPage
    {
        public ListPage()
        {
            var list = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemsSource = Items,
                HasUnevenRows = true,
                ItemTemplate = new DataTemplate(() => new ListViewCell()),
                RowHeight = -1
            };

            Content = list;
        }

        public ExtendedObservableCollection<Item> Items { get; set; } = new ExtendedObservableCollection<Item>();

        protected override void OnAppearing()
        {
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Item 1",
                    Subtitle = "Subtitle",
                    Icon = "https://icons.bitwarden.com/github.com/icon.png"
                },
                new Item
                {
                    Name = "Item 2",
                    Subtitle = "Subtitle",
                    Icon = "https://icons.bitwarden.com/google.com/icon.png"
                },
                new Item
                {
                    Name = "Item 3",
                    Subtitle = "Subtitle",
                    Icon = "https://icons.bitwarden.com/amazon.com/icon.png"
                },
                new Item
                {
                    Name = "Item 4",
                    Subtitle = "Subtitle",
                    Icon = "https://icons.bitwarden.com/codeschool.com/icon.png"
                },
                new Item
                {
                    Name = "Item 5",
                    Subtitle = "Subtitle",
                    Icon = "https://icons.bitwarden.com/yahoo.com/icon.png"
                }
            };

            Items.ResetWithRange(items);
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }
        public bool HasAttachments { get; set; }
        public bool Shared { get; set; }
    }

    public class LabeledDetailCell : ViewCell
    {
        public LabeledDetailCell()
        {
            Icon = new CachedImage
            {
                WidthRequest = 20,
                HeightRequest = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                ErrorPlaceholder = "login.png",
                CacheDuration = TimeSpan.FromDays(30),
                BitmapOptimizations = true
            };

            Label = new Label
            {
                LineBreakMode = LineBreakMode.TailTruncation,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };

            Detail = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                LineBreakMode = LineBreakMode.TailTruncation
            };

            LabelIcon = new CachedImage
            {
                WidthRequest = 16,
                HeightRequest = 16,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(5, 0, 0, 0)
            };

            LabelIcon2 = new CachedImage
            {
                WidthRequest = 16,
                HeightRequest = 16,
                HorizontalOptions = LayoutOptions.Start,
                Margin = new Thickness(5, 0, 0, 0)
            };

            Button = new Button
            {
                WidthRequest = 60
            };

            var grid = new Grid
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
                Padding = new Thickness(3, 3, 0, 3)
            };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40, GridUnitType.Absolute) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60, GridUnitType.Absolute) });
            grid.Children.Add(Icon, 0, 0);
            grid.Children.Add(Label, 1, 0);
            grid.Children.Add(Detail, 1, 1);
            grid.Children.Add(LabelIcon, 2, 0);
            grid.Children.Add(LabelIcon2, 3, 0);
            grid.Children.Add(Button, 4, 0);
            Grid.SetRowSpan(Icon, 2);
            Grid.SetRowSpan(Button, 2);
            Grid.SetColumnSpan(Detail, 3);

            if(Device.RuntimePlatform == Device.Android)
            {
                Label.TextColor = Color.Black;
            }

            View = grid;
        }

        public CachedImage Icon { get; private set; }
        public Label Label { get; private set; }
        public Label Detail { get; private set; }
        public CachedImage LabelIcon { get; private set; }
        public CachedImage LabelIcon2 { get; private set; }
        public Button Button { get; private set; }
    }

    public class ListViewCell : LabeledDetailCell
    {
        public ListViewCell()
        {
            Label.SetBinding(Label.TextProperty, nameof(Item.Name));
            Detail.SetBinding(Label.TextProperty, nameof(Item.Subtitle));
            LabelIcon.SetBinding(VisualElement.IsVisibleProperty, nameof(Item.Shared));
            LabelIcon2.SetBinding(VisualElement.IsVisibleProperty, nameof(Item.HasAttachments));

            Button.Image = "more.png";
            Button.BackgroundColor = Color.Transparent;

            LabelIcon.Source = "share.png";
            LabelIcon2.Source = "paperclip.png";
        }

        protected override void OnBindingContextChanged()
        {
            Icon.Source = null;

            if(BindingContext is Item item)
            {
                Icon.LoadingPlaceholder = "login.png";
                Icon.Source = item.Icon;
            }

            base.OnBindingContextChanged();
        }
    }

    public class ExtendedObservableCollection<T> : ObservableCollection<T>
    {
        public ExtendedObservableCollection() : base() { }
        public ExtendedObservableCollection(IEnumerable<T> collection) : base(collection) { }
        public ExtendedObservableCollection(List<T> list) : base(list) { }

        public void AddRange(IEnumerable<T> range)
        {
            foreach(var item in range)
            {
                Items.Add(item);
            }

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ResetWithRange(IEnumerable<T> range)
        {
            Items.Clear();
            AddRange(range);
        }
    }
}
