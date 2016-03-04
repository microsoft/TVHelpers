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
    public class RatingModel : ModelBase
    {
        #region Properties

        public string ID { get; set; }

        public string RatingSource { get; set; }

        public string RatingDetails { get; set; }

        public double RatingScore { get; set; }

        public double RatingScale { get; set; }

        public double RatingPercent
        {
            get
            {
                return (this.RatingScore / this.RatingScale) * 100;
            }
        }

        #endregion
    }
}
