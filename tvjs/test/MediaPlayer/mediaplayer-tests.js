// Helper functions

// the one downside of using a fake mediaElement is that we can create conditions that aren't actually possible in reality.
// While this is great for testing, calling depose on a test that does this will cause exceptions due to race conditions.
// that's why we create a helper function to safely dispose the mediaPlayer control's resources.
function safeDispose(mediaPlayer, assert) {
    setImmediate(function () {
        try {
            mediaPlayer.dispose();
        } catch (ex) { }
    });
};

function runSetMediaPlayerPropertyCase(propertyName, propertyValue, isExceptionExpected, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var wasExceptionThrown = false;

    try {
        mediaPlayer[propertyName] = propertyValue;
    } catch (exception) {
        wasExceptionThrown = true;
    }

    if (isExceptionExpected) {
        assert.ok(wasExceptionThrown, "Setting mediaPlayer." + propertyName + "to " + propertyValue + " was supposed to throw an exception, but it didn't.");
    } else {
        assert.notOk(wasExceptionThrown, "Setting mediaPlayer." + propertyName + "to " + propertyValue + " was not supposed to throw an exception, but it did.");
        assert.equal(propertyValue, mediaPlayer[propertyName], "mediaPlayer." + propertyName + " was not the expected value.");
    }
    safeDispose(mediaPlayer);
};

function runEnsureLegacyPropertyStillExistsTestCase(legacyPropertyName, newPropertyName, propertyValue, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer[legacyPropertyName] = propertyValue;
    assert.equal(propertyValue, mediaPlayer[legacyPropertyName], "mediaPlayer." + legacyPropertyName + " was not equal to the expected value: " + propertyValue + ", instead value was: " + mediaPlayer[legacyPropertyName]);
    assert.equal(propertyValue, mediaPlayer[newPropertyName], "mediaPlayer." + newPropertyName + " was not equal to the expected value: " + propertyValue + ", instead value was: " + mediaPlayer[newPropertyName]);
    safeDispose(mediaPlayer);
};

function runVerifyMediaCommandExecutedEventTest(mediaCommandEnum, methodName, assert, playbackRate) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaPlayer.castButtonEnabled = true;
    mediaPlayer.castButtonVisible = true;
    mediaPlayer.zoomButtonEnabled = true;
    mediaPlayer.zoomButtonVisible = true;
    mediaElement.playbackRate = playbackRate || 1;
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaPlayer.addEventListener("mediacommandexecuted", function mediaCommandExecuted(ev) {
        mediaPlayer.removeEventListener("mediacommandexecuted", mediaCommandExecuted);
        assert.equal(mediaCommandEnum, ev.detail.mediaCommand);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer[methodName].call(mediaPlayer);
};

function runIsPlayAllowedTestCase(isPlayAllowedValue, isEventExpectedToFire, assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaPlayer.mediaElementAdapter.playAllowed = isPlayAllowedValue;
    mediaElement.addEventListener("play", function play() {
        mediaElement.removeEventListener("play", play);
        if (isEventExpectedToFire) {
            safeDispose(mediaPlayer);
            assert.ok(true);
            done();
        } else {
            assert.ok(false, "Shouldn't be able to play.");
        }
    }, false);
    mediaElement.paused = true;
    mediaPlayer.play();
    if (!isEventExpectedToFire) {
        assert.ok(mediaElement.paused);
        safeDispose(mediaPlayer);
        done();
    }
};

function runIsPauseAllowedTestCase(isPauseAllowedValue, isEventExpectedToFire, assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaPlayer.mediaElementAdapter.pauseAllowed = isPauseAllowedValue;
    mediaElement.addEventListener("pause", function pause() {
        mediaElement.removeEventListener("pause", pause);
        if (isEventExpectedToFire) {
            safeDispose(mediaPlayer);
            assert.ok(true);
            done();
        } else {
            assert.ok(false, "Shouldn't be able to pause.");
        }
    }, false);
    mediaPlayer.pause();
    if (!isEventExpectedToFire) {
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }
};

function runIsSeekAllowedTestCase(isSeekAllowedValue, isEventExpectedToFire, assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaElement.autoplay = true;
    mediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaPlayer.mediaElementAdapter.seekAllowed = isSeekAllowedValue;
    mediaElement.addEventListener("seeked", function seek() {
        mediaElement.removeEventListener("seeked", seek);
        if (isEventExpectedToFire) {
            safeDispose(mediaPlayer);
            assert.ok(true);
            done();
        } else {
            assert.ok(false, "Shouldn't be able to seek.");
        }
    }, false);
    var oldCurrentTime = mediaElement.currentTime;
    mediaPlayer.seek(10);
    if (!isEventExpectedToFire) {
        assert.equal(oldCurrentTime, mediaElement.currentTime);
        safeDispose(mediaPlayer);
        done();
    }
};

function compareArrays(array1, array2) {
    var arraysAreEqual = true;
    for (var propertyName in array1) {
        if (array1[propertyName] !== array2[propertyName]) {
            arraysAreEqual = false;
            break;
        }
    }
    return arraysAreEqual;
};

function runNullMediaElementTestCase(mediaCommand, assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = null;

    try {
        mediaPlayer[mediaCommand].call(mediaPlayer);
    } catch (exception) {
        wasExceptionThrown = true;
    }

    assert.notOk(wasExceptionThrown, mediaCommand + "was called on an invalid media element and an exception was thrown.");
    safeDispose(mediaPlayer);
};

function runNewMediaElementTestCase(mediaCommand, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    try {
        mediaPlayer[mediaCommand].call(mediaPlayer);
    } catch (exception) {
        assert.ok(false, "When calling the '" + mediaCommand + "' method an exception was thrown when it should not have been.");
        wasExceptionThrown = true;
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
};

function runNullMediaElementSrcTestCase(mediaCommand, wasExceptionExpected, assert) {
    var done = assert.async();
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = null;

    try {
        mediaPlayer[mediaCommand].call(mediaPlayer);
    } catch (exception) {
        wasExceptionThrown = true;
    }

    assert.notOk(wasExceptionThrown, mediaCommand + "was called on an invalid media element and an exception was thrown.");
    safeDispose(mediaPlayer);
    done();
};

function runFastForwardTestCase(initialPlaybackRate, expectedPlaybackRateAfterFastForward, startPaused, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    if (!startPaused) {
        mockMediaElement.autoplay = true;
    }
    mockMediaElement.src = "notnull";
    mockMediaElement.playbackRate = initialPlaybackRate;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.fastForward();
    assert.ok(expectedPlaybackRateAfterFastForward, mockMediaElement.playbackRate);
    assert.ok(expectedPlaybackRateAfterFastForward, mediaPlayer.targetPlaybackRate);
    safeDispose(mediaPlayer);
};

function runRewindTestCase(initialPlaybackRate, expectedPlaybackRateAfterRewind, startPaused, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    if (!startPaused) {
        mockMediaElement.autoplay = true;
    }
    mockMediaElement.src = "notnull";
    mockMediaElement.playbackRate = initialPlaybackRate;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.rewind();
    assert.ok(expectedPlaybackRateAfterRewind, mockMediaElement.playbackRate);
    assert.ok(expectedPlaybackRateAfterRewind, mediaPlayer.targetPlaybackRate);
    safeDispose(mediaPlayer);
};

function runExitFastForwardOrRewindTest(mediaCommand, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.fastForward();
    mediaPlayer[mediaCommand].call(mediaPlayer);
    assert.notOk(mockMediaElement.paused);
    assert.equal(1, mockMediaElement.playbackRate);
    safeDispose(mediaPlayer);
};

function runPlayPauseToggleIconTest(mediaCommand, icon, isPausedInitially, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    if (!isPausedInitially) {
        mockMediaElement.autoplay = true;
    }
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer[mediaCommand].call(mediaPlayer);
    if (icon === "pauseicon") {
        assert.notOk(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-playicon"));
        assert.ok(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-pauseicon"));
    } else {
        assert.notOk(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-pauseicon"));
        assert.ok(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-playicon"));
    }
    safeDispose(mediaPlayer);
};

function runGetButtonBySelectorTestCase(buttonSelector, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var button = mediaPlayer.element.querySelector(buttonSelector);
    assert.ok(button, "UI element could not be retrieved: " + buttonSelector);
    safeDispose(mediaPlayer);
};

function runNoTransportBarButtonsTestCase(mediaCommand, parameter, assert) {
    var mediaPlayer = new TVJS.MediaPlayer();

    // remove all the buttons
    var buttons = mediaPlayer.element.querySelectorAll(".tv-mediaplayer-transportcontrols button");
    for (var i = buttons.length - 1; i >= 0; i--) {
        buttons[i].parentNode.removeChild(buttons[i]);
    }

    try {
        mediaPlayer[mediaCommand].call(mediaPlayer, parameter);
    } catch (ex) {
        assert.ok(false, "An exception was thrown trying to call mediaPlayer." + mediaCommand + "().");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
};

QUnit.module("Properties");
QUnit.test("When showControls then controls visible is true", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer._skipAnimations = true;
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement.src = "notnullstring";
    mediaPlayer.addEventListener("aftershowcontrols", function aftershowcontrols() {
        mediaPlayer.removeEventListener("aftershowcontrols", aftershowcontrols);
        assert.ok(mediaPlayer.isControlsVisible);
        assert.ok(mediaPlayer.controlsVisible);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.showControls();
});
QUnit.test("When hideControls then controls visible is false", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer._skipAnimations = true;
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement.src = "notnullstring";
    mediaPlayer.addEventListener("aftershowcontrols", function aftershowcontrols() {
        mediaPlayer.removeEventListener("aftershowcontrols", aftershowcontrols);
        mediaPlayer.addEventListener("afterhidecontrols", function afterhidecontrols() {
            assert.notOk(mediaPlayer.isControlsVisible);
            assert.notOk(mediaPlayer.controlsVisible);
            mediaPlayer.removeEventListener("afterhidecontrols", afterhidecontrols);
            safeDispose(mediaPlayer);
            done();
        }, false);
        mediaPlayer.hideControls();
    }, false);
    mediaPlayer.showControls();
});

QUnit.test("When set thumbnailEnabled to true then thumbnailEnabled is true", function (assert) {
    runSetMediaPlayerPropertyCase("thumbnailEnabled", true, false, assert);
});
QUnit.test("When set thumbnailEnabled to false then thumbnailEnabled is false", function (assert) {
    runSetMediaPlayerPropertyCase("thumbnailEnabled", false, false, assert);
});

QUnit.test("When endTime not set then endTime is duration", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaElement.src = "notnullstring";
    mediaElement.duration = 10;
    assert.equal(mediaPlayer.endTime, mediaElement.duration);
    safeDispose(mediaPlayer);
});

QUnit.module("endTime");
QUnit.test("When set endTime to 10 then endTime is 10", function (assert) {
    runSetMediaPlayerPropertyCase("endTime", 10, false, assert);
});
QUnit.test("When set endTime then cached totalTime gets updated", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mediaElement;
    mediaElement.src = "notnullstring";
    mediaElement.duration = 10;
    assert.equal(mediaElement.duration, mediaPlayer._totalTime);
    mediaElement.endTime = 8;
    assert.equal(mediaPlayer.endTime, mediaPlayer._totalTime);
    safeDispose(mediaPlayer);
});

QUnit.module("Custom buttons");
QUnit.test("When add custom buttons declaratively then new buttons are added", function (assert) {
    var testPassed = false;
    var mediaPlayerDiv = document.createElement("div");
    mediaPlayerDiv.innerHTML = '<button id="custom1"></button>';
    var mediaPlayer = new TVJS.MediaPlayer(mediaPlayerDiv);
    for (var i = 0, len = mediaPlayer.commands.length; i < len; i++) {
        if (mediaPlayer.commands.getAt(i).id === "custom1") {
            testPassed = true;
        }
    }
    assert.ok(testPassed, "We added a custom button, but that button did not get added to the mediaPlayer's commands collection.");
    safeDispose(mediaPlayer);
});
QUnit.test("When add custom buttons programatically then new buttons are added", function (assert) {
    var testPassed = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    var customButton = document.createElement("button");
    customButton.id = "custom1";
    mediaPlayer.commands.push(customButton);
    for (var i = 0, len = mediaPlayer.commands.length; i < len; i++) {
        if (mediaPlayer.commands.getAt(i).id === "custom1") {
            testPassed = true;
        }
    }
    assert.ok(testPassed, "We added a custom button, but that button did not get added to the mediaPlayer's commands collection.");
    safeDispose(mediaPlayer);
});

QUnit.module("Compact mode");
QUnit.test("When set compact to true then compact updates", function (assert) {
    runSetMediaPlayerPropertyCase("compact", true, false, assert);
});
QUnit.test("When set compact to false then compact updates", function (assert) {
    runSetMediaPlayerPropertyCase("compact", false, false, assert);
});
QUnit.test("When set compact to true then mediaPlayer has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.compact = true;
    assert.ok(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-singlerow"), "MediaPlayer does not have the 'tv-mediaplayer-singlerow' class.");
    assert.notOk(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-doublerow"), "MediaPlayer does not have the 'tv-mediaplayer-doublerow' class.");
    if (document.body.querySelector(".tv-toolbar-overflowarea")) {
        assert.notOk(TVJS.Utilities.hasClass(document.body.querySelector(".tv-toolbar-overflowarea"), "tv-mediaplayer-doublerow"), "MediaPlayer does not have the 'tv-mediaplayer-doublerow' class.");
    }
    safeDispose(mediaPlayer);
});
QUnit.test("When set compact to false then mediaPlayer has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.compact = false;
    assert.notOk(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-singlerow"), "MediaPlayer does not have the 'tv-mediaplayer-singlerow' class.");
    assert.ok(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-doublerow"), "MediaPlayer does not have the 'tv-mediaplayer-doublerow' class.");
    if (document.body.querySelector(".tv-toolbar-overflowarea")) {
        assert.ok(TVJS.Utilities.hasClass(document.body.querySelector(".tv-toolbar-overflowarea"), "tv-mediaplayer-doublerow"), "MediaPlayer does not have the 'tv-mediaplayer-doublerow' class.");
    }
    safeDispose(mediaPlayer);
});

QUnit.module("Full screen mode");
QUnit.test("When set fullscreen to true then fullscreen updates", function (assert) {
    runSetMediaPlayerPropertyCase("fullscreen", true, false, assert);
});
QUnit.test("When set fullscreen to false then fullscreen updates", function (assert) {
    runSetMediaPlayerPropertyCase("fullscreen", false, false, assert);
});
QUnit.test("When set is fullscreenLegacy property name then current property name updates value", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.isFullScreen = true;
    assert.ok(mediaPlayer.fullScreen);
    mediaPlayer.isFullScreen = false;
    assert.notOk(mediaPlayer.fullScreen);
    safeDispose(mediaPlayer);
});
QUnit.test("When set fullscreen to true then has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.fullScreen = true;
    assert.ok(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-fullscreen"), "MediaPlayer does not have the 'tv-mediaplayer-fullscreen' class.");
    assert.notOk(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-focusable"), "MediaPlayer does not have the 'tv-focusable' class.");
    safeDispose(mediaPlayer);
});
QUnit.test("When set fullscreen to false then has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.fullScreen = false;
    assert.notOk(TVJS.Utilities.hasClass(mediaPlayer.element, "tv-mediaplayer-fullscreen"), "MediaPlayer has the 'tv-mediaplayer-fullscreen' class.");
    safeDispose(mediaPlayer);
});
QUnit.test("When set fullscreen to true then toggle fullscreen Icon updates", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.fullScreen = true;
    assert.notOk(mediaPlayer.element.querySelector(".tv-mediaplayer-fullscreenbutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-fullscreenicon"), "fullscreen toggle button icon is incorrect.");
    safeDispose(mediaPlayer);
});
QUnit.test("When set fullscreen to false then toggle fullscreen Icon updates", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.fullScreen = false;
    assert.ok(mediaPlayer.element.querySelector(".tv-mediaplayer-fullscreenbutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-fullscreenicon"), "fullscreen toggle button icon is incorrect.");
    safeDispose(mediaPlayer);
});

QUnit.module("thumbnailEnabled");
QUnit.test("When set thumbnailEnabled to true then property updates", function (assert) {
    runSetMediaPlayerPropertyCase("thumbnailEnabled", true, false, assert);
});
QUnit.test("When set thumbnailEnabled to false then property updates", function (assert) {
    runSetMediaPlayerPropertyCase("thumbnailEnabled", false, false, assert);
});
QUnit.test("When set thumbnailEnabled to true then has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.thumbnailEnabled = true;
    assert.ok(TVJS.Utilities.hasClass(mediaPlayer._timeline, "tv-mediaplayer-thumbnailmode"), "Does not have the correct classes.");
    safeDispose(mediaPlayer);
});
QUnit.test("When set thumbnailEnabled to false then has correct classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.thumbnailEnabled = false;
    assert.notOk(TVJS.Utilities.hasClass(mediaPlayer._timeline, "tv-mediaplayer-thumbnailmode"), "Does not have the correct classes.");
    safeDispose(mediaPlayer);
});

QUnit.test("When set is thumbnailEnabledLegacy protperty name then current property name updates value", function (assert) {
    runEnsureLegacyPropertyStillExistsTestCase("isThumbnailEnabled", "thumbnailEnabled", true, assert);
});

QUnit.module("Markers");
QUnit.test("When addMarker then markers contains new marker and new marker properties are correct", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var newMarkerTime = 15;
    mediaPlayer.addMarker(newMarkerTime, TVJS.MarkerType.chapter, "Hello World!", "tv-displaynone");

    // Search for the marker we just added
    var markers = mediaPlayer._markers;
    var foundMarker = false;
    for (var i = 0; i < markers.length; i++) {
        if (markers[i].time === newMarkerTime) {
            foundMarker = true;
            assert.equal(newMarkerTime, markers[i].time, "The marker time did not match.");
            assert.equal(TVJS.MarkerType.chapter, markers[i].markerType, "The marker type did not match.");
            assert.equal("Hello World!", markers[i].data, "The marker data field did not match.");
            assert.equal("tv-displaynone", markers[i].extraClass, "The marker extraClass field did not match.");
        }
    }

    if (!foundMarker) {
        assert.ok(false, "Marker array does not contain the new marker.");
    }
    safeDispose(mediaPlayer);
});

QUnit.test("When add marker and type is chapter and extraClass is not specified then markers contains new marker and extraClass is default chapter marker extraClass", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var newMarkerTime = 15;
    mediaPlayer.addMarker(newMarkerTime, TVJS.MarkerType.chapter, "Hello World!");
    var markers = mediaPlayer._markers;
    var foundMarker = false;

    for (var i = 0; i < markers.length; i++) {
        if (markers[i].time === newMarkerTime) {
            foundMarker = true;
            assert.equal("tv-mediaplayer-chaptermarker", markers[i].extraClass, "The marker extraClass field did not have the default chapter marker class.");
        }
    }

    if (!foundMarker) {
        assert.ok(false, "Marker array does not contain the new marker.");
    }
    safeDispose(mediaPlayer);
});

QUnit.test("Given existing marker at 1 second when add new marker at 1 second then markers contain the new marker and the old one is overriden", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.addMarker(1, TVJS.MarkerType.chapter, "Marker 1");
    mediaPlayer.addMarker(1, TVJS.MarkerType.chapter, "Marker 2");
    assert.notEqual(2, mediaPlayer._markers.length, "The old marker did not get removed.");
    assert.equal("Marker 2", mediaPlayer._markers[0].data, "The old marker was not overriden.");
});

QUnit.test("Given valid marker at point 5 seconds when currentTime reaches point 5 seconds then marker reached contains correct properties", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 0.5;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        assert.equal(expectedTime, ev.detail.time, "Expected eventObj.time to be '" + expectedTime + "', but instead was: " + ev.detail.time + ".");
        assert.equal(expectedType, ev.detail.markerType, "Expected eventObj.markerType to be'" + expectedType + "', but instead was: " + ev.detail.markerType + ".");
        assert.equal(expectedData, ev.detail.data, "Expected eventObj.data to be '" + expectedData + "', but instead was: " + ev.detail.data + ".");
        assert.equal(expectedExtraClass, ev.detail.extraClass, "Expected eventObj.data to be '" + expectedExtraClass + "', but instead was: " + ev.detail.extraClass + ".");
        done();
        safeDispose(mediaPlayer);
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given marker at 1 second when currentTime reaches 1 second then marker reached event fires", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 1;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        assert.equal(expectedTime, ev.detail.time, "Expected eventObj.time to be '" + expectedTime + "', but instead was: " + ev.detail.time + ".");
        assert.equal(expectedType, ev.detail.markerType, "Expected eventObj.markerType to be'" + expectedType + "', but instead was: " + ev.detail.markerType + ".");
        assert.equal(expectedData, ev.detail.data, "Expected eventObj.data to be '" + expectedData + "', but instead was: " + ev.detail.data + ".");
        assert.equal(expectedExtraClass, ev.detail.extraClass, "Expected eventObj.data to be '" + expectedExtraClass + "', but instead was: " + ev.detail.extraClass + ".");
        safeDispose(mediaPlayer);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given marker at 0 seconds when currentTime reaches 0 seconds then marker reached event fires", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 0;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        assert.equal(expectedTime, ev.detail.time, "Expected eventObj.time to be '" + expectedTime + "', but instead was: " + ev.detail.time + ".");
        assert.equal(expectedType, ev.detail.markerType, "Expected eventObj.markerType to be'" + expectedType + "', but instead was: " + ev.detail.markerType + ".");
        assert.equal(expectedData, ev.detail.data, "Expected eventObj.data to be '" + expectedData + "', but instead was: " + ev.detail.data + ".");
        assert.equal(expectedExtraClass, ev.detail.extraClass, "Expected eventObj.data to be '" + expectedExtraClass + "', but instead was: " + ev.detail.extraClass + ".");
        safeDispose(mediaPlayer);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given marker at end of video when currentTime reaches end of video then marker reached event fires", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;

    var expectedTime = mockMediaElement.duration;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        assert.equal(expectedTime, ev.detail.time, "Expected eventObj.time to be '" + expectedTime + "', but instead was: " + ev.detail.time + ".");
        assert.equal(expectedType, ev.detail.markerType, "Expected eventObj.markerType to be'" + expectedType + "', but instead was: " + ev.detail.markerType + ".");
        assert.equal(expectedData, ev.detail.data, "Expected eventObj.data to be '" + expectedData + "', but instead was: " + ev.detail.data + ".");
        assert.equal(expectedExtraClass, ev.detail.extraClass, "Expected eventObj.data to be '" + expectedExtraClass + "', but instead was: " + ev.detail.extraClass + ".");
        safeDispose(mediaPlayer);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given marker at point 7 seconds when marker is removed and currentTime reaches point 7 seconds then markerreached event does not fire", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 0.7;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.removeMarker(expectedTime);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        assert.ok(false, "markerreached event should not have fired.");
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime;
    mockMediaElement.fireEvent("timeupdate");
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given existing marker when remove marker then markers does not contain removed marker", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.addMarker(11, TVJS.MarkerType.chapter, {});
    var markers = mediaPlayer._markers;
    mediaPlayer.removeMarker(11);
    var markers = mediaPlayer._markers;
    var foundMarker = false;
    for (var i = 0; i < markers.length; i++) {
        if (markers[i].time === 10) {
            foundMarker = true;
        }
    }

    if (foundMarker) {
        assert.ok(false, "Marker array should not contain the old timeline marker, but it does.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given no src set when add marker then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(12, TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on a media Transport control with no source threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and time is not a number then throws an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();

    var threwException = false;
    try {
        mediaPlayer.addMarker("foo", TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (!threwException) {
        assert.ok(false, "Calling addMarker on with a time value of 'foo' did not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker with negative time field then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(-1, TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on with a time value of '-1' threw an exception.");
    }
    assert.ok(true);
});

QUnit.test("When add marker and time is greater than endTime then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(mediaPlayer.endTime + 10, TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on with a time value greater than the endTime of the media threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and time is less than startTime then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(mediaPlayer.startTime - 10, TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on with a time value less than the startTime of the media threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

// add two markers at the same time & it's fine
QUnit.test("When add marker and time is null then throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(null, TVJS.MarkerType.chapter, {});
    } catch (exception) {
        threwException = true;
    }

    if (!threwException) {
        assert.ok(false, "Calling addMarker on with a time value of 'null' did not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and type is not valid Enum then throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(12, "invalidMarkerType", {});
    } catch (exception) {
        threwException = true;
    }

    if (!threwException) {
        assert.ok(false, "Calling addMarker on with a markerType of 'invalidMarkerType' did not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and type is null then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.addMarker(12, null, {});
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on with a markerType of 'null' should not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and type is not specified then defaults to chapter", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    mediaPlayer.addMarker(12);
    assert.equal(TVJS.MarkerType.chapter, mediaPlayer.markers[0].markerType);
    safeDispose(mediaPlayer);
});

QUnit.test("When removeMarker called on non existant marker then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    var threwException = false;
    try {
        mediaPlayer.removeMarker(999);
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling removeMarker on a non-existant marker did not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When add marker and extraClass is null then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = document.createElement("video");
    var threwException = false;
    try {
        mediaPlayer.addMarker(12, TVJS.MarkerType.chapter, {}, null);
    } catch (exception) {
        threwException = true;
    }

    if (threwException) {
        assert.ok(false, "Calling addMarker on with a extraClass of 'null' threw an exception even though it is an optional parameter.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given 3 consecutive markers each sepearted by one second when play then markerreached event fires For all three markers", function (assert) {
    var done = assert.async();
    var expectedTime1 = 10;
    var expectedTime2 = 11;
    var expectedTime3 = 12;

    var marker1Reached = false;
    var marker2Reached = false;
    var marker3Reached = false;

    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;

    var expectedTime = mockMediaElement.duration;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.addMarker(expectedTime1);
    mediaPlayer.addMarker(expectedTime2);
    mediaPlayer.addMarker(expectedTime3);

    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        if (ev.detail.time === expectedTime1) {
            marker1Reached = true;
        } else if (ev.detail.time === expectedTime2) {
            marker2Reached = true;
        } else if (ev.detail.time === expectedTime3) {
            marker3Reached = true;
        }
    }, false);

    mockMediaElement.addEventListener("timeupdate", function timeupdate() {
        if (mockMediaElement.currentTime === expectedTime3 + 1 &&
            marker1Reached &&
            marker2Reached &&
            marker3Reached) {
            mockMediaElement.removeEventListener("timeupdate", timeupdate);
            safeDispose(mediaPlayer);
            assert.ok(true);
            done();
        }
    });

    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime1;
    mockMediaElement.fireEvent("timeupdate");
    mockMediaElement.currentTime = expectedTime2;
    mockMediaElement.fireEvent("timeupdate");
    mockMediaElement.currentTime = expectedTime3;
    mockMediaElement.fireEvent("timeupdate");
    mockMediaElement.currentTime = expectedTime3 + 1;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given existing marker when seek past marker then markerreached event does not fire", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 1;
    var expectedType = TVJS.MarkerType.chapter;
    var expectedData = "Some data";
    var expectedExtraClass = "tv-displaynone";

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime, expectedType, expectedData, expectedExtraClass);
    mediaPlayer.removeMarker(expectedTime);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        assert.ok(false, "markerreached event should not have fired.");
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime + 3;
    mockMediaElement.fireEvent("timeupdate");
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given existing marker when seek to very close before the marker then markerreached event fires", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    var expectedTime = 1;

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(expectedTime);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime + 0.1;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given existing marker when seek very close before marker then markerreached event fires", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    var expectedTime = 1;
    mediaPlayer.addMarker(expectedTime);
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mockMediaElement.currentTime = expectedTime - 0.1;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given existing markers when video emptied event then old markers are removed", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();

    mediaPlayer.addMarker(1);
    mediaPlayer.addMarker(2);
    mediaPlayer.addMarker(3);

    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.fireEvent("ended");

    assert.equal(0, mediaPlayer.markers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given existing markers but media not loaded when mediaElementAdapter changes then old markers not removed", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;

    mediaPlayer.addMarker(0, TVJS.MarkerType.chapter, {});
    mediaPlayer.addMarker(1, TVJS.MarkerType.chapter, {});
    mediaPlayer.addMarker(2, TVJS.MarkerType.chapter, {});
    mediaPlayer.mediaElementAdapter = {};
    assert.ok(mediaPlayer.markers.length > 0);
    safeDispose(mediaPlayer);
});

QUnit.test("Given default chapter markers when currentTime passes defaultMarker then markerreached event does not fire", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.addEventListener("markerreached", function markerReached(ev) {
        mediaPlayer.removeEventListener("markerreached", markerReached);
        assert.ok(false, "markerreached event should not fire for default chapter markers");
    }, false);
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = mediaPlayer._MINIMUM_MEDIA_LENGTH_FOR_DEFAULT_MARKERS + 1;
    mockMediaElement.src = "notnullstring";
    mockMediaElement.currentTime = mediaPlayer._defaultChapterMarkers[0].time;
    mockMediaElement.fireEvent("timeupdate");
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given default chapter markers when chapter marker added then default chapter markers are cleared", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = mediaPlayer._MINIMUM_MEDIA_LENGTH_FOR_DEFAULT_MARKERS + 1;
    mockMediaElement.src = "notnullstring";
    mediaPlayer.addMarker(10, TVJS.MarkerType.chapter);
    assert.notOk(mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given media under 1 minute when video is loaded then default chapter markers are not added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = mediaPlayer._MINIMUM_MEDIA_LENGTH_FOR_DEFAULT_MARKERS - 1;
    mockMediaElement.src = "notnullstring";
    assert.notOk(mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("When add marker called and markers are inserted out of order then markers array is sorted", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.addMarker(20, TVJS.MarkerType.chapter, {});
    mediaPlayer.addMarker(10, TVJS.MarkerType.chapter, {});
    var markers = mediaPlayer._markers;
    assert.equal(markers[0].time, 10, "The markers array was not sorted properly.");
    assert.equal(markers[1].time, 20, "The markers array was not sorted properly.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given 3 markers within 200 miliseconds when currentTime is close to markers the all markerreached events fire", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;

    var marker1Time = 3.21;
    var marker2Time = 3.2;
    var marker3Time = 3.23;

    var marker1Fired = false;
    var marker2Fired = false;
    var marker3Fired = false;

    mediaPlayer.addMarker(marker1Time);
    mediaPlayer.addMarker(marker2Time);
    mediaPlayer.addMarker(marker3Time);

    mediaPlayer.addEventListener("markerreached", function handleMarkerReached(ev) {
        if (ev.detail.time === marker1Time) {
            marker1Fired = true;
        } else if (ev.detail.time === marker2Time) {
            marker2Fired = true;
        } else if (ev.detail.time === marker3Time) {
            marker3Fired = true;
        }
    }, false);
    mockMediaElement.addEventListener("timeupdate", function timeupdate() {
        if (mockMediaElement.currentTime === marker3Time + 1 &&
            marker1Fired &&
            marker2Fired &&
            marker3Fired) {
            mockMediaElement.removeEventListener("timeupdate", timeupdate);
            safeDispose(mediaPlayer);
            assert.ok(true);
            done();
        }
    }, false);
    mockMediaElement.currentTime = marker2Time;
    mockMediaElement.fireEvent("timeupdate");
    mockMediaElement.currentTime = marker3Time + 1;
    mockMediaElement.fireEvent("timeupdate");
});

QUnit.test("Given existing markers when media loaded then markers are gone", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "source";

    // add some markers
    mediaPlayer.addMarker(1);
    mediaPlayer.addMarker(2);
    mediaPlayer.addMarker(3);

    // set a new source
    mockMediaElement.fireEvent("emptied");
    mockMediaElement.src = "newsource";
    assert.notOk(mediaPlayer.markers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given no marker at the specified time when remove marker and no marker is removed then no exception is thrown", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    var wasExceptionThrown = false;
    try {
        mediaPlayer.removeMarker(3);
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown);
    safeDispose(mediaPlayer);
});

QUnit.test("When set markers with valid markers collection then markers are added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var myMarkers = [
        { time: 1, type: TVJS.MarkerType.chapter, data: {} },
        { time: 2, type: TVJS.MarkerType.custom, data: {} },
        { time: 3, type: TVJS.MarkerType.advertsing, data: {} }
    ];
    mediaPlayer.markers = myMarkers;
    assert.ok(compareArrays(myMarkers, mediaPlayer.markers), "MediaPlayer.markers does not contain the same values we added to it.");
    safeDispose(mediaPlayer);
});

QUnit.test("When set markers with invalid first marker then exception is not thrown", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var wasExceptionThrown = false;

    var myMarkers = [
        { invalidFieldName: "invalid" },
        { bar: 1, foo: TVJS.MarkerType.chapter, data: {} },
        { time: 2, type: TVJS.MarkerType.custom }
    ];

    try {
        mediaPlayer.markers = myMarkers;
    } catch (exception) {
        wasExceptionThrown = true;
    }

    assert.notOk(wasExceptionThrown, "Adding a markers collection with an invalid 1st marker throws an exception.");
    safeDispose(mediaPlayer);
});

QUnit.test("When set markers with empty array then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var wasExceptionThrown = false;
    var myMarkers = [];
    try {
        mediaPlayer.markers = myMarkers;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown, "Setting the markers collection to an empty array did not throw an exception, but it should have.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given existing markers when set markers with empty array then previous markers are cleared", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.markers = [
        { time: 1, type: TVJS.MarkerType.chapter, data: {} },
        { time: 2, type: TVJS.MarkerType.custom, data: {} },
        { time: 3, type: TVJS.MarkerType.advertsing, data: {} }
    ];
    var myMarkers = [];
    mediaPlayer.markers = myMarkers;
    assert.equal(0, mediaPlayer.markers.length, "Setting the markers collection to an empty array did not clear the previous markers when it should have.");
    safeDispose(mediaPlayer);
});

QUnit.test("When set markers set to null then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var wasExceptionThrown = false;
    try {
        mediaPlayer.markers = null;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "Setting the mediaPlayer.markers collection to null did not throw an exception when it should have.");
    safeDispose(mediaPlayer);
});

QUnit.test("When set markers an unsorted marker collection then markers are sorted", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var myUnsortedMarkers = [
        { time: 3, type: TVJS.MarkerType.chapter, data: {} },
        { time: 2, type: TVJS.MarkerType.custom, data: {} },
        { time: 1, type: TVJS.MarkerType.advertisement, data: {} }
    ];
    var mySortedMarkers = myUnsortedMarkers.sort(function (first, next) {
        return first.time - next.time;
    });
    mediaPlayer.markers = myUnsortedMarkers;
    assert.ok(compareArrays(mySortedMarkers, mediaPlayer.markers), "The mediaPlayer.markers collection was not sorted.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given existing markers when set markers then old markers are removed", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.addMarker(1, TVJS.MarkerType.chapter, {});
    mediaPlayer.markers = [];
    assert.equal(0, mediaPlayer.markers.length, "The old markers were not removed.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given markers array is set when video loaded then markers array is persisted", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    var markers = [
        { time: 3, type: TVJS.MarkerType.chapter, data: {} },
        { time: 2, type: TVJS.MarkerType.custom, data: {} },
        { time: 1, type: TVJS.MarkerType.advertisement, data: {} }
    ];
    mediaPlayer.markers = markers;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnullstring";
    assert.ok(compareArrays(markers, mediaPlayer.markers), "The mediaPlayer.markers collection was not persisted.");
    safeDispose(mediaPlayer);
});

QUnit.module("Audio / video tag");
QUnit.test("Given audio tag with controls enabled when mediaPlayer is instantiated then audio tag has no controls", function (assert) {
    var audio = document.createElement("audio");
    audio.controls = true;
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = audio;
    assert.notOk(mediaPlayer.mediaElementAdapter.mediaElement.controls, "Audio tag has controls enabled, but it should not have.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given video tag with controls enabled when mediaPlayer is instantiated then video tag has no controls", function (assert) {
    var video = document.createElement("video");
    video.controls = true;
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = video;
    assert.notOk(mediaPlayer.mediaElementAdapter.mediaElement.controls, "Video tag visible controls when it should not have.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given video tag with loop and end time set when currentTime reaches end time then video starts playing from the beginning", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.loop = true;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnullstring";
    mediaPlayer.endTime = 10;
    mockMediaElement.addEventListener("play", function () {
        assert.equal(0, mockMediaElement.currentTime);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mockMediaElement.currentTime = mediaPlayer.endTime + 1;
});

QUnit.test("Given video tag with autoplay and start time set when video is loaded then video starts playing", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.duration = 10;
    mediaPlayer.startTime = 1;
    // the mediaPlayer will seek to the startTime, but the readyState of the fake video tag needs to be 3 so that the seek will happen.
    mockMediaElement.readyState = 3;
    mockMediaElement.addEventListener("play", function () {
        assert.equal(1, mockMediaElement.currentTime);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mockMediaElement.src = "notnullstring";
});

QUnit.test("Given mediaElement is set to video 1 when mediaElement is set to null then video 1 does not have Css classes added by the mediaPlayer", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var videoTag = document.createElement("video");
    mediaPlayer.mediaElementAdapter.mediaElement = videoTag;
    mediaPlayer.mediaElementAdapter.mediaElement = null;
    assert.notOk(TVJS.Utilities.hasClass(videoTag, "tv-mediaplayer-video"), "CSS classes added by the mediaPlayer are still present on the video tag even though it was swapped out for a new one.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given mediaElement is set to video 1 when mediaElement is set to null then video 1 does not have the event listeners added by the mediaPlayer", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var oldMediaElement = mediaPlayer.mediaElementAdapter.mediaElement;
    mediaPlayer.mediaElementAdapter.mediaElement = null;
    assert.ok(mediaPlayer._mediaEventSubscriptions.length === 0, "The events listeners added by the mediaPlayer as still attached to the old mediaElement, but should have been cleaned up.");
    safeDispose(mediaPlayer);
});

QUnit.test("When mediaElement is set to new mediaElement then new mediaElement has appropriate classes", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var video = document.createElement("video");
    mediaPlayer.mediaElementAdapter.mediaElement = video;
    assert.ok(TVJS.Utilities.hasClass(video, "tv-mediaplayer-video"), "CSS class that was added by the mediaPlayer is not present.");
    safeDispose(mediaPlayer);
});

QUnit.module("Chapter skip");
QUnit.test("Given in the middle of the media when chapterSkipForward then currentTime is at the next chapter", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.duration = 60;
    mockMediaElement.currentTime = mockMediaElement.duration / 2;
    var nextMarkerTime = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.6;
    mediaPlayer.addMarker(nextMarkerTime, TVJS.MarkerType.chapter, "Next chapter");
    mediaPlayer.chapterSkipForward();
    assert.equal(nextMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given less than one chapter from the end when chapterSkipForward then position is at the end of the video", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.duration = 60;
    mockMediaElement.currentTime = mockMediaElement.duration - 0.1;
    mediaPlayer.chapterSkipForward();
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given at the end when chapterSkipForward then position is at the end", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.duration = 60;
    mockMediaElement.currentTime = mockMediaElement.duration;
    mediaPlayer.chapterSkipForward();
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given only slightly past an existing marker when chapterSkipBack then position jumps back two markers instead of one", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    var slightlyPastSecondMarker = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.1 + 0.25;
    mockMediaElement.currentTime = slightlyPastSecondMarker;
    mediaPlayer.chapterSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given in the middle when chapterSkipBack then position is at the previous chapter", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    var middleOfMedia = mediaPlayer.mediaElementAdapter.mediaElement.duration / 2;
    mockMediaElement.currentTime = middleOfMedia;
    var previousMarkerTime = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.4;
    mediaPlayer.chapterSkipBack();
    assert.equal(previousMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given less than one chapter from the beginning when chapterSkipBack then is at the beginning of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    var lessThanOneChapterAwayFromTheBeginning = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.1 - 5;
    mockMediaElement.currentTime = lessThanOneChapterAwayFromTheBeginning;
    mediaPlayer.chapterSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given at the beginning when chapterSkipBack then position is at the beginning", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    mockMediaElement.currentTime = 0;
    mediaPlayer.chapterSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given only slightly before an existing marker when chapterSkipForward then position jumps forward two markers instead of one", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    var slightlyPastSecondMarker = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.1 - 0.25;
    mockMediaElement.currentTime = slightlyPastSecondMarker;
    var expectedNextMarkerTime = mediaPlayer.mediaElementAdapter.mediaElement.duration * 0.2;
    mediaPlayer.chapterSkipForward();
    assert.equal(expectedNextMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given media with no chapterMarkers when mediaPlayer is initialized then correct number of default markers are added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 3;
    mockMediaElement.src = "notnull";
    mockMediaElement.duration = 90;
    assert.equal(11, mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given textTrack where kind is chapters when initialized then default chapter markers are not added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.textTracks = [{
        id: "control1",
        label: "chapter",
        src: "notnull",
        kind: "chapters",
        addEventListener: function () { }
    }];
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.duration = 90;
    mockMediaElement.src = "notnull";
    assert.notOk(mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given textTrack where kind is not chapters when initialized then chapterMarkers are not added For that textTrack", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.textTracks = [{
        id: "control1",
        label: "chapter",
        src: "notnull",
        kind: "custom",
        addEventListener: function () { }
    }];
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.duration = 90;
    mockMediaElement.src = "notnull";
    assert.notOk(mediaPlayer.markers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given a marker past the end time when mediaPlayer is initialized then the marker is added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.endTime = 10;
    mediaPlayer.addMarker(mockMediaElement.endTime + 1);
    assert.ok(mediaPlayer.markers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given a startTime is set when mediaPlayer markerAdded before startTime then the marker is added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.startTime = 10;
    mediaPlayer.addMarker(mockMediaElement.startTime - 1);
    assert.ok(mediaPlayer.markers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given on the first chapterMarker when chapterSkipBack then currentTime does not change", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    firstChapterMarkerTime = mediaPlayer._defaultChapterMarkers[0].time;
    mockMediaElement.currentTime = firstChapterMarkerTime;
    mediaPlayer.chapterSkipBack();
    assert.equal(firstChapterMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given on the first chapterMarker when chapterSkipBack then currentTime does not change", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    firstChapterMarkerTime = mediaPlayer._defaultChapterMarkers[0].time;
    mockMediaElement.currentTime = firstChapterMarkerTime;
    mediaPlayer.chapterSkipBack();
    assert.equal(firstChapterMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given on the last chapterMarker when chapterSkipForward then currentTime does not change", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    lastChapterMarkerTime = mediaPlayer._defaultChapterMarkers[mediaPlayer._defaultChapterMarkers.length - 1].time;
    mockMediaElement.currentTime = lastChapterMarkerTime;
    mediaPlayer.chapterSkipForward();
    assert.equal(lastChapterMarkerTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime and endTime set Such that totalTime is less than minimum time For chaptermarkers when mediaPlayer is initialized then no default markers are added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mediaPlayer.startTime = 1;
    mediaPlayer.endTime = 2;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    assert.notOk(mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given no chapter markers when mediaPlayer chapterSkipForward then currentTime is the end of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.readyState = 3;
    mediaPlayer.chapterSkipForward();
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given no chaptermarkers when mediaPlayer chapterSkipBack then currentTime is the beginning of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.currentTime = 10;
    mockMediaElement.readyState = 3;
    mediaPlayer.chapterSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.module("hideControls");
QUnit.test("Given controls visible when videControls then the afterHideControls event fires and controls are hidden", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mediaPlayer.addEventListener("afterhidecontrols", function afterhidecontrols() {
        mediaPlayer.removeEventListener("afterhidecontrols", afterhidecontrols);
        assert.notOk(mediaPlayer.controlsVisible);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.hideControls();
});

QUnit.test("Given controls visible when beforeHideControls event fires and preventDefault then controls are visible", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mediaPlayer.addEventListener("beforehidecontrols", function beforehidecontrols(ev) {
        mediaPlayer.removeEventListener("beforehidecontrols", beforehidecontrols);
        ev.preventDefault();
        assert.ok(mediaPlayer.controlsVisible);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.hideControls();
});

QUnit.module("showControls");
QUnit.test("Given controls hidden when before showControls event fires and preventDefault then controls are hidden", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.addEventListener("beforeshowcontrols", function beforeshowcontrols(ev) {
        mediaPlayer.removeEventListener("beforeshowcontrols", beforeshowcontrols);
        ev.preventDefault();
        assert.notOk(mediaPlayer.controlsVisible);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.showControls();
});

QUnit.test("Given controls hidden when showControls then after showControls event fires then controls are visible", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.addEventListener("aftershowcontrols", function aftershowcontrols(ev) {
        mediaPlayer.removeEventListener("aftershowcontrols", aftershowcontrols);
        assert.ok(mediaPlayer.controlsVisible);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.showControls();
});

QUnit.test("Given controls hidden when hide controls then beforehidecontrols event does not fire", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.addEventListener("beforehidecontrols", function beforeshowcontrols(ev) {
        mediaPlayer.removeEventListener("beforehidecontrols", beforeshowcontrols);
        assert.ok(false, mediaPlayer.controlsVisible);
    }, false);
    mediaPlayer.hideControls();
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given controls visible when showControls then beforeshowcontrols event does not fire", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mediaPlayer.addEventListener("beforeshowcontrols", function beforeshowcontrols(ev) {
        mediaPlayer.removeEventListener("beforeshowcontrols", beforeshowcontrols);
        assert.ok(false, mediaPlayer.controlsVisible);
    }, false);
    mediaPlayer.showControls();
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given controls hidden when show controls then the after showControls event fires and controls are visible", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    mediaPlayer.addEventListener("beforeshowcontrols", function beforeshowcontrols(ev) {
        mediaPlayer.removeEventListener("beforeshowcontrols", beforeshowcontrols);
        assert.ok(false, mediaPlayer.controlsVisible);
    }, false);
    mediaPlayer.showControls();
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When chapterSkipBack then mediaCommandExecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.chapterSkipBack, "chapterSkipBack", assert);
});

QUnit.test("When chapterSkipForward then mediaCommandExecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.chapterSkipForward, "chapterSkipForward", assert);
});

QUnit.test("When FastForward then mediaCommandExecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.fastForward, "fastForward", assert);
});

QUnit.test("When nextTrack then mediaCommandExecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.nextTrack, "nextTrack", assert);
});

QUnit.test("When previousTrack then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.previousTrack, "previousTrack", assert);
});

QUnit.test("When play then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.play, "play", assert);
});

QUnit.test("When pause then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.pause, "pause", assert);
});

QUnit.test("When rewind then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.rewind, "rewind", assert);
});

QUnit.test("When seek then mediacommandexecuted event fires with correct event arguments", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer.addEventListener("mediacommandexecuted", function mediacommandexecuted(ev) {
        mediaPlayer.removeEventListener("mediacommandexecuted", mediacommandexecuted);
        assert.equal("seek", ev.detail.mediaCommand);
        safeDispose(mediaPlayer);
        done();
    }, false);
    mediaPlayer.seek(10);
});

QUnit.test("When timeSkipBack then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.timeSkipBack, "timeSkipBack", assert);
});

QUnit.test("When timeSkipForward then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.timeSkipForward, "timeSkipForward", assert);
});

QUnit.test("When audioTracks then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.audioTracks, "_onAudioTracksCommandInvoked", assert);
});

QUnit.test("When Cast then mediacommandexecuted event fires with correct event arguments", function (assert) {
    if (TVJS.Utilities.hasWinRT) {
        runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.cast, "_onCastCommandInvoked", assert);
    } else {
        var done = assert.async();
        assert.ok(true);
        done();
    }
});

QUnit.test("When closedCaptions then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.closedCaptions, "_onClosedCaptionsCommandInvoked", assert);
});

QUnit.test("When playbackRate then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.playbackRate, "_onPlaybackRateCommandInvoked", assert);
});

QUnit.test("When Volume then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.volume, "_onVolumeCommandInvoked", assert);
});

QUnit.test("When Zoom then mediacommandexecuted event fires with correct event arguments", function (assert) {
    runVerifyMediaCommandExecutedEventTest(TVJS.MediaCommand.zoom, "_onZoomCommandInvoked", assert);
});

QUnit.test("Given paused and isPlayAllowed is true when play then media is playing", function (assert) {
    runIsPlayAllowedTestCase(true, true, assert);
});

QUnit.test("Given paused and isPlayAllowed is false when playthen media is paused", function (assert) {
    runIsPlayAllowedTestCase(false, false, assert);
});

QUnit.test("Given paused and isPlayAllowed is null when play then media is paused", function (assert) {
    runIsPlayAllowedTestCase(null, false, assert);
});

QUnit.test("Given paused and isPlayAllowed is invalid String when play then media is playing", function (assert) {
    runIsPlayAllowedTestCase("invalidValue", true, assert);
});

QUnit.test("Given playing and isPauseAllowed is true when pause then media is paused", function (assert) {
    runIsPauseAllowedTestCase(true, true, assert);
});

QUnit.test("Given playing and isPauseAllowed is false when pause then media is playing", function (assert) {
    runIsPauseAllowedTestCase(false, false, assert);
});

QUnit.test("Given playing and isPauseAllowed is nullWhenPause then media is playing", function (assert) {
    runIsPauseAllowedTestCase(null, false, assert);
});

QUnit.test("Given playing and isPauseAllowed is invalid value when pause then media is paused", function (assert) {
    runIsPauseAllowedTestCase("invalidValue", true, assert);
});

QUnit.test("Given paused and isSeekAllowed is true when seek then media seeked", function (assert) {
    runIsSeekAllowedTestCase(true, true, assert);
});

QUnit.test("Given playing and isSeekAllowed is false when seek then media did not seek", function (assert) {
    runIsSeekAllowedTestCase(false, false, assert);
});

QUnit.test("Given playing and isSeekAllowed is null when seek then media did not seek", function (assert) {
    runIsSeekAllowedTestCase(null, false, assert);
});

QUnit.test("Given playing and isSeekAllowed is invalid value whenSeekThenMediaSeeked", function (assert) {
    runIsSeekAllowedTestCase("invalidValue", true, assert);
});

QUnit.test("Given null mediaElement when chapterSkipBack then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("chapterSkipBack", assert);
});

QUnit.test("Given null mediaElement when chapterSkipForward then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("chapterSkipForward", assert);
});

QUnit.test("Given null mediaElement when FastForward then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("fastForward", assert);
});

QUnit.test("Given null mediaElement when goToLive then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("goToLive", assert);
});

QUnit.test("Given null mediaElement when play then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("play", assert);
});

QUnit.test("Given null mediaElement when pauseThen does not throw an exception", function (assert) {
    runNullMediaElementTestCase("pause", assert);
});

QUnit.test("Given null mediaElement when rewind then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("rewind", assert);
});

QUnit.test("Given null mediaElement when seek then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.mediaElementAdapter.mediaElement = null;
    try {
        mediaPlayer.seek(10);
    } catch (ex) {
        assert.ok(false, "Seeking with a null mediaPlayer.mediaElementAdapter.mediaElement threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given null mediaElement when timeSkipBack then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("timeSkipBack", assert);
});

QUnit.test("Given null mediaElement when timeSkipForward then does not throw an exception", function (assert) {
    runNullMediaElementTestCase("timeSkipForward", assert);
});

QUnit.module("New media element");
QUnit.test("Given new mediaElement when chapterSkipBack then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("chapterSkipBack", assert);
});

QUnit.test("Given new mediaElement when chapterSkipForward then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("chapterSkipForward", assert);
});

QUnit.test("Given new mediaElement when FastForward then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("fastForward", assert);
});

QUnit.test("Given new mediaElement when goToLive then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("goToLive", assert);
});

QUnit.test("Given new mediaElement when play then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("play", assert);
});

QUnit.test("Given new mediaElement when pause then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("pause", assert);
});

QUnit.test("Given new mediaElement when rewind then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("rewind", assert);
});

QUnit.test("Given new mediaElement when seek then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    try {
        mediaPlayer.seek(10);
    } catch (ex) {
        assert.ok(false, "Seeking with a null mediaPlayer.mediaElementAdapter.mediaElement threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given new mediaElement when timeSkipBack then does not throw an exception", function (assert) {
    runNewMediaElementTestCase("timeSkipBack", assert);
});

QUnit.test("Given new mediaElement when timeSkip forwardThen does not throw an exception", function (assert) {
    runNewMediaElementTestCase("timeSkipForward", assert);
});

QUnit.test("Given  mediaElement src is null when chapterSkipBack then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("chapterSkipBack", true, assert);
});

QUnit.test("Given  mediaElement src is null when chapterSkipForward then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("chapterSkipForward", true, assert);
});

QUnit.test("Given  mediaElement src is null when FastForward then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("fastForward", false, assert);
});

QUnit.test("Given  mediaElement src is null when goToLive then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("goToLive", true, assert);
});

QUnit.test("Given  mediaElement src is null when play then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("play", false, assert);
});

QUnit.test("Given  mediaElement src is null when pause then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("pause", false, assert);
});

QUnit.test("Given  mediaElement src is null when rewind then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("rewind", false, assert);
});

QUnit.test("Given  mediaElement src is null when seek then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = null;
    try {
        mediaPlayer.seek(10);
    } catch (ex) {
        assert.ok(false, "Seeking with a null mediaPlayer.mediaElementAdapter.mediaElement threw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given  mediaElement src is null when timeSkipBack then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("timeSkipBack", true, assert);
});

QUnit.test("Given  mediaElement src is null when timeSkipForward then throws an exception", function (assert) {
    runNullMediaElementSrcTestCase("timeSkipForward", true, assert);
});

QUnit.test("Given invalid media element when play then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.mediaElementAdapter.mediaElement = "foobar";
        mediaPlayer.play();
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "Play() was called on an invalid media element and an exception did not get thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given new mediaPlayer with innerHTML elements when constructed then innerHTML elements are preserved", function (assert) {
    var mediaElementDiv = document.createElement("div");
    mediaElementDiv.innerHTML = "<div id='customDiv'></div>";
    var mediaPlayer = new TVJS.MediaPlayer(mediaElementDiv);
    assert.ok(mediaPlayer.element.querySelector("#customDiv"), "InnerHTML was not preserved.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given mediaElement when src set to new src then endTime is updated", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.duration = 10;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 20;
    assert.equal(mockMediaElement.duration, mediaPlayer.endTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given mediaElement with metadata already loaded when set mediaPlayer mediaElement then startTime and endTime are correctly updated", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement1 = new Test.MockMediaElement();
    mockMediaElement1.initialTime = 1;
    mockMediaElement1.duration = 2;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement1;
    var mockMediaElement2 = new Test.MockMediaElement();
    mockMediaElement2.autoplay = true;
    mockMediaElement2.src = "notnull";
    mockMediaElement2.duration = 10;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement2;
    assert.equal(0, mediaPlayer.startTime);
    assert.equal(mockMediaElement2.duration, mediaPlayer.endTime);
    safeDispose(mediaPlayer);
});

QUnit.module("playbackRate");
QUnit.test("Given mediaPlayer playing when pause then playbackRate is paused", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.pause();
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given mediaPlayer paused when play then playbackRate is playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.play();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given mediaPlayer paused when FastForward then playbackRate is FF 0.5X", function (assert) {
    runFastForwardTestCase(1, 0.5, true, assert);
});

QUnit.test("Given playing when fastForward then playbackRate is FF 2X", function (assert) {
    runFastForwardTestCase(1, 2, null, assert);
});

QUnit.test("Given FF2X when fastForward then playbackRate is FF 4X", function (assert) {
    runFastForwardTestCase(2, 4, null, assert);
});

QUnit.test("Given FF4X when fastForward then playbackRate is FF 8X", function (assert) {
    runFastForwardTestCase(4, 8, null, assert);
});

QUnit.test("Given FF8X when fastForward then playbackRate is FF 16X", function (assert) {
    runFastForwardTestCase(8, 16, null, assert);
});

QUnit.test("Given FF16X when fastForward then playbackRate is FF 32X", function (assert) {
    runFastForwardTestCase(16, 32, null, assert);
});

QUnit.test("Given FF32X when fastForward then playbackRate is FF 64X", function (assert) {
    runFastForwardTestCase(32, 64, null, assert);
});

QUnit.test("Given FF64X when fastForward then playbackRate is FF MaxRate", function (assert) {
    runFastForwardTestCase(64, 128, null, assert);
});

QUnit.test("Given FFMaxRate when fastForward then playbackRate is FF MaxRate", function (assert) {
    runFastForwardTestCase(128, 128, null, assert);
});

QUnit.test("Given FF128X when rewind then playbackRate is FF 64X", function (assert) {
    runRewindTestCase(128, 64, null, assert);
});

QUnit.test("Given FF64X when rewind then playbackRate is FF 32X", function (assert) {
    runRewindTestCase(64, 32, null, assert);
});

QUnit.test("Given FF32X when rewind then playbackRate is FF 16X", function (assert) {
    runRewindTestCase(32, 16, null, assert);
});

QUnit.test("Given FF16X when rewind then playbackRate is FF 8X", function (assert) {
    runRewindTestCase(16, 8, null, assert);
});

QUnit.test("Given FF8X when rewind then playbackRate is FF 4X", function (assert) {
    runRewindTestCase(8, 4, null, assert);
});

QUnit.test("Given FF4X when rewind then playbackRate is FF 2X", function (assert) {
    runRewindTestCase(4, 2, null, assert);
});

QUnit.test("Given FF2X when rewind then playbackRate is playing", function (assert) {
    runRewindTestCase(2, 1, null, assert);
});

QUnit.test("Given mediaPlayer paused when rewind then playbackRate is negative 0.5X", function (assert) {
    runRewindTestCase(1, -0.5, true, assert);
});

QUnit.test("Given playing when  rewind then playbackRate is RR2X", function (assert) {
    runRewindTestCase(1, -2, null, assert);
});

QUnit.test("Given playbackRate of -1 when rewind then playbackRate is RR2X", function (assert) {
    runRewindTestCase(-1, -2, null, assert);
});

QUnit.test("Given RR2X when rewind then playbackRate is RR4X", function (assert) {
    runRewindTestCase(-2, -4, null, assert);
});

QUnit.test("Given RR4X when rewind then playbackRate is RR8X", function (assert) {
    runRewindTestCase(-4, -8, null, assert);
});

QUnit.test("Given RR8X when rewind then playbackRate is RR16X", function (assert) {
    runRewindTestCase(-8, -16, null, assert);
});

QUnit.test("Given RR16X when rewind then playbackRate is RR32X", function (assert) {
    runRewindTestCase(-16, -32, null, assert);
});

QUnit.test("Given RR32X when rewind then playbackRate is RR64X", function (assert) {
    runRewindTestCase(-32, -64, null, assert);
});

QUnit.test("Given RR64X when rewind then playbackRate is RRMaxRate", function (assert) {
    runRewindTestCase(-64, -128, null, assert);
});

QUnit.test("Given RRMaxRatewhen rewind then playbackRate is RRMaxRate", function (assert) {
    runRewindTestCase(-128, -128, null, assert);
});

QUnit.test("Given RR128X when fastForward then playbackRate is RR64X", function (assert) {
    runFastForwardTestCase(-128, -64, null, assert);
});

QUnit.test("Given RR64X when fastForward then playbackRate is RR32X", function (assert) {
    runFastForwardTestCase(-64, -32, null, assert);
});

QUnit.test("Given RR32X when fastForward then playbackRate is RR16X", function (assert) {
    runFastForwardTestCase(-32, -16, null, assert);
});

QUnit.test("Given RR16X when fastForward then playbackRate is RR8X", function (assert) {
    runFastForwardTestCase(-16, -8, null, assert);
});

QUnit.test("Given RR8X when fastForward then playbackRate is RR4X", function (assert) {
    runFastForwardTestCase(-8, -4, null, assert);
});

QUnit.test("Given RR4X when fastForward then playbackRate is RR2X", function (assert) {
    runFastForwardTestCase(-4, -2, null, assert);
});

QUnit.test("Given RR2X when fastForward then playbackRate is playing", function (assert) {
    runFastForwardTestCase(-2, 1, null, assert);
});

QUnit.test("Given Fast forward when pause then pause", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.fastForward();
    mediaPlayer.pause();
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewind when pause then pause", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.rewind();
    mediaPlayer.pause();
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given playbackRate is fractional value less thanOne when fastForward then playing", function (assert) {
    runFastForwardTestCase(0.9, 1, null, assert);
});

QUnit.test("Given playbackRate is rewindSlowMotionRate when fastForward then playing", function (assert) {
    runFastForwardTestCase(-0.5, 1, null, assert);
});

QUnit.test("Given playbackRate is fractional value less than rewindSlowMotionRate when fastForward then playing", function (assert) {
    runFastForwardTestCase(-0.49, 1, null, assert);
});

QUnit.test("Given playbackRate is fractional value greater than rewindSlowMotionRate when fastForward then playing", function (assert) {
    runFastForwardTestCase(-0.51, 1, null, assert);
});

QUnit.test("Given playbackRate is fractional value greater than one when fastForwardThenFF2X", function (assert) {
    runFastForwardTestCase(1.1, 2, null, assert);
});

QUnit.test("Given playingAndplaybackRate is fastForwardSlowMotionRatewhen rewindThenRR2X", function (assert) {
    runRewindTestCase(0.5, -2, null, assert);
});

QUnit.test("Given playbackRate is fractional value greater than fastForwardSlowMotionRate when rewindThenRR2X", function (assert) {
    runRewindTestCase(0.51, -2, null, assert);
});

QUnit.test("Given playingAndplaybackRate is fractional value less than fastForwardSlowMotionRate when rewind then playing", function (assert) {
    runRewindTestCase(0.49, 1, null, assert);
});

QUnit.test("Given playbackRate is playing when  rewindThenRR2X", function (assert) {
    runRewindTestCase(1, -2, null, assert);
});

QUnit.test("Given paused and playbackRate is fractional value greater than rewindSlowMotionRate when rewindThenRR2X", function (assert) {
    runRewindTestCase(-0.51, -2, true, assert);
});

QUnit.test("Given playingAndplaybackRate is fractional value greater than rewindSlowMotionRate when rewindThenRR2X", function (assert) {
    runRewindTestCase(-0.51, -2, null, assert);
});

QUnit.test("Given playbackRate is fractional value less thanRewindSlowMotionRate when rewindThenRR2X", function (assert) {
    runRewindTestCase(-0.49, -2, null, assert);
});

QUnit.test("Given fastforward when fastForwarding then controls Remain visible", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.fastForward();
    mediaPlayer.hideControls();
    assert.ok(mediaPlayer.controlsVisible);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewind when rewinding then controls remain visible", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.rewind();
    mediaPlayer.hideControls();
    assert.ok(mediaPlayer.controlsVisible);
    safeDispose(mediaPlayer);
});

QUnit.test("Given in fast forward or rewind mode when chapterSkipBack then not in fast forward or rewind mode", function (assert) {
    runExitFastForwardOrRewindTest("chapterSkipBack", assert);
});

QUnit.test("Given in fast forward or rewind mode when chapterSkipForward then not in fast forward or rewind mode", function (assert) {
    runExitFastForwardOrRewindTest("chapterSkipForward", assert);
});

QUnit.test("Given in fast forward or rewind mode when play then not in fast forward or rewind mode", function (assert) {
    runExitFastForwardOrRewindTest("play", assert);
});

QUnit.test("Given in fast forward or rewind mode when timeSkipBack then not in fast forward or rewind mode", function (assert) {
    runExitFastForwardOrRewindTest("timeSkipBack", assert);
});

QUnit.test("Given in fast forward or rewind mode when timeSkipForward then not in fast forward or rewind mode", function (assert) {
    runExitFastForwardOrRewindTest("timeSkipForward", assert);
});

QUnit.test("Given playing when pause then playpauseToggle is playicon", function (assert) {
    runPlayPauseToggleIconTest("pause", "playicon", null, assert);
});

QUnit.test("Given playing when fastForward then playpauseToggle is playicon", function (assert) {
    runPlayPauseToggleIconTest("fastForward", "playicon", null, assert);
});

QUnit.test("Given playing when rewind then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("rewind", "playicon", null, assert);
});

QUnit.test("Given playing when timeSkipBack then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("timeSkipBack", "pauseicon", null, assert);
});

QUnit.test("Given playing when timeSkipForward then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("timeSkipForward", "pauseicon", null, assert);
});

QUnit.test("Given playing when chapterSkipBack then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("chapterSkipBack", "pauseicon", null, assert);
});

QUnit.test("Given playing when chapterSkipForward then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("chapterSkipForward", "pauseicon", null, assert);
});

QUnit.test("Given paused when play then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("play", "pauseicon", true, assert);
});

QUnit.test("Given paused when fastForward then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("fastForward", "playicon", true, assert);
});

QUnit.test("Given paused when rewind then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("rewind", "playicon", true, assert);
});

QUnit.test("Given paused when timeSkipBack then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("timeSkipBack", "playicon", true, assert);
});

QUnit.test("Given paused when timeSkipForward then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("timeSkipForward", "playicon", true, assert);
});

QUnit.test("Given paused when chapterSkipBack then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("chapterSkipBack", "playicon", true, assert);
});

QUnit.test("Given paused when chapterSkipForward then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("chapterSkipForward", "playicon", true, assert);
});

QUnit.test("Given playing when pause is called on the video tag then playpause toggle is play icon", function (assert) {
    runPlayPauseToggleIconTest("pause", "playicon", null, assert);
});

QUnit.test("Given paused when play is called on the video tag then playpause toggle is pause icon", function (assert) {
    runPlayPauseToggleIconTest("play", "pauseicon", true, assert);
});

QUnit.test("Given paused when playbackRate is increased to greater than the default playbackRate on the video tag then playpause toggle is play icon", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mockMediaElement.playbackRate = 2;
    assert.notOk(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton .tv-mediaplayer-icon").classList.contains("tv-mediaplayer-pauseicon"));
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to beginning then currentTime is Zero", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mediaPlayer.seek(0);
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to end then currentTime is  equal to duration", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 10;
    mediaPlayer.seek(mockMediaElement.duration);
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to middle then currentTime is Equal Half of duration", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 10;
    mediaPlayer.seek(mockMediaElement.duration / 2);
    assert.equal(mockMediaElement.duration / 2, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to fractional value then seek is successful", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 20;
    mediaPlayer.seek(10.525);
    assert.equal(10.525, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to negative time then currentTime is  equal to Zero", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mediaPlayer.seek(-1);
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to past the end of the media then currentTime is equal to the media duration", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 10;
    mediaPlayer.seek(mockMediaElement.duration + 1);
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When invalid parameter passed to seek then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;

    try {
        mediaPlayer.seek("invalid parameter");
    } catch (ex) {
        assert.ok(false, "seek called with invalid parameter and should not throw exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When null passed to seek then does not throw an exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;

    try {
        mediaPlayer.seek(null);
    } catch (ex) {
        assert.ok(false, "seek called with invalid parameter and should not throw exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given paused when seek then paused", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mediaPlayer.seek(5);
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given playing when seek then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.seek(5);
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});
QUnit.test("Given fastf orwarding when seek then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.fastForward();
    mediaPlayer.seek(5);
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewinding when seek then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.rewind();
    mediaPlayer.seek(5);
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to before startTime then currentTime is startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 1;
    mediaPlayer.seek(mediaPlayer.startTime - 1);
    assert.equal(mediaPlayer.startTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When seek to after endTime then currentTime is endTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.endTime = 5;
    mediaPlayer.seek(mediaPlayer.endTime + 1);
    assert.equal(mediaPlayer.endTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When startTime is set to null then does not throw an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = null;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown, "startTime was set to 'null' and an exception was thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("When startTime set to invalid value then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = "invalid value";
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "startTime was set to 'invalid value' and an exception was not thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("When startTime set to negative value then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = -10;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "startTime was set to a negative number and an exception was not thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("When endTime is set to null then does not throw an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = null;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown, "startTime was set to 'null' and an exception was not thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("When endTime set to invalidValue then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = "invalid value";
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "startTime was set to 'invalid value' and an exception was thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("When endTime set to negative value then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.startTime = -10;
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.ok(wasExceptionThrown, "startTime was set to a negative number and an exception was not thrown.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set when timeSkipBack then currentTime at the startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 5;
    mediaPlayer.currentTime = mediaPlayer.startTime;
    mediaPlayer.timeSkipBack();
    assert.equal(mediaPlayer.startTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given at the end of the media when timeSkip forward then currentTime is the end of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.endTime = 5;
    mediaPlayer.currentTime = mediaPlayer.endTime;
    mediaPlayer.timeSkipForward();
    assert.equal(mediaPlayer.endTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set and first chaptermarker is before the startTime when chapterSkipBack then does not skip to the marker before the startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 5;
    mediaPlayer.addMarker(mediaPlayer.startTime - 1, TVJS.MarkerType.chapter, {});
    mediaPlayer.currentTime = mediaPlayer.startTime;
    mediaPlayer.chapterSkipBack();
    assert.equal(mediaPlayer.startTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given endTime is set and last chapterMarker is after the end time when chapterSkipForward then does not skip to the marker past the endTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.endTime = 5;
    mediaPlayer.addMarker(mediaPlayer.endTime + 1, TVJS.MarkerType.chapter, {});
    mediaPlayer.currentTime = mediaPlayer.endTime;
    mediaPlayer.chapterSkipForward();
    assert.equal(mediaPlayer.endTime, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given endTime is set when attmept to fastForward past the endTime then does not fast forward past end time", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.endTime = 5;
    mediaPlayer.currentTime = mediaPlayer.endTime;
    mediaPlayer.fastForward();
    // We inject a fake time delay so we can simulate time passing during a fastforward and the test doesn't have to wait
    mediaPlayer._lastFastForwardOrRewindTimerTime = new Date().getTime() - 9000000;
    mediaPlayer._onFastForwardRewindTimerTick();
    assert.ok(Math.abs(mediaPlayer.endTime - mediaPlayer.targetCurrentTime) < 0.2);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set when attmept to rewind before the startTime then does not rewind before startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 5;
    mediaPlayer.currentTime = mediaPlayer.startTime;
    mediaPlayer.rewind();
    // We inject a fake time delay so we can simulate time passing during a fastforward and the test doesn't have to wait
    mediaPlayer._lastFastForwardOrRewindTimerTime = new Date().getTime() - 9000000;
    mediaPlayer._onFastForwardRewindTimerTick();
    assert.ok(Math.abs(mediaPlayer.startTime - mediaPlayer.targetCurrentTime) < 0.2);
    safeDispose(mediaPlayer);
});

QUnit.test("Given default chapter markers when startTime is set then first default chapterMarker is now at the startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mediaPlayer.startTime = 5;
    mockMediaElement.src = "notnull";
    assert.equal(mediaPlayer.startTime, mediaPlayer._defaultChapterMarkers[0].time);
    safeDispose(mediaPlayer);
});

QUnit.test("Given defaultChapter markers when endTime is set then last default chapterMarker is now at the endTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = 80;
    mockMediaElement.src = "notnull";
    assert.equal(mediaPlayer.endTime, mediaPlayer._defaultChapterMarkers[mediaPlayer._defaultChapterMarkers.length - 1].time);
    safeDispose(mediaPlayer);
});

QUnit.test("Given total video duration long enough For default chapter markers when endTime set so duration is too short then node fault chapterMarkers", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.chapterSkipBackButtonVisible = true;
    mediaPlayer.chapterSkipForwardButtonVisible = true;
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 90;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = 10;
    mockMediaElement.src = "notnull";
    assert.notOk(mediaPlayer._defaultChapterMarkers.length);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set when media currentTime is set to before startTime then currentTime is startTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mediaPlayer.startTime = 5;
    mockMediaElement.src = "notnull";
    mockMediaElement.currentTime = mediaPlayer.startTime - 1;
    assert.ok(Math.abs(mediaPlayer.startTime - mockMediaElement.currentTime) < 0.2);
    safeDispose(mediaPlayer);
});

QUnit.test("Given endTime is set when media currentTime is set to after endTime then currentTime is endTime", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = 5;
    mockMediaElement.src = "notnull";
    mockMediaElement.currentTime = mediaPlayer.endTime + 1;
    assert.ok(Math.abs(mediaPlayer.endTime - mockMediaElement.currentTime) < 0.2);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set when hideControls then still subscribed to timeupdates", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mediaPlayer.startTime = 5;
    mockMediaElement.src = "notnull";

    // Search for the timeupdate event listener
    var isSubscribedToTimeUpdates = false;
    var mediaEventSubscriptions = mediaPlayer._mediaEventSubscriptions;
    for (var i = 0; i < mediaEventSubscriptions.length; i++) {
        if (mediaEventSubscriptions[i].eventName === "timeupdate") {
            isSubscribedToTimeUpdates = true;
        }
    }

    if (!isSubscribedToTimeUpdates) {
        assert.ok(false, "The mediaplayer is not subscribed to timeupdate");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given startTime is set when video loaded then startTime is persisted", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mediaPlayer.startTime = 5;
    mockMediaElement.src = "notnull";
    assert.equal(5, mediaPlayer.startTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given endTime is set when videoLoaded then endTime is persisted", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mediaPlayer.endTime = 5;
    mockMediaElement.src = "notnull";
    assert.equal(5, mediaPlayer.endTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given startTime is set when mediaElement Source changes then old startTime is cleared", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 5;
    mockMediaElement.src = "differentSource";
    assert.notEqual(5, mediaPlayer.startTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given endTime is set when mediaElement source changes then old endTime is cleared", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.endTime = 5;
    mockMediaElement.src = "differentSource";
    assert.notEqual(5, mediaPlayer.endTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given fast forwarding when time passes then targetCurrentTime updates", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.currentTime = 0;
    var oldTime = mediaPlayer.targetCurrentTime;
    mediaPlayer.fastForward();
    // We inject a fake time delay so we can simulate time passing during a fastforward and the test doesn't have to wait
    mediaPlayer._lastFastForwardOrRewindTimerTime = new Date().getTime() - 9000000;
    mediaPlayer._onFastForwardRewindTimerTick();
    assert.notEqual(oldTime, mediaPlayer.targetCurrentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewinding when time passes then targetCurrentTime updates", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.currentTime = mediaPlayer.duration;
    var oldTime = mediaPlayer.targetCurrentTime;
    mediaPlayer.rewind();
    // We inject a fake time delay so we can simulate time passing during a fastforward and the test doesn't have to wait
    mediaPlayer._lastFastForwardOrRewindTimerTime = new Date().getTime() - 9000000;
    mediaPlayer._onFastForwardRewindTimerTick();
    assert.notEqual(oldTime, mediaPlayer.targetCurrentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given null timeFormatter function when showControls then timeFormatter function is not null", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.timeFormatter = null;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    assert.ok(mediaPlayer.timeFormatter);
    safeDispose(mediaPlayer);
});

QUnit.test("Given valid timeFormatter function when showControls then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.timeFormatter = function (time) { return "Custom Text." };
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer._skipAnimations = true;
    mediaPlayer.showControls();
    assert.ok(mediaPlayer.timeFormatter);
    safeDispose(mediaPlayer);
});

QUnit.test("Given timeFormatter that is not a function when showControls then throws an exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.timeFormatter = 1;
    try {
        mediaPlayer._updateTimeDisplay();
    } catch (exception) {
        wasExceptionThrown = true;
    }
    if (!wasExceptionThrown) {
        assert.ok(false, "An exception should have been thrown, but wasn't.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given default timeFormatter when timeFormatter is passed a value that is not a number then returns an empty String", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    var returnValue = mediaPlayer.timeFormatter("notANumber");
    assert.equal("", returnValue);
    safeDispose(mediaPlayer);
});

QUnit.test("When timeSkipForward then currentTime is incremented", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    var oldCurrentTime = mockMediaElement.currentTime;
    mockMediaElement.readyState = 4;
    mediaPlayer.timeSkipForward();
    assert.ok(mockMediaElement.currentTime > oldCurrentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When timeSkipBack then currentTime is decremented", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 10;
    mockMediaElement.currentTime = mockMediaElement.duration;
    var oldCurrentTime = mockMediaElement.currentTime;
    mediaPlayer.timeSkipBack();
    assert.ok(mockMediaElement.currentTime < oldCurrentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given currentTime is less than 30 seconds from the end when timeSkipForward then currentTime is at the end of theMedia", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.duration = 10;
    mockMediaElement.currentTime = mockMediaElement.duration - (mediaPlayer._SKIP_FORWARD_INTERVAL - 1);
    mediaPlayer.timeSkipForward();
    assert.equal(mockMediaElement.duration, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given currentTime is less than the skipBackAmount from the beginning when timeSkipBack then currentTime is at beginning of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mockMediaElement.currentTime = mediaPlayer._SKIP_BACK_INTERVAL - 1;
    mediaPlayer.timeSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("Given playing when timeSkipBack then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.timeSkipBack();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given paused when timeSkipBackthen paused", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer.timeSkipBack();
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given fast forwarding when timeSkipBack then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.fastForward();
    mediaPlayer.timeSkipBack();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewinding when timeSkipBack then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.rewind();
    mediaPlayer.timeSkipBack();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given playing when timeSkipForward then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.timeSkipForward();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given paused when timeSkipForwardthen paused", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer.timeSkipForward();
    assert.ok(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given fast forwarding when timeSkipForward then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.fastForward();
    mediaPlayer.timeSkipForward();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given rewinding when timeSkipForward then playing", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.rewind();
    mediaPlayer.timeSkipForward();
    assert.notOk(mockMediaElement.paused);
    safeDispose(mediaPlayer);
});

QUnit.test("Given at the beginning of the media when timeSkipBack then currentTime is the beginning of the media", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.readyState = 4;
    mediaPlayer.timeSkipBack();
    assert.equal(0, mockMediaElement.currentTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When get container element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-controls", assert);
});

QUnit.test("When get timeline element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-timeline", assert);
});

QUnit.test("When get playfrombeginning button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-playfrombeginningbutton", assert);
});

QUnit.test("When get chapterskipback button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-chapterskipbackbutton", assert);
});

QUnit.test("When get rewind button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-rewindbutton", assert);
});

QUnit.test("When get timeskipback button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-timeskipbackbutton", assert);
});

QUnit.test("When get playpause button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-playpausebutton", assert);
});

QUnit.test("When get timeSkipforward button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-timeskipforwardbutton", assert);
});

QUnit.test("When get Fastforward button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-fastforwardbutton", assert);
});

QUnit.test("When get chapterskipforward button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-chapterskipforwardbutton", assert);
});

QUnit.test("When get zoom button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-zoombutton", assert);
});

QUnit.test("When get live button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-livebutton", assert);
});

QUnit.test("When get zoom button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-zoombutton", assert);
});

QUnit.test("When get playonremotedevice button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-playonremotedevicebutton", assert);
});

QUnit.test("When get closedcaptions button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-closedcaptionsbutton", assert);
});

QUnit.test("When get Volume button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-volumebutton", assert);
});

QUnit.test("When get audiotracks button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-audiotracksbutton", assert);
});

QUnit.test("When get fullscreen button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-fullscreenbutton", assert);
});

QUnit.test("When get playbackrate button element by class name then returns the element", function (assert) {
    runGetButtonBySelectorTestCase(".tv-mediaplayer-playbackratebutton", assert);
});

QUnit.test("Given no transport bar buttons when addMarker then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("addMarker", 10, assert);
});

QUnit.test("Given no transport bar buttons when chapterSkipBack then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("chapterSkipBack", null, assert);
});

QUnit.test("Given no transport bar buttons when chapterSkipForward then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("chapterSkipForward", null, assert);
});

QUnit.test("Given no transport bar buttons when fastForward then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("fastForward", null, assert);
});

QUnit.test("Given no transport bar buttons when GoToLive then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("goToLive", null, assert);
});

QUnit.test("Given no transport bar buttons when hideControls then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("hideControls", null, assert);
});

QUnit.test("Given no transport bar buttons when play then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("play", null, assert);
});

QUnit.test("Given no transport bar buttons when pause then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("pause", null, assert);
});

QUnit.test("Given no transport bar buttons when seekThen does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("seek", 10, assert);
});

QUnit.test("Given no transport bar buttons when showControls then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("showControls", null, assert);
});

QUnit.test("Given no transport bar buttons when stop then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("stop", null, assert);
});

QUnit.test("Given no transport bar buttons when timeSkipBack then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("timeSkipBack", null, assert);
});

QUnit.test("Given no transport bar buttons when timeSkipForward then does not throw an exception", function (assert) {
    runNoTransportBarButtonsTestCase("timeSkipForward", null, assert);
});

QUnit.test("When window resize then does not throw exception", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.mediaElementAdapter.mediaElement = new Test.MockMediaElement();
    try {
        mediaPlayer._windowResizeCallback();
    } catch (exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown, "An exception was thrown when calling the mediaPlayer's resize handler.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer is instantiated programatically with no video then default values For mediaPlayer properties are correct", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    assert.ok(mediaPlayer.mediaElementAdapter, "MediaPlayer.mediaElementAdapter was null.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer is instantiated programatically with video with valid source then default values For mediaPlayer properties are correct", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var video = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = video;
    video.src = "http://lizard.dns.microsoft.com/WMV/Halo4TrailerSmall.wmv";
    video.autoplay = true;
    if (mediaPlayer._tv) {
        assert.ok(mediaPlayer.fullScreen, "mediaPlayer.fullScreen did not default to 'false'.");
    } else {
        assert.notOk(mediaPlayer.fullScreen, "mediaPlayer.fullScreen did not default to 'false'.");
    }
    assert.ok(mediaPlayer.mediaElementAdapter, "MediaPlayer.mediaElementAdapter was null.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer constructor is called with a null element then a mediaPlayer control is created", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer(null);
    assert.ok(mediaPlayer.mediaElementAdapter, "MediaPlayer.mediaElementAdapter was null.");
    assert.equal(true, mediaPlayer.thumbnailEnabled, "mediaPlayer.thumbnailEnabled was not 'true'.");
    assert.equal(0, mediaPlayer.startTime, "mediaPlayer.startTime was not 0.");
    assert.ok(mediaPlayer.timeFormatter, "mediaPlayer.timeFormatter was was null.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer constructor is called with an element that is not in the DOM then a mediaPlayer control is created", function (assert) {
    var element = document.createElement("div");
    var mediaPlayer = new TVJS.MediaPlayer(element);
    assert.equal(element, mediaPlayer.element, "MediaPlayer.element was not the element we used to instantiate it with.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer constructor is called with an element that is in the DOM then a mediaPlayer control is created", function (assert) {
    var element = document.createElement("div");
    document.body.appendChild(element);
    var mediaPlayer = new TVJS.MediaPlayer(element);
    assert.equal(element, mediaPlayer.element, "MediaPlayer.element was not the element we used to instantiate it with.");
    // Clean up the element
    element.parentNode.removeChild(element);
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer constructor is called with a non Div element that is in the DOM then a mediaPlayer control is created", function (assert) {
    var element = document.createElement("img");
    document.body.appendChild(element);
    var mediaPlayer = new TVJS.MediaPlayer(element);
    assert.equal(element, mediaPlayer.element, "MediaPlayer.element was not the element we used to instantiate it with.");
    // Clean up the element
    element.parentNode.removeChild(element);
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when media player constructor is called with options then options are set", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer(null, { foo: "bar" });
    assert.equal("bar", mediaPlayer.foo, "We were able to set the options on the element.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer constructor is called then expected DOM methods are added", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    assert.ok(mediaPlayer.addEventListener, "The mediaPlayer control does not have all the expected DOM methods.");
    assert.ok(mediaPlayer.dispatchEvent, "The mediaPlayer control does not have all the expected DOM methods.");
    assert.ok(mediaPlayer.removeEventListener, "The mediaPlayer control does not have all the expected DOM methods.");
    safeDispose(mediaPlayer);
});

QUnit.test("Given TVJS when mediaPlayer dispose is called twice then no exception is thrown", function (assert) {
    var wasExceptionThrown = false;
    var mediaPlayer = new TVJS.MediaPlayer();
    mediaPlayer.dispose();
    try {
        mediaPlayer.dispose();
    } catch (Exception) {
        wasExceptionThrown = true;
    }
    assert.notOk(wasExceptionThrown, "Exception was thrown when mediaPlayer.dispose was called twice.");
    safeDispose(mediaPlayer);
});

QUnit.module("Thumbnail events");
QUnit.test("Given TVJS when fast fowarding and thumbnail enabled then raise thumbnailRequest event", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.thumbnailEnabled = true;
    mediaPlayer.addEventListener("thumbnailrequest", function thumbnailrequest() {
        mediaPlayer.removeEventListener("thumbnailrequest", thumbnailrequest);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mediaPlayer.fastForward();
    mediaPlayer._onFastForwardRewindTimerTick();
});

QUnit.test("Given TVJS when rewinding and thumbnail enabled then raise thumbnailRequest event", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.addEventListener("thumbnailrequest", function thumbnailrequest() {
        mediaPlayer.removeEventListener("thumbnailrequest", thumbnailrequest);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mediaPlayer.rewind();
    mediaPlayer._onFastForwardRewindTimerTick();
});

QUnit.test("Given TVJS when fast fowarding and not thumbnailEnabled then not thumbnailRequest event", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.thumbnailEnabled = false;
    mediaPlayer.addEventListener("thumbnailrequest", function thumbnailRequest() {
        mediaPlayer.removeEventListener("thumbnailrequest", thumbnailRequest);
        assert.ok(false, "The thumbnailRequest event should not have been raised.");
    }, false);
    mediaPlayer.fastForward();
    mediaPlayer._onFastForwardRewindTimerTick();
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given TVJS when rewinding and not thumbnailEnabled then no thumbnailRequest event", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.thumbnailEnabled = false;
    mediaPlayer.addEventListener("thumbnailrequest", function thumbnailRequest() {
        mediaPlayer.removeEventListener("thumbnailrequest", thumbnailRequest);
        assert.ok(false, "The thumbnailRequest event should not have been raised.");
    }, false);
    mediaPlayer.rewind();
    mediaPlayer._onFastForwardRewindTimerTick();
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("Given TVJS when fast forward then targetRatechanged event", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.addEventListener("targetratechange", function targetratechange() {
        mediaPlayer.removeEventListener("targetratechange", targetratechange);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mediaPlayer.fastForward();
    mediaPlayer.fastForward();
});

QUnit.test("Given TVJS when fastForward then targetTimeupdate event", function (assert) {
    var done = assert.async();
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.duration = 10;
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.addEventListener("targettimeupdate", function targettimeupdate() {
        mediaPlayer.removeEventListener("targettimeupdate", targettimeupdate);
        safeDispose(mediaPlayer);
        assert.ok(true);
        done();
    }, false);
    mediaPlayer.fastForward();
    mediaPlayer.fastForward();
});

// Regression test cases - all the following test cases are based on real bugs found in production
QUnit.test("When src set to null then startTime and endTime update", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.autoplay = true;
    mockMediaElement.duration = 10;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mockMediaElement.src = "notnull";
    mediaPlayer.startTime = 1;
    mediaPlayer.endTime = 2;
    mockMediaElement.src = null;
    assert.notEqual(1, mediaPlayer.startTime);
    assert.notEqual(2, mediaPlayer.endTime);
    safeDispose(mediaPlayer);
});

QUnit.test("When set buttons to Disabled and srcSet then disabled button state preserved", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mockMediaElement.autoplay = true;
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton").disabled = true;
    mockMediaElement.src = "notnull";
    assert.ok(mediaPlayer.element.querySelector(".tv-mediaplayer-playpausebutton").disabled);
    safeDispose(mediaPlayer);
});

QUnit.test("When focus in overlay and space or GamepadA then controls don't showControls", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    var overlay = document.createElement("button");
    mockMediaElement.autoplay = true;
    mockMediaElement.src = "notnull";
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    TVJS.Utilities.addClass(overlay, ".tv-overlay");
    document.body.appendChild(overlay);
    overlay.focus();
    mediaPlayer._onInputHandlerKeyDown({ keyCode: TVJS.Utilities.space });
    mediaPlayer._onInputHandlerKeyDown({ keyCode: TVJS.Utilities.gamepadA });
    assert.notOk(mediaPlayer.controlsVisible);
    document.body.removeChild(overlay);
    safeDispose(mediaPlayer);
});

QUnit.test("When addMarker with time zero then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    var mockMediaElement = new Test.MockMediaElement();
    mediaPlayer.mediaElementAdapter.mediaElement = mockMediaElement;
    try {
        mediaPlayer.addMarker(0);
    } catch (ex) {
        assert.ok(false, "Adding a marker with time 0 should not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});

QUnit.test("When call setContent metadata with empty objects then does not throw exception", function (assert) {
    var mediaPlayer = new TVJS.MediaPlayer();
    try {
        mediaPlayer.setContentMetadata({}, {});
    } catch (ex) {
        assert.ok(false, "Calling setContentMetadata with null should not throw an exception.");
    }
    safeDispose(mediaPlayer);
    assert.ok(true);
});