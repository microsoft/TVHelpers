using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Collection for holding model instances.
    /// </summary>
    /// <typeparam name="TItem">Type representing the items in this collection.</typeparam>
    public class ModelList<TItem> : ObservableCollection<TItem>, IModel, INotifyPropertyChanged where TItem : IModel
    {
        #region Events

        /// <summary>
        /// Multicast event for property change notifications.
        /// </summary>
        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Constructors

        public ModelList()
        {
        }

        public ModelList(IEnumerable<TItem> items)
        {
            this.AddRange(items);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds multiple items to this collection.
        /// </summary>
        /// <param name="items"></param>
        public void AddRange(IEnumerable<TItem> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                    this.Add(item);
            }
        }


        /// <summary>
        /// Sorts the list by a property found on each item.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns>Awaitable task is returned.</returns>
        public async Task SortAsync(string propertyName)
        {
            // Move all items into a temporary list
            var temp = new ModelList<TItem>();
            foreach (var item in this)
                temp.Add(item);

            // Perform sorting work in a separate thread
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    // Loop through all items in the temp list
                    for (int i = temp.Count - 1; i >= 0; i--)
                    {
                        for (int j = 1; j <= i; j++)
                        {
                            TItem obj1 = temp[j - 1];
                            TItem obj2 = temp[j];
                            
                            // Compare values
                            object obj1value = obj1.GetType().GetTypeInfo().GetDeclaredProperty(propertyName)?.GetValue(obj1);
                            object obj2value = obj2.GetType().GetTypeInfo().GetDeclaredProperty(propertyName)?.GetValue(obj2);

                            if (obj1value == null)
                                throw new ArgumentException(string.Format("Property '{0} was not found in the collection object!", propertyName));

                            IComparable compare1 = (IComparable)obj1value;
                            if (compare1 != null)
                            {
                                bool sort = compare1.CompareTo(obj2value) > 0;

                                if (sort)
                                {
                                    // Reposition item within the list
                                    temp.Remove(obj1);
                                    temp.Insert(j, obj1);
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format("Could not sort on property name '{0}'!", propertyName), ex);
                }
            });

            // Clear this current list so we aren't adding the same item back into this list
            this.Clear();

            // Move items from the temp list back into this list
            foreach (var item in temp)
                this.Add(item);

            // Clear the temp list
            temp.Clear();
        }

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
            if (object.Equals(storage, value))
                return false;

            storage = value;
            this.NotifyPropertyChanged(propertyName);
            return true;
        }

        internal void Add(ContentItemBase contentItemBase)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
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
        protected internal string GetPropertyName<T>(Expression<Func<T>> property)
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

        #endregion
    }
}