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

namespace MediaAppSample.Core.Models
{
    public class QueueModel : ModelBase
    {
        #region Properties

        private ContentItemBase _item;
        public ContentItemBase Item
        {
            get { return _item; }
            set
            {
                if (this.SetProperty(ref _item, value))
                    this.NotifyPropertyChanged(() => this.ImageResume);
            }
        }

        private string _ImageResume;
        public string ImageResume
        {
            get { return _ImageResume ?? this.Item?.ImageFeatured; } // TODO use medium image
            set { this.SetProperty(ref _ImageResume, value); }
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
