// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.
(function () {
    "use strict";

    var _KEY_GAMEPAD_DPAD_LEFT = 205,
        _KEY_GAMEPAD_DPAD_RIGHT = 206;

    QUnit.test("ScrollViewer instantiated", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        assert.ok(true);
    });
    QUnit.test("Dispose ScrollViewer", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer.dispose();
        assert.ok(true);
    });
    QUnit.test("Dispose ScrollViewer twice", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer.dispose();
        scrollViewer.dispose();
        assert.ok(true);
    });
    QUnit.test("When instantiated it is focusable", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        assert.ok(scrollViewer._scrollingContainer.tabIndex >= 0);
    });
    QUnit.test("Can be instantiated with no child elements", function (assert) {
        var element = document.createElement("div");
        element.innerHTML = '';
        var scrollViewer = new TVJS.ScrollViewer(element);
        assert.ok(true);
    });
    QUnit.test("Can be instantiated with 1 child element", function (assert) {
        var element = document.createElement("div");
        element.innerHTML = '<div id="child">Hello</div>';
        var scrollViewer = new TVJS.ScrollViewer(element);
        assert.ok(scrollViewer.element.contains(scrollViewer.element.querySelector("#child")));
    });
    QUnit.test("Can be instantiated with multiple child elements", function (assert) {
        var element = document.createElement("div");
        element.innerHTML = '<div id="child1">Hello 1</div>' + 
                            '<div id="child2">Hello 2</div>';
        var scrollViewer = new TVJS.ScrollViewer(element);
        assert.ok(scrollViewer.element.contains(scrollViewer.element.querySelector("#child1")));
        assert.ok(scrollViewer.element.contains(scrollViewer.element.querySelector("#child2")));
    });
    QUnit.test("Can be instantiated with text", function (assert) {
        var element = document.createElement("div");
        var text = "Hello";
        element.textContent = text;
        var scrollViewer = new TVJS.ScrollViewer(element);
        assert.ok(scrollViewer.element.textContent.match(/Hello/g));
    });
    QUnit.test("With deeply nested scrollable contents, the ScrollViewer contains the correct visuals", function (assert) {
        var done = assert.async();
        var element = document.createElement("div");
        document.body.appendChild(element);
        var text = "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" + 
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string" +
            "Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string, Long string";
        element.textContent = text;
        var scrollViewer = new TVJS.ScrollViewer(element);
        scrollViewer.element.style.height = "50px";
        scrollViewer.element.style.width = "50px";
        scrollViewer.refresh();
        setTimeout(function () {
            assert.ok(scrollViewer.element.querySelector(".tv-scrollable-down"));
            element.parentNode.removeChild(element);
            done();
        }, 500);
    });
    QUnit.test("Set scrollMode to text", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer.scrollMode = TVJS.ScrollMode.text;
        assert.ok(scrollViewer._scrollMode === TVJS.ScrollMode.text);
    });
    QUnit.test("Set scrollMode to nonModalText", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer.scrollMode = TVJS.ScrollMode.nonModalText;
        assert.ok(scrollViewer._scrollMode === TVJS.ScrollMode.nonModalText);
    });
    QUnit.test("Set scrollMode to list", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer.scrollMode = TVJS.ScrollMode.list;
        assert.ok(scrollViewer._scrollMode === TVJS.ScrollMode.list);
    });
    QUnit.test("Test entering active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer._setActive();
        assert.ok(scrollViewer._isActive());
        assert.notOk(TVJS.DirectionalNavigation.enabled);
    });
    QUnit.test("Test exiting active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        scrollViewer._setActive();
        assert.ok(scrollViewer._isActive());
        scrollViewer._setInactive();
        assert.notOk(scrollViewer._isActive());
        assert.ok(TVJS.DirectionalNavigation.enabled);
    });
    QUnit.test("When scrollMode is nonModalText and ScrollViewer has focus, it is in active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        document.body.appendChild(scrollViewer.element);
        scrollViewer.scrollMode = TVJS.ScrollMode.nonModalText;
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.ok(scrollViewer._isActive());
        scrollViewer.element.parentNode.removeChild(scrollViewer.element);
    });
    QUnit.test("When scrollMode is text and ScrollViewer has focus, it is in active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        document.body.appendChild(scrollViewer.element);
        scrollViewer.scrollMode = TVJS.ScrollMode.text;
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.ok(scrollViewer._isActive());
        scrollViewer.element.parentNode.removeChild(scrollViewer.element);
    });
    QUnit.test("When scrollMode is list and ScrollViewer has focus, it is not in active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        document.body.appendChild(scrollViewer.element);
        scrollViewer.scrollMode = TVJS.ScrollMode.list;
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.notOk(scrollViewer._isActive());
        scrollViewer.element.parentNode.removeChild(scrollViewer.element);
    });
    QUnit.test("When focus leaves the ScrollViewer, it is not in active mode", function (assert) {
        var scrollViewer = new TVJS.ScrollViewer();
        document.body.appendChild(scrollViewer.element);
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.ok(scrollViewer._isActive());
        scrollViewer._scrollingContainer.blur();
        assert.notOk(scrollViewer._isActive());
        scrollViewer.element.parentNode.removeChild(scrollViewer.element);
    });
    QUnit.test("When ScrollViewer is in active mode and move left to focusable left element, then ScrollViewer is in active mode", function (assert) {
        var rootContainer = document.createElement("div");
        var scrollViewerElement = document.createElement("div");
        var rightFocus = document.createElement("button");
        rootContainer.style.display = "flex";
        rootContainer.style.flexDirection = "row";
        rootContainer.appendChild(rightFocus);
        rootContainer.appendChild(scrollViewerElement);
        document.body.appendChild(rootContainer);
        var scrollViewer = new TVJS.ScrollViewer(scrollViewerElement);
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.ok(scrollViewer._isActive());
        scrollViewer._handleKeyDown({
            keyCode: _KEY_GAMEPAD_DPAD_LEFT
        });
        assert.ok(scrollViewer._isActive());
        rootContainer.parentNode.removeChild(rootContainer);
    });
    QUnit.test("When ScrollViewer is in active mode and move right to focusable left element, then ScrollViewer is in active mode", function (assert) {
        var rootContainer = document.createElement("div");
        var scrollViewerElement = document.createElement("div");
        var rightFocus = document.createElement("button");
        rootContainer.style.display = "flex";
        rootContainer.style.flexDirection = "row";
        rootContainer.appendChild(scrollViewerElement);
        rootContainer.appendChild(rightFocus);
        document.body.appendChild(rootContainer);
        var scrollViewer = new TVJS.ScrollViewer(scrollViewerElement);
        scrollViewer._scrollingContainer.focus();
        assert.equal(document.activeElement, scrollViewer._scrollingContainer);
        assert.ok(scrollViewer._isActive());
        scrollViewer._handleKeyDown({
            keyCode: _KEY_GAMEPAD_DPAD_RIGHT
        });
        assert.ok(scrollViewer._isActive());
        rootContainer.parentNode.removeChild(rootContainer);
    });
})();