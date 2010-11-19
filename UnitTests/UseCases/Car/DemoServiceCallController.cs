using System;
using System.Linq;

namespace NGineer.UnitTests.UseCases.Car
{
    public class DemoServiceCallController
    {
        public const int DistanceBetweenOilChanges = 10000;
        public const int DaysSinceOilChange = 100;

        public static bool RequiresService(Car car)
        {
            if (car.Wheels.Any(wheel => wheel.IsFlat))
                return true;
            
            if (car.DistanceSinceLastOilChange > DistanceBetweenOilChanges)
                return true;

            if (car.LastOilChange.HasValue 
                && DateTime.Now.Subtract(car.LastOilChange.Value).TotalDays > DaysSinceOilChange)
                return true;

            return false;
        }
    }
}