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

namespace MediaAppSample.UI.Controls
{
    /// <summary>
    /// This type is used to determine the state of the item selected and the
    /// previous items.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public enum RatingSelectionMode
    {
        /// <summary>
        /// All items before the selected ones are selected.
        /// </summary>
        Continuous,

        /// <summary>
        /// Only the item selected is visually distinguished.
        /// </summary>
        Individual
    }
}