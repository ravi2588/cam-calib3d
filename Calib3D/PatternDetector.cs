/*
 * Calib3D http://code.google.com/p/cam-calib3d/
 * Copyright (c) 2011, Christoph Heindl. All rights reserved.
 * Code license:	New BSD License
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;

namespace Calib3D {

  /// <summary>
  /// Base class for all pattern detectors.
  /// </summary>
  [InheritedExport]
  public abstract class PatternDetector {
    private Pattern _pattern;

    /// <summary>
    /// Get/Set the pattern to be detected.
    /// </summary>
    /// <exception cref="System.NotSupportedException">if detector does not support the pattern type.</exception>
    public Pattern Pattern {
      get { return _pattern; }
      set {
        if (!PatternDetector.SupportsPattern(this.GetType(), value.GetType()))
          throw new NotSupportedException("Pattern detector does not support this pattern");

        if (_pattern != null) {
          _pattern.PropertyChanged -= new System.ComponentModel.PropertyChangedEventHandler(PatternPropertyChanged);
        }
        _pattern = value;
        if (_pattern != null) {
          _pattern.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PatternPropertyChanged);
        }
        // Trigger pattern update
        this.PatternUpdated();
      }
    }

    /// <summary>
    /// Find pattern in image and return detection result.
    /// </summary>
    /// <param name="i">Image to find pattern in</param>
    /// <returns>Detection result</returns>
    public abstract DetectionResult FindPattern(Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i);

    /// <summary>
    /// Test if pattern detector type supports a specific pattern.
    /// </summary>
    /// <param name="pattern_detector">Type of pattern detector.</param>
    /// <param name="pattern">Type of pattern.</param>
    /// <returns></returns>
    public static bool SupportsPattern(Type pattern_detector, Type pattern) {
      Attribute[] attrs = Attribute.GetCustomAttributes(pattern_detector, typeof(SupportedPatternAttribute));
      return attrs.Any(a => ((SupportedPatternAttribute)a).PatternType.IsAssignableFrom(pattern));
    }

    /// <summary>
    /// Callback invoked when the pattern was updated.
    /// </summary>
    protected virtual void PatternUpdated() { }

    /// <summary>
    /// Callback notification when a single pattern property changed.
    /// </summary>
    /// <param name="sender">Pattern</param>
    /// <param name="e">Event arguments</param>
    private void PatternPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
      this.PatternUpdated();
    }

  }
}
