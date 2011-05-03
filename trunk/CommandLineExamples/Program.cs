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
using System.ComponentModel.Composition.Hosting;
using Microsoft.Test.CommandLineParsing;

namespace CommandLineExamples {
  /// <summary>
  /// Defines the interface for an example.
  /// </summary>
  [InheritedExport]
  public interface IExample {
    /// <summary>
    /// Get example description.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Run example.
    /// </summary>
    /// <param name="args">Commandline arguments</param>
    void Run(string[] args);
  }

  class Program {
    /// <summary>
    /// Let MEF import all available examples.
    /// </summary>
    [ImportMany(typeof(IExample))]
    public IEnumerable<IExample> Examples { get; set; }

    static void Main(string[] args) {
      Program p = new Program();

      // Compose
      var catalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
      var container = new CompositionContainer(catalog);
      var batch = new CompositionBatch();
      batch.AddPart(p);
      container.Compose(batch);

      if (String.Compare(args[0], "list", StringComparison.InvariantCultureIgnoreCase) == 0) {
        Console.WriteLine();
        Console.WriteLine("Registered Examples");
        foreach (IExample e in p.Examples) {
          Console.WriteLine(String.Format("{0} - {1}", e.GetType().Name, e.Description));
        }
      }
      else if (String.Compare(args[0], "run", StringComparison.InvariantCultureIgnoreCase) == 0) {
        IExample e = p.Examples.FirstOrDefault(ex => ex.GetType().Name == args[1]);
        if (e != null) {
          e.Run(args.Skip(2).ToArray());
        }
        else {
          Console.WriteLine(String.Format("{0} unknown", args[1]));
        }
      }
    }
  }
}
