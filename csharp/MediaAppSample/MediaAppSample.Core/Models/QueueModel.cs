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

    public sealed class QueueCollection : ModelList<QueueModel>
    {
        /// <summary>
        /// Checks to see if an item instance is in this collection by the item's ID property value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>True if this instance contains the item ID else false.</returns>
        public bool ContainsItem(ItemBase item)
        {
            if (item == null)
                return false;

            foreach (var qi in this)
                if (qi.Item.ID == item.ID)
                    return true;

            return false;
        }

        public bool RemoveItem(ItemBase item)
        {
            if (item == null)
                return false;

            foreach (var qi in this)
            {
                if (qi.Item.ID == item.ID)
                {
                    this.Remove(qi);
                    return true;
                }
            }
            return false;
        }
    }
}
