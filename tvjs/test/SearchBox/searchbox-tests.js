// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.
(function () {
    "use strict";

    QUnit.test("Clicking the symbols/letters toggle, toggles between letters and symbols", function (assert) {
        var sb = new TVJS.SearchBox();
        var fakeElement = document.createElement("div");
        fakeElement.id = "togglecharacters";
        assert.ok(sb._isShowingLettersNotSymbols);
        sb._keyDownHandler({
            key: "GamepadA",
            target: fakeElement
        });
        assert.notOk(sb._isShowingLettersNotSymbols);
        sb._keyDownHandler({
            key: "GamepadA",
            target: fakeElement
        });
        assert.ok(sb._isShowingLettersNotSymbols);
    });
    QUnit.test("When key is pressed, then shows up in the search query", function (assert) {
        var sb = new TVJS.SearchBox();
        assert.equal(sb.queryText, "");
        var character = sb.element.querySelector(".tv-searchbox-alphabetical .tv-searchbox-character");
        var characterValue = character.id.replace("__ms_searchboxletter_", "").toLowerCase();
        sb._keyDownHandler({
            key: "GamepadA",
            target: character
        });
        assert.equal(sb.queryText, characterValue);
    });
    QUnit.test("When key is pressed, querychanged event fires", function (assert) {
        var done = assert.async();
        var sb = new TVJS.SearchBox();
        sb.addEventListener("querychanged", function (ev) {
            assert.equal(ev.detail.queryText, characterValue);
            done();
        }, false);
        var character = sb.element.querySelector(".tv-searchbox-alphabetical .tv-searchbox-character");
        var characterValue = character.id.replace("__ms_searchboxletter_", "").toLowerCase();
        sb._keyDownHandler({
            key: "GamepadA",
            target: character
        });
    });
    QUnit.test("Basic instantiation", function (assert) {
        var sb = new TVJS.SearchBox();
        assert.ok(true);
    });
    QUnit.test("Dispose called twice", function (assert) {
        var sb = new TVJS.SearchBox();
        sb.dispose();
        sb.dispose();
        assert.ok(true);
    });
    QUnit.test("When set placeholder property, placeholder text is set", function (assert) {
        var sb = new TVJS.SearchBox();
        sb.placeHolder = "foo";
        assert.equal(sb.placeHolder, "foo");
    });
    QUnit.test("When set queryText, queryText is set", function (assert) {
        var sb = new TVJS.SearchBox();
        sb.queryText = "foo";
        assert.equal(sb.queryText, "foo");
    });
    QUnit.test("When backspace, then character is deleted", function (assert) {
        var sb = new TVJS.SearchBox();
        assert.equal(sb.queryText, "");
        var character = sb.element.querySelector(".tv-searchbox-alphabetical .tv-searchbox-character");
        var characterValue = character.id.replace("__ms_searchboxletter_", "").toLowerCase();
        sb._keyDownHandler({
            key: "GamepadA",
            target: character
        });
        assert.equal(sb.queryText, characterValue);
        var backspace = sb.element.querySelector("#backspace");
        sb._keyDownHandler({
            key: "GamepadA",
            target: backspace
        });
        assert.equal(sb.queryText, "");
    });
    QUnit.test("When space, then space character is added", function (assert) {
        var sb = new TVJS.SearchBox();
        assert.equal(sb.queryText, "");
        var space = sb.element.querySelector("#space");
        sb._keyDownHandler({
            key: "GamepadA",
            target: space
        });
        assert.equal(sb.queryText, " ");
    });
})();