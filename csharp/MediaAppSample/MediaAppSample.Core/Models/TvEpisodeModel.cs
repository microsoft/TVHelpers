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
    public class TvEpisodeModel : ContentItemBase
    {
        #region Properties

        public int SeasonNumber { get; set; }
           
        public int EpisodeNumber { get; set; }

        public int Season { get; set; }

        public DateTime AirDate { get; set; }

        public override int Year
        {
            get { return this.AirDate.Year; }
        }

        #endregion
    }
}
