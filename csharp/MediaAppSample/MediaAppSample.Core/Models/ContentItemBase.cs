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

using System.Windows.Input;

namespace MediaAppSample.Core.Models
{
    public abstract class ContentItemBase : ItemBase
    {
        #region Constructor

        public ContentItemBase()
        {
            this.Length = string.Empty;
        }

        #endregion

        #region Properties

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.SetProperty(ref _Title, value); }
        }

        private string _TvEpisodeImage;
        public string TvEpisodeImage
        {
            get { return _TvEpisodeImage; }
            set { this.SetProperty(ref _TvEpisodeImage, value); }
        }

        private string _ResumeImage;
        public string ResumeImage
        {
            get { return _ResumeImage; }
            set { this.SetProperty(ref _ResumeImage, value); }
        }

        private string _InlineImage;
        public string InlineImage
        {
            get { return _InlineImage; }
            set { this.SetProperty(ref _InlineImage, value); }
        }

        private string _URL;
        public string URL
        {
            get { return _URL; }
            set { this.SetProperty(ref _URL, value); }
        }

        private string _Length;
        public string Length
        {
            get { return _Length; }
            set { this.SetProperty(ref _Length, value); }
        }

        private ModelList<PersonModel> _Creators;
        public ModelList<PersonModel> Creators
        {
            get { return _Creators; }
            internal set { this.SetProperty(ref _Creators, value); }
        }

        private ModelList<PersonModel> _Cast;
        public ModelList<PersonModel> Cast
        {
            get { return _Cast; }
            internal set { this.SetProperty(ref _Cast, value); }
        }

        private ModelList<ReviewModel> _CriticReviews;
        public ModelList<ReviewModel> CriticReviews
        {
            get { return _CriticReviews; }
            set { this.SetProperty(ref _CriticReviews, value); }
        }

        private ModelList<RatingModel> _ContentRatings;
        public ModelList<RatingModel> ContentRatings
        {
            get { return _ContentRatings; }
            set { this.SetProperty(ref _ContentRatings, value); }
        }

        public abstract int Year { get; }

        public ICommand Command { get; set; }

        #endregion

        public override string ToString()
        {
            return "[ContentItemBase]:Title = " + this.Title + ", ContentId: " + this.ContentID;
        }
    }
}
