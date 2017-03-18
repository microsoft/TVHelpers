// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.
;(function() {
    "use strict";

    var rootContainer;
    var Keys = xyfocus.Key;

    var highlightStyle = document.createElement("style");
    highlightStyle.innerHTML = "button:focus { background-color: red; }";

    function createAndAppendFocusableElement(x, y, container, textContent, tagName, width, height) {
      if (tagName === undefined) {
            tagName = "button";
      }
      if (width === undefined) {
            width = 150;
      }
      if (height === undefined) {
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
      e.width = width;
      e.height = height;

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
        if (tagName === undefined) {
            tagName = "button";
        }
        if (!container) {
            container = document.createElement("div");
            container.style.position = "absolute";
            container.style.width = container.style.height = "600px";
        }
        return [
            container,
            createAndAppendFocusableElement(250, 50, container, "1", tagName),
            createAndAppendFocusableElement(50, 250, container, "2", tagName),
            createAndAppendFocusableElement(250, 250, container, "3", tagName),
            createAndAppendFocusableElement(450, 250, container, "4", tagName),
            createAndAppendFocusableElement(250, 450, container, "5", tagName)
        ];
    }

    function spinWait(evaluator) {
        return new Promise(function (c) {
            var count = 0;
            var handle = setInterval(function () {
                if (++count === 100) {
                    clearInterval(handle);
                    evaluator(); // Step into this call to debug the evaluator
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
        return spinWait(function () { return w.document.activeElement === e; });
    }

    /*------------------------------------------------------------------------*/

    QUnit.testStart(function() {
        document.body.appendChild(highlightStyle);
        rootContainer = document.createElement("div");
        rootContainer.style.position = "relative";
        document.body.appendChild(rootContainer);
        xyfocus.focusRoot = rootContainer;
    });

    QUnit.testDone(function() {
        document.body.removeChild(highlightStyle);
        rootContainer.parentElement.removeChild(rootContainer);
        rootContainer = xyfocus.focusRoot = null;

        // Clear event listeners
        xyfocus._listeners.focuschanging = [];
        xyfocus._listeners.focuschanged = [];
    });

    /*------------------------------------------------------------------------*/

    QUnit.module("xyfocus.findNextFocusElement");

    QUnit.test("should find focus elements by direction", function(assert) {
        assert.expect(5);

        var layout = createCrossLayout(rootContainer);

        layout[3].focus();
        assert.strictEqual(layout[3], document.activeElement);

        var target = xyfocus.findNextFocusElement("left");
        assert.strictEqual(layout[2], target);

        target = xyfocus.findNextFocusElement("right");
        assert.strictEqual(layout[4], target);

        target = xyfocus.findNextFocusElement("up");
        assert.strictEqual(layout[1], target);

        target = xyfocus.findNextFocusElement("down");
        assert.strictEqual(layout[5], target);
    });

    QUnit.test("should work with an unfocusable reference element and initial focus", function(assert) {
        assert.expect(1);

        var layout = [
            rootContainer,
            createAndAppendFocusableElement(50, 50, rootContainer, "1", "div", 150, 150),
            createAndAppendFocusableElement(200, 50, rootContainer, "2", "button", 150, 150),
        ];

        layout[1].setAttribute("tabIndex", "");
        layout[2].focus();

        var target = xyfocus.findNextFocusElement("right", { referenceElement: layout[1] });
        assert.strictEqual(layout[2], target);
    });

    QUnit.test("should work with a reference element", function(assert) {
        assert.expect(4);

        var layout = createCrossLayout(rootContainer);

        var target = xyfocus.findNextFocusElement("left", { referenceElement: layout[4] });
        assert.strictEqual(layout[3], target);

        target = xyfocus.findNextFocusElement("right", { referenceElement: layout[2] });
        assert.strictEqual(layout[3], target);

        target = xyfocus.findNextFocusElement("up", { referenceElement: layout[5] });
        assert.strictEqual(layout[3], target);

        target = xyfocus.findNextFocusElement("down", { referenceElement: layout[1] });
        assert.strictEqual(layout[3], target);
    });

    QUnit.test("should work with no initial focus", function(assert) {
        assert.expect(2);

        var layout = createCrossLayout(rootContainer);

        document.body.focus();
        assert.strictEqual(document.body, document.activeElement);

        var target = xyfocus.findNextFocusElement("right");
        assert.ok(target);
    });

    /*------------------------------------------------------------------------*/

    QUnit.module("xyfocus.moveFocus");

    QUnit.skip("should move focus by direction", function(assert) {
        assert.expect(9);

        var layout = createCrossLayout(rootContainer);

        layout[3].focus();
        assert.strictEqual(layout[3], document.activeElement);

        var target = xyfocus.moveFocus("left");
        assert.strictEqual(document.activeElement, target);
        assert.strictEqual(layout[2], document.activeElement);

        target = xyfocus.moveFocus("right");
        assert.strictEqual(document.activeElement, target);
        assert.strictEqual(layout[3], document.activeElement);

        target = xyfocus.moveFocus("up");
        assert.strictEqual(document.activeElement, target);
        assert.strictEqual(layout[1], document.activeElement);

        target = xyfocus.moveFocus("down");
        assert.strictEqual(document.activeElement, target);
        assert.strictEqual(layout[3], document.activeElement);
    });

    QUnit.skip("should respect focus root settings", function(assert) {
        assert.expect(7);

        var left = createCrossLayout();
        var right = createCrossLayout();

        right[0].style.top = "0px";
        right[0].style.left = "700px";

        rootContainer.appendChild(left[0]);
        rootContainer.appendChild(right[0]);

        left[3].focus();
        assert.strictEqual(left[3], document.activeElement);

        xyfocus.moveFocus("right");
        assert.strictEqual(left[4], document.activeElement);

        // Moving right should NOT move out of the left container
        var target = xyfocus.moveFocus("right", { focusRoot: left[0] });
        assert.strictEqual(left[4], document.activeElement);
        assert.notOk(target);

        // Try the same as above using global focus root settings
        xyfocus.focusRoot = left[0];
        target = xyfocus.moveFocus("right");
        assert.strictEqual(left[4], document.activeElement);
        assert.notOk(target);

        // Focus should move across containers w/o focus root settings
        xyfocus.focusRoot = null;
        target = xyfocus.moveFocus("right");
        assert.strictEqual(right[2], document.activeElement);
    });

    /*------------------------------------------------------------------------*/

    QUnit.module("focus history");

    QUnit.skip("should track history by direction", function(assert) {
        assert.expect(5);

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
        assert.strictEqual(layout[3], document.activeElement);

        xyfocus._xyfocus("left");
        assert.strictEqual(layout[1], document.activeElement);

        xyfocus._xyfocus("down");
        assert.strictEqual(layout[2], document.activeElement);

        // Move focus right from 2, then left and we should end up at 2 again
        xyfocus._xyfocus("right");
        assert.strictEqual(layout[3], document.activeElement);

        xyfocus._xyfocus("left");
        assert.strictEqual(layout[2], document.activeElement);
    });

    /*------------------------------------------------------------------------*/

    QUnit.module("iframe");

    QUnit.skip("should work with an uunfocusable body", function(assert) {
        assert.expect(2);

        var done = assert.async();

        /*
            [BUTTON] [IFRAME] [BUTTON]
        */
        var leftButton = createAndAppendFocusableElement(0, 0, rootContainer, null, "button", 200, 200);
        var iframeEl = createAndAppendFocusableElement(210, 0, rootContainer, null, "iframe", 200, 200);
        iframeEl.src = "iframe.html";

        var iframeWin = iframeEl.contentWindow;
        var rightButton = createAndAppendFocusableElement(420, 0, rootContainer, null, "button", 200, 200);

        window.addEventListener("message", function ready(e) {
            // The first crossframe message indicates that the iframe has loaded.
            window.removeEventListener("message", ready);

            // Make the body inside the iframe unfocusable
            iframeWin.document.body.tabIndex = -1;
            leftButton.focus();
            assert.strictEqual(leftButton, document.activeElement);

            // Going right from the left button should focus the iframe but the iframe should immediately
            // signal back a dFocusExit to the right which gets us to the expected right button.
            xyfocus._xyfocus("right");
            assert.strictEqual(iframeEl, document.activeElement);
            waitForFocus(window, rightButton).then(function() { done() }).catch(done);
        });
    });

    /*------------------------------------------------------------------------*/

    QUnit.config.reorder = false;
    QUnit.config.hidepassed = true;

}.call(this));
