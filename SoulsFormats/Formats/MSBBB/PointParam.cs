﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace SoulsFormats
{
    public partial class MSBBB
    {
        /// <summary>
        /// A section containing points and volumes for various purposes.
        /// </summary>
        public class PointParam : Section<Region>
        {
            internal override string Type => "POINT_PARAM_ST";

            public List<Region> Regions;

            /// <summary>
            /// Creates a new PointSection with no regions.
            /// </summary>
            public PointParam(int unk1 = 3) : base(unk1)
            {
                Regions = new List<Region>();
            }

            /// <summary>
            /// Returns every region in the order they will be written.
            /// </summary>
            public override List<Region> GetEntries()
            {
                //return SFUtil.ConcatAll<Region>(
                //    Points, Circles, Spheres, Cylinders, Boxes);
                return Regions;
            }

            internal override Region ReadEntry(BinaryReaderEx br)
            {
                var region = new Region(br);
                Regions.Add(region);
                return region;
            }

            internal override void WriteEntry(BinaryWriterEx bw, int id, Region entry)
            {
                entry.Write(bw, id);
            }
        }

        internal enum RegionType : uint
        {
            Point = 0,
            Circle = 1,
            Sphere = 2,
            Cylinder = 3,
            Square = 4,
            Box = 5,
        }

        /// <summary>
        /// A point or volumetric area used for a variety of purposes.
        /// </summary>
        public class Region : Entry
        {
            /// <summary>
            /// The name of this region.
            /// </summary>
            public override string Name { get; set; }

            /// <summary>
            /// Unknown.
            /// </summary>
            public int Unk2, Unk3, Unk4;

            /// <summary>
            /// The shape of this region.
            /// </summary>
            public Shape Shape;

            /// <summary>
            /// Center of the region.
            /// </summary>
            public Vector3 Position;

            /// <summary>
            /// Rotation of the region, in degrees.
            /// </summary>
            public Vector3 Rotation;

            public List<short> UnkA, UnkB;

            /// <summary>
            /// An ID used to identify this region in event scripts.
            /// </summary>
            public int EventEntityID;

            public Region(string name)
            {
                Name = name;
                Position = Vector3.Zero;
                Rotation = Vector3.Zero;
                EventEntityID = -1;
                UnkA = new List<short>();
                UnkB = new List<short>();
                Unk2 = 0;
                Unk3 = 0;
                Unk4 = 0;
            }

            public Region(Region clone)
            {
                Name = clone.Name;
                Position = clone.Position;
                Rotation = clone.Rotation;
                EventEntityID = clone.EventEntityID;
                Unk2 = clone.Unk2;
                UnkA = new List<short>(clone.UnkA);
                UnkB = new List<short>(clone.UnkB);
                Unk3 = clone.Unk3;
                Unk4 = clone.Unk4;
            }

            internal Region(BinaryReaderEx br)
            {
                long start = br.Position;

                long nameOffset = br.ReadInt64();
                br.AssertInt32(0);
                br.ReadInt32(); // ID
                ShapeType shapeType = br.ReadEnum32<ShapeType>();
                Position = br.ReadVector3();
                Rotation = br.ReadVector3();
                Unk2 = br.ReadInt32();

                long baseDataOffset1 = br.ReadInt64();
                long baseDataOffset2 = br.AssertInt64(baseDataOffset1 + 4);
                long shapeDataOffset = br.ReadInt64();
                long baseDataOffset3 = br.ReadInt64();

                Name = br.GetUTF16(start + nameOffset);

                br.Position = start + baseDataOffset1;
                short countA = br.ReadInt16();
                UnkA = new List<short>(br.ReadInt16s(countA));

                br.Position = start + baseDataOffset2;
                short countB = br.ReadInt16();
                UnkB = new List<short>(br.ReadInt16s(countB));

                br.Position = start + shapeDataOffset;
                switch (shapeType)
                {
                    case ShapeType.Point:
                        Shape = new Shape.Point();
                        break;

                    case ShapeType.Circle:
                        Shape = new Shape.Circle(br);
                        break;

                    case ShapeType.Sphere:
                        Shape = new Shape.Sphere(br);
                        break;

                    case ShapeType.Cylinder:
                        Shape = new Shape.Cylinder(br);
                        break;

                    case ShapeType.Box:
                        Shape = new Shape.Box(br);
                        break;

                    default:
                        throw new NotImplementedException($"Unsupported shape type: {shapeType}");
                }

                br.Position = start + baseDataOffset3;
                EventEntityID = br.ReadInt32();
            }

            internal void Write(BinaryWriterEx bw, int id)
            {
                long start = bw.Position;

                bw.ReserveInt64("NameOffset");
                bw.WriteInt32(0);
                bw.WriteInt32(id);
                bw.WriteUInt32((uint)Shape.Type);
                bw.WriteVector3(Position);
                bw.WriteVector3(Rotation);
                bw.WriteInt32(Unk2);

                bw.ReserveInt64("BaseDataOffset1");
                bw.ReserveInt64("BaseDataOffset2");


                bw.ReserveInt64("ShapeDataOffset");
                bw.ReserveInt64("BaseDataOffset3");

                bw.FillInt64("NameOffset", bw.Position - start);
                bw.WriteUTF16(ReambiguateName(Name), true);
                bw.Pad(4);

                bw.FillInt64("BaseDataOffset1", bw.Position - start);
                bw.WriteInt16((short)UnkA.Count);
                bw.WriteInt16s(UnkA);
                bw.Pad(4);

                bw.FillInt64("BaseDataOffset2", bw.Position - start);
                bw.WriteInt16((short)UnkB.Count);
                bw.WriteInt16s(UnkB);
                bw.Pad(8);

                Shape.Write(bw, start);

                bw.FillInt64("BaseDataOffset3", bw.Position - start);
                bw.WriteInt32(EventEntityID);

                bw.Pad(8);
            }

            internal virtual void GetNames(MSBBB msb, Entries entries)
            {
                //ActivationPartName = GetName(entries.Parts, ActivationPartIndex);
            }

            internal virtual void GetIndices(MSBBB msb, Entries entries)
            {
                //ActivationPartIndex = GetIndex(entries.Parts, ActivationPartName);
            }

            /// <summary>
            /// Returns the region type, ID, shape type, and name of this region.
            /// </summary>
            public override string ToString()
            {
                return $"{Shape.Type} : {Name}";
            }
        }
    }
}
