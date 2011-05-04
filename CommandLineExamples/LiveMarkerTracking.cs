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
using Calib3D.IO.Extensions;

namespace CommandLineExamples {

  /// <summary>
  /// 3D Marker tracking in live stream from camera.
  /// </summary>
  /// <example>
  /// CommandLineExamples.exe run LiveMarkerTracking /MarkerImage=../etc/patterns/marker/marker_a.png /CameraCalibration=calibration.cr
  /// </example>
  public class LiveMarkerTracking : IExample {

    /// <summary>
    /// Supported commandline arguments.
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Path to marker image")]
      public string MarkerImage { get; set; }

      [Description("Path to camera calibration")]
      public string CameraCalibration { get; set;}

      [Description("Size of marker")]
      public float? MarkerLength { get; set; }

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

      string image_path = Default.Get(a.MarkerImage, "marker.png");
      string camera_calib_path = Default.Get(a.CameraCalibration, "marker.png");
      int device_id = Default.Get(a.HardwareId, 0);
      float marker_length = Default.Get(a.MarkerLength, 50);

      // Setup pattern and detector
      Calib3D.Marker.MarkerPattern pat = new Calib3D.Marker.MarkerPattern();
      pat.MarkerImage = Calib3D.IO.Images.FromPath(image_path).First();
      pat.MarkerLength = marker_length;

      Calib3D.Marker.MarkerDetector det = new Calib3D.Marker.MarkerDetector();
      det.BinaryThreshold = 40;
      det.MaximumErrorNormed = 0.4f;
      det.Pattern = pat;

      // Load camera calibration
      Calib3D.IO.BinaryFileSerializer import = new Calib3D.IO.BinaryFileSerializer();
      import.FileName = camera_calib_path;
      Calib3D.CalibrationResult capture_calib = import.Import();
      Console.WriteLine(capture_calib.Intrinsics.PrettyPrint());

      // Show marker to be tracked
      Calib3D.IO.Images.Show(pat.MarkerImage, 1, "Marker");

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

        // When pattern was detected successfully, try to get extrinsics
        if (dr.Success) {
          Calib3D.CalibrationResult cr = Calib3D.Calibration.GetExtrinsics(dr.ViewCorrespondences, capture_calib.Intrinsics);
          cr.ResultRenderer.Render(i, 0);
          Console.Write(String.Format("\r Error: {0,10:f4}, Distance: {1,10:f2}", cr.ReprojectionError, cr.Extrinsics[0].TranslationVector.Norm));
        }

        Calib3D.IO.Images.Show(i, 30, "LiveFeed");
      }

    }
  }
}
