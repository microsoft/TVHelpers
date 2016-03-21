using MediaAppSample.Core.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls
{
    public sealed class ItemTypeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MovieTemplate { get; set; }
        public DataTemplate TvEpisodeTemplate { get; set; }
        public DataTemplate TvSeriesTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is ItemBase)
            {
                if (item is MovieModel)
                    return this.MovieTemplate;
                else if (item is TvSeriesModel)
                    return this.TvSeriesTemplate;
                else if (item is TvEpisodeModel)
                    return this.TvEpisodeTemplate;
                else
                    throw new NotImplementedException();
            }
            else
                throw new InvalidOperationException("Item is not of type ItemBase. This template selector expects all items to be of type ItemBase.");
        }
    }
}
