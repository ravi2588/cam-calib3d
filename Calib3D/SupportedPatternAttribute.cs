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
  /// Declares a pattern supported by a pattern detector.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class SupportedPatternAttribute : Attribute {
    
    public SupportedPatternAttribute(Type t) { _pattern = t; }

    Type _pattern;
    public Type PatternType {
      get { return _pattern; }
      set { _pattern = value; }
    }
  }
}
