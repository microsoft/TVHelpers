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
