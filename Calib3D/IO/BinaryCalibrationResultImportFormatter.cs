///
/// <project>Calib3D http://code.google.com/p/cam-calib3d/ </project>
/// <author>Christoph Heindl</author>
/// <copyright>Copyright (c) 2011, Christoph Heindl</copyright>
/// <license>New BSD License</license>
///

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calib3D.IO {

  /// <summary>
  /// Binary calibration result formatter.
  /// </summary>
  public class BinaryCalibrationResultImportFormatter : ICalibrationResultImportFormatter {
    System.Runtime.Serialization.IFormatter _formatter = new BinaryFormatter();

    public CalibrationResult Deserialize(System.IO.Stream s) {
      return _formatter.Deserialize(s) as CalibrationResult;
    }

  }
}
