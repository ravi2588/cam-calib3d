using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calib3D.IO {

  /// <summary>
  /// Binary calibration result formatter.
  /// </summary>
  public class BinaryCalibrationResultExportFormatter : ICalibrationResultExportFormatter {
    System.Runtime.Serialization.IFormatter _formatter = new BinaryFormatter();

    public void Serialize(System.IO.Stream s, CalibrationResult cr) {      
      _formatter.Serialize(s, cr);
    }
  }
}
