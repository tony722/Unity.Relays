# Unity.Relays
## Part of the open source Unity.Framework for Crestron
### Version 1.0.5
This is the beginning of series of modules to handle relays. Currently this includes:

* Shade Up/Down/Stop Relay Controller.

## To Use In SIMPL Windows:
Download the latest release: [AET Unity Relays v1.0.5](https://github.com/tony722/Unity.Relays/releases/download/v1.0.5/AET.Unity.Relays.v1.0.5.zip)

Add the AET Unity Shade Relay v1.umc to your program. 

### Parameters:
* Shade Moving Time: _Time for shade to raise or lower from fully lowered to fully open._
* Lockout Time: _Time for motors to stop/pause before changing directions._
* Press Hold Time: _Up/Down buttons will go up when tapped, and jog the shade up/down if it is held longer this this time._
* Relay Pulse Time: _Amount of time to pulse the relays for._
* Disable Mode: _What to do when the `Disable` input is pulsed or the `Disabled_State` input goes high. Shade can remain in place, open, or close._

### Inputs:
* Shade_Up, Shade_Down: _Tap to raise/lower. Hold to jog up/down._
* Oscillator_Source: _Connect a single oscillator to every shade, so that buttons blink in unison rather than randomly._Amount
* Disabled_State: _Up/Down controls locked out as long as this is high. Depending on `Disable Mode` parameter setting it may raise/lower when this goes high._Amount
* Disable: _Pulse to disable shade (rising edge)._
* Enable: _Pulse to enable shade (rising edge)._

### Outputs:
* Shade_Up_F, Shade_Down_F: _Feedback of up/down state of shade._
* Shade_Up_Relay, Shade_Stop_Relay, Shade_Down_Relay: _Connect to Up, Down, Stop relays._
* Shade_Up_Stop_Relay, Shade_Down_Stop_Relay: _Use instead of `Shade_Stop_Relay` for separate Up_Stop, and Down_Stop relays._
* Disabled_F: High when shade is disabled

## License: Apache License 2.0 - Commercial Use Freely Permitted.
 Please freely use this library in any Crestron application, including for-profit Crestron SIMPL Windows programs that you charge money to sell, and in SIMPL# libraries of your own, including ones you sell.

 If you modify the code in this library and feel those changes will enhance the usability of this library to other Crestron programmers, it would be greatly appreciated if you would branch this repository, and issue a pull request back to this repository. Thank you!
