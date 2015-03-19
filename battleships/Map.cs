using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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

        public List<Vector> GetOccupiedCells()
        {
            var directionVector = GetDirectionVector(ShipDirection);
            var occupiedCells = new List<Vector>();
            for (int i = 0; i < Size; i++)
            {
                var shipCell = directionVector.Mult(i).Add(Location);
                occupiedCells.Add(shipCell);
            }
            return occupiedCells;
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

        public enum Direction
        {
            Vertical,
            Horizontal
        } 
    }

    public class Map
    {
        private static Cell[,] Cells;

        public static Ship[,] FieldedShips;

        public List<Ship> Ships = new List<Ship>();

        public int Width { get; private set; }

        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[width, height];
            FieldedShips = new Ship[width, height];
        }

        public Cell this[Vector p]
        {
            get
            {
                return CheckBounds(p) ? Cells[p.X, p.Y] : Cell.Empty; // Благодаря этому трюку иногда можно будет не проверять на выход за пределы поля. 
            }
            private set
            {
                if (!CheckBounds(p))
                    throw new IndexOutOfRangeException(p + " is not in the map borders"); // Поможет отлавливать ошибки в коде.
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

            foreach (var cell in shipCells)
            {
                this[cell] = Cell.Ship;
                FieldedShips[cell.X, cell.Y] = ship;
            }
            Ships.Add(ship);
            return true;
        }

        public ShotEffect Shoot(Vector target)
        {
            var hit = CheckBounds(target) && this[target] == Cell.Ship;


            if (hit)
            {
                var ship = FieldedShips[target.X, target.Y];
                ship.AliveCells.Remove(target);
                this[target] = Cell.DeadOrWoundedShip;
                return ship.IsAlive ? ShotEffect.Wound : ShotEffect.Kill;
            }


            if (this[target] == Cell.Empty) this[target] = Cell.Miss;
            return ShotEffect.Miss;
        }

        ///<summary>Окрестность ячейки</summary>
        public IEnumerable<Vector> Neighbours(Vector cell)
        {
            return
                from i in new[] { -1, 0, 1 } //x
                from j in new[] { -1, 0, 1 } //y
                let c = cell.Add(new Vector(i, j))
                where CheckBounds(c)
                select c;
        }

        ///<summary>Проверка на выход за границы</summary>
        public bool CheckBounds(Vector p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }

        ///<summary>Есть ли хоть одна живая клетка</summary>
        public bool HasAliveShips()
        {
            for (int index = 0; index < Ships.Count; index++)
            {
                var s = Ships[index];
                if (s.IsAlive) return true;
            }
            return false;
        }
    }
}