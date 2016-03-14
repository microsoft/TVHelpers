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
    public class SeasonModel : ItemBase
    {
        #region Constructor

        public SeasonModel()
        {
            this.Episodes = new ContentItemList();
        }

        #endregion

        #region Properties

        public string SquareThumbnailImage { get; set; }

        public string LandscapeThumbnailImage { get; set; }

        public int SeasonNumber { get; set; }

        public ContentItemList Episodes { get; set; }

        #endregion
    }
}
