//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

namespace MediaAppSample.Core.Models
{
    public enum ItemTypes
    {
        Unknown,
        Movie,
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
        public virtual string ContentID
        {
            get { return _ID; }
            set { this.SetProperty(ref _ID, value); }
        }

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

        private double _averageUserRating;
        public double AverageUserRating
        {
            get { return _averageUserRating; }
            set { this.SetProperty(ref _averageUserRating, value); }
        }

        private string _genre;
        public string Genre
        {
            get { return _genre; }
            set { this.SetProperty(ref _genre, value); }
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

        private string _featuredImage;
        public string FeaturedImage
        {
            get { return _featuredImage; }
            set { this.SetProperty(ref _featuredImage, value); }
        }

        private string _mediaImage;
        public string MediaImage
        {
            get { return _mediaImage; }
            set { this.SetProperty(ref _mediaImage, value); }
        }

        private string _landscapeImage;
        public string LandscapeImage
        {
            get { return _landscapeImage; }
            set { this.SetProperty(ref _landscapeImage, value); }
        }

        private string _flag;
        public string Flag
        {
            get { return _flag; }
            set { this.SetProperty(ref _flag, value); }
        }

        public string ByLine => "'Lorem Ipsum dolor sit amet, consectetur adipiscing elit. Vesitibulum fringilla felis diam, elementum.' - Convallis";

        #endregion
    }
}
