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
using Windows.ApplicationModel;

namespace MediaAppSample.Core.Commands
{
    /// <summary>
    /// Command used in data binding. GenericCommand is wired up with logging and analytics calls.
    /// </summary>
    public class GenericCommand : GenericCommand<object>
    {
        #region Constructors

        /// <summary>
        /// Creates a new command instance.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">The logic to run when execute is called. No parameter will be passed to this execute action.</param>
        /// <param name="canExecute">The function which determines if this command can run or not.</param>
        public GenericCommand(string commandName, Action execute, Func<bool> canExecute = null) 
            : base(commandName, execute, canExecute)
        {
        }

        /// <summary>
        /// Creates a new command instance.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">The logic to run when execute is called. Execute parameter object will be passed to this action.</param>
        /// <param name="canExecute">The function which determines if this command can run or not.</param>
        public GenericCommand(string commandName, Action<object> execute, Func<object, bool> canExecute = null)
            : base(commandName, execute, canExecute)
        {
        }

        #endregion
    }

    /// <summary>
    /// Command used in data binding. GenericCommand is wired up with logging and analytics calls.
    /// </summary>
    /// <typeparam name="T">Type of the command parameter that will be passed in.</typeparam>
    public class GenericCommand<T> : CommandBase
    {
        #region Variables

        private string CommandName { get; set; }
        private Action<T> _execute;
        private Func<T, bool> _canExecute;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command instance.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">The logic to run when execute is called. No parameter will be passed to this execute action.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public GenericCommand(string commandName, Action execute, Func<bool> canExecute = null)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentNullException(nameof(commandName));

            this.CommandName = commandName;

            if(execute != null)
                _execute = (parameter) => execute();

            if(canExecute != null)
                _canExecute = (parameter) => canExecute();
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="commandName">Identifies this command instance, used for logging.</param>
        /// <param name="execute">The logic to run when execute is called. Execute parameter object must be of the generic type specified else it will be ignored.</param>
        /// <param name="canExecute">The function which determines if this command can run or not. CanExecute parameter object must be of the generic type specified else it will be ignored</param>
        public GenericCommand(string commandName, Action<T> execute, Func<T, bool> canExecute = null)
        {
            if (string.IsNullOrWhiteSpace(commandName))
                throw new ArgumentNullException(nameof(commandName));

            this.CommandName = commandName;
            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region Methods

        public override bool CanExecute(object parameter)
        {
            bool value = false;

            if (_canExecute != null && parameter is T)
                value = _canExecute((T)parameter);
            else if (_canExecute != null)
                value = _canExecute(default(T));
            else
                value = true;

            if (!DesignMode.DesignModeEnabled)
            {
                // Log information
                string name = string.Format("[{0} - CanExecute] {1}", this.GetType().Name, this.CommandName);
                Platform.Current.Logger.Log(LogLevels.Debug, "{0} - Return Value: {1}  Parameter: {2}", name, value, parameter);
            }

            return value;
        }

        public override void Execute(object parameter)
        {
            if (!DesignMode.DesignModeEnabled)
            {
                // Log information
                string name = string.Format("[{0} - Execute] {1}", this.GetType().Name, this.CommandName);
                Platform.Current.Analytics.Event(name, parameter);
                Platform.Current.Logger.Log(LogLevels.Information, "{0} - Parameter: {1}", name, parameter);
            }

            if (_execute != null)
            {
                if (parameter is T)
                    _execute((T)parameter);
                else
                    _execute(default(T));
            }
        }

        #endregion
    }
}