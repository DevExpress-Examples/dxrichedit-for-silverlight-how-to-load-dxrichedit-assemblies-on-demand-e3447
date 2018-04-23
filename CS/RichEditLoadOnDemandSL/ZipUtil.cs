﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RichEditLoadOnDemandSL {
    public class ZipUtil {
        /// <summary>
        /// Reads the file names from the header of the zip file
        /// </summary>
        /// <param name="zipStream">The stream to the zip file</param>
        /// <returns>An array of file names stored within the zip file. These file names may also include relative paths.</returns>
        public static string[] GetZipContents(System.IO.Stream zipStream) {
            List<string> names = new List<string>();
            BinaryReader reader = new BinaryReader(zipStream);

            while (reader.ReadUInt32() == 0x04034b50) {
                // Skip the portions of the header we don't care about
                reader.BaseStream.Seek(14, SeekOrigin.Current);
                uint compressedSize = reader.ReadUInt32();
                uint uncompressedSize = reader.ReadUInt32();
                int nameLength = reader.ReadUInt16();
                int extraLength = reader.ReadUInt16();
                byte[] nameBytes = reader.ReadBytes(nameLength);
                names.Add(Encoding.UTF8.GetString(nameBytes, 0, nameLength));
                reader.BaseStream.Seek(extraLength + compressedSize, SeekOrigin.Current);
            }

            // Move the stream back to the begining
            zipStream.Seek(0, SeekOrigin.Begin);
            return names.ToArray();
        }
    }
}
