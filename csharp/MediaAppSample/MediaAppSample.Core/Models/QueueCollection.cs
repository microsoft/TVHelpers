namespace MediaAppSample.Core.Models
{
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
                throw new System.ArgumentNullException("Item parameter cannot be null.");

            foreach (var qi in this)
                if (qi.Item.ContentID == item.ContentID)
                    return true;

            return false;
        }

        public bool RemoveItem(ItemBase item)
        {
            if (item == null)
                throw new System.ArgumentNullException("Item parameter cannot be null.");

            foreach (var qi in this)
            {
                if (qi.Item.ContentID == item.ContentID)
                {
                    this.Remove(qi);
                    return true;
                }
            }
            return false;
        }
    }
}
