using System;
using System.Text;
using AET.Unity.SimplSharp;
using AET.Unity.SimplSharp.Timer;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes

namespace AET.Unity.Relays {
  public enum DisableMode {
    HoldInCurrentPosition = 0,
    GoUp = 1,
    GoDown = 2
  }
  public class ShadeUpDownStopRelayController {
    private PressHold pressHoldUp, pressHoldDown;
    private ITimer shadeMovingTimer, lockoutTimer;
    
    private enum Dir { Stopped, Up, Down, JogUp, JogDown }

    private Dir movingDirection;

    public ShadeUpDownStopRelayController() {
      ShadeMovingTimeMs = 15000;
      PressHoldTimeMs = 1000;
      LockoutTimeMs = 1000;
    }

    public int PressHoldTimeMs {
      set {
        PressHoldUp.HoldTimeMs = value;
        PressHoldDown.HoldTimeMs = value;
      }
    }

    public DisableMode DisableMode { get; set; }
    public void SetDisableMode(int mode) { DisableMode = (DisableMode)mode; }
    public bool Disabled { get; set; }
    public int ShadeMovingTimeMs { get; set; }
    public int LockoutTimeMs { get; set; }


    private PressHold PressHoldUp {
      get { return pressHoldUp ?? (PressHoldUp = new PressHold()); }
      set {
        pressHoldUp = value;
        pressHoldUp.HoldStartAction = HoldUpStart;
        pressHoldUp.HoldStopAction = HoldUpStop;
        pressHoldUp.PressAction = Up;
      }
    }
    private PressHold PressHoldDown {
      get { return pressHoldDown ?? (PressHoldDown = new PressHold()); }
      set {
        pressHoldDown = value;
        pressHoldDown.HoldStartAction = HoldDownStart;
        pressHoldDown.HoldStopAction = HoldDownStop;
        pressHoldDown.PressAction = Down;
      }
    }

    public ITimer PressHoldUpTimer { set { PressHoldUp.Timer = value; } }
    public ITimer PressHoldDownTimer { set { PressHoldDown.Timer = value; } }

    #region Shade Moving
    public ITimer ShadeMovingTimer {
      get { return shadeMovingTimer ?? (ShadeMovingTimer = new CrestronTimer()); }
      set { shadeMovingTimer = value; }
    }

    public ITimer LockoutTimer {
      get { return lockoutTimer ?? (LockoutTimer = new CrestronTimer()); }
      set { lockoutTimer = value; }      
    }

    #endregion

    private void HoldUpStart() {
      movingDirection = Dir.JogUp;
      UpRelay();
    }

    private void HoldUpStop() {
      StopRelay();
    }

    private void HoldDownStart() {
      movingDirection = Dir.JogDown; 
      DownRelay(); 
      
    }

    private void HoldDownStop() {
      StopRelay();
    }


    private void Up() {
      UpRelay();
      MovingUp_F(1);
      movingDirection = Dir.Up;
      ShadeMovingTimer.Start(ShadeMovingTimeMs, (o) => MovingUp_F(0));
    }
   

    private void Down() { 
      DownRelay();
      MovingDown_F(1);
      movingDirection = Dir.Down;
      ShadeMovingTimer.Start(ShadeMovingTimeMs, (o) => MovingDown_F(0));
    }


    public void UpPress() {
      if (Disabled) return;
      if (movingDirection == Dir.JogDown || movingDirection == Dir.JogUp) return;
      if (StoppedShadeBecauseItWasMoving()) return;
      PressHoldUp.Press();
    }

    public void UpRelease() {
      if (Disabled) return;
      PressHoldUp.Release();
    }

    public void DownPress() {
      if (Disabled) return;
      if (movingDirection == Dir.JogDown || movingDirection == Dir.JogUp) return;
      if (StoppedShadeBecauseItWasMoving()) return;
      PressHoldDown.Press();      
    }

    public void DownRelease() {
      if (Disabled) return;
      PressHoldDown.Release();
    }

    private bool StoppedShadeBecauseItWasMoving() {
      if(movingDirection == Dir.Stopped) return false;
      if (ShadeMovingTimer.IsRunning) ShadeMovingTimer.Stop();
      if (movingDirection == Dir.Up) MovingUp_F(0); else MovingDown_F(0);
      StopRelay();      
      return true;
    }

    private void UpRelay() {
      if (LockoutTimer.IsRunning) LockoutTimer.TimerCallback = (o) => TriggerUpRelay();
      else TriggerUpRelay();
    }

    private void DownRelay() {
      if (LockoutTimer.IsRunning) LockoutTimer.TimerCallback = (o) => TriggerDownRelay();
      else TriggerDownRelay();
    }

    private void StopRelay() {
      if (movingDirection == Dir.Down || movingDirection == Dir.JogDown) TriggerDownStopRelay();
      if (movingDirection == Dir.Up || movingDirection == Dir.JogUp) TriggerUpStopRelay();
      movingDirection = Dir.Stopped;
      TriggerStopRelay();
      if(LockoutTimeMs > 0) LockoutTimer.Start(LockoutTimeMs, delegate {});
    }

    public void Disable() {
      Disabled_F(1);
      Disabled = true;
      StoppedShadeBecauseItWasMoving();
      switch (DisableMode) {
        case (DisableMode.GoUp): {
          Up();
          break;
        }
          case (DisableMode.GoDown): {
          Down();
          break;
        }
          default: {
          break;
        }
      }
    }

    public void Enable() {
      Disabled_F(0);
      Disabled = false;
    }
    
    public TriggerDelegate TriggerUpRelay { get; set; }
    public TriggerDelegate TriggerDownRelay { get; set; }
    public TriggerDelegate TriggerStopRelay { get; set; }
    public TriggerDelegate TriggerUpStopRelay { get; set; }
    public TriggerDelegate TriggerDownStopRelay { get; set; }
    public SetUshortOutputDelegate Up_F { get; set; }
    public SetUshortOutputDelegate Down_F { get; set; }
    public SetUshortOutputDelegate MovingUp_F { get; set; }
    public SetUshortOutputDelegate MovingDown_F { get; set; }
    public SetUshortOutputDelegate Disabled_F { get; set; }
  }
}
