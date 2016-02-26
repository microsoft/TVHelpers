// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.
(function scrollViewerInit() {
    "use strict";

    var SMALL_SCROLL_AMOUNT = 200;
    var PERCENTAGE_OF_PAGE_TO_SCROLL = 0.8;
    var THRESHOLD_TO_SHOW_TOP_ARROW = 50;
    var DelayBeforeCheckingSizeOfScrollableRegion = 500;

    var _KEY_PAGE_UP = 33,
        _KEY_PAGE_DOWN = 34,
        _KEY_LEFT_ARROW = 37,
        _KEY_UP_ARROW = 38,
        _KEY_RIGHT_ARROW = 39,
        _KEY_DOWN_ARROW = 40,
        _KEY_GAMEPAD_A = 195,
        _KEY_GAMEPAD_B = 196,
        _KEY_GAMEPAD_DPAD_UP = 203,
        _KEY_GAMEPAD_DPAD_DOWN = 204,
        _KEY_GAMEPAD_DPAD_LEFT = 205,
        _KEY_GAMEPAD_DPAD_RIGHT = 206,
        _KEY_GAMEPAD_LEFT_THUMBSTICK_UP = 211,
        _KEY_GAMEPAD_LEFT_THUMBSTICK_DOWN = 212,
        _KEY_GAMEPAD_LEFT_THUMBSTICK_LEFT = 214,
        _KEY_GAMEPAD_LEFT_THUMBSTICK_RIGHT = 213,
        _KEY_GAMEPAD_RIGHT_SHOULDER = 199,
        _KEY_GAMEPAD_LEFT_SHOULDER = 200,
        _KEY_THUMB_STICK_THRESHOLD = 0.75;

    var ScrollMode = {
            /// <field type="String" locid="WinJS.UI.ScrollMode.text" helpKeyword="WinJS.UI.ScrollMode.text">  
            /// Indicates the ScrollViewer contains text and must be invoked with the A button, then the contents can be scrolled  
            /// using directional navigation.  
            /// </field>  
            text: "text",
            /// <field type="String" locid="WinJS.UI.ScrollMode.nonModalText" helpKeyword="WinJS.UI.ScrollMode.nonModalText">  
            /// This mode is similar to text mode except the user does not need to press A to begin scrolling. Instead they move  
            /// focus to the ScrollViewer and are able to scroll text. This mode should only be used if there are no focusable   
            /// UI elements above or below the control.  
            /// </field>  
            nonModalText: "nonModalText",
            /// <field type="String" locid="WinJS.UI.ScrollMode.list" helpKeyword="WinJS.UI.ScrollMode.list">  
            /// Indicates the ScrollViewer contains focusable elements and those elements that are off-screen are scrolled into view  
            /// when the user selects those elements.  
            /// </field>  
            list: "list"
        };

    var _ScrollViewer = (function () {
        function _ScrollViewer(element, options) {
            var _this = this;
            if (options === void 0) { options = {}; }
            this._canScrollDown = false;
            this._canScrollUp = false;
            this._disposed = false;
            this._element = element || document.createElement("div");
            options = options || {};
            this._element["winControl"] = this;
            this._element.classList.add("tv-scrollviewer");
            this._handleFocus = this._handleFocus.bind(this);
            this._handleFocusOut = this._handleFocusOut.bind(this);
            this._handleKeyDown = this._handleKeyDown.bind(this);
            this._handleScroll = this._handleScroll.bind(this);
            this._scrollDownBySmallAmount = this._scrollDownBySmallAmount.bind(this);
            this._scrollUpBySmallAmount = this._scrollUpBySmallAmount.bind(this);
            this._scrollDownByLargeAmount = this._scrollDownByLargeAmount.bind(this);
            this._scrollUpByLargeAmount = this._scrollUpByLargeAmount.bind(this);
            this._scrollingIndicatorElement = document.createElement("div");
            this._scrollingIndicatorElement.className = "tv-scrollindicator";
            this._scrollingIndicatorElement.innerHTML =
                "<div class='tv-overlay-arrowindicators'>" +
                "  <div class='tv-overlay-scrollupindicator'></div>" +
                "  <div class='tv-overlay-scrolldownindicator'></div>" +
                "</div>";
            this._scrollingContainer = document.createElement("div");
            this._scrollingContainer.classList.add("tv-scrollviewer-contentelement");
            // Put the contents in a scrolling container  
            var child = this._element.firstChild;
            while (child) {
                var sibling = child.nextSibling;
                this._scrollingContainer.appendChild(child);
                child = sibling;
            }
            this._element.appendChild(this._scrollingContainer);
            this._element.appendChild(this._scrollingIndicatorElement);
            this._scrollingContainer.addEventListener("scroll", this._handleScroll, false);
            this._scrollingContainer.addEventListener("focus", this._handleFocus, false);
            this._element.addEventListener("focusout", this._handleFocusOut, false);
            // Set the default scroll mode  
            this.scrollMode = ScrollMode.text;
            this._refreshVisuals();
            // The scroll viewer has two interaction modes:
            //   1. Normal - In this state there is focusable content within the scrollable  
            //      region. Automatic focus handles directional navigation and scrolls  
            //      elements into view.   
            //   2. Text - In this state there is no focusable content within the scrollable  
            //      region. Typically, this case is free text. In this case, the ScrollViewer  
            //      handles directional navigation and scrolls up and down.  
            //  
            //   To determine which mode we are in, we look for focusable content. If there  
            //   is no focusable content, then we know we are in "Text" mode.  
            // We need to wait for processAll to finish on the inner contents of the scrollable region, because we need accurate   
            // sizing information to determine if a region is scrollable or not.  
            setTimeout(function () {
                if (_this._disposed) {
                    return;
                }
                _this._refreshVisuals();
            }, DelayBeforeCheckingSizeOfScrollableRegion);
        };
        Object.defineProperty(_ScrollViewer.prototype, "element", {
            /// <field type="HTMLElement" domElement="true" hidden="true" locid="WinJS.UI.ScrollViewer.element" helpKeyword="WinJS.UI.ScrollViewer.element">  
            /// Gets the DOM element that hosts the ScrollViewer.  
            /// </field>  
            get: function () {
                return this._element;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(_ScrollViewer.prototype, "scrollMode", {
            /// <field type="String" locid="WinJS.UI.ScrollViewer.interactionMode" helpKeyword="WinJS.UI.ScrollViewer.interactionMode">  
            /// Gets or sets a property that indicates whether there are focusable elements within the ScrollViewer. The default value is false.  
            /// </field>  
            get: function () {
                return this._scrollMode;
            },
            set: function (value) {
                this._scrollMode = value;
                // If there are no focusable elements then we need to listen for the A button.  
                if (this._scrollMode === ScrollMode.list) {
                    this._scrollingContainer.tabIndex = -1;
                    this._element.classList.remove("tv-scrollviewer-scrollmode-text");
                    this._scrollingContainer.classList.remove("tv-active");
                    this._element.classList.add("tv-scrollviewer-scrollmode-list");
                    this._element.removeEventListener("keydown", this._handleKeyDown, true);
                    this._setInactive();
                }
                else {
                    this._scrollingContainer.tabIndex = 0;
                    this._element.classList.remove("tv-scrollviewer-scrollmode-list");
                    this._element.classList.add("tv-scrollviewer-scrollmode-text");
                    this._scrollingContainer.classList.add("tv-active");
                    this._element.addEventListener("keydown", this._handleKeyDown, true);
                }
            },
            enumerable: true,
            configurable: true
        });
        /// <signature helpKeyword="WinJS.UI.ScrollViewer.dispose">  
        /// <summary locid="WinJS.UI.ScrollViewer.dispose">  
        /// Disposes the control.  
        /// </summary>  
        /// </signature>  
        _ScrollViewer.prototype.dispose = function () {
            if (this._disposed) {
                return;
            }
            this._disposed = true;
            this._element.classList.remove("tv-xyfocus-suspended");
        };
        /// <signature helpKeyword="WinJS.UI.ScrollViewer.refresh">  
        /// <summary locid="WinJS.UI.ScrollViewer.refresh">  
        /// Call this function whenever the contents of the ScrollViewer changes.  
        /// </summary>  
        /// </signature>  
        _ScrollViewer.prototype.refresh = function () {
            this._refreshVisuals();
        };
        _ScrollViewer.prototype._refreshVisuals = function () {
            var _this = this;
            // We call this function any time the size of the contents within the ScrollViewer changes. This function  
            // determines if we need to display the visual treatment for "more content".
            setTimeout(function () {
                if (_this._disposed) {
                    return;
                }
                // Set initial visibility for the arrow indicators if the contents of the scrollable region  
                // is bigger than the viewable area.  
                if (_this._scrollingContainer.clientHeight < _this._scrollingContainer.scrollHeight) {
                    if (_this._scrollingContainer.scrollTop > 0) {
                        _this._canScrollUp = true;
                    }
                    else {
                        _this._canScrollUp = false;
                    }
                    if (_this._scrollingContainer.scrollTop >= (_this._scrollingContainer.scrollHeight - _this._element.clientHeight)) {
                        _this._canScrollDown = false;
                    }
                    else {
                        _this._canScrollDown = true;
                    }
                    if (!_this._canScrollUp && !_this._canScrollDown) {
                        _this._scrollingIndicatorElement.classList.remove("tv-scrollable-down");
                        _this._scrollingIndicatorElement.classList.remove("tv-scrollable-up");
                    }
                    else if (!_this._canScrollUp && _this._canScrollDown) {
                        _this._scrollingIndicatorElement.classList.remove("tv-scrollable-up");
                        _this._scrollingIndicatorElement.classList.add("tv-scrollable-down");
                    }
                    else if (_this._canScrollUp && !_this._canScrollDown) {
                        _this._scrollingIndicatorElement.classList.add("tv-scrollable-up");
                        _this._scrollingIndicatorElement.classList.remove("tv-scrollable-down");
                    }
                    else {
                        _this._scrollingIndicatorElement.classList.add("tv-scrollable-down");
                        _this._scrollingIndicatorElement.classList.add("tv-scrollable-up");
                    }
                    // We only make the ScrollViewer focusable if it has text content and the  
                    // text content does not fit on the screen. If the text content does fit  
                    // on the screen then there is no reason to make the user scroll because  
                    // they can see all of the text.  
                    if (_this._scrollMode === ScrollMode.text || _this._scrollMode === ScrollMode.nonModalText) {
                        _this._scrollingContainer.classList.add("tv-focusable");
                    }
                    // Add a class to indicate that the content within the ScrollViewer is bigger than  
                    // the visible area which means the ScrollViewer will need to be able to Scroll.  
                    _this._element.classList.add("tv-scrollable");
                }
                else {
                    _this._element.classList.remove("tv-scrollable");
                }
            });
        };
        _ScrollViewer.prototype._scrollDownBySmallAmount = function () {
            if (this._isActive() && this._scrollingContainer.msZoomTo) {
                this._scrollingContainer.msZoomTo({ contentX: 0, contentY: this._scrollingContainer.scrollTop + SMALL_SCROLL_AMOUNT, viewportX: 0, viewportY: 0 });
            }
        };
        _ScrollViewer.prototype._scrollUpBySmallAmount = function () {
            if (this._isActive() && this._scrollingContainer.msZoomTo) {
                this._scrollingContainer.msZoomTo({ contentX: 0, contentY: this._scrollingContainer.scrollTop - SMALL_SCROLL_AMOUNT, viewportX: 0, viewportY: 0 });
            }
        };
        _ScrollViewer.prototype._scrollDownByLargeAmount = function () {
            if ((this._isActive()) && this._scrollingContainer.msZoomTo) {
                this._scrollingContainer.msZoomTo({ contentX: 0, contentY: this._scrollingContainer.scrollTop + (PERCENTAGE_OF_PAGE_TO_SCROLL * this._scrollingContainer.clientHeight), viewportX: 0, viewportY: 0 });
            }
        };
        _ScrollViewer.prototype._scrollUpByLargeAmount = function () {
            if ((this._isActive()) && this._scrollingContainer.msZoomTo) {
                this._scrollingContainer.msZoomTo({ contentX: 0, contentY: this._scrollingContainer.scrollTop - (PERCENTAGE_OF_PAGE_TO_SCROLL * this._scrollingContainer.clientHeight), viewportX: 0, viewportY: 0 });
            }
        };
        _ScrollViewer.prototype._isActive = function () {
            return this._scrollingContainer.classList.contains("tv-active");
        };
        _ScrollViewer.prototype._setActive = function () {
            this._scrollingContainer.classList.add("tv-active");
            if (TVJS.DirectionalNavigation) {
                TVJS.DirectionalNavigation.enabled = false;
            }
        };
        _ScrollViewer.prototype._setInactive = function () {
            this._scrollingContainer.classList.remove("tv-active");
            if (TVJS.DirectionalNavigation) {
                TVJS.DirectionalNavigation.enabled = true;
            }
        };
        _ScrollViewer.prototype._handleFocus = function (ev) {
            if (this._scrollMode === ScrollMode.nonModalText) {
                this._setActive();
            }
        };
        _ScrollViewer.prototype._handleFocusOut = function (ev) {
            // If focus leaves the ScrollViewer & it was in the "invoked" state,  
            // we need to reset it's state.  
            if (this._isActive() && !this._element.contains(document.activeElement)) {
                this._setInactive();
            }
        };
        _ScrollViewer.prototype._handleKeyDown = function (ev) {
            // Only set handled = true for shoulder button cases so that   
            // scroll viewer doesn't trigger a hub interaction.  
            var handled = false;
            switch (ev.keyCode) {
                case _KEY_UP_ARROW:
                case _KEY_GAMEPAD_DPAD_UP:
                case _KEY_GAMEPAD_LEFT_THUMBSTICK_UP:
                    if (this._scrollMode === ScrollMode.nonModalText) {
                        if (this._scrollingContainer.scrollTop >= THRESHOLD_TO_SHOW_TOP_ARROW) {
                        }
                        else {
                            var nextFocusElement = TVJS.DirectionalNavigation.findNextFocusElement("up");
                            if (nextFocusElement) {
                                nextFocusElement.focus();
                            }
                        }
                    }
                    this._scrollUpBySmallAmount();
                    break;
                case _KEY_DOWN_ARROW:
                case _KEY_GAMEPAD_DPAD_DOWN:
                case _KEY_GAMEPAD_LEFT_THUMBSTICK_DOWN:
                    if (this._scrollMode === ScrollMode.nonModalText) {
                        if (this._scrollingContainer.scrollTop >= (this._scrollingContainer.scrollHeight - this._element.clientHeight)) {
                            var nextFocusElement = TVJS.DirectionalNavigation.findNextFocusElement("down");
                            if (nextFocusElement) {
                                nextFocusElement.focus();
                            }
                        }
                        else {
                        }
                    }
                    this._scrollDownBySmallAmount();
                    break;
                case _KEY_LEFT_ARROW:
                case _KEY_GAMEPAD_DPAD_LEFT:
                case _KEY_GAMEPAD_LEFT_THUMBSTICK_LEFT:
                    var direction = "left";
                case _KEY_RIGHT_ARROW:
                case _KEY_GAMEPAD_DPAD_RIGHT:
                case _KEY_GAMEPAD_LEFT_THUMBSTICK_RIGHT:
                    var direction = direction || "right";
                    // If we successfully move focus to a new target element, then set the ScrollViewer as inactive  
                    if (this._isActive()) {
                        var previousFocusRectangleObject = this._scrollingContainer.getBoundingClientRect();
                        var previousFocusRectangle = {
                            top: previousFocusRectangleObject.top,
                            left: previousFocusRectangleObject.left,
                            width: previousFocusRectangleObject.width,
                            height: previousFocusRectangleObject.height
                        };
                        var nextFocusElement = TVJS.DirectionalNavigation.moveFocus(direction, { referenceRect: previousFocusRectangle });
                        if (nextFocusElement) {
                            this._setInactive();
                        }
                    }
                    break;
                case _KEY_PAGE_UP:
                case _KEY_GAMEPAD_LEFT_SHOULDER:
                    this._scrollUpByLargeAmount();
                    handled = true;
                    break;
                case _KEY_PAGE_DOWN:
                case _KEY_GAMEPAD_RIGHT_SHOULDER:
                    this._scrollDownByLargeAmount();
                    handled = true;
                    break;
                default:
                    break;
            }
            if (handled) {
                ev.stopPropagation();
            }
        };
        _ScrollViewer.prototype._handleScroll = function (ev) {
            if (this._scrollingContainer.scrollTop >= THRESHOLD_TO_SHOW_TOP_ARROW) {
                this._canScrollUp = true;
            }
            else {
                this._canScrollUp = false;
            }
            if (this._scrollingContainer.scrollTop >= (this._scrollingContainer.scrollHeight - this._element.clientHeight)) {
                this._canScrollDown = false;
            }
            else {
                this._canScrollDown = true;
            }
            // Note: We remove the classes in order so we can avoid labels flashing  
            if (!this._canScrollUp && !this._canScrollDown) {
                _this._scrollingIndicatorElement.classList.remove("tv-scrollable-down");
                _this._scrollingIndicatorElement.classList.remove("tv-scrollable-up");
            }
            else if (!this._canScrollUp && this._canScrollDown) {
                this._scrollingIndicatorElement.classList.remove("tv-scrollable-up");
                this._scrollingIndicatorElement.classList.add("tv-scrollable-down");
            }
            else if (this._canScrollUp && !this._canScrollDown) {
                _this._scrollingIndicatorElement.classList.add("tv-scrollable-up");
                this._scrollingIndicatorElement.classList.remove("tv-scrollable-down");
            }
            else {
                this._scrollingIndicatorElement.classList.add("tv-scrollable-up");
                this._scrollingIndicatorElement.classList.add("tv-scrollable-down");
            }
        };
        return _ScrollViewer;
    })();
    window.TVJS = window.TVJS || {};
    TVJS.ScrollViewer = _ScrollViewer;
    TVJS.ScrollMode = ScrollMode;
})();