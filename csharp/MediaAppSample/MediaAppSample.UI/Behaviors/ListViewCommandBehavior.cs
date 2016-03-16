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

using MediaAppSample.Core.Models;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Behaviors
{
    /// <summary>
    /// Creates an attached property for all ListViewBase controls allowing binding  a command object to it's ItemClick event.
    /// </summary>
    public static class ListViewBaseCommandBehavior
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(ListViewBaseCommandBehavior), new PropertyMetadata(null, OnCommandPropertyChanged));

        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        private static void OnCommandPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (ListViewBase)d;
            if (control != null)
            {
                // Remove the old click handler if there was a previous command
                if (e.OldValue != null)
                {
                    control.ItemClick -= OnItemClick;
                }

                control.ItemClick += OnItemClick;
            }
        }

        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = (ListViewBase)sender;
            if (control != null)
            {
                var command = GetCommand(control);

                object parameter = e.ClickedItem is ItemBase ? (e.ClickedItem as ItemBase).ID : e.ClickedItem;

                if (command != null && command.CanExecute(parameter))
                    command.Execute(parameter);
            }
        }
    }
}