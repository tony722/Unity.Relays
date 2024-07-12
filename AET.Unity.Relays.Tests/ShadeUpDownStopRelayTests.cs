using System.Collections.Generic;
using AET.Unity.SimplSharp.Timer;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AET.Unity.Relays.Tests {
  /// <summary>
  /// Summary description for UnitTest1
  /// </summary>
  [TestClass]
  public class ShadeUpDownStopRelayTests {
    private List<string> commands = new List<string>();
    private ShadeUpDownStopRelayController controller = new ShadeUpDownStopRelayController();
    private TestTimer pressHoldUpTimer = new TestTimer();
    private TestTimer pressHoldDownTimer = new TestTimer();
    private TestTimer shadeMovingTimer = new TestTimer();
    private TestTimer lockoutTimer = new TestTimer();
    string disabledF = null;

    [TestInitialize]
    public void TestInit() {
        controller.Disabled_F = value => disabledF = "Disabled_F=" + value;
        controller.PressHoldUpTimer = pressHoldUpTimer;
      controller.PressHoldDownTimer = pressHoldDownTimer;
      controller.ShadeMovingTimer = shadeMovingTimer;
      controller.LockoutTimer = lockoutTimer;
      controller.TriggerUpRelay = () => commands.Add("Up");
      controller.TriggerDownRelay = () => commands.Add("Down");
      controller.TriggerStopRelay = () => commands.Add("Stop");
      controller.Up_F = (v) => commands.Add("Up_F=" + v);
      controller.Down_F = (v) => commands.Add("Down_F=" + v);
      controller.MovingUp_F = (v) => commands.Add("MovingUp_F=" + v);
      controller.MovingDown_F = (v) => commands.Add("MovingDown_F=" + v);
      controller.TriggerUpStopRelay = delegate { };
      controller.TriggerDownStopRelay = delegate { };
    }

    #region Up

    [TestMethod]
    public void Up_NotHeld_ShadeStarts() {
      controller.UpPress();
      controller.UpRelease();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1");
      shadeMovingTimer.TimerElapsed();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0");
    }

    [TestMethod]
    public void Up_Held_ShadeJogsAsLongAsHeld() {
      pressHoldUpTimer.ElapseImmediately = true;
      controller.UpPress();
      controller.UpRelease();
      commands.Should().BeEquivalentTo("Up", "Stop");
    }

    [TestMethod]
    public void Up_AlreadyMoving_Stops() {
      controller.UpPress();
      controller.UpRelease();
      //Moving
      controller.UpPress();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "Stop");
    }

    #endregion

    #region Down

    [TestMethod]
    public void Down_NotHeld_ShadeStarts() {
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1");
      shadeMovingTimer.TimerElapsed();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1", "MovingDown_F=0");
    }

    [TestMethod]
    public void Down_Held_ShadeJogsAsLongAsHeld() {
      pressHoldDownTimer.ElapseImmediately = true;
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Down", "Stop");
    }

    [TestMethod]
    public void Down_AlreadyMoving_Stops() {
      controller.DownPress();
      controller.DownRelease();
      //Moving
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1", "MovingDown_F=0", "Stop");
    }

    [TestMethod]
    public void Down_AlreadyMovingUp_stops() {
      controller.UpPress();
      controller.UpRelease();
      //Moving
      controller.DownPress();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "Stop");
    }

    [TestMethod]
    public void HoldDown_AlreadyJoggingUp_IgnoreDown() {
      pressHoldUpTimer.ElapseImmediately = true;
      controller.UpPress();
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Up");
    }

    [TestMethod]
    public void ReverseDirection_DelayBeforeChangingDirection() {
      controller.UpPress();
      controller.UpRelease();
      //moving up
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "Stop");
      //stopped
      controller.DownPress();
      controller.DownRelease();
      //wait for lockoutTimeToExpire
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "Stop", "MovingDown_F=1");
      lockoutTimer.TimerElapsed();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "Stop", "MovingDown_F=1", "Down");
    }

    [TestMethod]
    public void UpStopAndDownStop_Work() {
      lockoutTimer.ElapseImmediately = true;
      controller.TriggerUpStopRelay = () => commands.Add("UpStop");
      controller.TriggerDownStopRelay = () => commands.Add("DownStop");
      controller.UpPress();
      controller.UpRelease();
      //moving up
      controller.DownPress();
      controller.DownRelease();
      //stopped
      controller.DownPress();
      controller.DownRelease();
      //moving down
      controller.DownPress();
      controller.DownRelease();
      //stopped
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1", "MovingUp_F=0", "UpStop", "Stop", "Down", "MovingDown_F=1", "MovingDown_F=0", "DownStop", "Stop");
    }

    [TestMethod]
    public void Down_HadPreviouslyFinishedGoingUp_GoesDown() {
      controller.UpPress();
      controller.UpRelease();
      pressHoldUpTimer.TimerElapsed();
      shadeMovingTimer.TimerElapsed();
      commands.Clear();
      controller.DownPress();
      controller.DownRelease();
      pressHoldDownTimer.TimerElapsed();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1");
    }
    #endregion

    #region Relay Disable Tests
    [TestMethod]
    public void DisabledActivated_DisableModeIsLock_UpRealayTriggered() {
      controller.DisableMode = DisableMode.GoUp;
      controller.Disable();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1");
    }

    [TestMethod]
    public void DisabledActivated_DisableModeIsGoUp_UpRealayTriggered() {
      controller.DisableMode = DisableMode.GoUp;
      controller.Disable();
      commands.Should().BeEquivalentTo("Up", "MovingUp_F=1");
    }

    [TestMethod]
    public void DisabledActivated_ShadeIsMovingDown_ReversesDirection() {      
      controller.DisableMode = DisableMode.GoUp;
      controller.DownPress();
      controller.DownRelease();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1");
      //moving down
      controller.Disable();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1", "MovingDown_F=0", "Stop", "MovingUp_F=1");
      lockoutTimer.TimerElapsed();
      commands.Should().BeEquivalentTo("Down", "MovingDown_F=1", "MovingDown_F=0", "Stop", "MovingUp_F=1", "Up");
    }

    [TestMethod]
    public void Disabled_Pressed_Disable_FFires() {
      controller.Disable();
      disabledF.Should().Be("Disabled_F=1", "because Disable_F should go high when controller is disabled");
    }
    [TestMethod]
    public void Enabled_Pressed_Disable_FClears() {
      controller.Enable();
      disabledF.Should().Be("Disabled_F=0", "because Disable_F should go low when controller is disabled");
    }

    [TestMethod]
    public void UpPress_ControllerIsDisabled_DoesNotSendCommands() {
      controller.Disable();
      commands.Clear();
      controller.UpPress();
      controller.UpRelease();
      commands.Should().BeEmpty("because no commands should be sent when the controller is disabled");
    }
    #endregion
  }
}
