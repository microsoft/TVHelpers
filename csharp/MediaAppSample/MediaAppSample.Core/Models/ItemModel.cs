using System;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Sample model class.
    /// </summary>
    public sealed class ItemModel : LocationModel
    {
        #region Properties

        private int _id;
        public int ID
        {
            get { return _id; }
            set { this.SetProperty(ref _id, value); }
        }

        private string _lineOne;
        public string LineOne
        {
            get { return _lineOne; }
            set { this.SetProperty(ref _lineOne, value); }
        }

        private string _lineTwo;
        public string LineTwo
        {
            get { return _lineTwo; }
            set { this.SetProperty(ref _lineTwo, value); }
        }

        private string _lineThree;
        public string LineThree
        {
            get { return _lineThree; }
            set { this.SetProperty(ref _lineThree, value); }
        }

        private string _LineFour;
        public string LineFour
        {
            get { return _LineFour; }
            set { this.SetProperty(ref _LineFour, value); }
        }

        #endregion
    }
}