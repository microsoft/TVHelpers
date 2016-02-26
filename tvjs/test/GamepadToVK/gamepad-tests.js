// Copyright (c) Microsoft Corporation.  All Rights Reserved. Licensed under the MIT License. See License.txt in the project root for license information.
(function () {
    "use strict";

    var _GAMEPAD_A_BUTTON_INDEX = 0,
    _GAMEPAD_B_BUTTON_INDEX = 1,
    _GAMEPAD_DPAD_UP_BUTTON_INDEX = 12,
    _GAMEPAD_DPAD_DOWN_BUTTON_INDEX = 13,
    _GAMEPAD_DPAD_LEFT_BUTTON_INDEX = 14,
    _GAMEPAD_DPAD_RIGHT_BUTTON_INDEX = 15,
    _GAMEPAD_A_KEY = "GamepadA",
    _GAMEPAD_B_KEY = "GamepadB",
    _GAMEPAD_DPAD_UP_KEY = "GamepadDPadUp",
    _GAMEPAD_DPAD_DOWN_KEY = "GamepadDPadDown",
    _GAMEPAD_DPAD_LEFT_KEY = "GamepadDPadLeft",
    _GAMEPAD_DPAD_RIGHT_KEY = "GamepadDPadRight",
    _GAMEPAD_LEFT_THUMBSTICK_UP_KEY = "GamepadLeftThumbStickUp",
    _GAMEPAD_LEFT_THUMBSTICK_DOWN_KEY = "GamepadLeftThumbStickDown",
    _GAMEPAD_LEFT_THUMBSTICK_LEFT_KEY = "GamepadLeftThumbStickLeft",
    _GAMEPAD_LEFT_THUMBSTICK_RIGHT_KEY = "GamepadLeftThumbStickRight",
    _GAMEPAD_A_KEYCODE = 195,
    _GAMEPAD_B_KEYCODE = 196,
    _GAMEPAD_DPAD_UP_KEYCODE = 203,
    _GAMEPAD_DPAD_DOWN_KEYCODE = 204,
    _GAMEPAD_DPAD_LEFT_KEYCODE = 205,
    _GAMEPAD_DPAD_RIGHT_KEYCODE = 206,
    _GAMEPAD_LEFT_THUMBSTICK_UP_KEYCODE = 211,
    _GAMEPAD_LEFT_THUMBSTICK_DOWN_KEYCODE = 212,
    _GAMEPAD_LEFT_THUMBSTICK_LEFT_KEYCODE = 214,
    _GAMEPAD_LEFT_THUMBSTICK_RIGHT_KEYCODE = 213,
    _THUMB_STICK_THRESHOLD = 0.75;

    function testSetUp() {
        // Mock the gamepads object
        var mockGamepads = [{
            axes: [
                0,
                0
            ],
            buttons: [
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {},
                {}
            ]
        }];
        navigator.getGamepads = function () {
            return mockGamepads;
        }
        return mockGamepads[0];
    };

    function runGamepadButtonPressTest(assert, gamepadButtonIndex, expectedKey, expectedKeyCode) {
        var done = assert.async();
        var gamepad = testSetUp();

        document.addEventListener("keydown", function handleKeyDown(e) {
            document.removeEventListener("keydown", handleKeyDown);
            assert.equal(e.key, expectedKey);
            assert.equal(e.keyCode, expectedKeyCode);
            document.addEventListener("keyup", function handleKeyUp(e2) {
                document.removeEventListener("keyup", handleKeyUp);
                assert.equal(e2.key, expectedKey);
                assert.equal(e2.keyCode, expectedKeyCode);
                done();
            });
            gamepad.buttons[gamepadButtonIndex].pressed = false;
        });
        gamepad.buttons[gamepadButtonIndex].pressed = true;
    };

    function runGamepadLeftThumbstickTest(assert, leftThumbstickX, leftThumbstickY, expectedKey, expectedKeyCode) {
        var done = assert.async();
        var gamepad = testSetUp();

        document.addEventListener("keydown", function handleKeyDown(e) {
            document.removeEventListener("keydown", handleKeyDown);
            assert.equal(e.key, expectedKey);
            assert.equal(e.keyCode, expectedKeyCode);
            document.addEventListener("keyup", function handleKeyUp(e2) {
                document.removeEventListener("keyup", handleKeyUp);
                assert.equal(e2.key, expectedKey);
                assert.equal(e2.keyCode, expectedKeyCode);
                done();
            });
            gamepad.axes[0] = 0;
            gamepad.axes[1] = 0;
        });
        gamepad.axes[0] = leftThumbstickX;
        gamepad.axes[1] = leftThumbstickY;
    };

    QUnit.test("test GamepadDPadUp pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_DPAD_UP_BUTTON_INDEX, _GAMEPAD_DPAD_UP_KEY, _GAMEPAD_DPAD_UP_KEYCODE);
    });
    QUnit.test("test GamepadDPadDown pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_DPAD_DOWN_BUTTON_INDEX, _GAMEPAD_DPAD_DOWN_KEY, _GAMEPAD_DPAD_DOWN_KEYCODE);
    });
    QUnit.test("test GamepadDPadLeft pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_DPAD_LEFT_BUTTON_INDEX, _GAMEPAD_DPAD_LEFT_KEY, _GAMEPAD_DPAD_LEFT_KEYCODE);
    });
    QUnit.test("test GamepadDPadRight pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_DPAD_RIGHT_BUTTON_INDEX, _GAMEPAD_DPAD_RIGHT_KEY, _GAMEPAD_DPAD_RIGHT_KEYCODE);
    });
    QUnit.test("test GamepadA pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_A_BUTTON_INDEX, _GAMEPAD_A_KEY, _GAMEPAD_A_KEYCODE);
    });
    QUnit.test("test GamepadB pressed fires expected events", function (assert) {
        runGamepadButtonPressTest(assert, _GAMEPAD_B_BUTTON_INDEX, _GAMEPAD_B_KEY, _GAMEPAD_B_KEYCODE);
    });
    QUnit.test("test GamepadLeftThumbstickUp pressed fires expected events", function (assert) {
        runGamepadLeftThumbstickTest(assert, 0, -1 * (_THUMB_STICK_THRESHOLD + 1), _GAMEPAD_LEFT_THUMBSTICK_UP_KEY, _GAMEPAD_LEFT_THUMBSTICK_UP_KEYCODE);
    });
    QUnit.test("test GamepadLeftThumbstickDown pressed fires expected events", function (assert) {
        runGamepadLeftThumbstickTest(assert, 0, _THUMB_STICK_THRESHOLD + 1, _GAMEPAD_LEFT_THUMBSTICK_DOWN_KEY, _GAMEPAD_LEFT_THUMBSTICK_DOWN_KEYCODE);
    });
    QUnit.test("test GamepadLeftThumbstickLeft pressed fires expected events", function (assert) {
        runGamepadLeftThumbstickTest(assert, -1 * (_THUMB_STICK_THRESHOLD + 1), 0, _GAMEPAD_LEFT_THUMBSTICK_LEFT_KEY, _GAMEPAD_LEFT_THUMBSTICK_LEFT_KEYCODE);
    });
    QUnit.test("test GamepadLeftThumbstickRight pressed fires expected events", function (assert) {
        runGamepadLeftThumbstickTest(assert, _THUMB_STICK_THRESHOLD + 1, 0, _GAMEPAD_LEFT_THUMBSTICK_RIGHT_KEY, _GAMEPAD_LEFT_THUMBSTICK_RIGHT_KEYCODE);
    });
})();