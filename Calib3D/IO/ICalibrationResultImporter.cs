using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Calib3D.IO {

  /// <summary>
  /// Interface for calibration result exporters.
  /// </summary>
  [InheritedExport]
  public interface ICalibrationResultImporter {

    /// <summary>
    /// Import calibration result.
    /// </summary>
    /// <returns>Calibration result</returns>
    CalibrationResult Import();

  }
}
