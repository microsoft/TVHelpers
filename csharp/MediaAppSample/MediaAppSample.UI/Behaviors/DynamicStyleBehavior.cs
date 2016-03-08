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

using MediaAppSample.Core;
using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaAppSample.UI.Behaviors
{
    public static class DynamicStyleBehavior
    {

        public static readonly DependencyProperty TemplateNameProp =
            DependencyProperty.RegisterAttached("TemplateName", typeof(string),
            typeof(DynamicStyleBehavior), new PropertyMetadata(null, OnTemplateNamePropertyChanged));

        // Private XAML properties (used only as attached storage)
        public static readonly DependencyProperty TemplateDataProp =
            DependencyProperty.RegisterAttached("$TemplateData", typeof(object),
            typeof(DynamicStyleBehavior), null);

        private static TemplateData GetTemplateDataForElement(DependencyObject d)
        {
            var data = (TemplateData)d.GetValue(TemplateDataProp) ?? new TemplateData
            {
                LastWidth = 0.0,
                LastTemplate = ""
            };
            return data;
        }

        public static void SetTemplateName(DependencyObject d, string value)
        {
            d.SetValue(TemplateNameProp, value);
        }

        public static string GetTemplateName(DependencyObject d)
        {
            return (string)d.GetValue(TemplateNameProp);
        }

        private static void OnTemplateNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ListViewBase)d;
            if (control == null) return;
            Platform.Current.ViewModel.PropertyChanged += (s2, e2) =>
            {
                if (e2.PropertyName == "DeviceWindowWidth")
                {
                    UpdateTileTemplate(d, e, control, 200);
                }
            };
            UpdateTileTemplate(d, e, control, 0); // run the first time.
        }

        private static void UpdateTileTemplate(DependencyObject d, DependencyPropertyChangedEventArgs e, ListViewBase control, double minCHeckWidth)
        {
            var width = Platform.Current.ViewModel.DeviceWindowWidth;
            var data = GetTemplateDataForElement(d as DependencyObject);
            if (!(Math.Abs(width - data.LastWidth) >= minCHeckWidth)) return;

            data.LastWidth = width;
            var templateName = e.NewValue as string;
            switch (Platform.DeviceFamily)
            {
                case DeviceFamily.Desktop:
                    if (width > 1800)
                    {
                        GetTemplate(control, templateName, d, data);
                    }
                    else if (width > 1080)
                    {
                        GetTemplate(control, templateName + "23", d, data);
                    }
                    else
                    {
                        GetTemplate(control, templateName + "13", d, data);
                    }
                    break;
                default:
                    GetTemplate(control, templateName, d, data);
                    break;
            }
        }

        private static void GetTemplate(ItemsControl element, string resourceName, DependencyObject d, TemplateData data)
        {
            // only set if Template changed
            if (resourceName == data.LastTemplate) return;
            //Debug.WriteLine("resourceName:'" + resourceName + "' data.LastTemplate:'" + data.LastTemplate + "'");
            data.LastTemplate = resourceName;
            d.SetValue(TemplateDataProp, data);

            var dt = Application.Current.Resources[resourceName] as DataTemplate;
            if (dt != null)
            {
                element.ItemTemplate = dt;
            }
        }

        private class TemplateData
        {
            public double LastWidth { get; set; }
            public string LastTemplate { get; set; }
        }

    }


}
