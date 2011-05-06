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
using System.ComponentModel;
using Microsoft.Test.CommandLineParsing;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Threading;

namespace CommandLineExamples {

  /// <summary>
  /// Image detection based on features in live stream from camera.
  /// </summary>
  /// <example>
  /// CommandLineExamples.exe run LiveFeatureDetection /ModelImage=../etc/patterns/feature/box.png
  /// </example>
  public class LiveFeatureDetection : IExample {

    /// <summary>
    /// Supported commandline arguments.
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Path to model image to detect")]
      public string ModelImage { get; set; }

      [Description("Desired image size")]
      public System.Drawing.Size? FrameSize { get; set; }

      [Description("Device ID of camera")]
      public int? HardwareId { get; set; }
    }

    public string Description {
      get { return "Marker detection in live stream from camera."; }
    }

    public void Run(string[] args) {
      CommandLineArguments a = new CommandLineArguments();
      CommandLineParser.ParseArguments(a, args);

      if (a.Help.GetValueOrDefault(false)) {
        CommandLineParser.PrintUsage(a);
        return;
      }

      string image_path = Default.Get(a.ModelImage, "model.png");
      int device_id = Default.Get(a.HardwareId, 0);

      // Setup pattern and detector
      Calib3D.Feature.TexturedRectanglePattern pat = new Calib3D.Feature.TexturedRectanglePattern();
      pat.Image = Calib3D.IO.Images.FromPath(image_path).First();

      Calib3D.Feature.SurfDetector det = new Calib3D.Feature.SurfDetector();
      det.Pattern = pat;
      
      Calib3D.IO.Images.Show(pat.Image, 1, "Model");

      // Start capturing from camera.
      Capture capture = new Capture(device_id);

      if (a.FrameSize.HasValue) {
        capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, a.FrameSize.Value.Width);
        capture.SetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, a.FrameSize.Value.Height);
      }

      while (!Console.KeyAvailable) {
        Emgu.CV.Image<Bgr, byte> i = capture.QueryFrame().Clone();
        Calib3D.DetectionResult dr = det.FindPattern(i);
        dr.ResultRenderer.Render(i);
        Calib3D.IO.Images.Show(i, 30, "LiveFeed");
      }

    }
  }
}
