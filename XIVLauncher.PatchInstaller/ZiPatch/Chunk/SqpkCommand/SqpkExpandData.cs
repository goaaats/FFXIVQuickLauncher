﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIVLauncher.PatchInstaller.Util;
using XIVLauncher.PatchInstaller.ZiPatch.Util;

namespace XIVLauncher.PatchInstaller.ZiPatch.Chunk.SqpkCommand
{
    class SqpkExpandData : SqpkChunk
    {
        public new static string Command = "E";


        private const int BLOCK_SIZE = 128;

        public SqpackDatFile File { get; protected set; }
        public int BlockOffset { get; protected set; }
        public int BlockNumber { get; protected set; }
        

        public SqpkExpandData(ChecksumBinaryReader reader, int size) : base(reader, size) {}

        protected override void ReadChunk()
        {
            var start = reader.BaseStream.Position;

            reader.ReadBytes(3);

            File = new SqpackDatFile(reader);

            BlockOffset = reader.ReadInt32BE() << 7;
            BlockNumber = reader.ReadInt32BE();

            reader.ReadUInt32(); // Reserved

            reader.ReadBytes(Size - (int)(reader.BaseStream.Position - start));
        }

        public override void ApplyChunk(ZiPatchConfig config)
        {
            File.ResolvePath(config.Platform);
            
            var file = File.OpenStream(config.GamePath, FileMode.OpenOrCreate);
            SqpackDatFile.WriteEmptyFileBlockAt(file, BlockOffset, BlockNumber);
        }

        public override string ToString()
        {
            return $"{Type}:{Command}:{BlockOffset}:{BlockNumber}";
        }
    }
}