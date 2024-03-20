using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace AET.Unity.Relays.Ui {
  public class ShadeUpDownStopRelaySplus {    
    public ShadeUpDownStopRelaySplus() {
      Relays = new ShadeUpDownStopRelayController();
    }

    public ShadeUpDownStopRelayController Relays { get; set; }
  }
}