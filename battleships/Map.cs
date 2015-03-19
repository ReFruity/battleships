using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace battleships
{
    public enum Cell
    {
        Empty,
        Ship,
        DeadOrWoundedShip,
        Miss
    }

    public enum ShotEffect
    {
        Miss,
        Wound,
        Kill
    }

    public class Ship
    {
        public int Size { get; private set; }

        public Direction ShipDirection { get; private set; }

        public HashSet<Vector> AliveCells;

        public bool IsAlive { get { return AliveCells.Any(); } }

        public Vector Location { get; private set; }

        public Ship(Vector location, int size, Direction shipDirection)
        {
            Location = location;
            Size = size;
            ShipDirection = shipDirection;
            AliveCells = new HashSet<Vector>(GetOccupiedCells());
        }

        public enum Direction
        {
            Vertical,
            Horizontal
        } 

        public List<Vector> GetOccupiedCells()
        {
            var directionVector = GetDirectionVector(ShipDirection);
            return (from i in Enumerable.Range(0, Size) 
                    select directionVector.Mult(i).Add(Location))
                    .ToList();
        }

        public static Vector GetDirectionVector(Direction direction)
        {
            switch (direction)
            {
                case Direction.Horizontal: return new Vector(1, 0);
                case Direction.Vertical: return new Vector(0, 1);
                default: throw new ArgumentException("Unknown ship direction.");
            }
        }
    }

    public class Map
    {
        private static Cell[,] Cells;

        public static Ship[,] ShipsMap;

        public List<Ship> Ships = new List<Ship>();

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width, height];
            ShipsMap = new Ship[width, height];
        }

        public Cell this[Vector p]
        {
            get
            {
                if (!CheckBounds(p))
                    throw new IndexOutOfRangeException(p + " is not in the map borders");
                return Cells[p.X, p.Y];
            }
            private set
            {
                if (!CheckBounds(p))
                    throw new IndexOutOfRangeException(p + " is not in the map borders");
                Cells[p.X, p.Y] = value;
            }
        }

        public bool PutShip(Vector location, int length, Ship.Direction direction)
        {
            var ship = new Ship(location, length, direction);
            var shipCells = ship.GetOccupiedCells();

            //Если рядом есть непустая клетка, то поместить корабль нельзя!
            if (shipCells.SelectMany(Neighbours).Any(c => this[c] != Cell.Empty)) return false;
            //Если корабль не помещается — тоже нельзя
            if (!shipCells.All(CheckBounds)) return false;

            shipCells.ForEach(cell =>
            {
                this[cell] = Cell.Ship;
                ShipsMap[cell.X, cell.Y] = ship;
            });

            Ships.Add(ship);

            return true;
        }

        public ShotEffect Shoot(Vector target)
        {
            var hit = CheckBounds(target) && this[target] == Cell.Ship;
            if (hit)
            {
                var ship = ShipsMap[target.X, target.Y];
                ship.AliveCells.Remove(target);
                this[target] = Cell.DeadOrWoundedShip;
                return ship.IsAlive ? ShotEffect.Wound : ShotEffect.Kill;
            }

            if (this[target] == Cell.Empty) this[target] = Cell.Miss;
            return ShotEffect.Miss;
        }

        public IEnumerable<Vector> Neighbours(Vector cell)
        {
            return
                from x in new[] { -1, 0, 1 }
                from y in new[] { -1, 0, 1 }
                let c = cell.Add(new Vector(x, y))
                where CheckBounds(c)
                select c;
        }

        public bool CheckBounds(Vector p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }

        public bool HasAliveShips()
        {
            return Ships.Any(s => s.IsAlive);
        }
    }
}