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

using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Interface that all model objects should implement.
    /// </summary>
    public interface IModel
    {
    }

    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify models.
    /// </summary>
    public partial class ModelBase : INotifyPropertyChanged, IModel
    {
        #region Events

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                this.NotifyPropertyChanged(propertyName);
                return true;
            }
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Optional name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <typeparam name="T">Type of the property in the expression.</typeparam>
        /// <param name="property">Expression to retrieve the property. Example: () => this.FirstName</param>
        protected void NotifyPropertyChanged<T>(Expression<Func<T>> property)
        {
            var propertyName = this.GetPropertyName<T>(property);
            this.NotifyPropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the string name of a property expression.
        /// </summary>
        /// <typeparam name="T">Type of the property in the expression.</typeparam>
        /// <param name="property">Expression to retrieve the property. Example: () => this.FirstName</param>
        /// <returns>String value representing the property name.</returns>
        private string GetPropertyName<T>(Expression<Func<T>> property)
        {
            MemberExpression memberExpression = GetMememberExpression<T>(property);
            return memberExpression.Member.Name;
        }

        /// <summary>
        /// Gets the MemberExpression from a property expression.
        /// </summary>
        /// <typeparam name="T">Type of the property in the expression.</typeparam>
        /// <param name="property">Expression to retrieve the property. Example: () => this.FirstName</param>
        /// <returns>MemberExpression instance presenting the property expression.</returns>
        private MemberExpression GetMememberExpression<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
                memberExpression = (MemberExpression)lambda.Body;

            return memberExpression;
        }

        /// <summary>
        /// Retrieves a PropertyInfo object representing the property in the specified expression.
        /// </summary>
        /// <typeparam name="T">Type of the property in the expression.</typeparam>
        /// <param name="property">Expression to retrieve the property. Example: () => this.FirstName</param>
        /// <returns>PropertyInfo object of the expression property else null if not found.</returns>
        protected internal PropertyInfo GetPropertyInfo<T>(Expression<Func<T>> property)
        {
            if (property != null && property.Body is MemberExpression)
            {
                var mex = (MemberExpression)property.Body;
                if (mex != null && mex.Member is PropertyInfo)
                    return mex.Member as PropertyInfo;
            }
            return null;
        }

        /// <summary>
        /// Sets a PropertyInfo object with passed in value.
        /// </summary>
        /// <param name="pi">PropertyInfo instance representing the property that needs to be set.</param>
        /// <param name="value">Value to set to the specified PropertyInfo instance.</param>
        protected internal void SetPropertyValue(PropertyInfo pi, object value)
        {
            if (pi != null && pi.CanWrite)
                pi.SetValue(this, value, null);
        }

        #endregion
    }
}