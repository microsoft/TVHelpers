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

using System;

namespace MediaAppSample.Core.Models
{
    public class MovieModel : ContentItemBase
    {
        #region Properties

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

        private string _MediaType;
        public string MediaType
        {
            get { return _MediaType; }
            set { this.SetProperty(ref _MediaType, value); }
        }

        public string AcquisitionType { get; set; }
        
        public string AcquisitionContext { get; set; }
        
        public string AcquisitionContextType { get; set; }

        public string AcquisitionContextID { get; set; }

        public string MediaSubscriptionType { get; set; }

        public string SubscriptionTier { get; set; }

        public override int Year
        {
            get { return this.ReleaseDate.Year; }
        }

        #endregion
    }
}
