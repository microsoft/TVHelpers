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

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Controls.Primitives
{
    /// <summary>
    /// Clips a ratio of its content.
    /// </summary>
    public abstract class Clipper : ContentControl
    {
        #region public double RatioVisible
        /// <summary>
        /// Gets or sets the percentage of the item visible.
        /// </summary>
        public double RatioVisible
        {
            get { return (double)GetValue(RatioVisibleProperty); }
            set { SetValue(RatioVisibleProperty, value); }
        }

        /// <summary>
        /// Identifies the RatioVisible dependency property.
        /// </summary>
        public static readonly DependencyProperty RatioVisibleProperty =
            DependencyProperty.Register(
                "RatioVisible",
                typeof(double),
                typeof(Clipper),
                new PropertyMetadata(1.0, OnRatioVisibleChanged));

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="d">PartialView that changed its RatioVisible.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnRatioVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Clipper source = (Clipper)d;
            double oldValue = (double)e.OldValue;
            double newValue = (double)e.NewValue;
            source.OnRatioVisibleChanged(oldValue, newValue);
        }

        /// <summary>
        /// RatioVisibleProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnRatioVisibleChanged(double oldValue, double newValue)
        {
            if (newValue >= 0.0 && newValue <= 1.0)
            {
                ClipContent();
            }
            else
            {
                if (newValue < 0.0)
                {
                    this.RatioVisible = 0.0;
                }
                else if (newValue > 1.0)
                {
                    this.RatioVisible = 1.0;
                }
            }
        }

        #endregion public double RatioVisible

        /// <summary>
        /// Initializes a new instance of the Clipper class.
        /// </summary>
        protected Clipper()
        {
            this.SizeChanged += delegate { ClipContent(); };
        }

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected abstract void ClipContent();
    }
}
