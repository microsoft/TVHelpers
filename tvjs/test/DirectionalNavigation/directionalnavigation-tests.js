// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.

var Keys = {
    w: 87,
    a: 65,
    s: 83,
    d: 68,
    navigationUp: 138,
    navigationDown: 139,
    navigationLeft: 140,
    navigationRight: 141,
    gamepadA: 195,
    gamepadB: 196,
    gamepadDPadUp: 203,
    gamepadDPadDown: 204,
    gamepadDPadLeft: 205,
    gamepadDPadRight: 206,
    gamepadLeftThumbstickUp: 211,
    gamepadLeftThumbstickDown: 212,
    gamepadLeftThumbstickRight: 213,
    gamepadLeftThumbstickLeft: 214
};

var highlightStyle = document.createElement("style");
highlightStyle.innerHTML = "button:focus { background-color: red; }";
function setImmediateCrossBrowser(callBack) {
    if (window.setImmediate) {
        setImmediate(callBack);
    } else {
        setTimeout(callBack, 0);
    }
};
function helperKeydown(element, keyCode, locale) {
    /// <summary>
    ///  Throw keydown event from element.
    /// </summary>
    /// <param name="element" type="object">
    ///  Handle to element to throw keydown from
    /// </param>
    /// <param name="key" type="object">
    ///  Key identifier to throw
    /// </param>
    /// <param name="locale" type="string" isOptional="true">
    ///  Key identifier to throw
    /// </param>
    if (element) {
        locale = locale || "en-US";
        // We are purposely creating a CustomEvent instead of a KeyboardEvent because we cannot assign the keyCode.
        // This method works as long as there is no need to specify modifier keys.
        var event = document.createEvent("CustomEvent");
        event.initCustomEvent("keydown", true, true, null);
        event.keyCode = keyCode;
        event.locale = locale;
        element.dispatchEvent(event);
    }
}

function createAndAppendFocusableElement(x, y, container, textContent, tagName, width, height, extraClass) {
    if (!tagName) {
        tagName = "button";
    }
    if (!width) {
        width = 150;
    }
    if (!height) {
        height = 150;
    }
    var e = document.createElement(tagName);
    e.style.position = "absolute";
    e.style.width = width + "px";
    e.style.height = height + "px";
    e.style.minWidth = e.style.minHeight = "0px";
    e.style.left = x + "px";
    e.style.top = y + "px";
    e.tabIndex = 0;
    e.classList.add(extraClass);

    e["width"] = width;
    e["height"] = height;

    if (textContent) {
        e.textContent = textContent;
    }
    container.appendChild(e);
    return e;
}

function createCrossLayout(container, tagName) {
    /*
     *   1
     * 2 3 4
     *   5
     */

    if (!tagName) {
        tagName = "button";
    }

    if (!container) {
        container = document.createElement("div");
        container.style.position = "absolute";
        container.style.width = container.style.height = "600px";
    }
    return [
        container,
        createAndAppendFocusableElement(250, 50, container, "1", tagName, null, null, "top"),
        createAndAppendFocusableElement(50, 250, container, "2", tagName, null, null, "left"),
        createAndAppendFocusableElement(250, 250, container, "3", tagName, null, null, "middle"),
        createAndAppendFocusableElement(450, 250, container, "4", tagName, null, null, "right"),
        createAndAppendFocusableElement(250, 450, container, "5", tagName, null, null, "bottom")
    ];
}

function spinWait(evaluator) {
    return new Promise(function (c) {
        var count = 0;
        var handle = setInterval(function () {
            if (++count === 100) {
                clearInterval(handle);
                evaluator();    // Step into this call to debug the evaluator
                throw "test timeout";
            }
            if (evaluator()) {
                c();
                clearInterval(handle);
            }
        }, 50);
    });
}

function waitForFocus(w, e) {
    return spinWait(function () {
        return w.document.activeElement === e;
    });
}

// Setup
function testStart() {
    document.body.appendChild(highlightStyle);
    var rootContainer = document.createElement("div");
    rootContainer.style.position = "relative";
    document.body.appendChild(rootContainer);
    TVJS.DirectionalNavigation.focusRoot = rootContainer;
    return rootContainer;
};

// Cleanup
function testDone(rootContainer) {
    document.body.removeChild(highlightStyle);
    rootContainer.parentElement.removeChild(rootContainer);
    rootContainer = null;
    TVJS.DirectionalNavigation.focusRoot = null;

    // Clear event listeners
    TVJS.DirectionalNavigation["_listeners"].focuschanging = [];
    TVJS.DirectionalNavigation["_listeners"].focuschanged = [];
};

QUnit.test("FindNextFocusElement", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    var target = TVJS.DirectionalNavigation.findNextFocusElement("left");
    assert.equal(target, layout[2]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("right");
    assert.equal(target, layout[4]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("up");
    assert.equal(target, layout[1]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("down");
    assert.equal(target, layout[5]);
    testDone(rootContainer);
});

QUnit.test("FindNextFocusElement with unfocusable reference element and initial focus", function (assert) {
    var rootContainer = testStart();
    var layout = [
        rootContainer,
        createAndAppendFocusableElement(50, 50, rootContainer, "1", "div", 150, 150),
        createAndAppendFocusableElement(200, 50, rootContainer, "2", "button", 150, 150),
    ];

    layout[1].setAttribute("tabIndex", "");
    layout[2].focus();

    var target = TVJS.DirectionalNavigation.findNextFocusElement("right", { referenceElement: layout[1] });
    assert.equal(target, layout[2]);
    testDone(rootContainer);
});

QUnit.test("FindNextFocusElement with reference element", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    var target = TVJS.DirectionalNavigation.findNextFocusElement("left", { referenceElement: layout[4] });
    assert.equal(target, layout[3]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("right", { referenceElement: layout[2] });
    assert.equal(target, layout[3]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("up", { referenceElement: layout[5] });
    assert.equal(target, layout[3]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("down", { referenceElement: layout[1] });
    assert.equal(target, layout[3]);
    testDone(rootContainer);
});

QUnit.test("FindNextFocusElement with no initial focus", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    document.body.focus();
    assert.equal(document.activeElement, document.body);

    var target = TVJS.DirectionalNavigation.findNextFocusElement("right");
    assert.ok(target);
    testDone(rootContainer);
});

QUnit.test("Move focus", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(layout[3], document.activeElement);

    var target = TVJS.DirectionalNavigation.moveFocus("left");
    assert.equal(target, document.activeElement);
    assert.equal(document.activeElement, layout[2]);

    target = TVJS.DirectionalNavigation.moveFocus("right");
    assert.equal(target, document.activeElement);
    assert.equal(document.activeElement, layout[3]);

    target = TVJS.DirectionalNavigation.moveFocus("up");
    assert.equal(target, document.activeElement);
    assert.equal(document.activeElement, layout[1]);

    target = TVJS.DirectionalNavigation.moveFocus("down");
    assert.equal(target, document.activeElement);
    assert.equal(document.activeElement, layout[3]);
    testDone(rootContainer);
});

QUnit.test("FocusRoot", function (assert) {
    var rootContainer = testStart();
    var left = createCrossLayout();
    var right = createCrossLayout();
    right[0].style.top = "0px";
    right[0].style.left = "700px";

    rootContainer.appendChild(left[0]);
    rootContainer.appendChild(right[0]);

    left[3].focus();
    assert.equal(left[3], document.activeElement);

    TVJS.DirectionalNavigation.moveFocus("right");
    assert.equal(left[4], document.activeElement);

    // Moving right should NOT move out of the left container
    var target = TVJS.DirectionalNavigation.moveFocus("right", { focusRoot: left[0] });
    assert.equal(left[4], document.activeElement);
    assert.notOk(target);

    // Try the same as above using global focus root settings
    TVJS.DirectionalNavigation.focusRoot = left[0];
    target = TVJS.DirectionalNavigation.moveFocus("right");
    assert.equal(left[4], document.activeElement);
    assert.notOk(target);

    testDone(rootContainer);
});

QUnit.test("Focus history", function (assert) {
    var rootContainer = testStart();
    /**
        * ??????????????? ???????????????
        * ?      1      ? ?             ?
        * ??????????????? ?             ?
        * ??????????????? ?             ?
        * ?             ? ?      3      ?
        * ?      2      ? ?             ?
        * ?             ? ?             ?
        * ??????????????? ???????????????
        *
        * Normally, if focus was on 3, left would resolve to 2 since 2 occupies a bigger portion of 3's shadow.
        * However, if focus initially was on 1, then was moved right to 3, then a following left should resolve to 1.
    **/

    var layout = [
        rootContainer,
        createAndAppendFocusableElement(50, 50, rootContainer, "1", "button", 200, 100),
        createAndAppendFocusableElement(50, 200, rootContainer, "2", "button", 200, 300),
        createAndAppendFocusableElement(350, 50, rootContainer, "3", "button", 200, 450)
    ];

    // Move focus left from 3
    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);
    TVJS.DirectionalNavigation._xyFocus("left");
    assert.equal(document.activeElement, layout[1]);

    TVJS.DirectionalNavigation._xyFocus("down");
    assert.equal(document.activeElement, layout[2]);

    // Move focus right from 2, then left and we should end up at 2 again
    TVJS.DirectionalNavigation._xyFocus("right");
    assert.equal(document.activeElement, layout[3]);
    TVJS.DirectionalNavigation._xyFocus("left");
    assert.equal(document.activeElement, layout[2]);
    testDone(rootContainer);
});

QUnit.test("Focus history with fractional pixels", function (assert) {
    var rootContainer = testStart();
    /**
     *  ??????????????????????????????
     *  ?             ??             ?
     *  ?             ??      2      ?
     *  ?             ??             ?
     *  ?      1      ????????????????
     *  ?             ??             ?
     *  ?             ??      3      ?
     *  ?             ??             ?
     *  ??????????????????????????????
     *
     * Normally, if focus was on 3, left would resolve to 2 since 2 occupies a bigger portion of 3's shadow.
     * However, if focus initially was on 1, then was moved right to 3, then a following left should resolve to 1.
    **/

    var layout = [
        rootContainer,
        createAndAppendFocusableElement(50, 50.25, rootContainer, "1", "button", 428, 212),
        createAndAppendFocusableElement(480, 50.25, rootContainer, "2", "button", 104, 104),
        createAndAppendFocusableElement(480, 158.25, rootContainer, "3", "button", 104, 104)
    ];

    // Move focus left from 3 to 1
    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);
    TVJS.DirectionalNavigation._xyFocus("left");
    assert.equal(document.activeElement, layout[1]);

    // Move focus right from 1 should land us on 3 again
    TVJS.DirectionalNavigation._xyFocus("right");
    assert.equal(document.activeElement, layout[3]);
    testDone(rootContainer);
});

QUnit.test("Prevent focus moving with the FocusChanging event", function (assert) {
    var rootContainer = testStart();
    var eventReceived = false;
    TVJS.DirectionalNavigation.addEventListener("focuschanging", function (e) {
        assert.equal(layout[1], e.detail.nextFocusElement);
        e.preventDefault();
        eventReceived = true;
    });

    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(layout[3], document.activeElement);

    TVJS.DirectionalNavigation.moveFocus("up");
    assert.equal(layout[3], document.activeElement);
    assert.ok(eventReceived);
    testDone(rootContainer);
});

QUnit.test("Prevent directional navigation with keydown preventDefault", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    var eventReceived = false;
    layout[3].addEventListener("keydown", function (e) {
        eventReceived = true;
        e.preventDefault()
    });

    helperKeydown(layout[3], Keys.gamepadDPadUp);
    assert.ok(eventReceived);
    assert.equal(document.activeElement, layout[3]);
    testDone(rootContainer);
});

QUnit.test("Override attribute", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);
    for (var i = 1; i < layout.length; i++) {
        layout[i].id = "btn" + i;
    }
    layout[3].setAttribute("data-tv-focus-left", "#btn4");
    layout[3].setAttribute("data-tv-focus-right", "#btn2");
    layout[3].setAttribute("data-tv-focus-up", "#btn5");
    layout[3].setAttribute("data-tv-focus-down", "#btn1");

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    var target = TVJS.DirectionalNavigation.findNextFocusElement("up");
    assert.equal(target, layout[5]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("down");
    assert.equal(target, layout[1]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("left");
    assert.equal(target, layout[4]);

    target = TVJS.DirectionalNavigation.findNextFocusElement("right");
    assert.equal(target, layout[2]);
    testDone(rootContainer);
});

QUnit.test("Directional Focus with disabled elements", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);
    layout[3].disabled = true;

    layout[5].focus();
    assert.equal(document.activeElement, layout[5]);

    TVJS.DirectionalNavigation.moveFocus("up");
    assert.equal(document.activeElement, layout[1]);
    testDone(rootContainer);
});

QUnit.test("Directional Navigation with tabIndex", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer, "div");
    layout[3].tabIndex = -1;

    layout[5].focus();
    assert.equal(document.activeElement, layout[5]);

    TVJS.DirectionalNavigation.moveFocus("up");
    assert.equal(document.activeElement, layout[1]);

    layout[3].tabIndex = 0;
    TVJS.DirectionalNavigation.moveFocus("down");
    assert.equal(document.activeElement, layout[3]);
    testDone(rootContainer);
});

QUnit.test("Directional Navigation with default mappings", function (assert) {
    var rootContainer = testStart();
    var done = assert.async();
    function doKeydown(targetElement, keyCode, expNextEl) {
        expectedKeyCode = keyCode;
        expectedNextElement = expNextEl;
        helperKeydown(targetElement, keyCode);
    }

    var numEventsReceived = 0;
    var expectedKeyCode = -1;
    var expectedNextElement;
    TVJS.DirectionalNavigation.addEventListener("focuschanging", function (e) {
        assert.equal(e.detail.keyCode, expectedKeyCode);
        assert.equal(e.detail.nextFocusElement, expectedNextElement);
        numEventsReceived++;
    });

    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    doKeydown(layout[3], Keys.gamepadLeftThumbstickUp, layout[1]);
    waitForFocus(window, layout[1])
        .then(function () {
            doKeydown(layout[1], Keys.gamepadLeftThumbstickDown, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.gamepadLeftThumbstickLeft, layout[2]);
            return waitForFocus(window, layout[2]);
        }).then(function () {
            doKeydown(layout[2], Keys.gamepadLeftThumbstickRight, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.gamepadDPadUp, layout[1]);
            return waitForFocus(window, layout[1]);
        }).then(function () {
            doKeydown(layout[1], Keys.gamepadDPadDown, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.gamepadDPadLeft, layout[2]);
            return waitForFocus(window, layout[2]);
        }).then(function () {
            doKeydown(layout[2], Keys.gamepadDPadRight, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.navigationUp, layout[1]);
            return waitForFocus(window, layout[1]);
        }).then(function () {
            doKeydown(layout[1], Keys.navigationDown, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.navigationLeft, layout[2]);
            return waitForFocus(window, layout[2]);
        }).then(function () {
            doKeydown(layout[2], Keys.navigationRight, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            assert.equal(numEventsReceived, 12);
            testDone(rootContainer);
            done();
        });
});

QUnit.test("Directional Navigation with custom key mappings", function (assert) {
    var rootContainer = testStart();
    var done = assert.async();
    function doKeydown(targetElement, keyCode, expNextEl) {
        expectedKeyCode = keyCode;
        expectedNextElement = expNextEl;
        helperKeydown(targetElement, keyCode);
    }

    var numEventsReceived = 0;
    var expectedKeyCode = -1;
    var expectedNextElement;
    TVJS.DirectionalNavigation.addEventListener("focuschanging", function (e) {
        assert.equal(e.detail.keyCode, expectedKeyCode);
        assert.equal(e.detail.nextFocusElement, expectedNextElement);
        numEventsReceived++;
    });

    TVJS.DirectionalNavigation.keyCodeMap["up"].push(Keys.w);
    TVJS.DirectionalNavigation.keyCodeMap["down"].push(Keys.s);
    TVJS.DirectionalNavigation.keyCodeMap["left"].push(Keys.a);
    TVJS.DirectionalNavigation.keyCodeMap["right"].push(Keys.d);
    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    doKeydown(layout[3], Keys.w, layout[1]);
    waitForFocus(window, layout[1])
        .then(function () {
            doKeydown(layout[1], Keys.s, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            doKeydown(layout[3], Keys.a, layout[2]);
            return waitForFocus(window, layout[2]);
        }).then(function () {
            doKeydown(layout[2], Keys.d, layout[3]);
            return waitForFocus(window, layout[3]);
        }).then(function () {
            assert.equal(numEventsReceived, 4);
            testDone(rootContainer);
            done();
        });
});

QUnit.test("FocusChanged event", function (assert) {
    var rootContainer = testStart();
    var done = assert.async();
    TVJS.DirectionalNavigation.addEventListener("focuschanged", function (e) {
        assert.equal(e.detail.previousFocusElement, layout[3]);
        assert.equal(document.activeElement, layout[1]);
        assert.equal(e.detail.keyCode, Keys.gamepadDPadUp);
        testDone(rootContainer);
        done();
    });

    var layout = createCrossLayout(rootContainer);

    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    helperKeydown(layout[3], Keys.gamepadDPadUp);
});

QUnit.test("FocusChanged event", function (assert) {
    var done = assert.async();
    var rootContainer = testStart();
    /**
     *        1 2
     *      ???????
     *    8 ? 0 1 ? 3
     *    7 ? 2 3 ? 4
     *      ???????
     *        6 5
    **/

    var layout = [
        rootContainer,
        createAndAppendFocusableElement(125, 25, rootContainer, "1", "button", 50, 50),
        createAndAppendFocusableElement(200, 25, rootContainer, "2", "button", 50, 50),

        createAndAppendFocusableElement(300, 125, rootContainer, "3", "button", 50, 50),
        createAndAppendFocusableElement(300, 200, rootContainer, "4", "button", 50, 50),

        createAndAppendFocusableElement(200, 300, rootContainer, "5", "button", 50, 50),
        createAndAppendFocusableElement(125, 300, rootContainer, "6", "button", 50, 50),

        createAndAppendFocusableElement(25, 200, rootContainer, "7", "button", 50, 50),
        createAndAppendFocusableElement(25, 125, rootContainer, "8", "button", 50, 50),

    ];
    var iframeEl = createAndAppendFocusableElement(100, 100, rootContainer, null, "iframe", 175, 175);
    iframeEl.src = "DirectionalNavigationPage.html";
    var iframeWin = iframeEl.contentWindow;

    window.addEventListener("message", function ready(e) {
        // The first crossframe message indicates that the iframe has loaded.
        window.removeEventListener("message", ready);
        iframeLayout = iframeWin.document.querySelectorAll("button");

        layout[1].focus();
        assert.equal(layout[1], document.activeElement);

        TVJS.DirectionalNavigation._xyFocus("down");
        assert.equal(iframeEl, document.activeElement);
        waitForFocus(iframeWin, iframeLayout[0])
            .then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("right");
                return waitForFocus(iframeWin, iframeLayout[1]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("up");
                return waitForFocus(window, layout[2]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("down");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[1]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("right");
                return waitForFocus(window, layout[3]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("left");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[1]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("down");
                return waitForFocus(iframeWin, iframeLayout[3]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("right");
                return waitForFocus(window, layout[4]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("left");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[3]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("down");
                return waitForFocus(window, layout[5]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("up");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[3]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("left");
                return waitForFocus(iframeWin, iframeLayout[2]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("down");
                return waitForFocus(window, layout[6]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("up");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[2]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("left");
                return waitForFocus(window, layout[7]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("right");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[2]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("up");
                return waitForFocus(iframeWin, iframeLayout[0]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("left");
                return waitForFocus(window, layout[8]);
            }).then(function () {
                TVJS.DirectionalNavigation._xyFocus("right");
                assert.equal(iframeEl, document.activeElement);
                return waitForFocus(iframeWin, iframeLayout[0]);
            }).then(function () {
                iframeWin["TVJS"].DirectionalNavigation._xyFocus("up");
                return waitForFocus(window, layout[1]);
            }).then(function () {
                testDone(rootContainer);
                done();
            });
    });
});

QUnit.test("Directional Navigation with no active element does not cause exceptions", function (assert) {
    var rootContainer = testStart();
    var body = document.body;
    document.body.parentElement.removeChild(body);
    helperKeydown(document.documentElement, Keys.gamepadDPadUp);
    document.documentElement.appendChild(body);
    assert.ok(true);
});

QUnit.test("Iframe with focusable body", function (assert) {
    var rootContainer = testStart();
    var done = assert.async();
    /*
        [BUTTON] [IFRAME] [BUTTON]
    */

    var leftButton = createAndAppendFocusableElement(0, 0, rootContainer, null, "button", 200, 200);

    var iframeEl = createAndAppendFocusableElement(210, 0, rootContainer, null, "iframe", 200, 200);
    iframeEl.src = "BlankDirectionalFocusPageWithFocusableBody.html";
    var iframeWin = iframeEl.contentWindow;

    var rightButton = createAndAppendFocusableElement(420, 0, rootContainer, null, "button", 200, 200);

    window.addEventListener("message", function ready(e) {
        // The first crossframe message indicates that the iframe has loaded.
        window.removeEventListener("message", ready);

        leftButton.focus();
        assert.equal(document.activeElement, leftButton);

        TVJS.DirectionalNavigation._xyFocus("right");
        assert.equal(document.activeElement, iframeEl);
        setTimeout(function () {
            assert.equal(document.activeElement, iframeEl, "Focus should not be automatically exiting the iframe");
            iframeWin["TVJS"].DirectionalNavigation._xyFocus("right");
            return waitForFocus(window, rightButton).then(function () {
                testDone(rootContainer);
                done();
            });
        }, 500);
    });
});

QUnit.test("Directional Navigation with no active element does not cause exceptions", function (assert) {
    var rootContainer = testStart();
    var done = assert.async();
    /*
        [BUTTON] [IFRAME] [BUTTON]
    */

    var leftButton = createAndAppendFocusableElement(0, 0, rootContainer, null, "button", 200, 200);

    var iframeEl = createAndAppendFocusableElement(210, 0, rootContainer, null, "iframe", 200, 200);
    iframeEl.src = "BlankDirectionalFocusPageWithFocusableBody.html";
    var iframeWin = iframeEl.contentWindow;

    var rightButton = createAndAppendFocusableElement(420, 0, rootContainer, null, "button", 200, 200);

    window.addEventListener("message", function ready(e) {
        // The first crossframe message indicates that the iframe has loaded.
        window.removeEventListener("message", ready);

        // Make the body inside the iframe unfocusable
        iframeWin.document.body.tabIndex = -1;

        leftButton.focus();
        assert.equal(document.activeElement, leftButton);

        // Going right from the left button should focus the iframe but the iframe should immediately
        // signal back a dFocusExit to the right which gets us to the expected right button.
        TVJS.DirectionalNavigation._xyFocus("right");
        assert.equal(document.activeElement, iframeEl);
        waitForFocus(window, rightButton).then(function () {
            testDone(rootContainer);
            done();
        });
    });
});

QUnit.test("Directional Navigation moves focus when enabled", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer);

    layout[5].focus();
    assert.equal(document.activeElement, layout[5]);

    TVJS.DirectionalNavigation.enabled = true;

    TVJS.DirectionalNavigation._handleKeyDownEvent({
        preventDefault: function () { },
        keyCode: Keys.navigationUp
    });
    assert.equal(document.activeElement, layout[3]);

    testDone(rootContainer);
});

QUnit.test("Directional Navigation doesn't move focus when disabled", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer, "div");

    layout[5].focus();
    assert.equal(document.activeElement, layout[5]);

    TVJS.DirectionalNavigation.enabled = false;

    TVJS.DirectionalNavigation._handleKeyDownEvent({
        keyCode: Keys.navigationUp
    });
    assert.notEqual(document.activeElement, layout[3]);

    testDone(rootContainer);
});

QUnit.test("Accept keyup fires a click event", function (assert) {
    var done = assert.async();

    var div = document.createElement("div");
    document.body.appendChild(div);

    div.addEventListener("click", function handleClick() {
        div.removeEventListener("click", handleClick);
        assert.ok(true);
        div.parentNode.removeChild(div);
        done();
    });

    TVJS.DirectionalNavigation._handleKeyUpEvent({
        keyCode: Keys.gamepadA,
        srcElement: div
    });
});

QUnit.test("A non-accept keyup does not fire a click event", function (assert) {
    var done = assert.async();
    var clickDidNotFired = true;
    var div = document.createElement("div");
    document.body.appendChild(div);

    div.addEventListener("click", function handleClick() {
        div.removeEventListener("click", handleClick);
        clickDidNotFired = false;
    });

    TVJS.DirectionalNavigation._handleKeyUpEvent({
        keyCode: Keys.gamepadDPadUp,
        srcElement: div
    });
    // Wait for a little while to be sure the click didn't happen
    setImmediateCrossBrowser(function () {
        setImmediateCrossBrowser(function () {
            assert.ok(clickDidNotFired);
            div.parentNode.removeChild(div);
            done();
        });
    });
});

QUnit.test("Accept keydown does not fire a click event", function (assert) {
    var done = assert.async();
    var clickDidNotFired = true;
    var div = document.createElement("div");
    document.body.appendChild(div);

    div.addEventListener("click", function handleClick() {
        div.removeEventListener("click", handleClick);
        clickDidNotFired = false;
    });

    TVJS.DirectionalNavigation._handleKeyDownEvent({
        keyCode: Keys.gamepadA,
        srcElement: div
    });
    // Wait for a little while to be sure the click didn't happen
    setImmediate(function () {
        setImmediate(function () {
            assert.ok(clickDidNotFired);
            div.parentNode.removeChild(div);
            done();
        });
    });
});

QUnit.test("Elements that are not normally focusable, can get focus if they are in the focusableSelectors list", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer, "div");
    layout[3].tabIndex = 0;
    layout[3].focus();
    assert.equal(document.activeElement, layout[3]);

    layout[1].className = "foo";
    TVJS.DirectionalNavigation.focusableSelectors.push(".foo");

    target = TVJS.DirectionalNavigation.findNextFocusElement("up");
    assert.equal(target, layout[1]);

    testDone(rootContainer);
});

QUnit.test("If an element in the focusable selectors list doesn't have a tabIndex set on it, then one is set", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer, "div");
    layout[3].tabIndex = 0;
    layout[3].focus();
    layout[3].className = "foo";
    TVJS.DirectionalNavigation.focusableSelectors.push(".foo");
    assert.equal(document.activeElement, layout[3]);

    assert.notOk(layout[1].tabIndex);
    target = TVJS.DirectionalNavigation.findNextFocusElement("up");
    assert.equal(layout[1].tabIndex, 0);

    testDone(rootContainer);
});

QUnit.test("If an element in the focusable selectors list has a tabIndex of -1, it doesn't get unset to something else.", function (assert) {
    var rootContainer = testStart();
    var layout = createCrossLayout(rootContainer, "div");
    layout[3].tabIndex = 0;
    layout[3].focus();
    layout[3].className = "foo";
    TVJS.DirectionalNavigation.focusableSelectors.push(".foo");
    assert.equal(document.activeElement, layout[3]);

    layout[1].tabIndex = -1;
    target = TVJS.DirectionalNavigation.findNextFocusElement("up");
    assert.equal(layout[1].tabIndex, -1);

    testDone(rootContainer);
});