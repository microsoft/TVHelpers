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
    public class PersonModel : ModelBase
    {
        #region Properties

        public string ID { get; set; }

        public string FullName { get; set; }

        public string Role { get; set; }

        public string Image { get; set; }

        #endregion
    }
}
