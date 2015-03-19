using NUnit.Framework;

namespace battleships
{
	[TestFixture]
	public class Map_should
	{
		[Test]
		public void put_ship_inside_map_bounds()
		{
			var map = new Map(100, 10);
			Assert.IsTrue(map.PutShip(new Vector(0, 0), 5, Ship.Direction.Horizontal));
			Assert.IsTrue(map.PutShip(new Vector(95, 9), 5, Ship.Direction.Horizontal));
		}

		[Test]
		public void not_put_ship_outside_map()
		{
			var map = new Map(100, 10);
			Assert.IsFalse(map.PutShip(new Vector(99, 9), 2, Ship.Direction.Horizontal));
			Assert.IsFalse(map.PutShip(new Vector(99, 9), 2, Ship.Direction.Vertical));
		}

		[Test]
		public void kill_ship()
		{
			var map = new Map(100, 10);
			map.PutShip(new Vector(0, 0), 1, Ship.Direction.Horizontal);
			Assert.AreEqual(ShotEffect.Kill, map.Shoot(new Vector(0, 0)));
			Assert.AreEqual(Cell.DeadOrWoundedShip, map[new Vector(0, 0)]);
		}

		[Test]
		public void wound_ship()
		{
			var map = new Map(100, 10);
			map.PutShip(new Vector(0, 0), 2, Ship.Direction.Horizontal);
			Assert.AreEqual(ShotEffect.Wound, map.Shoot(new Vector(0, 0)));
			Assert.AreEqual(Cell.DeadOrWoundedShip, map[new Vector(0, 0)]);
			Assert.AreEqual(Cell.Ship, map[new Vector(1, 0)]);
		}
	}
}