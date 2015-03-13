using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace SampleAi
{
	class Program
	{
		// Управляющая программа battleships.exe будет запускать этот файл и перенаправлять стандартные потоки ввода и вывода.
		//
		// Вам нужно читать информацию с консоли и писать команды на консоль.
		// Конец ввода — это сигнал к завершению программы.

		static void Main()
		{
			var r = new Random();
		    var aim = new Point(0, 0);
		    var size = new Point(20, 20);
		    var nonTargetCells = new HashSet<Point>();
//            Console.WriteLine(new Point(1,2)  new Point(2,3));
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

			    if (!nonTargetCells.Contains(aim))
			    {
                    Console.WriteLine("{0} {1}", aim.X, aim.Y);
			        nonTargetCells.Add(aim);
//			        GetDiagonalCells(aim, size).Select(nonTargetCells.Add);
			    }

			    aim = NextCell(aim, size);
			}
		}

	    private static Point NextCell(Point cell, Point size)
	    {
            cell.Y++;
            if (cell.Y >= size.Y)
            {
                cell.Y = 0;
                cell.X++;
            }
            if (cell.X >= size.X)
            {
                cell.X = 0;
                cell.Y = 0;
            }	    
	        return cell;
	    }

	    private static IEnumerable<Point> GetDiagonalCells(Point cell, Point size)
	    {
            var diagonals = new List<Size> { new Size(1, 1), new Size(1, -1), new Size(-1, -1), new Size(-1, 1) };
	        return diagonals.Select(diagonal => Point.Add(cell, diagonal));
	    }
	}
}
