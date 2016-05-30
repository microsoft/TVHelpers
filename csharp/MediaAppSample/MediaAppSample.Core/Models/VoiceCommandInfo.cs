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
using System.Linq;
using Windows.Media.SpeechRecognition;

namespace MediaAppSample.Core.Models
{
    /// <summary>
    /// Wrapper for SpeechRecognitionResult class instance improving access to the key data needed to process voice.
    /// </summary>
    public sealed class VoiceCommandInfo
    {
        public VoiceCommandInfo(SpeechRecognitionResult speechRecognitionResult)
        {
            this.Result = speechRecognitionResult;
            this.VoiceCommandName = speechRecognitionResult?.RulePath[0];
            this.TextSpoken = speechRecognitionResult?.Text;
        }

        /// <summary>
        /// Access the wrapped SpeechRecognitionResult object instance.
        /// </summary>
        public SpeechRecognitionResult Result { get; private set; }

        /// <summary>
        /// Gets the voice command name that was activated.
        /// </summary>
        public string VoiceCommandName { get; private set; }

        /// <summary>
        /// Gets the text that was spoken by the user.
        /// </summary>
        public string TextSpoken { get; private set; }

        /// <summary>
        /// Returns the semantic interpretation of a speech result. Returns null if there is no interpretation for
        /// that key.
        /// </summary>
        /// <param name="interpretationKey">The interpretation key.</param>
        /// <returns>Gets the item in the phrase list that was spoken.</returns>
        public string GetSemanticInterpretation(string interpretationKey)
        {
            if (string.IsNullOrEmpty(interpretationKey))
                throw new ArgumentNullException(nameof(interpretationKey));

            if (this.Result.SemanticInterpretation.Properties.ContainsKey(interpretationKey))
                return this.Result.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
            else
                throw new ArgumentException(string.Format("intrepretationKey of '{0}' not found!", interpretationKey));
        }
    }
}
