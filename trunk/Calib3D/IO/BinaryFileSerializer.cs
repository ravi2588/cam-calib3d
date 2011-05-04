using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Calib3D.IO {
  
  /// <summary>
  /// Serialize calibration result from/to file using binary formatting.
  /// </summary>
  public class BinaryFileSerializer : ICalibrationResultExporter, ICalibrationResultImporter {

    /// <summary>
    /// Construct a new instance of the BinaryFileSerializer class.
    /// </summary>
    public BinaryFileSerializer() {
      FileName = "calibration_result.bin";
    }

    /// <summary>
    /// Get/Set the filename to serialize to/from.
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Export calibration result.
    /// </summary>
    /// <param name="cr">Calibration result</param>
    public void Export(CalibrationResult cr) {
      using (System.IO.Stream s = System.IO.File.OpenWrite(this.FileName)) {
        System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
        formatter.Serialize(s, cr);
        s.Close();
      }
    }

    /// <summary>
    /// Import calibration result.
    /// </summary>
    /// <returns>Calibration result</returns>
    public CalibrationResult Import() {
      using (System.IO.Stream s = System.IO.File.OpenRead(this.FileName)) {
        System.Runtime.Serialization.IFormatter formatter = new BinaryFormatter();
        CalibrationResult cr = formatter.Deserialize(s) as CalibrationResult;
        s.Close();
        return cr;
      }
    }

  }
}
