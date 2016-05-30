// The MIT License (MIT)
//
// Copyright (c) 2016 Microsoft. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace MediaAppSample.Core.Models
{
    public enum ItemTypes
    {
        Unknown,
        Movie,
        Trailer,
        TvSeries,
        TvEpisode,
        Music,
        Video,
        Radio,
    }

    public enum Genres
    {
        Drama,
        SciFi,
        Action,
        Crime,
        Family,
        Horror,
        Comedy,
    }

    public abstract class ItemBase : ModelBase
    {
        #region Properties

        private string _ID;
        public virtual string ID
        {
            get { return _ID; }
            set { this.SetProperty(ref _ID, value); }
        }

        private ItemTypes _itemType;
        public ItemTypes ItemType
        {
            get { return _itemType; }
            set { this.SetProperty(ref _itemType, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { this.SetProperty(ref _description, value); }
        }

        private string _flag;
        public string Flag
        {
            get { return _flag; }
            set { this.SetProperty(ref _flag, value); }
        }

        public string ByLine => "'Lorem Ipsum dolor sit amet, consectetur adipiscing elit. Vesitibulum fringilla felis diam, elementum.' - Convallis";

        private string _genre;
        public string Genre
        {
            get { return _genre; }
            set { this.SetProperty(ref _genre, value); }
        }

        #region Ratings

        private string _contentRating;
        public string ContentRating
        {
            get { return _contentRating; }
            set { this.SetProperty(ref _contentRating, value); }
        }

        private double _userRating;
        public double UserRating
        {
            get { return _userRating; }
            set { this.SetProperty(ref _userRating, value); }
        }

        #endregion

        #region Images

        private string _imageFeatured;
        public string ImageFeatured
        {
            get { return _imageFeatured; }
            set { this.SetProperty(ref _imageFeatured, value); }
        }

        private string _imageThumbLandscapeLarge;
        public string ImageThumbLandscapeLarge
        {
            get { return _imageThumbLandscapeLarge; }
            set { this.SetProperty(ref _imageThumbLandscapeLarge, value); }
        }

        private string _imageThumbLandscapeSmall;
        public string ImageThumbLandscapeSmall
        {
            get { return _imageThumbLandscapeSmall; }
            set { this.SetProperty(ref _imageThumbLandscapeSmall, value); }
        }

        private string _ImageCustom;
        public string ImageCustom
        {
            get { return _ImageCustom; }
            set { this.SetProperty(ref _ImageCustom, value); }
        }

        #endregion

        #endregion
    }
}
