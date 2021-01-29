using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace CsvImporter.NUnitTest
{
    [TestFixture]
    [Category("Recorded")]
    public abstract class BaseAzure
    {

        /// <summary>
        /// Get a connection string to use from our test settings.
        /// </summary>
        public string ConnectionString => "string de conexion AzureStorage";

        /// <summary>
        /// Get a random name so we won't have any conflicts when creating
        /// resources.
        /// </summary>
        /// <param name="prefix">Optional prefix for the random name.</param>
        /// <returns>A random name.</returns>
        public string Randomize(string prefix = "sample") =>
            $"{prefix}-{Guid.NewGuid()}";

        /// <summary>
        /// Lorem Ipsum sample file content
        /// </summary>
        protected const string SampleFileContent = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras dolor purus, interdum in turpis ut, ultrices ornare augue. Donec mollis varius sem, et mattis ex gravida eget. Duis nibh magna, ultrices a nisi quis, pretium tristique ligula. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Vestibulum in dui arcu. Nunc at orci volutpat, elementum magna eget, pellentesque sem. Etiam id placerat nibh. Vestibulum varius at elit ut mattis.  Suspendisse ipsum sem, placerat id blandit ac, cursus eget purus. Vestibulum pretium ante eu augue aliquam, ultrices fermentum nibh condimentum. Pellentesque pulvinar feugiat augue vel accumsan. Nulla imperdiet viverra nibh quis rhoncus. Nunc tincidunt sollicitudin urna, eu efficitur elit gravida ut. Quisque eget urna convallis, commodo diam eu, pretium erat. Nullam quis magna a dolor ullamcorper malesuada. Donec bibendum sem lectus, sit amet faucibus nisi sodales eget. Integer lobortis lacus et volutpat dignissim. Suspendisse cras amet.";

        /// <summary>
        /// Create a temporary path for creating files.
        /// </summary>
        /// <param name="extension">An optional file extension.</param>
        /// <returns>A temporary path for creating files.</returns>
        public string CreateTempPath(string extension = ".txt") =>
            Path.ChangeExtension(Path.GetTempFileName(), extension);

        /// <summary>
        /// Create a temporary file on disk.
        /// </summary>
        /// <param name="content">Optional content for the file.</param>
        /// <returns>Path to the temporary file.</returns>
        public string CreateTempFile(string content = SampleFileContent)
        {
            string path = CreateTempPath();
            File.WriteAllText(path, content);
            return path;
        }
    }
}
