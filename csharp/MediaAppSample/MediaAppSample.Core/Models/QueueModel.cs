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
    public class QueueModel : ModelBase
    {
        #region Properties

        private ContentItemBase _item;
        public ContentItemBase Item
        {
            get { return _item; }
            set { this.SetProperty(ref _item, value); }
        }

        #endregion
    }
}
