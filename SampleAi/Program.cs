using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace SampleAi
{
	class Program
	{
		// Управляющая программа battleships.exe будет запускать этот файл и перенаправлять стандартные потоки ввода и вывода.
		//
		// Вам нужно читать информацию с консоли и писать команды на консоль.
		// Конец ввода — это сигнал к завершению программы.

        private static readonly IEnumerable<Size> diagonals = 
            new List<Size> { new Size(1, 1), new Size(1, -1), new Size(-1, -1), new Size(-1, 1) };

        private static readonly IEnumerable<Size> adjacency = 
            new List<Size> { new Size(0, 1), new Size(1, 0), new Size(0, -1), new Size(-1, 0) };

	    private static readonly IEnumerable<Size> neighbours = diagonals.Union(adjacency);

		static void Main()
		{
			var random = new Random();
		    var aim = new Point(0, 0);
		    var boardSize = new Size(0, 0);
		    var nonTargetCells = new HashSet<Point>();
		    var hotspots = new Stack<Point>();
		    var woundedCells = new HashSet<Point>();

			while (true)
			{
				var line = Console.ReadLine();
				if (line == null) return;
				// line имеет один из следующих форматов:
				// Init <map_width> <map_height> <ship1_size> <ship2_size> ...
				// Wound <last_shot_X> <last_shot_Y>
				// Kill <last_shot_X> <last_shot_Y>
				// Miss <last_shot_X> <last_shot_Y>
				// Один экземпляр вашей программы может быть использван для проведения нескольких игр подряд.

				// Сообщение Init сигнализирует о том, что началась новая игра.

                var message = line.Split(' ');
			    switch (message[0])
			    {
			        case "Init": 
                        nonTargetCells.Clear();
                        hotspots.Clear();
                        woundedCells.Clear();
			            boardSize.Width = int.Parse(message[1]);
			            boardSize.Height = int.Parse(message[2]);
			            break;

                    case "Miss":
                        nonTargetCells.Add(aim);
                        break;

                    case "Wound":
    			        GetOffsetCells(aim, boardSize, diagonals).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        nonTargetCells.Add(aim);
                        hotspots.Push(aim);
			            woundedCells.Add(aim);
			            break;

                    case "Kill":
                        GetOffsetCells(aim, boardSize, neighbours).ToList().ForEach(cell => nonTargetCells.Add(cell));
                        nonTargetCells.Add(aim);
			            woundedCells.Add(aim);
			            MarkDeadShipAdjacency(aim, boardSize, woundedCells, nonTargetCells);
			            break;
			    }

                aim = NextCell(boardSize, nonTargetCells, hotspots, random);
			    Console.WriteLine("{0} {1}", aim.X, aim.Y);
			}
		}

	    private static bool WithinBoard(Point cell, Size size)
	    {
	        var withinVertically = 0 <= cell.X && cell.X < size.Width;
	        var withinHorizontally = 0 <= cell.Y && cell.Y < size.Height;
	        return withinVertically && withinHorizontally;
	    }

	    private static void MarkDeadShipAdjacency(Point aim, Size boardSize, 
            ICollection<Point> woundedCells, ICollection<Point> nonTargetCells)
	    {
	        GetOffsetCells(aim, boardSize, adjacency).Where(woundedCells.Contains).ToList().ForEach(toMark =>
	        {
                var direction = new Size(toMark) - new Size(aim);
	            while (woundedCells.Contains(toMark) && WithinBoard(toMark, boardSize))
	            {
	                toMark += direction;
	            }
                if (WithinBoard(toMark, boardSize))
                    nonTargetCells.Add(toMark);
	        });
	    }

	    private static Point NextCell(Size size, ICollection<Point> excluded, Stack<Point> hotspots, Random random)
	    {
	        while (hotspots.Count > 0)
	        {
	            var hotspot = hotspots.Peek();
	            var candidates = GetOffsetCells(hotspot, size, adjacency).Where(cell => !excluded.Contains(cell)).ToList();
	            if (candidates.Any()) return candidates[0];
	            hotspots.Pop();
	        }

	        Point nextCell;
	        do { nextCell = new Point(random.Next(size.Width), random.Next(size.Height)); } 
            while (excluded.Contains(nextCell));
	        return nextCell;
	    }

	    private static IEnumerable<Point> GetOffsetCells(Point cell, Size size, IEnumerable<Size> offsets)
	    {
	        return from offset in offsets
	            let point = Point.Add(cell, offset)
	            where WithinBoard(point, size)
	            select point;
	    } 
	}
}
