
namespace Space.Server
{
    using Space.Mmo.Server;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using GameMath;
    using Nebula.Engine;
    using Space.Game;

    public abstract class GridWorld : IWorld, IDisposable 
    {
        private readonly ItemCache itemCache;
        private readonly BoundingBox rectangleArea;
        private readonly Vector tileDimensions;
        private readonly Vector tileSize;
        private readonly Region[][][] worldRegions;
        protected readonly ReaderWriterLockSlim readWriteLock;

        public GridWorld(Vector corner1, Vector corner2, Vector tileDimensions, ItemCache itemCache )
            : this(BoundingBox.CreateFromPoints(corner1, corner2), tileDimensions, itemCache )
        { 
        
        }

        public GridWorld(BoundingBox boundingBox, Vector tileDimensions, ItemCache itemCache )
        {
            this.readWriteLock = new ReaderWriterLockSlim();

            this.rectangleArea = new BoundingBox
            {
                Min = new Vector { X = boundingBox.Min.X, Y = boundingBox.Min.Y, Z = boundingBox.Min.Z },
                Max = new Vector { X = boundingBox.Max.X, Y = boundingBox.Max.Y, Z = boundingBox.Max.Z }
            };
            var size = new Vector { X = boundingBox.Max.X - boundingBox.Min.X + 1, Y = boundingBox.Max.Y - boundingBox.Min.Y + 1, Z = boundingBox.Max.Z - boundingBox.Min.Z + 1 };
            if(tileDimensions.X <= 0)
            {
                tileDimensions.X = size.X;
            }
            if(tileDimensions.Y <= 0)
            {
                tileDimensions.Y = size.Y;
            }
            if(tileDimensions.Z <= 0 )
            {
                tileDimensions.Z = size.Z;
            }
            this.tileDimensions = tileDimensions;
            this.tileSize = new Vector { X = tileDimensions.X - 1, Y = tileDimensions.Y - 1, Z = tileDimensions.Z - 1 };
            this.itemCache = itemCache;

            var regionsX = (int)Math.Ceiling(size.X / (double)tileDimensions.X);
            var regionsY = (int)Math.Ceiling(size.Y / (double)tileDimensions.Y);
            var regionsZ = (int)Math.Ceiling(size.Z / (double)tileDimensions.Z);

            this.worldRegions = new Region[regionsX][][];
            Vector current = boundingBox.Min;
            for(int x = 0; x < regionsX; x++ )
            {
                this.worldRegions[x] = new Region[regionsY][];
                for(int y = 0; y < regionsY; y++ )
                {
                    this.worldRegions[x][y] = new Region[regionsZ];
                    for(int z = 0; z < regionsZ; z++)
                    {
                        this.worldRegions[x][y][z] = new Region(current);
                        current.Z += tileDimensions.Z;
                    }
                    current.Y += tileDimensions.Y;
                    current.Z = boundingBox.Min.Z;
                }
                current.X += tileDimensions.X;
                current.Y = boundingBox.Min.Y;
                current.Z = boundingBox.Min.Z;
            }
            //PrintRegions(boundingBox);
        }

        private void PrintRegions(BoundingBox boundingBox)
        {
            /*
            var size = new Vector { X = boundingBox.Max.X - boundingBox.Min.X + 1, Y = boundingBox.Max.Y - boundingBox.Min.Y + 1, Z = boundingBox.Max.Z - boundingBox.Min.Z + 1 };
            var regionsX = (int)Math.Ceiling(size.X / (double)tileDimensions.X);
            var regionsY = (int)Math.Ceiling(size.Y / (double)tileDimensions.Y);
            var regionsZ = (int)Math.Ceiling(size.Z / (double)tileDimensions.Z);
            for (int x = 0; x < regionsX; x++)
            {
                for (int y = 0; y < regionsY; y++)
                {
                    for (int z = 0; z < regionsZ; z++)
                    {
                        ConsoleLogging.Get.Print("region[{0},{1},{2}]={3}", x, y, z, worldRegions[x][y][z].Coordinate);
                    }
                }
            }*/
        }

        ~GridWorld()
        {
            this.Dispose(false);
        }

        public BoundingBox Area
        {
            get
            {
                return rectangleArea;
            }
        }

        public ItemCache ItemCache
        {
            get
            {
                return itemCache;
            }
        }

        public Vector TileDimensions
        {
            get
            {
                return tileDimensions;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        public Region GetRegion(Vector position )
        {
            if(rectangleArea.Contains(position))
            {
                return GetRegionAt(position);
            }
            return null;
        }

        public BoundingBox GetRegionAlignedBoundingBox(BoundingBox area)
        {
            area = this.rectangleArea.IntersectWith(area);
            if(area.IsValid())
            {
                var result = new BoundingBox { Min = this.GetRegionAt(area.Min).Coordinate, Max = this.GetRegionAt(area.Max).Coordinate + this.tileSize };
                return result;
            }
            return area;
        }

        public HashSet<Region> GetRegions(BoundingBox area )
        {
            return new HashSet<Region>(this.GetRegionEnumerable(area));
        }

        public HashSet<Region> GetRegionsExcept(BoundingBox area, BoundingBox except )
        {
            var result = new HashSet<Region>();

            if( area.Min.X < except.Min.X)
            {
                var box = new BoundingBox { Min = area.Min, Max = new Vector { X = Math.Min(area.Max.X, except.Min.X - 1), Y = area.Max.Y, Z = area.Max.Z } };
                result.UnionWith(GetRegionEnumerable(box));
            }

            if(area.Min.Y < except.Min.Y)
            {
                var box = new BoundingBox { Min = area.Min, Max = new Vector { X = area.Max.X, Y = Math.Min(area.Max.Y, except.Min.Y - 1), Z = area.Max.Z } };
                result.UnionWith(GetRegionEnumerable(box));
            }

            if(area.Min.Z < except.Min.Z )
            {
                var box = new BoundingBox { Min = area.Min, Max = new Vector { X = area.Max.X, Y = area.Max.Y, Z = Math.Min(area.Max.Z, except.Min.Z - 1) } };
                result.UnionWith(GetRegionEnumerable(box));
            }

            if(area.Max.X > except.Max.X)
            {
                var box = new BoundingBox { Min = new Vector { X = Math.Max(area.Min.X, except.Max.X + 1), Y = area.Min.Y, Z = area.Min.Z }, Max = area.Max };
                result.UnionWith(GetRegionEnumerable(box));
            }
            if(area.Max.Y > except.Max.Y)
            {
                var box = new BoundingBox { Min = new Vector { X = area.Min.X, Y = Math.Max(area.Min.Y, except.Max.Y + 1), Z = area.Min.Z }, Max = area.Max  };
                result.UnionWith(GetRegionEnumerable(box));
            }
            if(area.Max.Z > except.Max.Z)
            {
                var box = new BoundingBox { Min = new Vector { X = area.Min.X, Y = area.Min.Y, Z = Math.Max(area.Min.Z, except.Max.Z + 1) }, Max = area.Max };
                result.UnionWith(GetRegionEnumerable(box));
            }
            return result;
        }

        protected virtual void Dispose(bool disposing )
        {
            if(disposing )
            {
                foreach(Region[][] regions in this.worldRegions)
                {
                    foreach(Region[] regions2 in regions)
                    {
                        foreach(Region region in regions2)
                        {
                            region.Dispose();
                        }
                    }
                }
            }
            this.readWriteLock.Dispose();
        }

        private Region GetRegionAt(Vector coordinate )
        {
            Vector relativePoint = coordinate - this.rectangleArea.Min;
            int indexX = (int)(relativePoint.X / tileDimensions.X);
            int indexY = (int)(relativePoint.Y / tileDimensions.Y);
            int indexZ = (int)(relativePoint.Z / tileDimensions.Z);
            //ConsoleLogging.Get.Print("Region at coordinate: {0} is {1},{2},{3}", coordinate, indexX, indexY, indexZ);
            return this.worldRegions[indexX][indexY][indexZ];
        }


        private IEnumerable<Region> GetRegionEnumerable(BoundingBox area )
        {
            BoundingBox overlap = this.rectangleArea.IntersectWith(area);
            Vector current = overlap.Min;
            while(current.Z <= overlap.Max.Z)
            {
                foreach(Region region in this.GetRegionsForXY(overlap, current))
                {
                    yield return region;
                }
                current.Z += this.tileDimensions.Z;
            }
            if(current.Z > overlap.Max.Z)
            {
                current.Z = overlap.Max.Z;
                foreach(Region region in this.GetRegionsForXY(overlap, current))
                {
                    yield return region;
                }
            }
            yield break;
        }

        private IEnumerable<Region> GetRegionsForXY(BoundingBox overlap, Vector current )
        {
            current.Y = overlap.Min.Y;
            while(current.Y <= overlap.Max.Y )
            {
                foreach(Region region in this.GetRegionsForY(overlap, current))
                {
                    yield return region;
                }
                current.Y += this.tileDimensions.Y;
            }
            if(current.Y > overlap.Max.Y)
            {
                current.Y = overlap.Max.Y;
                foreach(Region region in this.GetRegionsForY(overlap, current))
                {
                    yield return region;
                }
            }
        }

        private IEnumerable<Region> GetRegionsForY(BoundingBox overlap, Vector current )
        {
            current.X = overlap.Min.X;
            while(current.X <= overlap.Max.X)
            {
                yield return this.GetRegionAt(current);
                current.X += this.tileDimensions.X;
            }
            if(current.X > overlap.Max.X)
            {
                current.X = overlap.Max.X;
                yield return this.GetRegionAt(current);
            }
        }

        public abstract bool TryGetObject(byte objectType, string objectId, out NebulaObject obj);

        public abstract void RemoveObject(byte objectType, string objectId);

        public abstract bool AddObject(NebulaObject obj);

        public abstract List<NebulaObject> Filter(Func<NebulaObject, bool> filter);

        public abstract IRes Resource();
    }
}
