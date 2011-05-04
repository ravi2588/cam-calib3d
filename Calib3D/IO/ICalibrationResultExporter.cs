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
  public interface ICalibrationResultExporter {

    /// <summary>
    /// Export calibration result.
    /// </summary>
    /// <param name="cr">Calibration result</param>
    void Export(CalibrationResult cr);
  }
}
