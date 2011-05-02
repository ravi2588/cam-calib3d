using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.CommandLineParsing;
using System.ComponentModel;
using System.IO;

namespace CommandLineExamples {

  /// <summary>
  /// Simple Chessboard calibration
  /// </summary>
  /// <example>
  /// CommandLineExamples.exe run CheckerboardCalibration /ImageDirectory=../etc/images/checkerboard ^ 
  ///                             /PatternSize=9,6 /PatternSquareLength=25 /Verbose
  /// </example>
  public class CheckerboardCalibration : IExample {

    /// <summary>
    /// Supported commandline arguments.
    /// </summary>
    class CommandLineArguments {

      [Description("Print this help")]
      public bool? Help { get; set; }

      [Description("Directory containing the calibration images")]
      public string ImageDirectory { get; set; }

      [Description("Size of a single square in units of your choice")]
      public float? PatternSquareLength { get; set; }

      [Description("Provide verbose output information")]
      public bool? Verbose { get; set; }

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
      bool verbose = a.Verbose.GetValueOrDefault(false);
      
      // Prepare pattern and detector
      Calib3D.CheckerBoard.CheckerBoardPattern pattern = new Calib3D.CheckerBoard.CheckerBoardPattern();
      pattern.CornerCount = corners;
      pattern.SquareLength = size;

      Calib3D.CheckerBoard.CheckerBoardDetector detect = new Calib3D.CheckerBoard.CheckerBoardDetector();
      detect.Pattern = pattern;

      // Load all images and collect model/image correspondences.
      System.Drawing.Size image_size = System.Drawing.Size.Empty;
      Calib3D.Correspondences c = new Calib3D.Correspondences();

      List<Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>> images = new List<Emgu.CV.Image<Emgu.CV.Structure.Bgr,byte>>();
      foreach (string path in Calib3D.IO.Directory.GetFiles(image_dir, "*.png;*.jpg")) {
        System.Console.WriteLine(String.Format("  Processing image {0}", new FileInfo(path).Name));
        Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte> i = Calib3D.IO.Images.FromPath(path).First();
        images.Add(i);
        image_size = i.Size;
        
        Calib3D.DetectionResult dr = detect.FindPattern(i);
        c.AddView(dr);

        if (verbose) {
          dr.ResultRenderer.Render(i);
        }
      }

      // Perform intrinsic calibration 
      Calib3D.CalibrationResult cr = Calib3D.Calibration.GetIntrinsics(c, image_size);
      System.Console.WriteLine(String.Format("Reprojection Error {0} pixels.", cr.ReprojectionError));

      if (verbose) {
        for (int i = 0; i < c.ViewCount; ++i) {
          cr.ResultRenderer.Render(images[i], i);
          Calib3D.IO.Images.Show(images[i], true);
        }
      }

    }
  }
}
