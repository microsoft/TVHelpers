// #      The MIT License (MIT)
// #
// #      Copyright (c) 2016 Microsoft. All rights reserved.
// #
// #      Permission is hereby granted, free of charge, to any person obtaining a copy
// #      of this software and associated documentation files (the "Software"), to deal
// #      in the Software without restriction, including without limitation the rights
// #      to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// #      copies of the Software, and to permit persons to whom the Software is
// #      furnished to do so, subject to the following conditions:
// #
// #      The above copyright notice and this permission notice shall be included in
// #      all copies or substantial portions of the Software.
// #
// #      THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// #      IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// #      FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// #      AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// #      LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// #      OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// #      THE SOFTWARE.
(function searchboxInit() {
    "use strict";

    var _KEY_BACKSPACE = 8;
    var _KEY_ENTER = 13;
    var _KEY_ESCAPE = 27;
    var _KEY_SPACE = 32;

    // Enums
    var ClassName = {
        searchBox: "tv-searchbox",
        searchBoxInput: "tv-searchbox-input",
        searchBoxButton: "tv-searchbox-button",
        searchBoxFlyout: "tv-searchbox-flyout",
        searchBoxSuggestionResult: "tv-searchbox-suggestion-result",
        searchBoxSuggestionQuery: "tv-searchbox-suggestion-query",
        searchBoxSuggestionSeparator: "tv-searchbox-suggestion-separator",
        searchBoxSuggestionSelected: "tv-searchbox-suggestion-selected",
        searchBoxFlyoutHighlightText: "tv-searchbox-flyout-highlighttext",
        searchBoxButtonInputFocus: "tv-searchbox-button-input-focus",
        searchBoxInputFocus: "tv-searchbox-input-focus",
        searchBoxSuggestionResultText: "tv-searchbox-suggestion-result-text",
        searchboxDisabled: "tv-searchbox-disabled",
        searchBoxToggle: "tv-searchbox-toggleletters"
    };

    var EventName = {
        querychanged: "querychanged",
        querysubmitted: "querysubmitted",
        resultsuggestionchosen: "resultsuggestionchosen",
        suggestionsrequested: "suggestionsrequested",
        receivingfocusonkeyboardinput: "receivingfocusonkeyboardinput"
    };

    var SearchSuggestionKind = {
        Query: 0,
        Result: 1,
        Separator: 2
    };

    function _setOptions(control, options) {
        if (typeof options === "object") {
            var keys = Object.keys(options);
            for (var i = 0, len = keys.length; i < len; i++) {
                var key = keys[i];
                var value = options[key];
                if (key.length > 2) {
                    var ch1 = key[0];
                    var ch2 = key[1];
                    if ((ch1 === 'o' || ch1 === 'O') && (ch2 === 'n' || ch2 === 'N')) {
                        if (typeof value === "function") {
                            if (control.addEventListener) {
                                control.addEventListener(key.substr(2), value);
                                continue;
                            }
                        }
                    }
                }
                control[key] = value;
            }
        }
    };

    // TODO: Figure out localization, hardcoded strings for now.
    var strings = {
        letters: "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
        secondaryLetters: "String_Empty",
        numbers: "1234567890$.,:;?!#@%-&",
        toggleNumbersAndLettersLabelNumbers: "&12",
        toggleNumbersAndLettersLabelLetters: "abc",
        toggleNumbersAndLettersLabelSecondaryLetters: "String_Empty"
    };

    var _SearchBox = (function () {
        function _SearchBox(element, options) {

            element = element || document.createElement("div");

            if (element.winControl) {
                throw new Error("Invalid argument: Controls may only be instantiated one time for each DOM element");
            }
            element.winControl = this;

            // Elements
            this._element = null;
            this._inputElement = null;
            this._isShowingLettersNotSymbols = true;
            // This variable tracks which set of characters we are currently showing the user. For instance,
            // the control may be showing letters or numbers. In some cases, like Russian there are three options:
            // (1) Cyrillic characters, (2) Latin characters and (3) numbers.
            this._characterSetDisplayed = "primaryCharacterSet";
            this._firstCharacter = "";
            this._lastCharacter = "";
            this._secondaryAlphabetFirstCharacter = "";
            this._secondaryAlphabetLastCharacter = "";
            this._firstNumberIndex = -1;
            this._lastNumberIndex = -1;
            this._toggleLettersOrSymbolsElement = null;

            // Variables
            this._disposed = false;

            // Set options
            _setOptions(this, options);

            if (!this.strings) {
                this.strings = strings;
            }

            // We perform the following check to see if we are running in a language that requires
            // three character sets. Most languages require only two: (1) alphabet and (2) numbers
            // and symbols. But languages like Russian require 3: (1) Cyrillic alphabet, (2) Latin
            // alphabet and (3) numbers and symbols.
            this.usesThreeCharacterSets = false;

            this._setElementBind = this._setElement.bind(this);
            this._setElementBind(element);
            element.classList.add("tv-disposable");

            this._keyDownHandlerBind = this._keyDownHandler.bind(this);
            this._element.addEventListener("keydown", this._keyDownHandlerBind, false);
            this._keyUpHandlerBind = this._keyUpHandler.bind(this);
            this._element.addEventListener("keyup", this._keyUpHandlerBind, false);
            this._keyPressHandlerBind = this._keyPressHandler.bind(this);
            this._element.addEventListener("keypress", this._keyPressHandlerBind, false);
        };
        // Properties
        /// <field type="HTMLElement" domElement="true" hidden="true" locid="TVJS.SearchBox.element" helpKeyword="TVJS.SearchBox.element">
        /// The DOM element that hosts the SearchBox.
        /// </field>
        Object.defineProperty(_SearchBox.prototype, "element", {
            get: function () {
                return this._element;
            },
            enumerable: true,
            configurable: true
        });
        /// <field type="String" locid="TVJS.SearchBox.placeholderText" helpKeyword="TVJS.SearchBox.placeholderText">
        /// Gets or sets the placeholder text for the SearchBox. This text is displayed if there is no
        /// text in the input box.
        /// </field>
        Object.defineProperty(_SearchBox.prototype, "placeholderText", {
            get: function () {
                return this._inputElement.placeholder;
            },
            set: function (value) {
                this._inputElement.placeholder = value;
            },
            enumerable: true,
            configurable: true
        });
        /// <field type="String" locid="TVJS.SearchBox.queryText" helpKeyword="TVJS.SearchBox.queryText">
        /// Gets or sets the query text for the SearchBox.
        /// </field>
        Object.defineProperty(_SearchBox.prototype, "queryText", {
            get: function () {
                return this._inputElement.value;
            },
            set: function (value) {
                this._inputElement.value = value;
            },
            enumerable: true,
            configurable: true
        });
        _SearchBox.prototype.dispose = function () {
            /// <signature helpKeyword="XboxJS.UI.SearchBox.dispose">
            /// <summary locid="XboxJS.UI.SearchBox.dispose">
            /// Disposes the control.
            /// </summary>
            /// </signature>

            if (this._disposed) {
                return;
            }
            this._disposed = true;

            this._element.removeEventListener("keydown", this._keyDownHandlerBind);
            this._keyDownHandlerBind = null;
            this._element.removeEventListener("keyup", this._keyUpHandlerBind);
            this._keyUpHandlerBind = null;
            this._element.removeEventListener("keypress", this._keyPressHandlerBind);
            this._keyPressHandlerBind = null;

            this._setElementBind = null;
            this._inputElement = null;
            this._isShowingLettersNotSymbols = null;
            this._characterSetDisplayed = null;
            this._spaceElement = null;
            this._toggleLettersOrSymbolsElement = null;
            this.usesThreeCharacterSets = null;

            this._firstCharacter = null;
            this._lastCharacter = null;
            this._secondaryAlphabetFirstCharacter = null;
            this._secondaryAlphabetLastCharacter = null;
            this._firstNumberIndex = null;
            this._lastNumberIndex = null;

            this._element.winControl = null;
            this._element = null;
        };
        _SearchBox.prototype._setElement = function (element) {
            this._element = element;
            this._element.classList.add(ClassName.searchBox);

            var characterArray = this.strings.letters;
            var characterArrayLength = characterArray.length;
            this._firstCharacter = characterArray[0];
            this._lastCharacter = characterArray[characterArrayLength - 1];

            var initialToggleLettersAndNumbersString = this.strings.toggleNumbersAndLettersLabelNumbers;
            if (this.usesThreeCharacterSets) {
                var initialToggleLettersAndNumbersString = this.strings.toggleNumbersAndLettersLabelSecondaryLetters;
            }
            var html = '<input class="' + ClassName.searchBoxInput + '" type="text" placeHolder="' + "search..." + '" />' +
                       '<div class="tv-searchbox-letterscontainer">' +
                       '    <div id="togglecharacters" class="tv-searchbox-character ' + ClassName.searchBoxToggle + '" data-tv-focus-right="#__ms_searchboxletter_' + this._firstCharacter + '" tabindex="0">' + initialToggleLettersAndNumbersString + '</div>' +
                       '    <div class="tv-searchbox-alphabetical">';

            for (var i = 0; i < characterArrayLength; i++) {
                var focusLeftSelector = "";
                var focusRightSelector = "";
                if (i === 0) {
                    focusLeftSelector = "#togglecharacters";
                    focusRightSelector = "#__ms_searchboxletter_" + characterArray[i + 1];
                } else if (i === characterArray.length - 1) {
                    focusLeftSelector = "#__ms_searchboxletter_" + characterArray[i - 1];
                    focusRightSelector = "#space";
                } else {
                    focusLeftSelector = "#__ms_searchboxletter_" + characterArray[i - 1];
                    focusRightSelector = "#__ms_searchboxletter_" + characterArray[i + 1];
                }
                html += '<div id="__ms_searchboxletter_' + characterArray[i] + '" class="tv-searchbox-character" tabindex="0" data-tv-focus-left="' + focusLeftSelector + '" data-tv-focus-right="' + focusRightSelector + '">' + characterArray[i] + '</div>';
            }

            // Create an additional set of letters if we are in a language that requires three character sets.
            if (this.usesThreeCharacterSets) {
                var secondaryAlphabetArray = this.strings.secondaryLetters;
                var secondaryAlphabetArrayLength = secondaryAlphabetArray.length;
                this._secondaryAlphabetFirstCharacter = secondaryAlphabetArray[0];
                this._secondaryAlphabetLastCharacter = secondaryAlphabetArray[secondaryAlphabetArrayLength - 1];
                html += '</div><div class="tv-searchbox-secondaryalphabet" style="display: none;">';
                for (var i = 0; i < secondaryAlphabetArrayLength; i++) {
                    var focusLeftSelector = "";
                    var focusRightSelector = "";
                    if (i === this._firstNumberIndex) {
                        focusLeftSelector = "#togglecharacters";
                        focusRightSelector = "#__ms_searchboxletter_" + secondaryAlphabetArray[i + 1];
                    } else if (i === this._lastNumberIndex) {
                        focusLeftSelector = "#__ms_searchboxletter_" + secondaryAlphabetArray[i - 1];
                        focusRightSelector = "#space";
                    } else {
                        focusLeftSelector = "#__ms_searchboxletter_" + secondaryAlphabetArray[i - 1];
                        focusRightSelector = "#__ms_searchboxletter_" + secondaryAlphabetArray[i + 1];
                    }
                    html += '<div id="__ms_searchboxletter_' + secondaryAlphabetArray[i] + '" class="tv-searchbox-character" tabindex="0" data-tv-focus-left="' + focusLeftSelector + '" data-tv-focus-right="' + focusRightSelector + '">' + secondaryAlphabetArray[i] + '</div>';
                }
            }

            var numbersAndSymbolsArray = this.strings.numbers;
            var numbersAndSymbolsArrayLength = numbersAndSymbolsArray.length;
            this._firstNumberIndex = 0;
            this._lastNumberIndex = numbersAndSymbolsArrayLength - 1;
            html += '</div><div class="tv-searchbox-numericandsymbols" style="display:none">';
            for (var i = 0; i < numbersAndSymbolsArrayLength; i++) {
                var focusLeftSelector = "";
                var focusRightSelector = "";
                if (i === this._firstNumberIndex) {
                    focusLeftSelector = "#togglecharacters";
                    focusRightSelector = "#__ms_searchboxsymbol_" + (i + 1);
                } else if (i === this._lastNumberIndex) {
                    focusLeftSelector = "#__ms_searchboxsymbol_" + (i - 1);
                    focusRightSelector = "#space";
                } else {
                    focusLeftSelector = "#__ms_searchboxsymbol_" + (i - 1);
                    focusRightSelector = "#__ms_searchboxsymbol_" + (i + 1);
                }
                html += '<div id="__ms_searchboxsymbol_' + i + '" class="tv-searchbox-character" tabindex="0" data-tv-focus-left="' + focusLeftSelector + '" data-tv-focus-right="' + focusRightSelector + '">' + numbersAndSymbolsArray[i] + '</div>';
            }

            html += '    </div>' +
                    '    <div class="tv-searchbox-secondaryletters">' +
                    '        <div id="space" class="tv-searchbox-character tv-spaceicon" tabindex="0" data-tv-focus-left="#__ms_searchboxletter_' + this._lastCharacter + '" data-tv-focus-right="#backspace"></div>' +
                    '        <div id="backspace" class="tv-searchbox-character tv-backspaceicon" tabindex="0" data-tv-focus-left="#space"></div>' +
                    '    </div>' +
                    '</div>';
            this._element.innerHTML = html;

            this._inputElement = this._element.querySelector("." + ClassName.searchBoxInput);
            this._spaceElement = this._element.querySelector("#space");
            this._toggleLettersOrSymbolsElement = this._element.querySelector("." + ClassName.searchBoxToggle);

            // We do not want the input textbox to be non-focusable
            this._inputElement.tabIndex = -1;
            this._aphabetCharactersElement = this._element.querySelector(".tv-searchbox-alphabetical");
            this._secondaryAphabetCharactersElement = this._element.querySelector(".tv-searchbox-secondaryalphabet");
            this._numbersAndSymbolsElement = this._element.querySelector(".tv-searchbox-numericandsymbols");
        };
        _SearchBox.prototype._fireEvent = function (type, detail) {
            // Returns true if ev.preventDefault() was not called
            var event = document.createEvent("CustomEvent");
            event.initCustomEvent(type, true, true, detail);
            return this._element.dispatchEvent(event);
        };
        _SearchBox.prototype._keyDownHandler = function (ev) {
            // Update the textbox
            var shouldFireQueryChanged = false;
            if (ev.key === "GamepadA" ||
                ev.key === "Enter") {
                var upperCaseKey = ev.target.id;
                // Convert the key to lowercase
                var key = upperCaseKey.toLowerCase();
                // Because the id of the letters and symbols start with "__ms_searchboxletter_*",
                // we are only interested in the last letter to use for comparison which will be
                // the letter. The reason the ids aren't just the letter and are longer strings is
                // because we want to avoid name collisions with other ids in the document.
                var adjustedKey = "";
                if (this._isShowingLettersNotSymbols) {
                    adjustedKey = key[key.length - 1];
                } else {
                    var adjustedKeyIndex = key.split("__ms_searchboxsymbol_")[1];
                    adjustedKey = this.strings.numbers[adjustedKeyIndex];
                }
                if (key === 'space') {
                    this._inputElement.value += ' ';
                    shouldFireQueryChanged = true;
                    this._playSelectionAnimation(ev.target);
                } else if (key === 'backspace') {
                    this._inputElement.value = this._inputElement.value.substring(0, this._inputElement.value.length - 1);
                    shouldFireQueryChanged = true;
                    this._playSelectionAnimation(ev.target);
                } else if (key === 'togglecharacters') {
                    var firstCharacter = "";
                    var lastCharacter = "";
                    if (this._characterSetDisplayed === "primaryCharacterSet" &&
                        this.usesThreeCharacterSets) {
                        this._aphabetCharactersElement.style.display = "none";
                        if (this._secondaryAphabetCharactersElement) {
                            this._secondaryAphabetCharactersElement.style.display = "inline-flex";
                        }
                        this._numbersAndSymbolsElement.style.display = "none";
                        this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelNumbers;
                        this._characterSetDisplayed = "secondaryCharacterSet";
                        firstCharacter = "#__ms_searchboxletter_" + this._secondaryAlphabetFirstCharacter;
                        lastCharacter = "#__ms_searchboxletter_" + this._secondaryAlphabetLastCharacter;
                        this._isShowingLettersNotSymbols = true;
                    } else if ((!this.usesThreeCharacterSets && this._characterSetDisplayed === "primaryCharacterSet") ||
                        (this.usesThreeCharacterSets && this._characterSetDisplayed === "secondaryCharacterSet")) {
                        this._aphabetCharactersElement.style.display = "none";
                        if (this._secondaryAphabetCharactersElement) {
                            this._secondaryAphabetCharactersElement.style.display = "none";
                        }
                        this._numbersAndSymbolsElement.style.display = "inline-flex";
                        this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelLetters;
                        this._characterSetDisplayed = "thirdCharacterSet";
                        firstCharacter = "#__ms_searchboxsymbol_" + this._firstNumberIndex;
                        lastCharacter = "#__ms_searchboxsymbol_" + this._lastNumberIndex;
                        this._isShowingLettersNotSymbols = false;
                    } else {
                        this._aphabetCharactersElement.style.display = "inline-flex";
                        if (this._secondaryAphabetCharactersElement) {
                            this._secondaryAphabetCharactersElement.style.display = "none";
                        }
                        this._numbersAndSymbolsElement.style.display = "none";
                        if (this.usesThreeCharacterSets) {
                            this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelSecondaryLetters;
                        } else {
                            this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelNumbers;
                        }
                        this._characterSetDisplayed = "primaryCharacterSet";
                        firstCharacter = "#__ms_searchboxletter_" + this._firstCharacter;
                        lastCharacter = "#__ms_searchboxletter_" + this._lastCharacter;
                        this._isShowingLettersNotSymbols = true;
                    }
                    this._spaceElement.setAttribute("data-tv-focus-left", lastCharacter);
                    this._spaceElement.setAttribute("data-tv-focus-right", "#backspace");
                    this._toggleLettersOrSymbolsElement.setAttribute("data-tv-focus-right", firstCharacter);
                    this._playSelectionAnimation(ev.target);
                } else {
                    // This must mean the key was a symbol
                    this._inputElement.value += adjustedKey;
                    shouldFireQueryChanged = true;
                    this._playSelectionAnimation(ev.target);
                }
                // Gamepad shortcuts
            } else if (ev.key === "GamepadX" ||
                       ev.keyCode === _KEY_BACKSPACE) {
                this._inputElement.value = this._inputElement.value.substring(0, this._inputElement.value.length - 1);
                shouldFireQueryChanged = true;
                this._playSelectionAnimation(this._element.querySelector("#backspace"));
            } else if (ev.key === "GamepadY" ||
                       ev.keyCode === _KEY_SPACE) {
                this._inputElement.value += ' ';
                shouldFireQueryChanged = true;
                this._playSelectionAnimation(this._element.querySelector("#space"));
            } else if (ev.key === "GamepadLeftTrigger") {
                var firstCharacter = "";
                var lastCharacter = "";
                if (this._characterSetDisplayed === "primaryCharacterSet" &&
                    this.usesThreeCharacterSets) {
                    this._aphabetCharactersElement.style.display = "none";
                    if (this._secondaryAphabetCharactersElement) {
                        this._secondaryAphabetCharactersElement.style.display = "inline-flex";
                    }
                    this._numbersAndSymbolsElement.style.display = "none";
                    this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelNumbers;
                    this._characterSetDisplayed = "secondaryCharacterSet";
                    firstCharacter = "#__ms_searchboxletter_" + this._secondaryAlphabetFirstCharacter;
                    lastCharacter = "#__ms_searchboxletter_" + this._secondaryAlphabetLastCharacter;
                    this._isShowingLettersNotSymbols = true;
                } else if ((!this.usesThreeCharacterSets && this._characterSetDisplayed === "primaryCharacterSet") ||
                    (this.usesThreeCharacterSets && this._characterSetDisplayed === "secondaryCharacterSet")) {
                    this._aphabetCharactersElement.style.display = "none";
                    if (this._secondaryAphabetCharactersElement) {
                        this._secondaryAphabetCharactersElement.style.display = "none";
                    }
                    this._numbersAndSymbolsElement.style.display = "inline-flex";
                    this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelLetters;
                    this._characterSetDisplayed = "thirdCharacterSet";
                    firstCharacter = "#__ms_searchboxsymbol_" + this._firstNumberIndex;
                    lastCharacter = "#__ms_searchboxsymbol_" + this._lastNumberIndex;
                    this._isShowingLettersNotSymbols = false;
                } else {
                    this._aphabetCharactersElement.style.display = "inline-flex";
                    if (this._secondaryAphabetCharactersElement) {
                        this._secondaryAphabetCharactersElement.style.display = "none";
                    }
                    this._numbersAndSymbolsElement.style.display = "none";
                    if (this.usesThreeCharacterSets) {
                        this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelSecondaryLetters;
                    } else {
                        this._toggleLettersOrSymbolsElement.textContent = this.strings.toggleNumbersAndLettersLabelNumbers;
                    }
                    this._characterSetDisplayed = "primaryCharacterSet";
                    firstCharacter = "#__ms_searchboxletter_" + this._firstCharacter;
                    lastCharacter = "#__ms_searchboxletter_" + this._lastCharacter;
                    this._isShowingLettersNotSymbols = true;
                }
                this._spaceElement.setAttribute("data-tv-focus-left", lastCharacter);
                this._spaceElement.setAttribute("data-tv-focus-right", "#backspace");
                this._toggleLettersOrSymbolsElement.setAttribute("data-tv-focus-right", firstCharacter);
                this._playSelectionAnimation(this._element.querySelector("#togglecharacters"));
            }

            if (shouldFireQueryChanged) {
                this._fireEvent("querychanged", {
                    queryText: this._inputElement.value
                });
            }
        };
        _SearchBox.prototype._keyUpHandler = function (ev) {
            var pressedElements = this._element.querySelectorAll(".tv-pressed");
            for (var i = 0, len = pressedElements.length; i < len; i++) {
                pressedElements[i].classList.remove("tv-pressed");
            }
        };
        // Handles input from a keyboard device (like a chatpad)
        _SearchBox.prototype._keyPressHandler = function (ev) {
            if (ev.key &&
                ev.keyCode !== _KEY_BACKSPACE &&
                ev.keyCode !== _KEY_ESCAPE &&
                ev.keyCode !== _KEY_ENTER) {
                this._inputElement.value += ev.key;
                this._playSelectionAnimation();
                this._fireEvent("querychanged", {
                    queryText: this._inputElement.value
                });
            }
        };
        _SearchBox.prototype._playSelectionAnimation = function (element) {
            if (element) {
                element.classList.add("tv-pressed");
            }
        };
        _SearchBox.prototype.addEventListener = function (name, callback, useCapture) {
            this._element.addEventListener(name, callback, useCapture);
        };
        return _SearchBox;
    })();
    window.TVJS = window.TVJS || {};
    TVJS.SearchBox = _SearchBox;

})();