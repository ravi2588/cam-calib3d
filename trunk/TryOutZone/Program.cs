using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.IO;

namespace TryOutZone {
  class Program {
    static void Main(string[] args) {

      Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i = Calib3D.IO.Images.FromPath("marker.png").First();

      /*
      Calib3D.Addins a = new Calib3D.Addins(Calib3D.Addins.DefaultCatalog);
      Calib3D.Pattern p = a.FindByFullName(a.Patterns, "Calib3D.Marker.MarkerDetector");
      Calib3D.PatternDetector pd = a.FindDetectorsSupportingPattern(a.Detectors, p).FirstOrDefault();

      Calib3D.DetectionResult dr = pd.FindPattern(p, i);
      dr.PatternDetector.OverlayProvider.Overlay(i, dr);
       */

      Calib3D.Marker.MarkerPattern p = new Calib3D.Marker.MarkerPattern();
      Calib3D.Marker.MarkerDetector md = new Calib3D.Marker.MarkerDetector();

      p.MarkerImage = new Image<Emgu.CV.Structure.Bgr, byte>("marker_a.png");
      p.MarkerLength = 50;
      md.Pattern = p;

      Calib3D.DetectionResult dr = md.FindPattern(i);
      dr.ResultRenderer.Render(i);
      Calib3D.IO.Images.Show(i, 0);
    }
  }
}
