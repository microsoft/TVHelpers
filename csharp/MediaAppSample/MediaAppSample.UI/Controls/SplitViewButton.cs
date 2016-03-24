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

namespace MediaAppSample.UI.Controls
{
    /// <summary>
    /// User control used in the ShellView page's SplitView control's pane.  Each instance represents a navigation button for the main navigation menu.
    /// </summary>
    public sealed class SplitViewButton : RadioButton
    {
        public SplitViewButton()
        {
            this.DefaultStyleKey = typeof(SplitViewButton);
        }

        public static readonly DependencyProperty SymbolProperty = DependencyProperty.Register("Symbol", typeof(Symbol), typeof(SplitViewButton), new PropertyMetadata(Symbol.Accept));
        public Symbol Symbol
        {
            get { return (Symbol)GetValue(SymbolProperty); }
            set { SetValue(SymbolProperty, value); }
        }
    }
}
