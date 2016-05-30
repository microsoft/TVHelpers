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

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Command for navigating to a page.
    /// </summary>
    public sealed class NavigationCommand : GenericCommand<object>
    {
        #region Constructors

        /// <summary>
        /// Command for navigating to a page based on the IModel parameter passed into the Execute method.
        /// </summary>
        public NavigationCommand()
            : base("NavigateToModelCommand", Platform.Current.Navigation.NavigateTo, null)
        {
        }

        /// <summary>
        /// Command for navigation to a page.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">Logic to execute, Execute command object will be ignored.</param>
        /// <param name="canExecute">Logic to determine if this command can execute, CanExecute parameter will be ignored.</param>
        public NavigationCommand(string commandName, Action execute, Func<bool> canExecute = null)
            : base(commandName, execute, canExecute)
        {
        }

        /// <summary>
        /// Command for navigation to a page.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">Logic to execute, Execute command object must be an IModel type.</param>
        /// <param name="canExecute">Logic to determine if this command can execute, CanExecute parameter must be an IModel type.</param>
        public NavigationCommand(string commandName, Action<object> execute, Func<object, bool> canExecute = null)
            : base(commandName, execute, canExecute)
        {
        }

        #endregion
    }
}