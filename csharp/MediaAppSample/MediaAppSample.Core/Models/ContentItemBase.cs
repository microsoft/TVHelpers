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

using System;
using System.Collections.Generic;

namespace MediaAppSample.Core.Models
{
    public abstract class ContentItemBase : ItemBase
    {
        #region Constructor

        public ContentItemBase()
        {
        }

        #endregion

        #region Properties

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.SetProperty(ref _Title, value); }
        }

        private string _Subtitle;
        public virtual string Subtitle
        {
            get { return _Subtitle; }
            private set { this.SetProperty(ref _Subtitle, value); }
        }

        private Uri _MediaUrl;
        public Uri MediaUrl
        {
            get { return _MediaUrl; }
            set { this.SetProperty(ref _MediaUrl, value); }
        }

        private DateTime _ReleaseDate;
        public DateTime ReleaseDate
        {
            get { return _ReleaseDate; }
            set
            {
                if (this.SetProperty(ref _ReleaseDate, value))
                    this.NotifyPropertyChanged(() => this.Year);
            }
        }

        private int _Length;
        public int Length
        {
            get { return _Length; }
            set
            {
                if (this.SetProperty(ref _Length, value))
                    this.NotifyPropertyChanged(() => this.DisplayLength);
            }
        }

        public string DisplayLength
        {
            get
            {
                var hourString = System.Math.Floor((double)this.Length / 60) <= 1 ? Strings.Resources.TextHour : Strings.Resources.TextHours;
                var minString = this.Length % 60 == 1 ? Strings.Resources.TextMinute : Strings.Resources.TextMinutes;

                if (Length < 60)
                    return string.Format("{0} {1}", this.Length, minString);
                else if (this.Length == 60)
                    return "1 " + hourString;
                else if (this.Length < 120)
                    return string.Format("1 {0} {1} {2}", hourString, this.Length % 60, minString);
                else if(this.Length % 60 == 0)
                    return string.Format("{0} {1}", System.Math.Floor((double)this.Length / 60), hourString);
                else
                    return string.Format("{0} {1} {2} {3}", System.Math.Floor((double)this.Length / 60), hourString, this.Length % 60, minString);
            }
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

        #endregion
    }

    public sealed class ContentItemList : ModelList<ContentItemBase>
    {
        #region Constructors

        public ContentItemList()
        {
        }

        public ContentItemList(IEnumerable<ContentItemBase> items)
        {
            this.AddRange(items);
        }

        #endregion

        public bool ContainsItem(ContentItemBase item)
        {
            if (item == null)
                throw new System.ArgumentNullException("Item parameter cannot be null.");

            foreach (var qi in this)
                if (qi.ID == item.ID)
                    return true;

            return false;
        }
    }
}
