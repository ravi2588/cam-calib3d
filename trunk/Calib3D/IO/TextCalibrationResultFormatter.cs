using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Calib3D.IO.Extensions;

namespace Calib3D.IO {

  /// <summary>
  /// Export calibration result as plain text.
  /// </summary>
  public class TextCalibrationResultFormatter : ICalibrationResultExportFormatter {
    
    /// <summary>
    /// Construct with default values.
    /// </summary>
    public TextCalibrationResultFormatter() {
      this.WriteIntrinsics = true;
      this.WriteExtrinsics = true;
    }

    /// <summary>
    /// Get/Set a boolean value indicating whether to print intrinsic info or not.
    /// </summary>
    public bool WriteIntrinsics { get; set;}

    /// <summary>
    /// Get/Set a boolean value indicating whether to print extrinsic info or not.
    /// </summary>
    public bool WriteExtrinsics { get; set; }

    /// <summary>
    /// Serialize calibration result to output
    /// </summary>
    /// <param name="s">Stream</param>
    /// <param name="cr">Calibration result</param>
    public void Serialize(System.IO.Stream s, CalibrationResult cr) {
      using (TextWriter tw = new StreamWriter(s)) {
        tw.WriteLine(cr.PrettyPrint(this.WriteIntrinsics, this.WriteExtrinsics));
      }
    }
  }
}
