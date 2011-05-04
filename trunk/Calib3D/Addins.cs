///
/// <project>Calib3D http://code.google.com/p/cam-calib3d/ </project>
/// <author>Christoph Heindl</author>
/// <copyright>Copyright (c) 2011, Christoph Heindl</copyright>
/// <license>New BSD License</license>
///

using System;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel.Composition;
using System.Collections.Generic;

namespace Calib3D {

  public class Addins {

    /// <summary>
    /// Default part catalog.
    /// </summary>
    private static ComposablePartCatalog _default_catalog = 
      new DirectoryCatalog(Environment.CurrentDirectory + @"\Addins");

    private ComposablePartCatalog _catalog;
    private CompositionContainer _container;

    /// <summary>
    /// Get the default part catalog.
    /// </summary>
    public static ComposablePartCatalog DefaultCatalog {
      get { return _default_catalog; }
    }

    /// <summary>
    /// Construct from catalog.
    /// </summary>
    /// <param name="catalog"></param>
    public Addins(ComposablePartCatalog catalog) {
      _catalog = catalog;
      _container = new CompositionContainer(_catalog);
      CompositionBatch cb = new CompositionBatch();
      cb.AddPart(this);
      _container.Compose(cb);
    }

    /// <summary>
    /// Get list of available patterns.
    /// </summary>
    [ImportMany(typeof(Calib3D.Pattern))]
    public IEnumerable<Calib3D.Pattern> Patterns { get; set; }

    /// <summary>
    /// Get list of available pattern detectors.
    /// </summary>
    [ImportMany(typeof(Calib3D.PatternDetector))]
    public IEnumerable<Calib3D.PatternDetector> Detectors { get; set; }

    /// <summary>
    /// Get list of available calibration result exporter formatters.
    /// </summary>
    [ImportMany(typeof(Calib3D.IO.ICalibrationResultExporter))]
    public IEnumerable<Calib3D.IO.ICalibrationResultExporter> CalibrationResultExporters { get; set; }

    /// <summary>
    /// Get list of available calibration result import formatters.
    /// </summary>
    [ImportMany(typeof(Calib3D.IO.ICalibrationResultImporter))]
    public IEnumerable<Calib3D.IO.ICalibrationResultImporter> CalibrationResultImporters { get; set; }

    /// <summary>
    /// Find object by reflecting its full name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="c"></param>
    /// <param name="full_name"></param>
    /// <returns></returns>
    public T FindByFullName<T>(IEnumerable<T> c, string full_name) {
      return c.FirstOrDefault<T>(t => t.GetType().FullName == full_name);
    }

    /// <summary>
    /// Find all detectors capable of detecting a specific pattern.
    /// </summary>
    /// <param name="c">Collection of pattern detectors</param>
    /// <param name="p">Pattern to test</param>
    /// <returns></returns>
    public IEnumerable<Calib3D.PatternDetector> FindDetectorsSupportingPattern(
      IEnumerable<Calib3D.PatternDetector> c, 
      Calib3D.Pattern p) 
    {
      return c.Where(pd => PatternDetector.SupportsPattern(pd.GetType(), p.GetType()));
    }

  }

}
