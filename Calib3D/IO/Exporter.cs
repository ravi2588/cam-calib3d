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
using System.ComponentModel.Composition;

namespace Calib3D.IO {

  /// <summary>
  /// Interface for export formatters.
  /// </summary>
  [InheritedExport]
  public interface ICalibrationResultExportFormatter {

    /// <summary>
    /// Serialize calibration result to stream
    /// </summary>
    /// <param name="s">Stream</param>
    /// <param name="cr">Calibration result</param>
    void Serialize(System.IO.Stream s, CalibrationResult cr);
  }

  /// <summary>
  /// Export data using various formatters.
  /// </summary>
  public class Exporter {
    private Addins _addins;

    /// <summary>
    /// Construct with default values.
    /// </summary>
    public Exporter() {
      _addins = null;
    }

    /// <summary>
    /// Get/Set addin store to retrieve exporters from.
    /// </summary>
    /// <remarks>Allocates a default addin container if no
    /// explicit Addins object is provided.</remarks>
    public Addins Addins {
      get {
        if (_addins == null)
          _addins = new Addins(Addins.DefaultCatalog);
        return _addins;
      }
      set { _addins = value; }
    }

    /// <summary>
    /// Lookup calibration result formatter by full type name.
    /// </summary>
    /// <param name="full_name">Full type name of formatter</param>
    /// <returns>Formatter</returns>
    /// <exception cref="System.ArgumentException">If formatter is not found in addin collection</exception>
    public ICalibrationResultExportFormatter GetCalibrationResultExportFormatter(string full_name) {
      ICalibrationResultExportFormatter f =
        this.Addins.FindByFullName<ICalibrationResultExportFormatter>(this.Addins.CalibrationResultExporterFormatters, full_name);

      if (f == null)
        throw new ArgumentException(String.Format("No export formatter with name '{0}' found.", full_name));

      return f;
    }

    /// <summary>
    /// Export to file using default formatter.
    /// </summary>
    /// <param name="path">Path to export</param>
    /// <param name="cr">Calibration result</param>
    public void ToFile(string path, CalibrationResult cr) {
      ToFile(path, cr, new BinaryCalibrationResultExportFormatter());
    }

    /// <summary>
    /// Export to file using custom formatter.
    /// </summary>
    /// <param name="path">Path to export</param>
    /// <param name="f">Formatter</param>
    /// <param name="cr">Calibration result</param>
    public void ToFile(string path, CalibrationResult cr, ICalibrationResultExportFormatter f) {
      using (System.IO.Stream s = System.IO.File.OpenWrite(path)) {
        ToStream(s, cr, f);
      }
    }

    /// <summary>
    /// Export to stream using specific formatter.
    /// </summary>
    /// <param name="s">Stream</param>
    /// <param name="f">Formatter</param>
    /// <param name="cr">Calibration result</param>
    public void ToStream(System.IO.Stream s, CalibrationResult cr, ICalibrationResultExportFormatter f) {
      if (cr == null)
        throw new ArgumentNullException("Calibration result cannot be null.");
      
      f.Serialize(s, cr);
    }
  }
}
