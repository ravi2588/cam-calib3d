using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.CommandLineParsing;
using System.ComponentModel;

namespace CommandLineExamples {

  /// <summary>
  /// Simple Chessboard calibration
  /// </summary>
  public class CheckerboardCalibration : IExample {

    /// <summary>
    /// Parsed arguments
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Directory containing the calibration images")]
      public string ImageDirectory { get; set; }

      [Description("Size of a single square in units of your choice")]
      public float? PatternSquareLength { get; set; }

      [Description("Number of inner corners per row and column")]
      public System.Drawing.Size? PatternSize { get; set; }
    }

    /// <summary>
    /// Example description
    /// </summary>
    public string Description {
      get { return "Performs camera calibration based on checkerboard patterns."; }
    }

    public void Run(string[] args) {
      CommandLineArguments a = new CommandLineArguments();
      CommandLineParser.ParseArguments(a, args);

      if (a.Help.GetValueOrDefault(false)) {
        CommandLineParser.PrintUsage(a);
        return;
      }

      string image_dir = a.ImageDirectory == null ? "./" : a.ImageDirectory;
      float size = a.PatternSquareLength.GetValueOrDefault(15);
      System.Drawing.Size corners = a.PatternSize.GetValueOrDefault(new System.Drawing.Size(9, 6));
      
      // Prepare pattern and detector
      Calib3D.CheckerBoard.CheckerBoardPattern pattern = new Calib3D.CheckerBoard.CheckerBoardPattern();
      pattern.CornerCount = corners;
      pattern.SquareLength = size;

      Calib3D.CheckerBoard.CheckerBoardDetector detect = new Calib3D.CheckerBoard.CheckerBoardDetector();
      detect.Pattern = pattern;

      // Load all images and collect model/image correspondences.
      System.Drawing.Size image_size = System.Drawing.Size.Empty;
      Calib3D.Correspondences c = new Calib3D.Correspondences();
      foreach (Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i in Calib3D.IO.Images.FromPath(image_dir)) {
        image_size = i.Size;
        Calib3D.DetectionResult dr = detect.FindPattern(i);
        c.AddView(dr);
      }

      // Perform intrinsic calibration 
      Calib3D.CalibrationResult cr = Calib3D.Calibration.GetIntrinsics(c, image_size);
      System.Console.WriteLine(String.Format("Reprojection Error {0}", cr.ReprojectionError));

    }
  }
}
