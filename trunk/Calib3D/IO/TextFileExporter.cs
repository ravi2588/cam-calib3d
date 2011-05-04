using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calib3D.IO.Extensions;

namespace Calib3D.IO {

  /// <summary>
  /// Serialize calibration result to file using plain text formatting.
  /// </summary>
  public class TextFileExporter : ICalibrationResultExporter {

    /// <summary>
    /// Construct a new instance of the TextFileExporter class.
    /// </summary>
    public TextFileExporter() {
      FileName = "calibration_result.txt";
    }

    /// <summary>
    /// Get/Set the filename to serialize to/from.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Get/Set a boolean value indicating whether to print intrinsic info or not.
    /// </summary>
    public bool WriteIntrinsics { get; set; }

    /// <summary>
    /// Get/Set a boolean value indicating whether to print extrinsic info or not.
    /// </summary>
    public bool WriteExtrinsics { get; set; }

    /// <summary>
    /// Export calibration result.
    /// </summary>
    /// <param name="cr">Calibration result</param>
    public void Export(CalibrationResult cr) {
      using (System.IO.Stream s = System.IO.File.OpenWrite(this.FileName))
      using (System.IO.TextWriter tw = new System.IO.StreamWriter(s))
      {
        tw.WriteLine(cr.PrettyPrint(this.WriteIntrinsics, this.WriteExtrinsics));
        s.Close();
      }
    }

  }
}
