namespace MediaAppSample.Core.Models
{
    public sealed class ContentItemCollection<T> : ModelList<T> where T : ContentItemBase
    {
        public bool ContainsItem(ContentItemBase item)
        {
            if (item == null)
                throw new System.ArgumentNullException("Item parameter cannot be null.");

            foreach (var qi in this)
                if (qi.ContentID == item.ContentID)
                    return true;

            return false;
        }
    }
}
