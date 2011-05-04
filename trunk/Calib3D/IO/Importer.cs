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
  /// Interface for import formatters.
  /// </summary>
  [InheritedExport]
  public interface ICalibrationResultImportFormatter {

    /// <summary>
    /// Deserialize calibration result from stream
    /// </summary>
    /// <param name="s">Stream</param>
    /// <returns>Calibration result</returns>
    CalibrationResult Deserialize(System.IO.Stream s);
  }

  /// <summary>
  /// Import data using various formatters.
  /// </summary>
  public class Importer {
    private Addins _addins;

    /// <summary>
    /// Construct with default values.
    /// </summary>
    public Importer() {
      _addins = null;
    }

    /// <summary>
    /// Get/Set addin store to retrieve importer formatters from.
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
    public ICalibrationResultImportFormatter GetCalibrationResultImportFormatter(string full_name) {
      ICalibrationResultImportFormatter f =
        this.Addins.FindByFullName<ICalibrationResultImportFormatter>(this.Addins.CalibrationResultImportFormatters, full_name);

      if (f == null)
        throw new ArgumentException(String.Format("No import formatter with name '{0}' found.", full_name));

      return f;
    }

    /// <summary>
    /// Import from file using default formatter
    /// </summary>
    /// <param name="path">Path to import from</param>
    /// <returns>Calibration result</returns>
    public CalibrationResult FromFile(string path) {
      return FromFile(path, new BinaryCalibrationResultFormatter());
    }

    /// <summary>
    /// Import from file using specific formatter
    /// </summary>
    /// <param name="path">Path to import from</param>
    /// <param name="f">Formatter</param>
    /// <returns>Calibration result</returns>
    public CalibrationResult FromFile(string path, ICalibrationResultImportFormatter f) {
      using (System.IO.Stream s = System.IO.File.OpenRead(path)) {
        return FromStream(s, f);
      }
    }

    /// <summary>
    /// Import from stream using specific formatter.
    /// </summary>
    /// <param name="s">Stream</param>
    /// <param name="f">Formatter</param>
    /// <returns>Calibration result</returns>
    public CalibrationResult FromStream(System.IO.Stream s, ICalibrationResultImportFormatter f) {
      CalibrationResult cr = f.Deserialize(s);
      if (cr == null)
        throw new ArgumentException("Could not deserialize calibration result from stream");

      return cr;
    }
  }
}
